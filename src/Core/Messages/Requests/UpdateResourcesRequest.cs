namespace Core.Messages.Requests;

public class UpdateResourcesRequest
{
    public ResourceType ResourceType { get; set; }
    public int ResourceValue { get; set; }
}