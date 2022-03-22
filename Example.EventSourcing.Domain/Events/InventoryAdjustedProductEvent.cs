using Example.EventSourcing.Domain.Abstractions;

namespace Example.EventSourcing.Domain.Events;

public record InventoryAdjustedProductEvent(string Sku, int Quantity, DateTime DateTime, string Reason) : IProductEvent;