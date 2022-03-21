using Example.EventSourcing.ConsoleApp.Domain.Abstractions;

namespace Example.EventSourcing.ConsoleApp.Domain.Events;

public record InventoryAdjustedProductEvent(string Sku, int Quantity, DateTime DateTime, string Reason) : IProductEvent;