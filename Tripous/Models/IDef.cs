namespace Tripous.Models;

public interface IDef
{
    string Name { get; set; }
    string TitleKey { get; set; }
    [JsonIgnore] 
    public string Title { get; }
}