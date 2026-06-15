/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Displays a read-only SQL view using a filter sidebar and a grid.
/// </summary>
public partial class ReadOnlyViewForm : AppForm
{
    // ● private fields
    SelectDef fSelectDef;
    ToolBar fToolBar;
    SqlFilterPanelHandler fFilterPanelHandler;
    bool fFiltersSideBarVisible = true;

    // ● private methods
    void CreateToolBar()
    {
        fToolBar = new();
        fToolBar.Panel = pnlToolBar;
        fToolBar.AddButton("table_refresh.png", "Refresh", async () => await RefreshView());
        fToolBar.AddButton("find.png", "Toggle Filters", ToggleFilters);
        fToolBar.AddButton("textfield_clear.png", "Clear Filters", async () => await ClearFilters());
        fToolBar.AddSeparator();
        fToolBar.AddButton("door_out.png", "Close", CloseForm);
    }
    void ToggleFilters()
    {
        FiltersSideBarVisible = !FiltersSideBarVisible;
    }
    async Task ClearFilters()
    {
        fFilterPanelHandler.Clear();
        await RefreshView();
    }
    async Task RefreshView()
    {
        if (fSelectDef == null)
            return;

        string SqlText = fSelectDef.SqlText;
        string Where = fFilterPanelHandler.GetWhere();
        if (!string.IsNullOrWhiteSpace(Where))
            SqlText = $"select * from ({SqlText}) X where {Where}";

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DataGridBinder.UnBindGrid(grid);
            MemTable Table = AppHost.Store.Select(SqlText);
            DataGridBinder.BindGrid(fSelectDef, grid, Table.DataView, SupportsRecycling: false, GoToFirst: true);
            string Message = $"{fSelectDef.Title} - Rows: {Table.Rows.Count}";
            AppHost.Log(Message);
            if (Table.Rows.Count == 0)
                AppHost.Log($"{fSelectDef.Title} returned no rows.");
        });
    }
    void ApplyFiltersSideBarVisible()
    {
        bool HasFilters = fSelectDef != null && fSelectDef.FilterDefs.Count > 0;
        bool IsVisible = HasFilters && fFiltersSideBarVisible;
        pnlSideBar.IsVisible = IsVisible;
        Splitter.IsVisible = IsVisible;
        pnlMain.ColumnDefinitions[0].Width = IsVisible ? new GridLength(250) : new GridLength(0);
        pnlMain.ColumnDefinitions[0].MinWidth = pnlMain.ColumnDefinitions[0].Width.Value;
        pnlMain.ColumnDefinitions[1].Width = IsVisible ? new GridLength(4) : new GridLength(0);
    }

    // ● protected methods
    /// <summary>
    /// Reads the view definition from the form context.
    /// </summary>
    protected override void Setup()
    {
        fSelectDef = Context.Tag as SelectDef;
        if (fSelectDef == null)
            throw new TripousException($"{nameof(ReadOnlyViewForm)} requires a {nameof(SelectDef)} in the form context tag.");

        TitleText = fSelectDef.Title;
    }
    /// <summary>
    /// Initializes the form controls.
    /// </summary>
    protected override void FormInitialize()
    {
        CreateToolBar();
        fFilterPanelHandler = new(pnlFilters);
        fFilterPanelHandler.CreateFilterControls(fSelectDef.FilterDefs);
        ApplyFiltersSideBarVisible();
    }
    /// <summary>
    /// Executes the first view refresh.
    /// </summary>
    protected override async Task Start()
    {
        await RefreshView();
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public ReadOnlyViewForm()
    {
        InitializeComponent();
    }

    // ● properties
    /// <summary>
    /// Gets or sets a value indicating whether the filters sidebar is visible.
    /// </summary>
    public bool FiltersSideBarVisible
    {
        get => fFiltersSideBarVisible;
        set
        {
            if (fFiltersSideBarVisible != value)
            {
                fFiltersSideBarVisible = value;
                ApplyFiltersSideBarVisible();
            }
        }
    }
}
