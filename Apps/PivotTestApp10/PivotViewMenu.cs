
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
    MenuItem mnuAddToRows;
    MenuItem mnuAddToColumns;
    MenuItem mnuAddToValues;
    Separator sepAdd;
    MenuItem mnuPivotDefDialog;
    MenuItem mnuShowSubtotals;
    MenuItem mnuShowGrandTotals;
    MenuItem mnuShowValuesOnRows;
    MenuItem mnuRepeatRowHeaders;
    

    private PivotDef PivotDef => PivotView.PivotDef;
    private DataGridColumn SelectedColumn;
    private PivotFieldDef SelectedFieldDef => SelectedColumn != null? PivotView.GetFieldDef(SelectedColumn) : null;
  
    // ● event handlers
    private async void AnyClick(object sender, RoutedEventArgs ea)
    {
        if (mnuPivotDefDialog == sender)
            await ShowPivotDefDialog();
        else if (mnuShowSubtotals== sender)
            ShowSubtotalsChanged();
        else if (mnuShowGrandTotals == sender)
            ShowGrandTotalsChanged();
        else if (mnuShowValuesOnRows == sender)
            ShowValuesOnRowsChanged();
        else if (mnuRepeatRowHeaders == sender)
            RepeatRowHeaders();
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

    void ShowSubtotalsChanged()
    {
        PivotDef.ShowSubtotals = mnuShowSubtotals.IsChecked;
        PivotView.Refresh();
    }
    void ShowGrandTotalsChanged()
    {
        PivotDef.ShowGrandTotals = mnuShowGrandTotals.IsChecked;
        PivotView.Refresh();
    }
    void ShowValuesOnRowsChanged()
    {
        PivotDef.ShowValuesOnRows = mnuShowValuesOnRows.IsChecked;
        PivotView.Refresh();
    }
    void RepeatRowHeaders()
    {
        PivotDef.RepeatRowHeaders = mnuRepeatRowHeaders.IsChecked;
        PivotView.Refresh();
        
    }

    // ● private
    void CreateContextMenu()
    {
        mnuContextMenu = new ContextMenu();
   
        mnuAddToRows = mnuContextMenu.Items.AddMenuItem("Add to Rows", null);
        mnuAddToColumns = mnuContextMenu.Items.AddMenuItem("Add to Columns", null);
        mnuAddToValues = mnuContextMenu.Items.AddMenuItem("Add to Values", null);
        sepAdd = mnuContextMenu.Items.AddSeparator();
        mnuShowSubtotals = mnuContextMenu.Items.AddCheckBoxMenuItem("Show Subtotals", false, AnyClick);
        mnuShowGrandTotals = mnuContextMenu.Items.AddCheckBoxMenuItem("Show Grand Totals", false, AnyClick);
        mnuShowValuesOnRows = mnuContextMenu.Items.AddCheckBoxMenuItem("Show Values on Rows", false, AnyClick);
        mnuRepeatRowHeaders = mnuContextMenu.Items.AddCheckBoxMenuItem("Repeat Row Headers", false, AnyClick);
        mnuContextMenu.Items.AddSeparator();
        mnuPivotDefDialog = mnuContextMenu.Items.AddMenuItem("Edit Pivot", AnyClick);
    }
    void UpdateContextMenu()
    {
        if (SelectedFieldDef != null)
        {
            string Caption = SelectedFieldDef.Caption;
            
            if (PivotDef.CanBeRow(SelectedFieldDef))
                mnuAddToRows.Header = $"Add to Rows [{Caption}]";
            else
                mnuAddToRows.IsVisible = false;
            
            if (PivotDef.CanBeColumn(SelectedFieldDef))
                mnuAddToColumns.Header = $"Add to Columns [{Caption}]";
            else
                mnuAddToColumns.IsVisible = false;
            
            if (PivotDef.CanBeValue(SelectedFieldDef))
                mnuAddToValues.Header = $"Add to Values [{Caption}]";
            else
                mnuAddToValues.IsVisible = false;

            sepAdd.IsVisible = mnuAddToRows.IsVisible || mnuAddToColumns.IsVisible || mnuAddToValues.IsVisible;
        }
        else
        {
            mnuAddToRows.IsVisible = false;
            mnuAddToColumns.IsVisible = false;
            mnuAddToValues.IsVisible = false;
            sepAdd.IsVisible = false;
        }
        
        
        mnuShowSubtotals.IsChecked = PivotDef.ShowSubtotals;
        mnuShowGrandTotals.IsChecked = PivotDef.ShowGrandTotals;
        mnuShowValuesOnRows.IsChecked = PivotDef.ShowValuesOnRows;
    }
    void GridColumnListChanged()
    {
    }
 
    async Task ShowPivotDefDialog()
    {
        
        PivotDef PivotDef2 = PivotDef.Clone();
        PivotDef2.IsNameReadOnly = true;
        
        DialogData data = await DialogWindow.ShowModal<PivotDefDialog>(PivotDef2);
        if (data.Result)
        {
            // ΕΔΩ
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