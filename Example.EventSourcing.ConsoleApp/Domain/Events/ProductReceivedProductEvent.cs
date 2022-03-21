using Example.EventSourcing.ConsoleApp.Domain.Abstractions;

namespace Example.EventSourcing.ConsoleApp.Domain.Events;

public record ProductReceivedProductEvent(string Sku, int Quantity, DateTime DateTime) : IProductEvent;