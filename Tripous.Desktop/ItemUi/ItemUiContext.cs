namespace Tripous.Desktop;

public class ItemUiContext
{
    DataModule fModule;
    int fColumnCount = 2;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemUiContext()
    {         
        Binders.Add(ItemBinder);
    }
    
    // ● public
    /// <summary>
    /// Returns the row provider of a table.
    /// </summary>
    public IRowProvider GetRowProvider(TableDef TableDef) => RowProviderHost.GetRowProvider(TableDef.Name);
    /// <summary>
    /// Creates a binder for a one-to-one detail table.
    /// </summary>
    public ItemBinder CreateOneToOneBinder(TableDef TableDef)
    {
        ItemBinder Result = new();
        Result.RowProvider = GetRowProvider(TableDef);
        return Result;
    }
 
    // ● properties
    /// <summary>
    /// The main item binder.
    /// </summary>
    public ItemBinder ItemBinder { get; } = new();
    /// <summary>
    /// The binders of this instance.
    /// </summary>
    public List<ItemBinder> Binders { get; } = new();
    /// <summary>
    /// The data module.
    /// <para>NOTE: setting the module creates the <see cref="TopTableUiInfo"/>
    /// and the <see cref="ItemBinder.RowProvider"/> too.</para>
    /// </summary>
    public DataModule Module
    {
        get => fModule;
        set
        {
            if (fModule != null)
                throw new TripousDesktopException($"{this.GetType().FullName} module is already defined.");
            if (value == null)
                throw new TripousArgumentNullException(nameof(Module));
            fModule = value;
            
            TopTableUiInfo = ItemPageUi.CreateTopTableUiInfo(Module);
            ItemBinder.RowProvider = GetRowProvider(fModule.ModuleDef.Table);
            ItemBinder.TableInfo = TopTableUiInfo;
        }
    }
    public ModuleDef ModuleDef => Module.ModuleDef;
    /// <summary>
    /// Provides access to multiple <see cref="IRowProvider"/>.
    /// <para>Useful when multiple tables are in a one-to-one relationship, such as a Trade, a StoreTrade and a FinTrade table.</para>
    /// </summary>
    public IRowProviderHost RowProviderHost => Module.RowProviderHost;
    /// <summary>
    /// The main ui container. Contains the <see cref="ParentControl"/>
    /// </summary>
    //public ContentControl ContentControl { get; set; }
    /// <summary>
    /// The parent control
    /// </summary>
    public Control ParentControl { get; set; }
    public UiTableInfo TopTableUiInfo { get; private set; }
    public int ColumnCount
    {
        get => fColumnCount;
        set => fColumnCount = ItemPageUi.NormalizeColumnCount(value);
    }
    /// <summary>
    /// The current data row.
    /// </summary>
    public DataRow CurrentRow => ItemBinder.CurrentRow;
    public Func<FieldDef, ItemBinder, Control> CreateEditorFunc { get; set; }
}