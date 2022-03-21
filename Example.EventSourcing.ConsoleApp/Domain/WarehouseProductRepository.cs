using Example.EventSourcing.ConsoleApp.Domain.Abstractions;

namespace Example.EventSourcing.ConsoleApp.Domain;

public class WarehouseProductRepository
{
    private readonly IDictionary<string, List<IProductEvent>> _inMemoryStreams = new Dictionary<string, List<IProductEvent>>();

    public WarehouseProduct Get(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException(nameof(sku));
        }
        
        var warehouseProduct = new WarehouseProduct(sku);

        if (_inMemoryStreams.ContainsKey(sku))
        {
            foreach (var productEvent in _inMemoryStreams[sku])
            {
                warehouseProduct.Apply(productEvent);
            }
        }

        return warehouseProduct;
    }
}