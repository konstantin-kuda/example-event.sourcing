using Example.EventSourcing.ConsoleApp.Domain.Abstractions;

namespace Example.EventSourcing.ConsoleApp.Domain.Events;

public record ProductShippedProductEvent(string Sku, int Quantity, DateTime DateTime) : IProductEvent;