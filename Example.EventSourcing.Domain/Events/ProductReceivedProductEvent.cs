using Example.EventSourcing.Domain.Abstractions;

namespace Example.EventSourcing.Domain.Events;

public record ProductReceivedProductEvent(string Sku, int Quantity, DateTime DateTime) : IProductEvent;