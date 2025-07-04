using Core.Domain.Enums;

namespace Core.Domain.Exceptions;

public class InsufficientResourcesException : Exception
{
    public ResourceType ResourceType { get; }

    public InsufficientResourcesException(
        ResourceType resourceType,
        string message) : base(message)
    {
        ResourceType = resourceType;
    }
}