
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Tripous.Data;

namespace Tripous.Avalon;

public class PivotViewMenu
{
    bool fIsEnabled;
    
    ContextMenu mnuContextMenu;
    
    MenuItem mnuShowSubtotals;
    MenuItem mnuShowGrandTotals;
    MenuItem mnuRepeatRowHeaders;
    MenuItem mnuShowDefDialog;

    private PivotViewDef ViewDef => PivotView.ViewDef;
    private DataGridColumn SelectedColumn;
    private PivotFieldDef SelectedFieldDef => SelectedColumn != null? PivotView.GetFieldDef(SelectedColumn) : null;
  
    // ● event handlers
    private async void AnyClick(object sender, RoutedEventArgs ea)
    {
        if (mnuShowSubtotals== sender)
            ShowSubtotalsChanged();
        else if (mnuShowGrandTotals == sender)
            ShowGrandTotalsChanged();
        else if (mnuRepeatRowHeaders == sender)
            RepeatRowHeaders();
        
        else if (mnuShowDefDialog == sender)
            await ShowDefDialog();
    }
    private void Grid_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var props = e.GetCurrentPoint(Grid).Properties;
        if (!props.IsRightButtonPressed) 
            return;
        
        Point P = e.GetPosition(Grid);
        DataGridColumn GridColumn = null;

        IInputElement Hit = Grid.InputHitTest(P);
        Control Ctrl = Hit as Control;

        while (Ctrl != null && GridColumn == null)
        {
            GridColumn = DataGridColumn.GetColumnContainingElement(Ctrl);
            Ctrl = Ctrl.Parent as Control;
        }

        SelectedColumn = GridColumn;
        if (SelectedColumn != null)
        {
            mnuContextMenu.Placement = PlacementMode.Pointer;
            UpdateContextMenu();
            mnuContextMenu.Open(Grid);
            e.Handled = true;
        }
    }

    // ● private
    void ShowSubtotalsChanged()
    {
        ViewDef.ShowSubtotals = mnuShowSubtotals.IsChecked;
        PivotView.Refresh();
    }
    void ShowGrandTotalsChanged()
    {
        ViewDef.ShowGrandTotals = mnuShowGrandTotals.IsChecked;
        PivotView.Refresh();
    }
    void RepeatRowHeaders()
    {
        ViewDef.RepeatRowHeaders = mnuRepeatRowHeaders.IsChecked;
        PivotView.Refresh();
        
    }
    
    void CreateContextMenu()
    {
        mnuContextMenu = new ContextMenu();

        mnuShowSubtotals = mnuContextMenu.Items.AddCheckBoxMenuItem("Show Subtotals", false, AnyClick);
        mnuShowGrandTotals = mnuContextMenu.Items.AddCheckBoxMenuItem("Show Grand Totals", false, AnyClick);
        mnuRepeatRowHeaders = mnuContextMenu.Items.AddCheckBoxMenuItem("Repeat Row Headers", false, AnyClick);
        mnuContextMenu.Items.AddSeparator();
        mnuShowDefDialog = mnuContextMenu.Items.AddMenuItem("Edit Pivot", AnyClick);
    }
    void UpdateContextMenu()
    {
        mnuShowSubtotals.IsChecked = ViewDef.ShowSubtotals;
        mnuShowGrandTotals.IsChecked = ViewDef.ShowGrandTotals;
    }
    void GridColumnListChanged()
    {
    }
 
    async Task ShowDefDialog()
    {
        PivotView.UpdateDataTypes(ViewDef);
        
        PivotViewDef PivotViewDef2 = ViewDef.Clone();
        PivotViewDef2.IsNameReadOnly = true;
        
        DialogData data = await DialogWindow.ShowModal<PivotDefDialog>(PivotViewDef2);
        if (data.Result)
        {
            ViewDef.AssignFrom(PivotViewDef2);
            PivotView.Refresh();
        }
            
        await Task.CompletedTask;
    }



    // ● construction
    public PivotViewMenu()
    {
    }

    // ● public
 
 
    
    // ● properties
    public PivotView PivotView { get; set; }
    public DataGrid Grid => PivotView != null ? PivotView.Grid : null;
    public bool IsEnabled 
    {
        get => fIsEnabled;
        set
        {
            if (fIsEnabled != value)
            {
                fIsEnabled = value;
                if (value)
                {
                    Grid.IsReadOnly = true;
                    Grid.CanUserReorderColumns = true;
                    Grid.CanUserResizeColumns = true;

                    if (mnuContextMenu == null) 
                    {
                        CreateContextMenu();
                        Grid.AddHandler(InputElement.PointerPressedEvent, Grid_PointerPressed,  RoutingStrategies.Bubble, true);
                    }
                    
                    GridColumnListChanged();
                }
                else
                {
                    Grid.RemoveHandler(InputElement.PointerPressedEvent, Grid_PointerPressed);
                }
            }
        }
    }
    
}