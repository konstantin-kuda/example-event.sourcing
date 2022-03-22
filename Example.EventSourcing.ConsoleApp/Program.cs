using Cocona;
using Example.EventSourcing.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


var builder = CoconaApp.CreateBuilder();
builder.Logging.AddDebug();
builder.Services.AddTransient<WarehouseProductRepository>();

var app = builder.Build();

app.AddCommand("ship", async ([Option] string sku, [Option] int quantity, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    product.ShipProduct(quantity);
    await repository.SaveAsync(product, ctx.CancellationToken);
    
    logger.LogInformation("Product Sku: {Sku}. Shipped {Quantity} item(s). Current quantity: {CurrentQuantity}", product.Sku, quantity, product.GetQuantity());
});

app.AddCommand("receive", async ([Option] string sku, [Option] int quantity, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    product.ReceiveProduct(quantity);
    await repository.SaveAsync(product, ctx.CancellationToken);
    
    logger.LogInformation("Product Sku: {Sku}. Received {Quantity} item(s). Current quantity: {CurrentQuantity}", product.Sku, quantity, product.GetQuantity());
});

app.AddCommand("setQuantity", async ([Option] string sku, [Option] int quantity, [Option] string reason, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    product.AdjustInventory(quantity, reason);
    await repository.SaveAsync(product, ctx.CancellationToken);
    
    logger.LogInformation("Product Sku: {Sku}. Adjusted quantity to {Quantity} item(s). Current quantity: {CurrentQuantity}", product.Sku, quantity, product.GetQuantity());
});

app.AddCommand("getQuantity", async ([Option] string sku, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    logger.LogInformation("Product Sku: {Sku}. Current quantity: {CurrentQuantity}", product.Sku, product.GetQuantity());
});

app.AddCommand("getEvents", async ([Option] string sku, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var events = await repository.GetEventsAsync(sku, ctx.CancellationToken);
    if (events != null)
    {
        logger.LogInformation("Events of Product Sku: {Sku}: ", sku);
        
        var jsonOptions = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        foreach (var @event in events)
        {
            var json = JsonConvert.SerializeObject(@event, jsonOptions);
            logger.LogInformation("{ProductEventType}: {ProductEventJson}", @event.GetType().FullName, json);
        }
    }
    else
    {
        logger.LogInformation("Product Sku: {Sku}. No events", sku);
    }
});

app.Run();