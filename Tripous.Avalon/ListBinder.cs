namespace Tripous.Avalon;


/// <summary>
/// Binds a <see cref="MemTable"/> to a ProDataGrid <see cref="Avalonia.Controls.DataGrid"/>.
/// <para>It handles the list of <see cref="DataRow"/>s of its associated table. </para>
/// <para>When its table is not a detail, it presents all table rows to the grid.  </para>
/// <para>When its table is a detail table, then it only shows rows under the <see cref="MemTable.CurrentRow"/> of a master table. </para>
/// </summary>
public class ListBinder: GridBinder
{
    // ● private
    void MasterTableChanging(object sender, EventArgs ea)
    {
        if (MasterTable != null)
            MasterTable.CurrentRowChanged -= MasterRowChanged;
    }
    void MasterTableChanged(object sender, EventArgs ea)
    {
        if (MasterTable != null)
            MasterTable.CurrentRowChanged += MasterRowChanged;

        Refresh();
    }
    void MasterRowChanged(object sender, EventArgs ea)
    {
        Refresh();
    }

    protected override void DataViewChanging()
    {
        if (this.Table.IsDetail)
            MasterTableChanging(null, null);
    }
    protected override void DataViewChanged()
    {
        MasterTableChanged(null, null);

        if (this.Table.IsDetail)
        {
            this.Table.MasterChanging += MasterTableChanging;
            this.Table.MasterChanged += MasterTableChanged;
 
            if (MasterTable != null)
                MasterTable.CurrentRowChanged += MasterRowChanged;

            Refresh();
        }
    }
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public ListBinder(DataGrid Grid, MemTable Table, bool UseGridViewHandler = false)
        : base(Grid, Table.DataView, UseGridViewHandler)
    {
    }
 
    // ● public
    /// <summary>
    /// Refreshes the associated <see cref="DataGridCollectionView"/>, the ProDataGrid ItemsSource and the associated <see cref="MemTable.CurrentRow"/>
    /// </summary>
    public void Refresh()
    {
        if (Table.IsDetail)
            Table.MasterRowChanged();

        CollectionView.Refresh();

        if (Grid.SelectedItem is DataRowView drv)
            Table.CurrentRow = drv.Row;
        else if (Table.DataView.Count > 0)
            Table.CurrentRow = Table.DataView[0].Row;
        else
            Table.CurrentRow = null;

        OnPropertyChanged(nameof(CollectionView));
    }
    
    // ● properties
    /// <summary>
    /// The table this instance presents.
    /// </summary>
    public MemTable Table => this.DataTable as MemTable;
    /// <summary>
    /// When non null, then this is a detail view, showing only rows under the <see cref="MemTable.CurrentRow"/> of the master table.
    /// </summary>
    public MemTable MasterTable => Table.IsDetail? Table.Master : null;
    /// <summary>
    /// The current data row in the grid, if any, else null.
    /// </summary>
    public override DataRowView CurrentRow
    {
        get => this.Table.CurrentRowView;
        set
        {
            if (!ReferenceEquals(this.Table.CurrentRowView, value))
            {
                this.Table.CurrentRowView = value;
                OnCurrentRowChanged();
            }
        }
    }
    
    /// <summary>
    /// True when this binds to a grid with a detail <see cref="MemTable"/>
    /// </summary>
    public bool IsDetailBinder => this.Table.IsDetail;
    /// <summary>
    /// True when this binds to a grid with a non-detail <see cref="MemTable"/>
    /// </summary>
    public bool IsListBinder => !IsDetailBinder;
}
 