namespace Tripous.Data;

public class SelectDef: BaseDef
{
    private string fSqlText;
    private Dictionary<string, string> fDisplayLabels;

    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(this.SqlText))
            Sys.Throw(Texts.GS($"E_{typeof(SelectDef)}_NoSql", $"{typeof(SelectDef)} must have an SQL statement"));
    }
    
    // ● properties
    public string SqlText
    {
        get => fSqlText;
        set
        {
            if (fSqlText != value)
            {
                fSqlText = value;
                NotifyPropertyChanged(nameof(SqlText));
            }
        }
    }
    public Dictionary<string, string> DisplayLabels
    {
        get => fDisplayLabels ??= new();
        set
        {
            if (fDisplayLabels != value)
            {
                fDisplayLabels = value;
                NotifyPropertyChanged(nameof(DisplayLabels));
            }
        }
    }
    [JsonIgnore]
    public object Owner { get; set; }
}