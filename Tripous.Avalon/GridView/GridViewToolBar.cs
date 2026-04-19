using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Tripous.Data;

namespace Tripous.Avalon;

[Flags]
public enum GridViewToolBarButtons : ulong
{
    None = 0,
    Add = 0x1,
    Edit = 0x2,
    Delete = 0x4,
    
    ExpandAll = 0x8,
    CollapseAll = 0x10,
    
    ToggleIds = 0x20,
    
    Save = 0x40,
    SaveAs = 0x80,
    
    ExportDialog = 0x100,
    ViewDefDialog = 0x200,
    
    AddViewDef = 0x400,
    EditViewDef = 0x800,
    DeleteViewDef = 0x1000,

    EditRowButtons = Add | Edit | Delete,
    EditViewButtons = AddViewDef | EditViewDef | DeleteViewDef,
    ExpandButtons = ExpandAll | CollapseAll,
    SaveButtons = Save | SaveAs,
    
    All = 0xFFFFFFFF
}

public class GridViewToolBar: ToolBar
{
    Button btnAdd;
    Button btnEdit;
    Button btnDelete;
    
    Button btnExpandAll;
    Button btnCollapseAll;

    ToggleButton btnToggleIds;
    
    Button btnSave;
    Button btnSaveAs;
    
    Button btnExportDialog;
    Button btnViewDefDialog;
    
    Button btnAddViewDef;
    Button btnEditViewDef;
    Button btnDeleteViewDef;
    
    ComboBox cboViewDefs;
    
    private GridViewToolBarButtons fVisibleButtons = GridViewToolBarButtons.All;
    private Border sepEdit;
    private Border sepSave;
    private Border sepExpand;
    
    private bool fIsMultiDef;
    private GridView fGridView;
    private bool ControlsCreated;
    
    // ● event handlers
    void cboViewDefs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PreviouslySelectedDef = (e.RemovedItems != null && e.RemovedItems.Count > 0)
            ? e.RemovedItems[0] as GridViewDef
            : null;

