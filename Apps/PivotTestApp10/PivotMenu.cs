using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Tripous.Data;

namespace Tripous.Avalon;

public class PivotMenu
{
    private ContextMenu mnuContextMenu;
    private MenuItem mnuAddToRows;
    private MenuItem mnuAddToColumns;
    private MenuItem mnuAddToValues;
    private MenuItem mnuRemoveAll;
    private MenuItem mnuPivot;

    private PivotBinder PivotBinder;
    private PivotDef PivotDef => PivotBinder.PivotDef;
  
    // ● event handlers
    private async void AnyClick(object sender, RoutedEventArgs ea)
    {
        if (mnuRemoveAll == sender)
            RemoveAll();
        else if (mnuPivot == sender)
            await ShowPivotDefDialog();
    }

    // ● private
    void CreateContextMenu()
    {
        mnuContextMenu = new ContextMenu();
   
        mnuAddToRows = mnuContextMenu.Items.AddMenuItem("Add to Rows", null);
        mnuAddToColumns = mnuContextMenu.Items.AddMenuItem("Add to Columns", null);
        mnuAddToValues = mnuContextMenu.Items.AddMenuItem("Add to Values", null);
        mnuRemoveAll = mnuContextMenu.Items.AddMenuItem("Remove All", AnyClick);
        mnuPivot = mnuContextMenu.Items.AddMenuItem("Show Config", AnyClick);
 
        // CHECK: Grid.ColumnHeaderContextMenu = mnuContextMenu;
        //--------------------------------------------
        mnuContextMenu.Opening += (sender, args) =>
        {
            //PivotDef
        };
    }
    void RemoveAll()
    {
        
    }
    async Task ShowPivotDefDialog()
    {
        DialogData data = await DialogWindow.ShowModal<PivotDefDialog>(PivotBinder.PivotDef);
        if (data.Result)
        {
            // ΕΔΩ
        }
            
        await Task.CompletedTask;
    }
    
    // ● construction
    public PivotMenu(DataGrid Grid)
    {
        this.Grid = Grid;

        // CHECK: PivotBinder = PivotBinder.GetGridPivotBinderBinder(Grid);
        
        Grid.IsReadOnly = true;
        // CHECK: Grid.CanUserPaste = false;
        // CHECK: Grid.CanUserAddRows = false;
        // CHECK: Grid.CanUserDeleteRows = false;
        // CHECK: Grid.CanUserReorderRows = false;
        // CHECK: Grid.CanUserSelectRows = false;
        // CHECK: Grid.CanUserReorderColumns = true;
        // CHECK: Grid.CanUserHideColumns = true;
        // CHECK: Grid.CanUserResizeColumns = true;
        // CHECK: Grid.CanUserSelectColumns = true;
        // CHECK: Grid.CanUserResizeColumnsOnDoubleClick = true;
        // CHECK: Grid.ShowTotalSummary = true;
        // CHECK: Grid.ShowGroupSummary = true;
        // CHECK: Grid.TotalSummaryPosition = DataGridSummaryRowPosition.Bottom;
        // CHECK: Grid.GroupSummaryPosition = DataGridGroupSummaryPosition.Footer;
         
        CreateContextMenu();
    }

    // ● public
    public void Apply(PivotDef Def)
    {
        PivotBinder.PivotDef = Def;
    }

    public void SaveTo(PivotDef Def)
    {
        ///Def.ClearLists();
        ///
        ///Def.RowFields.AddRange(PivotDef.RowFields.ToArray());
        ///Def.ColumnFields.AddRange(PivotDef.ColumnFields.ToArray());
        ///Def.ValueFields.AddRange(PivotDef.ValueFields.Select(x => x.Clone()).ToArray());
///
        Def.ShowSubtotals = PivotDef.ShowSubtotals;
        Def.ShowGrandTotals = PivotDef.ShowGrandTotals;
        Def.ShowValuesOnRows = PivotDef.ShowValuesOnRows;
    }
    
    // ● properties
    public DataGrid Grid { get;  }
}