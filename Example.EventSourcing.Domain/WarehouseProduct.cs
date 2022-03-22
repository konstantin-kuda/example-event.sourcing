using Example.EventSourcing.Domain.Abstractions;
using Example.EventSourcing.Domain.Events;
using Example.EventSourcing.Domain.Exceptions;

namespace Example.EventSourcing.Domain;

public class WarehouseProduct
{
    public string Sku { get; }

    private readonly IList<IProductEvent> _allEvents = new List<IProductEvent>();
    private readonly IList<IProductEvent> _uncommitedEvents = new List<IProductEvent>();

    private readonly WarehouseProductState _currentState = new();

    public WarehouseProduct(string sku)
    {
        Sku = sku;
    }

    public void ShipProduct(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(nameof(quantity));
        }
        
        if (_currentState.Quantity < quantity)
        {
            throw new InvalidDomainException($"Not enough product(s) to ship. Sku: {Sku}");
        }

        AddEvent(
            new ProductShippedProductEvent(Sku, quantity, DateTime.UtcNow)    
        );
    }

    public void ReceiveProduct(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(nameof(quantity));
        }
        
        AddEvent(
            new ProductReceivedProductEvent(Sku, quantity, DateTime.UtcNow)    
        );
    }
    
    public void AdjustInventory(int quantity, string reason)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(nameof(quantity));
        }
        
        AddEvent(
            new InventoryAdjustedProductEvent(Sku, quantity, DateTime.UtcNow, reason)
        );
    }

    private void AddEvent(IProductEvent productProductEvent)
    {
        Apply(productProductEvent);
        
        _uncommitedEvents.Add(productProductEvent);
    }

    public void Apply(IProductEvent productProductEvent)
    {
        switch (productProductEvent)
        {
            case ProductShippedProductEvent shipProduct:
                Apply(shipProduct);
                break;
            
            case ProductReceivedProductEvent receiveProduct:
                Apply(receiveProduct);
                break;
            
            case InventoryAdjustedProductEvent receiveProduct:
                Apply(receiveProduct);
                break;
            
            default:
                throw new InvalidOperationException("Unsupported Event.");
        }
        _allEvents.Add(productProductEvent);
    }

    private void Apply(ProductShippedProductEvent productEvent)
    {
        _currentState.Quantity -= productEvent.Quantity;
    }
    
    private void Apply(ProductReceivedProductEvent productEvent)
    {
        _currentState.Quantity += productEvent.Quantity;
    }
    
    private void Apply(InventoryAdjustedProductEvent productEvent)
    {
        _currentState.Quantity = productEvent.Quantity;
    }

    public IList<IProductEvent> GetUncommitedEvents()
    {
        return new List<IProductEvent>(_uncommitedEvents);
    }
    
    public IList<IProductEvent> GetAllEvents()
    {
        return new List<IProductEvent>(_allEvents);
    }

    public void CommitEvents()
    {
        _uncommitedEvents.Clear();
    }

    public int GetQuantity() => _currentState.Quantity;
}