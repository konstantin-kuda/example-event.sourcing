using Example.EventSourcing.Domain;
using Example.EventSourcing.Domain.Abstractions;
using Newtonsoft.Json;

namespace Example.EventSourcing.Application;

public class WarehouseProductRepository
{
    private readonly string _filePath;

    public WarehouseProductRepository()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "DB.WarehouseProduct.json");
    }

    private async Task<Dictionary<string, List<IProductEvent>>> GetAllEventsAsync(CancellationToken cancellationToken)
    {
        if (File.Exists(_filePath))
        {
            var text = await File.ReadAllTextAsync(_filePath, cancellationToken);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var jsonOptions = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                return JsonConvert.DeserializeObject<Dictionary<string, List<IProductEvent>>>(text, jsonOptions);
            }
        }
        return new Dictionary<string, List<IProductEvent>>();
    }

    private async Task SaveAllEventsAsync(Dictionary<string, List<IProductEvent>> allEvents, CancellationToken cancellationToken)
    {
        var jsonOptions = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        var json = JsonConvert.SerializeObject(allEvents, jsonOptions);
        await File.WriteAllTextAsync(_filePath, json, cancellationToken);
    }
    
    public async Task<IEnumerable<IProductEvent>?> GetEventsAsync(string sku, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException(nameof(sku));
        }
        
        var warehouseProduct = new WarehouseProduct(sku);

        var allEvents = await GetAllEventsAsync(cancellationToken);

        if (allEvents.ContainsKey(sku))
        {
            return allEvents[sku];
        }

        return null;
    }

    public async Task<WarehouseProduct> GetAsync(string sku, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException(nameof(sku));
        }
        
        var warehouseProduct = new WarehouseProduct(sku);

        var allEvents = await GetAllEventsAsync(cancellationToken);

        if (allEvents.ContainsKey(sku))
        {
            foreach (var productEvent in allEvents[sku])
            {
                warehouseProduct.Apply(productEvent);
            }
        }

        return warehouseProduct;
    }

    public async Task SaveAsync(WarehouseProduct product, CancellationToken cancellationToken)
    {
        var allEvents = await GetAllEventsAsync(cancellationToken);
        
        List<IProductEvent> productEvents;
        if (allEvents.ContainsKey(product.Sku))
        {
            productEvents = allEvents[product.Sku];
        }
        else
        {
            productEvents = new List<IProductEvent>();
            allEvents[product.Sku] = productEvents;
        }

        productEvents.AddRange(
            product.GetUncommitedEvents()    
        );
        product.CommitEvents();

        await SaveAllEventsAsync(allEvents, cancellationToken);
    }
}