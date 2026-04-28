namespace Tripous.Data;

public class LookupSource  : IDef// ILookupSource
{
    // ● construction  
    public LookupSource(LookupDef LookupDef)
    {
        this.LookupDef = LookupDef ?? throw new ArgumentNullException(nameof(LookupDef));
    }
    
    // ● public  
    public void Select()
    {
        List.Clear();
        if (!string.IsNullOrWhiteSpace(LookupDef.SqlText))
        {
            SqlStore Store = SqlStores.CreateSqlStore(LookupDef.ConnectionName);
            DataTable Table = Store.Select(LookupDef.SqlText);
            LoadForm(Table);
        }
    }
    public void LoadForm(DataTable Table)
    {
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
    public void LoadFrom(Enum Enum)
    {
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
    static public LookupSource GetLookupSource(string Name) => DataRegistry.LookupSources.Get(Name);
 
 
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