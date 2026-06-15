/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Holds the shared state required while building and binding an item page UI.
/// </summary>
public class UiItemContext
{
    DataModule fModule;
    int fColumnCount = 2;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public UiItemContext()
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
    /// The grid handler.
    /// </summary>
    public IGridHandler GridHandler { get; set; }
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
            
            TopTableUiInfo = UiItemInfo.CreateTopTableUiInfo(Module);
            ItemBinder.RowProvider = GetRowProvider(fModule.ModuleDef.Table);
            ItemBinder.TableInfo = TopTableUiInfo;
        }
    }
    /// <summary>
    /// The definition of the data module.
    /// </summary>
    public ModuleDef ModuleDef => Module.ModuleDef;
    /// <summary>
    /// Provides access to multiple <see cref="IRowProvider"/>.
    /// <para>Useful when multiple tables are in a one-to-one relationship, such as a Trade, a StoreTrade and a FinTrade table.</para>
    /// </summary>
    public IRowProviderHost RowProviderHost => Module.RowProviderHost;
    /// <summary>
    /// The control that receives the generated item page content.
    /// </summary>
    public Control ParentControl { get; set; }
    /// <summary>
    /// UI information for the top table and its visible detail tables.
    /// </summary>
    public UiTableInfo TopTableUiInfo { get; private set; }
    /// <summary>
    /// The normalized number of visual columns used by the item page layout.
    /// </summary>
    public int ColumnCount
    {
        get => fColumnCount;
        set => fColumnCount = UiItemPage.NormalizeColumnCount(value);
    }
    /// <summary>
    /// The current data row.
    /// </summary>
    public DataRow CurrentRow => ItemBinder.CurrentRow;
    /// <summary>
    /// Creates the editor control used for a field.
    /// </summary>
    public Func<FieldDef, ItemBinder, Control> CreateEditorFunc { get; set; }
}
