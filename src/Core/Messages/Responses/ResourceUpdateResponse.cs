using Core.Domain.Enums;

namespace Core.Messages.Responses;

public class ResourceUpdateResponse : ServerResponse
{
    public ResourceType ResourceType { get; set; }
    public int NewBalance { get; set; }
}