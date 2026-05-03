namespace Tripous.Data;

/// <summary>
/// Represents a lookup list.
/// <para>Lookup list items may come from</para>
/// <list type="bullet">
/// <item>a SELECT statement, given in the <see cref="SqlText"/> property</item>
/// <item>a SELECT statement constructed using the <see cref="TableName"/></item>
/// <item>a <see cref="DataTable"/> passed to <see cref="LoadForm"/>() method</item>
/// <item>an enum type, given in the <see cref="EnumTypeName"/> property</item>
/// <item>as a last resort using the <see cref="Name"/> as a <see cref="TableName"/></item>
/// </list>
/// </summary>
public class LookupSource : BaseDef
{
    bool fUseNullItem;
    string fValueField = "Id";
    string fDisplayField = "Name";
    string fSqlText;
    string fTableName;
    string fConnectionName;
    string fEnumTypeName;
    string fZoomCommand;
    List<LookupItem> List;
    SqlStore fStore;

    SqlStore Store
    {
        get
        {
            if (fStore == null)
                fStore = SqlStores.CreateSqlStore(ConnectionName);

            return fStore;
        }
    }
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public LookupSource()
    {
    }
    
    // ● public 
    /// <summary>
    /// Fills the list using a SELECT statement
    /// </summary>
    public void Select(string SqlText)
    {
        ClearList();
        if (!string.IsNullOrWhiteSpace(SqlText))
        {
            DataTable Table = Store.Select(SqlText);
            LoadForm(Table);
        }
    }
    /// <summary>
    /// Fills the list using a <see cref="DataTable"/>
    /// </summary>
    public void LoadForm(DataTable Table)
    {
        if (Table == null)
            throw new TripousArgumentNullException(nameof(Table));

        if (string.IsNullOrWhiteSpace(ValueField) || !Table.Columns.Contains(ValueField))
            throw new TripousDataException($"Lookup: ValueField '{ValueField}' not found.");

        if (string.IsNullOrWhiteSpace(DisplayField) || !Table.Columns.Contains(DisplayField))
            throw new TripousDataException($"Lookup: DisplayField '{DisplayField}' not found.");

        ClearList();

        LookupItem LI;
        if (UseNullItem)
        {
            LI = new LookupItem(null, string.Empty, true);
            List.Add(LI);
        }
            

        foreach (DataRow Row in Table.Rows)
        {
            object Value = Row[ValueField];
            string Display = Row[DisplayField]?.ToString();
            LI = new LookupItem(Value, Display);
            List.Add(LI);
        }
    }
    /// <summary>
    /// Fills the list using an enum type.
    /// </summary>
    /// <param name="Enum"></param>
    public void LoadFrom(Enum Enum)
    {
        if (Enum == null)
            throw new TripousArgumentNullException(nameof(Enum));

        Type EnumType = Enum.GetType();
        
        if (!EnumType.IsEnum)
            throw new TripousDataException($"Type {EnumType.FullName} is not an enum type");

        ClearList();
        
        if (UseNullItem)
            List.Add(new LookupItem(null, string.Empty, true));

        foreach (var Value in System.Enum.GetValues(EnumType))
        {
            string Display = Value.ToString();
            List.Add(new LookupItem(Value, Display));
        }
    }

    /// <summary>
    /// Returns the lookup list, full of items.
    /// </summary>
    public List<LookupItem> GetList()
    {
        if (List != null)
            return List;

        if (!string.IsNullOrWhiteSpace(TableName))
        {
            Select($"select * from {TableName}");
            return List;
        }
        
        if (!string.IsNullOrWhiteSpace(SqlText))
        {
            Select(SqlText);
            return List;
        }

        if (!string.IsNullOrWhiteSpace(EnumTypeName))
        {
            Type T = Type.GetType(EnumTypeName);
            if (T == null || !T.IsEnum)
                throw new TripousDataException($"Type {EnumTypeName} is not an enum type");
 
            var value = Enum.GetValues(T).GetValue(0);
            LoadFrom((Enum)value);
            return List;
        }

        if (!string.IsNullOrWhiteSpace(ConnectionName))
        {
            if (Store.TableExists(Name))
                Select($"select * from {Name}");
            return List;
        }
        
        return List?? [];
    }
    /// <summary>
    /// Clears the lookup list.
    /// </summary>
    public void ClearList()
    {
        if (List == null)
            List = [];
        else
            List.Clear();
    }
    
    // ● properties
    /// <summary>
    /// When true then the first item in the list is a null item.
    /// </summary>
    public bool UseNullItem
    {
        get => fUseNullItem;
        set
        {
            if (fUseNullItem != value)
            {
                fUseNullItem = value;
                NotifyPropertyChanged(nameof(UseNullItem));
            }
        }
    }
    /// <summary>
    /// The field used in getting the value.
    /// <para>Used only when <see cref="TableName"/> or <see cref="SqlText"/> is defined or the list is loaded using <see cref="Select"/> or <see cref="LoadForm"/> a <see cref="DataTable"/></para>
    /// </summary>
    public string ValueField
    {
        get => fValueField;
        set
        {
            if (fValueField != value)
            {
                fValueField = value;
                NotifyPropertyChanged(nameof(ValueField));
            }
        }
    }  
    /// <summary>
    /// The field used in getting the display value.
    /// <para>Used only when <see cref="TableName"/> or <see cref="SqlText"/> is defined or the list is loaded using <see cref="Select"/> or <see cref="LoadForm"/> a <see cref="DataTable"/></para>
    /// </summary>
    public string DisplayField
    {
        get => fDisplayField;
        set
        {
            if (fDisplayField != value)
            {
                fDisplayField = value;
                NotifyPropertyChanged(nameof(DisplayField));
            }
        }
    }
    /// <summary>
    /// The connection name used in getting an <see cref="SqlStore"/> in order to execute the <see cref="SqlText"/> SELECT statement.
    /// </summary>
    public string ConnectionName  
    {
        get => !string.IsNullOrWhiteSpace(fConnectionName)? fConnectionName: SysConfig.DefaultConnectionName;
        set { if (fConnectionName != value) { fConnectionName = value; NotifyPropertyChanged(nameof(ConnectionName)); } }
    }
    /// <summary>
    /// The SELECT statement
    /// </summary>
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
    /// <summary>
    /// When not empty results in a SELECT statement like <c>select * from TableName</c>
    /// </summary>
    public string TableName
    {
        get => fTableName;
        set
        {
            if (fTableName != value)
            {
                fTableName = value;
                NotifyPropertyChanged(nameof(TableName));
            }
        }
    }
    /// <summary>
    /// An enum type used in filling the list
    /// </summary>
    public string EnumTypeName
    {
        get => fEnumTypeName;
        set
        {
            if (fEnumTypeName != value)
            {
                fEnumTypeName = value;
                NotifyPropertyChanged(nameof(EnumTypeName));
            }
        }
    }
    /// <summary>
    /// The name of a <see cref="Command"/> that displays a form displaying the table.
    /// </summary>
    public string ZoomCommand
    {
        get => fZoomCommand;
        set
        {
            if (fZoomCommand != value)
            {
                fZoomCommand = value;
                NotifyPropertyChanged(nameof(ZoomCommand));
            }
        }
    }
 
}

 