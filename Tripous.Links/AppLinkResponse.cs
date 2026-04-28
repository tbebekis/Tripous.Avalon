namespace Tripous;

public class AppLinkResponse
{
    public string RequestId { get; set; }
    public bool Result { get; set; } = false;
    public string Message { get; set; } = "";
    public object Data { get; set; }
}