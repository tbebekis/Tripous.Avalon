using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
 
using System.Reflection;
using Tripous.Data;

namespace Tripous.Avalon;

public class PivotView
{
    // ● private fields
    private DataGrid fGrid;
    private PivotViewDef fViewDef;
    private DataView fDataView;
    private IEnumerable EnumerableSource;
    private Type EnumerableSourceType;
    
    // ● constructor
    public PivotView()
    {
        ToolBar.PivotView = this;
        Menu.PivotView = this;
    }
    
    // ● public
    static public PivotView Create(DataGrid Grid, DataView DataView, PivotViewDef ViewDef)
    {
        PivotView Result = new();
        Result.Grid = Grid;
        Result.SetSource(DataView);
        Result.ViewDef = ViewDef;
        return Result;
    }
    static public PivotView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, PivotViewDef ViewDef)
    {
        PivotView Result = new();
        Result.Grid = Grid;
        Result.SetSource(Sequence);
        Result.ViewDef = ViewDef;
        return Result;
    }

    public void SetSource(DataView DataView) => this.DataView = DataView;
    public void SetSource<T>(IEnumerable<T> Sequence)
    {
        if (Sequence == null)
            throw new ArgumentNullException(nameof(IEnumerable<T>));
        if (Grid == null)
            throw  new ApplicationException($"No {nameof(DataGrid)} defined");
        if (ViewDef == null)
            throw  new ApplicationException($"No {nameof(ViewDef)} defined");

        EnumerableSourceType = typeof(T);
        EnumerableSource = Sequence;
        fDataView = null;
        
        ViewDef.UpdateDataTypes(EnumerableSourceType);
        
        if (CanRefresh())
            Refresh();
    }
    
    public DataGridColumn GetColumn(string FieldName) => Grid.Columns.FirstOrDefault(x => FieldName.IsSameText((x.Tag as PivotFieldDef).FieldName));
    public PivotFieldDef GetFieldDef(DataGridColumn Column) => Column.Tag as PivotFieldDef;

    public void UpdateDataTypes(PivotViewDef ViewDef)
    {
        if (DataView != null)
            ViewDef.UpdateDataTypes(DataView);
        else if (EnumerableSourceType != null)
            ViewDef.UpdateDataTypes(EnumerableSourceType);
    }

    public bool CanRefresh() => Grid != null && (DataView != null || EnumerableSource != null) && ViewDef != null;
    public void Refresh()
    {
        // -----------------------------------------------------------
        void RefreshWithDataView()
        {
            ViewDef.UpdateDataTypes(DataView);
            PivotData = PivotEngine.Execute(DataView, ViewDef);
            PivotGridRenderer.Show(Grid, PivotData, ViewDef);
        }
        // -----------------------------------------------------------
        void RefreshWithEnumerable()
        {
            ViewDef.UpdateDataTypes(EnumerableSourceType);

            MethodInfo Method = typeof(PivotEngine)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == nameof(PivotEngine.Execute)
                            && x.IsGenericMethodDefinition
                            && x.GetParameters().Length == 2);

            MethodInfo GenericMethod = Method.MakeGenericMethod(EnumerableSourceType);
            PivotData = (PivotData)GenericMethod.Invoke(null, new object[] { EnumerableSource, ViewDef });
            PivotGridRenderer.Show(Grid, PivotData, ViewDef);
        }
        // -----------------------------------------------------------
        
        if (Grid == null)
            throw new ApplicationException($"No {nameof(DataGrid)} defined");
        if (ViewDef == null)
            throw new ApplicationException($"No {nameof(ViewDef)} defined");

        if (DataView != null)
        {
            Ui.ShowWaitCursor(RefreshWithDataView, Grid);
            return;
        }

        if (EnumerableSource != null && EnumerableSourceType != null)
        {
            Ui.ShowWaitCursor(RefreshWithEnumerable, Grid);
            return;
        }

        throw new ApplicationException("No pivot source defined.");
    }
    
    public virtual async Task SaveViewDefs()
    {
        if (!string.IsNullOrWhiteSpace(ViewDefs.FilePath))
            ViewDefs.SaveToFile();
        else
            await SaveViewDefsAs();
    }
    public virtual async Task SaveViewDefsAs()
    {
        string FilePath = null;
        if (DefsFilePathNeeded != null)
        {
            FilePathEventArgs Args = new();
            DefsFilePathNeeded(this, Args);
            if (!string.IsNullOrWhiteSpace(Args.FilePath))
                FilePath = Args.FilePath;
        }
        else
        {
            FilePath = await Ui.SaveFileDialog(Grid, "json");
        }

        if (!string.IsNullOrWhiteSpace(FilePath))
        {
            ViewDefs.FilePath = FilePath;
            ViewDefs.SaveToFile();
        }
    }
 
    public virtual async Task Export(PivotViewExportOptions Options = null)
    {
        Options = Options ?? new();
        
        if (string.IsNullOrWhiteSpace(Options.ExportFilePath))
            Options.ExportFilePath = await Ui.SaveFileDialog(Grid, Options.Format.GetExportFileExtension());

        if (!string.IsNullOrWhiteSpace(Options.ExportFilePath))
            PivotViewExporter.Export(this, Options);
        
        await Task.CompletedTask;
    }

    public PivotViewDef CreateDefaultViewDef()
    {
        if (DataView != null)
            return PivotViewDef.Create(DataView);
        if (EnumerableSourceType != null)
            return PivotViewDef.Create(EnumerableSourceType);
        return null;
    }
    
    // ● properties
    public DataGrid Grid
    {
        get => fGrid;
        set
        {
            if (fGrid != null)
                throw new ApplicationException($"{nameof(PivotView)} has already a Grid.");
            if (value == null)
                throw new ApplicationException($"{nameof(PivotView)} needs a Grid.");
            fGrid = value;
        }
    }
    public DataView DataView
    {
        get => fDataView;
        set
        {
            if (fDataView != value)
            {
                fDataView = value;
                if (value != null)
                {
                    EnumerableSource = null;
                    EnumerableSourceType = null;
                }
                else
                {
                    if (ViewDef != null)
                        ViewDef.UpdateDataTypes(DataView);
                }
                
                if (CanRefresh())
                    Refresh();
            }
        }
    }
    public PivotViewDefs ViewDefs { get; } = new();
    public PivotViewDef ViewDef
    {
        get => fViewDef;
        set
        {
            if (fViewDef != value)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(ViewDef));
                
                if (!ViewDefs.Contains(value))
                    ViewDefs.Add(value);
                
                fViewDef = value;     
                
                if (DataView != null)
                    ViewDef.UpdateDataTypes(DataView);
                else if (EnumerableSourceType != null)
                    ViewDef.UpdateDataTypes(EnumerableSourceType);
                
                if (CanRefresh())
                    Refresh();
            }
        }
    }

    public PivotData PivotData { get; private set; }
    public PivotViewToolBar ToolBar { get; } = new();
    public PivotViewMenu Menu { get; } = new();
    
    // ● events
    public event EventHandler<FilePathEventArgs> DefsFilePathNeeded;
}