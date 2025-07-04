namespace Core.Messages.Responses;

public class SuccessResponse : ServerResponse
{
    public string Message { get; set; } = "Success";
}