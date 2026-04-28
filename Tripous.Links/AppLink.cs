namespace Tripous;

public abstract class AppLink
{
    // ● construction
    public AppLink()
    {
    }

    // ● public
    public abstract Task Initialize();

    public abstract Task<AppLinkResponse> Execute(AppLinkRequest Request);
    public virtual async Task<AppLinkResponse> Execute(string Name, object Params, string RequestId = null)
    {
        AppLinkRequest Request = new();
        Request.Id = !string.IsNullOrWhiteSpace(RequestId) ? RequestId : Sys.GenId(UseBrackets: false);
        Request.Name = Name;
        Request.Params = Params;
        
        AppLinkResponse Response = await  Execute(Request);
        return Response;
    }
    
    // ● properties
    public virtual bool IsInitialized { get; protected set; }
}