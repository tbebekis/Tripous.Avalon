namespace Tripous.Models;

public interface ILookupSource: IDef
{
    // ● properties
    LookupDef LookupDef { get; }
    List<LookupItem> List { get; }
}