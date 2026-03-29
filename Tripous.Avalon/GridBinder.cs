namespace Tripous.Avalon;

/// <summary>
/// Binds a <see cref="DataTable"/> to a ProDataGrid <see cref="Avalonia.Controls.DataGrid"/>.
/// <para>It handles the list of <see cref="DataRow"/>s of its associated table. </para>
/// </summary>
public class GridBinder: ObservableObject
{
    DataRowView fCurrentRow;

    /// <summary>
    /// Creates the columns of the grid
    /// </summary>
    protected virtual void CreateGridColumns()
    {
        Grid.CreateColumns(this.DataView.Table);
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
    public GridBinder(DataGrid Grid, DataTable Table, bool UseGridViewHandler = false)
    {
        SetGrid(Grid, Table.DefaultView, UseGridViewHandler);
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public GridBinder(DataGrid Grid, DataView DataView, bool UseGridViewHandler = false)
    {
        SetGrid(Grid, DataView, UseGridViewHandler);
    }
 
    // ● attached property
    /// <summary>
    /// Defines the attached <see cref="GridBinder"/> property to the <see cref="DataGrid"/> class.
    /// </summary>
    public static readonly AttachedProperty<GridBinder> GridBinderProperty =
        AvaloniaProperty.RegisterAttached<GridBinder, DataGrid, GridBinder>("GridBinder");

    /// <summary>
    /// Returns the <see cref="GridBinder"/> of a <see cref="DataGrid"/>
    /// </summary>
    public static GridBinder GetGridBinder(DataGrid element) => element.GetValue(GridBinderProperty);
    /// <summary>
    /// Sets the <see cref="GridBinder"/> of a <see cref="DataGrid"/>
    /// </summary>
    public static void SetGridBinder(DataGrid element, GridBinder value) => element.SetValue(GridBinderProperty, value);
    
    // ● public
    public virtual void SetGrid(DataGrid Grid, DataTable Table, bool UseGridViewHandler = false)
    {
        SetGrid(Grid, Table.DefaultView, UseGridViewHandler);
    }
    public virtual void SetGrid(DataGrid Grid, DataView DataView, bool UseGridViewHandler = false)
    {
        this.DataView = DataView ?? throw new ArgumentNullException(nameof(DataView));
        this.Grid = Grid ?? throw new ArgumentNullException(nameof(Grid));
        this.Grid.Tag = this;
        GridBinder.SetGridBinder(Grid, this);

        this.DataTable = DataView.Table;
 
        CollectionView = new DataGridCollectionView(DataView);
        this.Grid.ItemsSource = CollectionView;

        CreateGridColumns();

        if (UseGridViewHandler)
            GridViewHandler = new DataGridViewHandler(Grid);

        this.Grid.EditTriggers = DataGridEditTriggers.TextInput; 
        this.Grid.SelectionChanged += (s, e) =>
        {
            if (this.Grid.SelectedItem is DataRowView DRV)
                this.CurrentRow = DRV;
            else
                this.CurrentRow = null;
        };
        
        // NOT WORKING
        this.Grid.BeginningEdit += (s,e) =>
        {
            /*
            if (e.EditingEventArgs is TextInputEventArgs TextArgs)
            {
                TextArgs.Text = "";
                var targetColumn = e.Column;

                Dispatcher.UIThread.Post(() =>
                {
                    Grid.CurrentColumn = targetColumn;
                }, DispatcherPriority.Input);
            }
            */
        };
    }

    
    
    // ● properties
    public DataTable DataTable { get; protected set; }
    /// <summary>
    /// The table this instance presents.
    /// </summary>
    public DataView DataView { get; protected set; }
    /// <summary>
    /// The grid where this instance presents the table
    /// </summary>
    public DataGrid Grid { get; protected set; }
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
    /// <summary>
    /// Handles a <see cref="Avalonia.Controls.DataGrid"/>
    /// by displaying a context menu with items
    /// for handling groups, summmaries, filters and column visibility.
    /// </summary>
    public DataGridViewHandler GridViewHandler { get; protected set; }

    // ● events
    /// <summary>
    /// Occurs when the current row changes in the grid, i.e. the position.
    /// </summary>
    public event EventHandler CurrentRowChanged;
}