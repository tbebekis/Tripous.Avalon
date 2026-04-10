namespace Tripous.Avalon;

/// <summary>
/// Binds a <see cref="DataTable"/> to a ProDataGrid <see cref="Avalonia.Controls.DataGrid"/>.
/// <para>It handles the list of <see cref="DataRow"/>s of its associated table. </para>
/// </summary>
public class GridBinder: ObservableObject
{
    /* The hierarchy is
        Grid
        DataTable
        DataView
        ViewDef
     */  
    
    private DataGrid fGrid;
    private DataView fDataView;
    private GridViewDef fViewDef;
    DataRowView fCurrentRow;


    protected virtual void DataViewChanging()
    {
    }
    protected virtual void DataViewChanged()
    {
    }
    /// <summary>
    /// Triggers the associated event
    /// </summary>
    protected virtual void OnCurrentRowChanged()
    {
        CurrentRowChanged?.Invoke(this, EventArgs.Empty);
    }
 
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public GridBinder(DataGrid Grid, DataTable Table, bool UseViewMenu = false)
        : this(Grid, Table.DefaultView, UseViewMenu)
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public GridBinder(DataGrid Grid, DataView DataView, bool UseViewMenu = false)
    {
        this.Grid = Grid;
        this.DataView = DataView;
        this.IsMenuEnabled = UseViewMenu;
    }
 
    // ● attached property
    /// <summary>
    /// Defines the attached <see cref="GridBinder"/> property to the <see cref="DataGrid"/> class.
    /// </summary>
    static public readonly AttachedProperty<GridBinder> GridBinderProperty =
        AvaloniaProperty.RegisterAttached<GridBinder, DataGrid, GridBinder>("GridBinder");

    /// <summary>
    /// Returns the <see cref="GridBinder"/> of a <see cref="DataGrid"/>
    /// </summary>
    static public GridBinder GetGridBinder(DataGrid element) => element.GetValue(GridBinderProperty);
    /// <summary>
    /// Sets the <see cref="GridBinder"/> of a <see cref="DataGrid"/>
    /// </summary>
    static public void SetGridBinder(DataGrid element, GridBinder value) => element.SetValue(GridBinderProperty, value);
 
    // ● properties
    /// <summary>
    /// The grid where this instance presents the table
    /// </summary>
    public DataGrid Grid
    {
        get => fGrid;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(Grid));

            if (value != null && value != fGrid)
            {
                fDataView = null;
                fViewDef = null;
                ColumnInfoList = null;
                CollectionView = null;
                fCurrentRow = null;
                
                fGrid = value;
                fGrid.Tag = this;
                SetGridBinder(Grid, this);
                
                // CHECK
                // this.Grid.EditTriggers = DataGridEditTriggers.TextInput; 
                // this.Grid.SelectionChanged += (s, e) =>
                // {
                //     CurrentRow = Grid.SelectedItem is DataRowView? Grid.SelectedItem as DataRowView: null;
                // };
                // Menu = new ViewMenu(fGrid);
            }
        }
    }
    /// <summary>
    /// The DataTable of the DataView
    /// </summary>
    public DataTable DataTable => DataView != null ? DataView.Table : null;
    /// <summary>
    /// The table this instance presents.
    /// </summary>
    public DataView DataView
    {
        get => fDataView;
        set
        {
            if (value != null && fDataView != value)
            {
                if (Grid == null)
                    throw new ApplicationException("Cannot set DataView without a Grid");

                DataViewChanging();
                
                fViewDef = null;
                ColumnInfoList = null;
                CollectionView = null;
                fCurrentRow = null;
                
                fDataView = value;
                
                CollectionView = new DataGridCollectionView(DataView);
                Grid.ItemsSource = CollectionView;
                
                // CHECK: Grid.CreateColumns(DataTable);
                ColumnInfoList = Grid.Columns.Select(x => x.Tag as GridColumnInfo).ToList();

                fViewDef = GridViewDef.Create(DataView);  
                
                DataViewChanged();
            }
        }
    }
    /// <summary>
    /// The definition of the view, i.e. groups, summaries, etc.
    /// </summary>
    public GridViewDef ViewDef
    {
        get => fViewDef;
        set
        {
            if (value != null && fViewDef != value)
            {
                if (Grid == null)
                    throw new ApplicationException("Cannot set ViewDef without a Grid");
                if (DataView == null)
                    throw new ApplicationException("Cannot set ViewDef without a DataView");
                
                fViewDef = value;
                
                // apply
                // CHECK:Menu.Apply(ViewDef);
            }
        }
    }
    /// <summary>
    ///  A list of <see cref="DataGridColumn"/> items based on the current <see cref="DataView"/>.
    /// </summary>
    public List<GridColumnInfo> ColumnInfoList { get; private set; }
    /// <summary>
    /// The ProDataGrid <see cref="DataGridCollectionView"/> which displays the table to the grid.
    /// </summary>
    public DataGridCollectionView CollectionView { get; protected set; }
    /// <summary>
    /// The current data row in the grid, if any, else null.
    /// </summary>
    public virtual DataRowView CurrentRow
    {
        get => fCurrentRow;
        set
        {
            if (!ReferenceEquals(fCurrentRow, value))
            {
                fCurrentRow = value;
                OnCurrentRowChanged();
            }
        }
    }
    public bool IsMenuEnabled
    {
        get;
        set;
        // CHECK:
        // get => Menu != null && Menu.IsEnabled;
        // set
        // {
        //     if (Menu != null)
        //         Menu.IsEnabled = value;
        // }
    }
 

    // ● events
    /// <summary>
    /// Occurs when the current row changes in the grid, i.e. the position.
    /// </summary>
    public event EventHandler CurrentRowChanged;
}