        if (cboViewDefs.SelectedItem is GridViewDef ViewDef)
        {
            Dispatcher.UIThread.Post(() => 
            {  
                GridView.ViewDef = ViewDef;
                SelectedDefChanged?.Invoke(this, EventArgs.Empty);

            }, DispatcherPriority.Background);
        }
    }
    
    // ● private
    void CreateControls()
    {
        btnAdd = AddButton("table_add.png", "Add Row", 
            async (sender, args) => await GridView.AddItemAsync());
        btnEdit = AddButton("table_edit.png", "Edit Row", 
            async (sender, args) => await GridView.EditItemAsync());
        btnDelete = AddButton("table_delete.png", "Delete Row", 
            async (sender, args) => await GridView.DeleteItemAsync());
        
        sepEdit = AddSeparator();
        btnExpandAll = AddButton("arrow_out.png", "Expand All", 
            (s, e) => GridView.ExpandAll());
        btnCollapseAll = AddButton("arrow_in.png", "Collapse All",
            (s, e) => GridView.CollapseAll());

        btnToggleIds = AddToggleButton("table_select_column.png", "Hide Ids",
            (s, e) => btnToggleIds_Clicked());
        
        sepExpand = AddSeparator();
        btnExportDialog = AddButton("table_export.png", "Export", 
            async (sender, args) => await ShowExportDialog());
        
        sepSave = AddSeparator();
        btnSave = AddButton("disk.png", "Save Definitions", 
            async (s, e) => await GridView.SaveViewDefs());
        btnSaveAs = AddButton("disk_multiple.png", "Save Definitions As", 
            async (s, e) => await GridView.SaveViewDefsAs());
 
        int Index = ViewDefList.IndexOf(GridView.ViewDef);
        cboViewDefs = AddComboBox(ViewDefList, Index != -1 ? Index : 0);
        cboViewDefs.SelectionChanged += cboViewDefs_SelectionChanged;
        btnViewDefDialog = AddButton("setting_tools.png", "Edit Configuration", 
            async (sender, args) => await ShowViewDefDialog());
        btnAddViewDef = AddButton("application_add.png", "Add View Def",
            async (sender, args) => await AddViewDef());
        btnEditViewDef = AddButton("application_edit.png", "Edit View Def",
            async (sender, args) => await EditViewDef());
        btnDeleteViewDef = AddButton("application_delete.png", "Delete View Def",
            async (sender, args) => await DeleteViewDef());

        ControlsCreated = true;
        VisibleControlsChanged();
    }
    void btnToggleIds_Clicked()
    {
        bool Flag = !(btnToggleIds.IsChecked == true); // Checked = hide, else = show
        GridView.IdColumnsVisible = Flag;  
        string S = GridView.IdColumnsVisible ? "Hide Ids" : "Show Ids";
        ToolTip.SetTip(btnToggleIds, S);
    }
    void VisibleControlsChanged()
    {
        if (!ControlsCreated)
            return;
        
        btnAdd.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Add);
        btnEdit.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Edit);
        btnDelete.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Delete);
        
        btnExpandAll.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ExpandAll);
        btnCollapseAll.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.CollapseAll);
        
        btnToggleIds.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ToggleIds);
        
        btnSave.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Save);
        btnSaveAs.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.SaveAs);
        
        btnExportDialog.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ExportDialog);
        btnViewDefDialog.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ViewDefDialog) && !IsMultiDef;

        sepEdit.IsVisible = btnAdd.IsVisible || btnEdit.IsVisible || btnDelete.IsVisible;
        sepSave.IsVisible = btnSave.IsVisible || btnSaveAs.IsVisible;
        sepExpand.IsVisible = btnExpandAll.IsVisible || btnCollapseAll.IsVisible;
 
        cboViewDefs.IsVisible = IsMultiDef; 
        btnAddViewDef.IsVisible = IsMultiDef;  
        btnEditViewDef.IsVisible = IsMultiDef;   
        btnDeleteViewDef.IsVisible = IsMultiDef;   
    }

    async Task ShowExportDialog()
    {
        GridViewExportOptions Options = new();
        Options.Load();
        
        DialogData data = await DialogWindow.ShowModal<GridViewExportDialog>(Options);
        if (data.Result)
        {
            Options.Save();
            await GridView.Export(Options);
            await MessageBox.Info("Done.");
        }
    }
    async Task ShowViewDefDialog()
    {
        GridViewDef ViewDef2 = GridView.ViewDef.Clone();
        ViewDef2.IsNameReadOnly = true;
        
        DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(ViewDef2);
        if (data.Result)
        {
            GridView.ViewDef.AssignFrom(ViewDef2);
            //string JsonText = Json.Serialize(ViewDef);
            GridView.Refresh();
        }
    }

    async Task AddViewDef()
    {
        GridViewDef ViewDef = GridView.CreateDefaultViewDef();
        GridView.ViewDef.IsNameReadOnly = false;
        DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(ViewDef);
        if (data.Result)
        {
            //string JsonText = Json.Serialize(ViewDef);
            GridView.ViewDefs.DefList.Add(ViewDef);
            ViewDefList.Add(ViewDef);
            cboViewDefs.SelectedItem = ViewDef;
        }
    }
    async Task EditViewDef()
    {
        if (cboViewDefs.SelectedItem is GridViewDef ViewDef)
        {
            int Index = GridView.ViewDefs.DefList.IndexOf(ViewDef);
            ViewDef.IsNameReadOnly = Index == 0; // not the default
            
            GridViewDef ViewDef2 = ViewDef.Clone();
            
            DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(ViewDef2);
            if (data.Result)
            {
                ViewDef.AssignFrom(ViewDef2);
                
                Index = ViewDefList.IndexOf(ViewDef);
                if (Index != -1)
                    ViewDefList[Index] = ViewDef; // force ObservableCollection to re-read the item
                cboViewDefs.SelectedItem = ViewDef;
                GridView.Refresh();
            }
        }
    }
    async Task DeleteViewDef()
    {
        if (cboViewDefs.SelectedItem is GridViewDef ViewDef)
        {
            int Index = GridView.ViewDefs.DefList.IndexOf(ViewDef);
            if (Index == 0)
                return; // not the default
            string Message = $"Delete selected {nameof(GridViewDef)}: {ViewDef.Name}?";
            bool Flag = await MessageBox.YesNo(Message, this.Panel);
            if (Flag)
            {
                GridView.ViewDefs.Remove(ViewDef);
                ViewDefList.Remove(ViewDef);
                cboViewDefs.SelectedIndex = 0;
            }
        }
    }
    
    // ● overrides
    protected override void RemovedAll()
    {
        base.RemovedAll();
 
        cboViewDefs.SelectionChanged -= cboViewDefs_SelectionChanged;
        
        fIsMultiDef = false;

        btnAdd = null;
        btnEdit  = null;
        btnDelete = null;
        btnExpandAll = null;
        btnCollapseAll = null;
        btnExportDialog = null;
        btnViewDefDialog = null;
        cboViewDefs = null;
        btnAddViewDef = null;
        btnEditViewDef = null;
        btnDeleteViewDef = null;

        ControlsCreated = false;
    }
    protected override void PanelChanged()
    {
        base.PanelChanged();
        if (Panel != null)
        {
            CreateControls();
        }
    }

    // ● construction
    public GridViewToolBar()
    {
    }
   
    // ● properties
    public GridView GridView
    {
        get => fGridView;
        set
        {
            if (fGridView != value)
            {
                fGridView = value;
                VisibleControlsChanged();
            }
        }
    }
    public ObservableCollection<GridViewDef> ViewDefList => GridView != null? GridView.ViewDefs.DefList: null;
    public GridViewDef SelectedDef
    {
        get => (cboViewDefs != null && cboViewDefs.SelectedItem != null)
            ? cboViewDefs.SelectedItem as GridViewDef
            : null;
        set
        {
            if (cboViewDefs != null)
            {
                if (value == null)
                {
                    cboViewDefs.SelectedItem = null;
                }
                else
                {
                    if (ViewDefList.Contains(value) && cboViewDefs.Items.IndexOf(value) != -1)
                        cboViewDefs.SelectedItem = value;
                }
            }

        }
    }
    public GridViewDef PreviouslySelectedDef { get; private set; }
    public bool IsMultiDef
    {
        get => fIsMultiDef;
        set
        {
            if (fIsMultiDef != value)
            {
                fIsMultiDef = value;
                VisibleControlsChanged();
            }
        }
    }
 
    public GridViewToolBarButtons VisibleButtons
    {
        get => fVisibleButtons;
        set
        {
            if (fVisibleButtons != value)
            {
                fVisibleButtons = value;
                VisibleControlsChanged();
            }
        }
    }
    public bool IsReadOnlyView
    {
        get 
        {
            var Buttons = GridViewToolBarButtons.Add | GridViewToolBarButtons.Edit | GridViewToolBarButtons.Delete;
            return (VisibleButtons & Buttons) == 0;
        }
        set
        {
            if (IsReadOnlyView != value)
            {
                var Buttons = GridViewToolBarButtons.Add | GridViewToolBarButtons.Edit | GridViewToolBarButtons.Delete;
            
                if (value)
                    VisibleButtons &= ~Buttons; // Αφαίρεση (Bitwise NAND)
                else
                    VisibleButtons |= Buttons;  // Προσθήκη (Bitwise OR)
            }
        }
    }
    
    // ● events
    public event EventHandler SelectedDefChanged;
}