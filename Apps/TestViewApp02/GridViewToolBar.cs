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
    Export = 0x20,
    ViewDefDialog = 0x40,

    All = 0xFFFFFFFF
}

public class GridViewToolBar: ToolBar
{
    private GridViewToolBarButtons fVisibleButtons = GridViewToolBarButtons.All;
    private Border sepEdit;
    private Border sepExpand;

    void CreateContextMenu()
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
        btnExport = AddDropDownButton("table_export.png", "Export", mnuExport);
        btnViewDefDialog = AddButton("setting_tools.png", "View Configuration", 
            async (sender, args) => await ButtonClicked(sender as Button));
        
        VisibleButtonsChanged();
    }
    protected virtual void VisibleButtonsChanged()
    {
        btnAdd.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Add);
        btnEdit.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Edit);
        btnDelete.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Delete);
        
        btnExpandAll.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ExpandAll);
        btnCollapseAll.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.CollapseAll);
        
        btnExport.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.Export);
        btnViewDefDialog.IsVisible = fVisibleButtons.HasFlag(GridViewToolBarButtons.ViewDefDialog);

        sepEdit.IsVisible = btnAdd.IsVisible || btnEdit.IsVisible || btnDelete.IsVisible;
        sepExpand.IsVisible = btnExpandAll.IsVisible || btnCollapseAll.IsVisible;
        
    }
    protected virtual async Task ButtonClicked(Button Button)
    {
    }
    
    // ● construction
    public GridViewToolBar(StackPanel Panel, GridView GridView)
        : base(Panel)
    {
        this.GridView = GridView;
        CreateContextMenu();
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