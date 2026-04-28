namespace Tripous;

public class AppLinkRequest
{
    public string Id { get; set; } = Sys.GenId(UseBrackets: false);
    public string Name { get; set; }
    public object Params { get; set; }
}