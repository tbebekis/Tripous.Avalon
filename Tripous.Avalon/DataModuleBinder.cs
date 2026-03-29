namespace  Tripous.Avalon;
 
public class DataModuleBinder
{
    // ● private
    private readonly List<ListBinder> fDetails = new();

    // ● construction
    public DataModuleBinder(DataModule Module)
    {
        this.Module = Module ?? throw new ArgumentNullException(nameof(Module));
        ItemBinder = new ItemBinder(this.Module.tblItem);
    }

    // ● public
    public ListBinder BindList(DataGrid Grid)
    {
        ListBinder = new ListBinder(Grid, Module.tblList);
        return ListBinder;
    }
    public ListBinder BindDetail(DataGrid Grid, MemTable DetailTable)
    {
        var Adapter = new ListBinder(Grid, DetailTable);
        fDetails.Add(Adapter);
        return Adapter;
    }

    // ● bind - simple controls
    /// <summary>
    /// Binds a TextBox to a specific property of the current record.
    /// </summary>
    public void Bind(TextBox Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds a normal ComboBox to the current record.
    /// </summary>
    public void Bind(ComboBox Box, string PropertyName, object[] ItemsSource = null)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds a CheckBox to a boolean property of the current record.
    /// </summary>
    public void Bind(CheckBox Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds a ToggleSwitch to a boolean property of the current record.
    /// </summary>
    public void Bind(ToggleSwitch Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds a DatePicker to a date property of the current record.
    /// </summary>
    public void Bind(DatePicker Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds a NumericUpDown to a numeric property of the current record.
    /// </summary>
    public void Bind(NumericUpDown Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds a MaskedTextBox to a string property of the current record.
    /// </summary>
    public void Bind(MaskedTextBox Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds an AutoCompleteBox to a string property of the current record.
    /// </summary>
    public void Bind(AutoCompleteBox Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }
    /// <summary>
    /// Binds a RadioButton to a boolean property of the current record.
    /// </summary>
    public void Bind(RadioButton Box, string PropertyName)
    {
        ItemBinder.Bind(Box, PropertyName);
    }

    // ● bind - lookup controls
    /// <summary>
    /// Binds a ListBox as a lookup control.
    /// </summary>
    public void Bind(ListBox Box, MemTable LookupTable, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        ItemBinder.Bind(Box,  LookupTable, DisplayMember, ValueMember, TargetPropertyName);
    }
    /// <summary>
    /// Binds a ComboBox as a lookup control.
    /// </summary>
    public void Bind(ComboBox Box, MemTable LookupTable, string DisplayMember, string ValueMember, string TargetPropertyName)
    {
        ItemBinder.Bind(Box,  LookupTable, DisplayMember, ValueMember, TargetPropertyName);
    }    
    
    public MemTable GetTable(string TableName)
    {
        return Module.GetTable(TableName);
    }
    public bool TableExists(string TableName)
    {
        return Module.TableExists(TableName);
    }
    public MemTable FindTable(string TableName)
    {
        return Module.FindTable(TableName);
    }

    public ListBinder GetPresenter(string Name)
    {
        ListBinder Result = FindPresenter(Name);
        if (Result == null)
            throw new ApplicationException($"{nameof(ListBinder)} {Name} not found");
        return Result;
    }
    public bool PresenterExists(string Name)
    {
        return FindPresenter(Name) != null;
    }
    public ListBinder FindPresenter(string Name)
    {
        foreach (ListBinder Item in Details)
            if (string.Compare(Name, Item.Table.TableName, true) == 0)
                return Item;
        return null;
     
    }
    
    public DataRow InsertDetail(MemTable Table)
    {
        ListBinder binder = fDetails.FirstOrDefault(x => x.Table == Table);

        if (binder == null)
            throw new ApplicationException($"No {nameof(ListBinder)} found for table '{Table.TableName}'.");

        DataRow Row = Table.AddNewRow();
        binder.Refresh();
        Table.CurrentRow = Row;

        return Row;
    }
    
    // ● properties
    public DataModule Module { get; }
    public ItemBinder ItemBinder { get; }
    public ListBinder ListBinder { get; private set; }
    public IReadOnlyList<ListBinder> Details => fDetails;
    public MemTable this[string TableName] => Module.GetTable(TableName);
    public bool DetailsActive
    {
        get => Module.DetailsActive;
        set => Module.DetailsActive = value;
    }

    public DataRow CurrentRow => ItemBinder.CurrentRow;
    public bool HasRow => ItemBinder.HasRow;
    
}