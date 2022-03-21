namespace Example.EventSourcing.ConsoleApp.Domain.Exceptions;

public class InvalidDomainException : Exception
{
    public InvalidDomainException(string? message) : base(message)
    {
    }
}