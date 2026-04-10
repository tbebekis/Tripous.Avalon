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
    Add = 1,
    Edit = 2,
    Delete = 4,
    ExpandAll = 8,
    CollapseAll = 0x10,
    ExportDialog = 0x20,
    ViewDefDialog = 0x40,

    All = 0xFFFFFFFF
}

public class GridViewToolBar: ToolBar
{
    private GridViewToolBarButtons fVisibleButtons = GridViewToolBarButtons.All;
    private Border sepEdit;
    private Border sepExpand;
    
    private bool fIsMultiDef;
    private bool fIsReadOnlyView;
    private GridView fGridView;
    private bool ControlsCreated;
    private ObservableCollection<GridViewDef> ViewDefs;
    
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
        
        sepExpand = AddSeparator();
        btnExport = AddButton("table_export.png", "Export", 
            async (sender, args) => await ShowGridViewExportDialog());
        btnViewDefDialog = AddButton("setting_tools.png", "View Configuration", 
            async (sender, args) => await ShowViewDefDialog());

        ViewDefs = new ObservableCollection<GridViewDef>(GridView.ViewDefs.DefList) ;
        int Index = GridView.ViewDefs.DefList.IndexOf(GridView.ViewDef);
        cboViewDefs = AddComboBox(ViewDefs, Index != -1 ? Index : 0);
        cboViewDefs.SelectionChanged += cboViewDefs_SelectionChanged;
        btnAddViewDef = AddButton("application_add.png", "Add View Def",
            async (sender, args) => await AddViewDef());
        btnEditViewDef = AddButton("application_edit.png", "Edit View Def",
            async (sender, args) => await EditViewDef());
        btnDeleteViewDef = AddButton("application_delete.png", "Delete View Def",
            async (sender, args) => await DeleteViewDef());

        ControlsCreated = true;
        VisibleControlsChanged();
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
        
        btnExport.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ExportDialog);
        btnViewDefDialog.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ViewDefDialog) && !IsMultiDef;

        sepEdit.IsVisible = btnAdd.IsVisible || btnEdit.IsVisible || btnDelete.IsVisible;
        sepExpand.IsVisible = btnExpandAll.IsVisible || btnCollapseAll.IsVisible;
 
        cboViewDefs.IsVisible = IsMultiDef; 
        btnAddViewDef.IsVisible = IsMultiDef;  
        btnEditViewDef.IsVisible = IsMultiDef;   
        btnDeleteViewDef.IsVisible = IsMultiDef;   
    }

    async Task ShowGridViewExportDialog()
    {
        GridViewExportOptions Options = new();
        Options.Load();
        
        DialogData data = await DialogWindow.ShowModal<GridViewExportDialog>(Options);
        if (data.Result)
        {
            Options.Save();
            GridViewExporter.Export(GridView, Options);
            await MessageBox.Info("Done.");
        }
    }
    async Task ShowViewDefDialog()
    {
        GridView.ViewDef.IsNameReadOnly = true;
        DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(GridView.ViewDef);
        if (data.Result)
        {
            //string JsonText = Json.Serialize(GridView.ViewDef);
            GridView.Refresh();
        }
    }

    async Task AddViewDef()
    {
        GridViewDef ViewDef = GridView.Controller.ViewSource.CreateDefaultViewDef();
        GridView.ViewDef.IsNameReadOnly = false;
        DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(ViewDef);
        if (data.Result)
        {
            //string JsonText = Json.Serialize(ViewDef);
            GridView.ViewDefs.DefList.Add(ViewDef);
            ViewDefs.Add(ViewDef);
            cboViewDefs.SelectedItem = ViewDef;
        }
    }
    async Task EditViewDef()
    {
        if (cboViewDefs.SelectedItem is GridViewDef ViewDef)
        {
            int Index = GridView.ViewDefs.DefList.IndexOf(ViewDef);
            GridView.ViewDef.IsNameReadOnly = Index == 0; // not the default
            
            DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(ViewDef);
            if (data.Result)
            {
                Index = ViewDefs.IndexOf(ViewDef);
                if (Index != -1)
                    ViewDefs[Index] = ViewDef; // force ObservableCollection to re-read the item
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
                ViewDefs.Remove(ViewDef);
                cboViewDefs.SelectedIndex = 0;
            }
        }
    }
    void cboViewDefs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cboViewDefs.SelectedItem is GridViewDef ViewDef)
            //GridView.ApplyViewDef(ViewDef);
            GridView.ViewDef = ViewDef;
    }
    
    // ● overrides
    protected override void RemovedAll()
    {
        base.RemovedAll();

        ViewDefs = null;
        cboViewDefs.SelectionChanged -= cboViewDefs_SelectionChanged;
        
        fIsMultiDef = false;

        btnAdd = null;
        btnEdit  = null;
        btnDelete = null;
        btnExpandAll = null;
        btnCollapseAll = null;
        btnExport = null;
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
 
    public Button btnAdd { get; protected set; }
    public Button btnEdit { get; protected set; }
    public Button btnDelete { get; protected set; }
    
    public Button btnExpandAll { get; protected set; }
    public Button btnCollapseAll { get; protected set; }
    
    public Button btnExport { get; protected set; }
    public Button btnViewDefDialog { get; protected set; }
    
    public ComboBox cboViewDefs { get; protected set; }
    public Button btnAddViewDef { get; protected set; }
    public Button btnEditViewDef { get; protected set; }
    public Button btnDeleteViewDef { get; protected set; }

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
        get => fIsReadOnlyView;
        set
        {
            if (fIsReadOnlyView != value)
            {
                var Buttons = GridViewToolBarButtons.Add | GridViewToolBarButtons.Edit | GridViewToolBarButtons.Delete;
                fIsReadOnlyView = value;
                if (value)
                    VisibleButtons &= ~(Buttons);
                else
                    VisibleButtons |= Buttons;
            }
        }
    }
}