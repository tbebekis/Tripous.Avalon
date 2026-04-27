namespace Tripous.Models;

public class LookUpSource : ILookupSource
{
    // ● construction  
    public LookUpSource(LookupDef LookupDef, DataTable Table)
    {
        this.LookupDef = LookupDef ?? throw new ArgumentNullException(nameof(LookupDef));
        if (Table == null)
            throw new ArgumentNullException(nameof(Table));

        string ValueField = LookupDef.ValueField;
        string DisplayField = LookupDef.DisplayField;

        if (string.IsNullOrWhiteSpace(ValueField) || !Table.Columns.Contains(ValueField))
            throw new ApplicationException($"Lookup: ValueField '{ValueField}' not found.");

        if (string.IsNullOrWhiteSpace(DisplayField) || !Table.Columns.Contains(DisplayField))
            throw new ApplicationException($"Lookup: DisplayField '{DisplayField}' not found.");

        if (LookupDef.UseNullItem)
            List.Add(new LookupItem(null, string.Empty, true));

        foreach (DataRow Row in Table.Rows)
        {
            object Value = Row[ValueField];
            string Display = Row[DisplayField]?.ToString();
            List.Add(new LookupItem(Value, Display));
        }
    }
    public LookUpSource(LookupDef LookupDef, Enum Enum)
    {
        this.LookupDef = LookupDef ?? throw new ArgumentNullException(nameof(LookupDef));
        if (Enum == null)
            throw new ArgumentNullException(nameof(Enum));

        Type EnumType = Enum.GetType();

        if (LookupDef.UseNullItem)
            List.Add(new LookupItem(null, string.Empty, true));

        foreach (var Value in System.Enum.GetValues(EnumType))
        {
            string Display = Value.ToString();
            List.Add(new LookupItem(Value, Display));
        }
    }
    
    // ● static  
    static public ILookupSource GetLookupSource(string Name)
    {
        ILookupSource Result = null;

        if (!string.IsNullOrWhiteSpace(Name))
            Result = Registry.LookupSources.Find(Name);

        if (Result == null)
            throw new ApplicationException($"{typeof(LookUpSource)} {Name} not registered.");

        return Result;
    }
 
    // ● properties  
    public string Name
    {
        get => LookupDef.Name;
        set { }
    }
    public string TitleKey
    {
        get => LookupDef.TitleKey;
        set { }
    }
    [JsonIgnore]
    public string Title => LookupDef.Title;
    public LookupDef LookupDef { get; }
    public List<LookupItem> List { get; } = new();
}