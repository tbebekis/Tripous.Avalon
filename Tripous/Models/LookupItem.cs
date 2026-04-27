namespace Tripous.Models;

public class LookupItem
{
    // ● construction
    public LookupItem(object Value, string DisplayText, bool IsNullItem = false)
    {
        this.Value = Value;
        this.DisplayText = DisplayText;
        this.IsNullItem = IsNullItem;
    }

    // ● public
    public override string ToString() =>  DisplayText ?? string.Empty;

    // ● properties
    public object Value { get; }
    public string DisplayText { get; }
    public bool IsNullItem { get; }
}