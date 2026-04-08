using System;
using System.Collections;
using System.Collections.Generic;
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

    // ● private
    void CreateControls()
    {
        mnuExport = new ContextMenu();
        
        btnAdd = AddButton("table_add.png", "Add", 
            async (sender, args) => await GridView.AddItemAsync());
        btnEdit = AddButton("table_edit.png", "Edit", 
            async (sender, args) => await GridView.EditItemAsync());
        btnDelete = AddButton("table_delete.png", "Delete", 
            async (sender, args) => await GridView.DeleteItemAsync());
        
        sepEdit = AddSeparator();
        btnExpandAll = AddButton("arrow_out.png", "Expand All", 
            (s, e) => GridView.ExpandAll());
           // async (sender, args) => await ButtonClicked(sender as Button));
        btnCollapseAll = AddButton("arrow_in.png", "Collapse All", 
            //async (sender, args) => await ButtonClicked(sender as Button));
            (s, e) => GridView.CollapseAll());
        
        sepExpand = AddSeparator();
        btnExport = AddButton("table_export.png", "Export", 
            async (sender, args) => await ShowGridViewExportDialog());
        btnViewDefDialog = AddButton("setting_tools.png", "View Configuration", 
            async (sender, args) => await ShowViewDefDialog());

        if (GridView.ViewDefs != null && GridView.ViewDefs.DefList.Count > 0)
        {
            var ViewDefs = GridView.ViewDefs.DefList;
            int Index = GridView.ViewDefs.DefList.IndexOf(GridView.ViewDef);
            cboViewDefs = AddComboBox(ViewDefs, Index != -1 ? Index : 0);
            cboViewDefs.SelectionChanged += cboViewDefs_SelectionChanged;
            btnAddViewDef = AddButton("application_add.png", "Add View Def",
                async (sender, args) => await AddViewDef());
            btnEditViewDef = AddButton("application_edit.png", "Edit View Def",
                async (sender, args) => await EditViewDef());
            btnDeleteViewDef = AddButton("application_delete.png", "Delete View Def",
            async (sender, args) => await DeleteViewDef());
        }
        
        VisibleButtonsChanged();
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
        DialogData data = await DialogWindow.ShowModal<GridViewDefDialog>(GridView.ViewDef);
        if (data.Result)
        {
            //string JsonText = Json.Serialize(GridView.ViewDef);
            GridView.Refresh();
        }
    }
    void VisibleButtonsChanged()
    {
        btnAdd.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Add);
        btnEdit.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Edit);
        btnDelete.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Delete);
        
        btnExpandAll.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ExpandAll);
        btnCollapseAll.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.CollapseAll);
        
        btnExport.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ExportDialog);
        btnViewDefDialog.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ViewDefDialog);

        sepEdit.IsVisible = btnAdd.IsVisible || btnEdit.IsVisible || btnDelete.IsVisible;
        sepExpand.IsVisible = btnExpandAll.IsVisible || btnCollapseAll.IsVisible;
        
    }

    async Task AddViewDef()
    {
        DialogData Data = await Ui.InputBox($"Provide a {nameof(GridViewDef)} Name", "", this.Panel);
        if (Data.Result && Data.ResultData is InputBoxData InputBoxData)
        {
            if (GridView.ViewDefs.Contains(InputBoxData.Value))
            {
                await MessageBox.Error($"{nameof(GridViewDef)} already exists in list: {InputBoxData.Value}");
                return;
            }
            
            GridViewDef ViewDef = GridView.Controller.Source.CreateDefaultViewDef();
            ViewDef.Name = InputBoxData.Value;
            GridView.ViewDefs.DefList.Add(ViewDef);
            cboViewDefs.ItemsSource = GridView.ViewDefs.DefList;
            cboViewDefs.SelectedItem = ViewDef;
        }
    }
    async Task EditViewDef()
    {
        
    }
    async Task DeleteViewDef()
    {
        
    }
    void cboViewDefs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cboViewDefs.SelectedItem is GridViewDef ViewDef)
            GridView.ApplyViewDef(ViewDef);
    }
    
    
    // ● construction
    public GridViewToolBar(StackPanel Panel, GridView GridView)
        : base(Panel)
    {
        this.GridView = GridView;
        CreateControls();
    }
   
    // ● properties
    public GridView GridView { get; private set; }
    public ContextMenu mnuExport { get; protected set; }
    
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
                VisibleButtonsChanged();
            }
        }
    }
}