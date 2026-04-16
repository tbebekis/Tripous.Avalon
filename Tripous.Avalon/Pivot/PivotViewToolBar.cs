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
public enum PivotViewToolBarButtons : ulong
{
    None = 0,
    
    Save = 0x20,
    SaveAs = 0x40,
    
    ExportDialog = 0x80,
    ViewDefDialog = 0x100,
    
    AddViewDef = 0x200,
    EditViewDef = 0x400,
    DeleteViewDef = 0x800,

    All = 0xFFFFFFFF
}


public class PivotViewToolBar: ToolBar
{
    Button btnSave;
    Button btnSaveAs;
    
    Button btnExportDialog;
    Button btnViewDefDialog;
    
    Button btnAddViewDef;
    Button btnEditViewDef;
    Button btnDeleteViewDef;
    
    ComboBox cboViewDefs;
    
    private PivotViewToolBarButtons fVisibleButtons = PivotViewToolBarButtons.All;
 
    private Border sepSave;
 
    
    private bool fIsMultiDef;
    private bool fIsReadOnlyView;
    private PivotView fPivotView;
    private bool ControlsCreated;
    private ObservableCollection<PivotViewDef> ViewDefs;
    
    
        // ● event handlers
    void cboViewDefs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cboViewDefs.SelectedItem is PivotViewDef ViewDef)
            PivotView.ViewDef = ViewDef;
    }
    
    // ● private
    void CreateControls()
    {
        btnExportDialog = AddButton("table_export.png", "Export", 
            async (sender, args) => await ShowExportDialog());
        
        sepSave = AddSeparator();
        btnSave = AddButton("disk.png", "Save Definitions", 
            async (s, e) => await PivotView.SaveViewDefs());
        btnSaveAs = AddButton("disk_multiple.png", "Save Definitions As", 
            async (s, e) => await PivotView.SaveViewDefsAs());

        ViewDefs = new ObservableCollection<PivotViewDef>(PivotView.ViewDefs.DefList) ;
        int Index = PivotView.ViewDefs.DefList.IndexOf(PivotView.ViewDef);
        cboViewDefs = AddComboBox(ViewDefs, Index != -1 ? Index : 0);
        cboViewDefs.SelectionChanged += cboViewDefs_SelectionChanged;
        btnViewDefDialog = AddButton("setting_tools.png", "Edit Configuration", 
            async (sender, args) => await ShowDefDialog());
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
        
        btnSave.IsVisible = fVisibleButtons.HasFlag(PivotViewToolBarButtons.Save);
        btnSaveAs.IsVisible = fVisibleButtons.HasFlag(PivotViewToolBarButtons.SaveAs);
        
        btnExportDialog.IsVisible = fVisibleButtons.HasFlag(PivotViewToolBarButtons.ExportDialog);
        btnViewDefDialog.IsVisible = fVisibleButtons.HasFlag(PivotViewToolBarButtons.ViewDefDialog) && !IsMultiDef;

        sepSave.IsVisible = btnSave.IsVisible || btnSaveAs.IsVisible;
 
        cboViewDefs.IsVisible = IsMultiDef; 
        btnAddViewDef.IsVisible = IsMultiDef;  
        btnEditViewDef.IsVisible = IsMultiDef;   
        btnDeleteViewDef.IsVisible = IsMultiDef;   
    }

    async Task ShowExportDialog()
    {
        PivotViewExportOptions Options = new();
        Options.Load();
        
        DialogData data = await DialogWindow.ShowModal<PivotViewExportDialog>(Options);
        if (data.Result)
        {
            Options.Save();
            await PivotView.Export(Options);
            await MessageBox.Info("Done.");
        }
        
    }
    async Task ShowDefDialog()
    {
        PivotView.UpdateDataTypes(PivotView.ViewDef);
        PivotViewDef ViewDef2  = PivotView.ViewDef.Clone();
 
        ViewDef2.IsNameReadOnly = true;
        
        DialogData data = await DialogWindow.ShowModal<PivotDefDialog>(ViewDef2);
        if (data.Result)
        {
            PivotView.ViewDef.AssignFrom(ViewDef2);
            PivotView.Refresh();
        }
            
        await Task.CompletedTask;
    }

    async Task AddViewDef()
    {
        PivotViewDef ViewDef = PivotView.CreateDefaultViewDef();
        PivotView.UpdateDataTypes(ViewDef);
        PivotView.ViewDef.IsNameReadOnly = false;
        DialogData data = await DialogWindow.ShowModal<PivotDefDialog>(ViewDef);
        if (data.Result)
        {
            //string JsonText = Json.Serialize(ViewDef);
            PivotView.ViewDefs.DefList.Add(ViewDef);
            ViewDefs.Add(ViewDef);
            cboViewDefs.SelectedItem = ViewDef;
        }
    }
    async Task EditViewDef()
    {
        if (cboViewDefs.SelectedItem is PivotViewDef ViewDef)
        {
            int Index = PivotView.ViewDefs.DefList.IndexOf(ViewDef);
            ViewDef.IsNameReadOnly = Index == 0; // not the default
            
            PivotViewDef ViewDef2 = ViewDef.Clone();
            
            DialogData data = await DialogWindow.ShowModal<PivotDefDialog>(ViewDef2);
            if (data.Result)
            {
                ViewDef.AssignFrom(ViewDef2);
                
                Index = ViewDefs.IndexOf(ViewDef);
                if (Index != -1)
                    ViewDefs[Index] = ViewDef; // force ObservableCollection to re-read the item
                cboViewDefs.SelectedItem = ViewDef;
                PivotView.Refresh();
            }
        }
    }
    async Task DeleteViewDef()
    {
        if (cboViewDefs.SelectedItem is PivotViewDef ViewDef)
        {
            int Index = PivotView.ViewDefs.DefList.IndexOf(ViewDef);
            if (Index == 0)
                return; // not the default
            string Message = $"Delete selected {nameof(PivotViewDef)}: {ViewDef.Name}?";
            bool Flag = await MessageBox.YesNo(Message, this.Panel);
            if (Flag)
            {
                PivotView.ViewDefs.Remove(ViewDef);
                ViewDefs.Remove(ViewDef);
                cboViewDefs.SelectedIndex = 0;
            }
        }
    }
    
    // ● overrides
    protected override void RemovedAll()
    {
        base.RemovedAll();

        ViewDefs = null;
        cboViewDefs.SelectionChanged -= cboViewDefs_SelectionChanged;
        
        fIsMultiDef = false;
 
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
    public PivotViewToolBar()
    {
    }
    
    // ● properties
    public PivotView PivotView
    {
        get => fPivotView;
        set
        {
            if (fPivotView != value)
            {
                fPivotView = value;
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
 
    public PivotViewToolBarButtons VisibleButtons
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
                fIsReadOnlyView = value;
            }
        }
    }
}