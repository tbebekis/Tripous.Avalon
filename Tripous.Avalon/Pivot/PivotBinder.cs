namespace Tripous.Avalon;

public class PivotBinder: ObservableObject
{
    /* The hierarchy is
        Grid
        DataTable
        DataView
        PivotDef
        PivotData
     */  

    private DataGrid fGrid;
    private DataView fDataView;
    private PivotDef fPivotDef;
    private PivotData fPivotDataModel;
 
    // ● construction
    public PivotBinder(DataGrid Grid, DataTable Table)
        : this(Grid, new DataView(Table))
    {
    }
    public PivotBinder(DataGrid Grid, DataView DataView)
    {
        this.Grid = Grid;
        this.DataView = DataView;
    }
    
    // ● public
    public virtual void PivotDefChanged()
    {
        if (Grid != null && DataView != null && PivotDef != null)
        {
            PivotDataModel = PivotEngine.Execute(DataView, PivotDef);
        }
    }
    
    // ● attached property
    /// <summary>
    /// Defines the attached <see cref="PivotBinder"/> property to the <see cref="DataGrid"/> class.
    /// </summary>
    static public readonly AttachedProperty<PivotBinder> GridPivotBinderProperty =
        AvaloniaProperty.RegisterAttached<PivotBinder, DataGrid, PivotBinder>("GridPivotBinder");

    /// <summary>
    /// Returns the <see cref="GridBinder"/> of a <see cref="DataGrid"/>
    /// </summary>
    static public PivotBinder GetGridPivotBinderBinder(DataGrid element) => element.GetValue(GridPivotBinderProperty);
    /// <summary>
    /// Sets the <see cref="GridBinder"/> of a <see cref="DataGrid"/>
    /// </summary>
    static void SetGridPivotBinder(DataGrid element, PivotBinder value) => element.SetValue(GridPivotBinderProperty, value);
    
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
                fPivotDef = null;
                ColumnInfoList = null;
                PivotDataModel = null;
                
                fGrid = value;
                fGrid.Tag = this;
                SetGridPivotBinder(fGrid, this);
            
                Menu = new PivotMenu(fGrid);
            }
        }
    }
    /// <summary>
    /// The DataTable of the DataView
    /// </summary>
    public DataTable DataTable
    {
        get => DataView != null ? DataView.Table : null;
        set
        {
            if (value != null && value != this.DataTable)
                this.DataView = value.DefaultView;
        }
    }
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
                
                fPivotDef = null;
                PivotDataModel = null;
                
                fDataView = value;
                ColumnInfoList = fDataView.CreatePivotColumnInfoList();
                PivotDef = DataView.CreateDefaultPivotDef();
            }
        }
    }
    /// <summary>
    /// The definition of the pivot
    /// </summary>
    public PivotDef PivotDef
    {
        get => fPivotDef;
        set
        {
            if (value != null && fPivotDef != value)
            {
                if (Grid == null)
                    throw new ApplicationException("Cannot set PivotDef without a Grid");
                if (DataView == null)
                    throw new ApplicationException("Cannot set PivotDef without a DataView");
                
                PivotDataModel = null; // force to re-create the pivot data model
                fPivotDef = value;
                PivotDef.DataView = DataView;
                PivotDefChanged();
            }
        }
    }
    /// <summary>
    /// The data model the grid is bound to.
    /// </summary>
    public PivotData PivotDataModel
    {
        get => fPivotDataModel;
        set
        {
            if (fPivotDataModel != value)
            {
                fPivotDataModel = value;
                //  PivotGridRenderer.Show(Grid, fPivotDataModel);
            }
        }
    }
    /// <summary>
    ///  A list of <see cref="PivotColumnInfo"/> items based on the current <see cref="DataView"/>.
    /// </summary>
    public List<PivotColumnInfo> ColumnInfoList { get; private set; }
 
    /// <summary>
    /// Handles the UI menus, etc.
    /// </summary>
    public PivotMenu Menu { get; protected set; }
    
    
  
}