using Example.EventSourcing.Domain.Abstractions;

namespace Example.EventSourcing.Domain.Events;

public record ProductShippedProductEvent(string Sku, int Quantity, DateTime DateTime) : IProductEvent;