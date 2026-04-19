namespace Tripous.Avalon;

public partial class ucGridView : UserControl
{
    // ● private
    void AfterCreate()
    {
     
        View.Grid = ViewGrid;
        View.ToolBar.Panel = pnlToolBar;
        View.SelectedDefChanged += (sender, args) => SelectedDefChanged?.Invoke(this, EventArgs.Empty);
 
    }
    
    
    // ● constructor
    public ucGridView()
    {
        InitializeComponent();
        
        if (!Design.IsDesignMode)
            AfterCreate();
    }
    
    // ● public methods
    /// <summary>
    /// Discards the ViewSource and the ViewDef.
    /// </summary>
    public void Close()
    {
        View.Close();
    }
    
    public void SetSource(DataView DataViewSource)
    {
        View.SetSource(DataViewSource);
    }
    public void SetSource<T>(IEnumerable<T> SequenceSource)
    {
        View.SetSource(SequenceSource);
    }

    public void CloseParentTabPage()
    {
        TabItem Page = this.FindLogicalAncestorOfType<TabItem>();
        if (Page != null)
        {
            TabControl Pager = Page.FindLogicalAncestorOfType<TabControl>();
            if (Pager != null)
                Pager.Items.Remove(Page);
        }
    }
    
    // ● properties
    public GridView View { get; } = new();
    public DataGrid Grid => View.Grid;
    public GridViewToolBar ToolBar => View.ToolBar;
    public GridViewMenu Menu => View.Menu;
    public GridViewDefs ViewDefs  => View.ViewDefs;
    public GridViewDef ViewDef
    {
        get => View.ViewDef;
        set
        {
            if (View.ViewDef != value)
            {
                View.ViewDef = value;
                SelectedDefChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public DataView DataView
    {
        get => View.DataView;
        set => View.DataView = value;
    }
    public int Position
    {
        get => View.Position;
        set => View.Position = value;
    }
    public ObservableCollection<GridDataRow> Rows => View.Rows;
    public bool IsEmpty => View.IsEmpty;

    //public GridViewController Controller => View.Controller;
    public GridDataRow Current => View.Current;

    //public GridViewContext Context => View.Context;
    public LookupRegistry LookupRegistry => View.LookupRegistry;

    public bool IsGridVisible
    {
        get => GridContainer.IsVisible;
        set => GridContainer.IsVisible = value;
    }
    public bool IsToolBarVisible
    {
        get => ToolBar.IsVisible;
        set => ToolBar.IsVisible = value;
    }
    public GridViewToolBarButtons VisibleButtons
    {
        get => ToolBar.VisibleButtons;
        set => ToolBar.VisibleButtons = value;
    }
    public bool IsMultiDef
    {
        get => View.ToolBar.IsMultiDef;
        set => View.ToolBar.IsMultiDef = value;
    }
    
    public bool IsMenuEnabled
    {
        get => View.Menu.IsEnabled;
        set => View.Menu.IsEnabled = value;
    }
    public bool IsReadOnlyView
    {
        get => View.ToolBar.IsReadOnlyView;
        set => View.ToolBar.IsReadOnlyView = value;
    }
    
    // ● events
    public event EventHandler SelectedDefChanged;
}