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
    private PivotViewDefs fViewDefs;
    private PivotViewDef fViewDef;

    private DataView LastSourceDataView;
    private IEnumerable LastSourceEnumerable;
    private Type LastSourceType;
    
    // ● constructor
    public PivotView()
    {
        ToolBar.PivotView = this;
        Menu.PivotView = this;
    }
    
    // ● public
    static public PivotView Create(DataGrid Grid, DataView DataView, PivotViewDef PivotViewDef)
    {
        PivotViewDefs ViewDefs = new();
        ViewDefs.DefList.Add(PivotViewDef);
        return Create(Grid, DataView, ViewDefs);
    }
    static public PivotView Create(DataGrid Grid, DataView DataView, PivotViewDefs ViewDefs)
    {
        PivotView Result = new();
        Result.Grid = Grid;
        Result.ViewDefs = ViewDefs;
        Result.SetSource(DataView);
        return Result;
    }
    
    static public PivotView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, PivotViewDef PivotViewDef)
    {
        PivotViewDefs ViewDefs = new();
        ViewDefs.DefList.Add(PivotViewDef);
        return Create(Grid, Sequence, ViewDefs);
    }
    static public PivotView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, PivotViewDefs ViewDefs)
    {
        PivotView Result = new();
        Result.Grid = Grid;
        Result.ViewDefs = ViewDefs;
        Result.SetSource(Sequence);
        return Result;
    }
    
    public void SetSource(DataView DataView)
    {
        if (DataView == null)
            throw new ArgumentNullException(nameof(DataView));
        if (Grid == null)
            throw  new ApplicationException($"No {nameof(DataGrid)} defined");
        if (ViewDef == null)
            throw  new ApplicationException($"No {nameof(ViewDef)} defined");

        LastSourceDataView = DataView;
        LastSourceEnumerable = null;
        LastSourceType = null;
        
        ViewDef.UpdateDataTypes(DataView);
        
        var top = TopLevel.GetTopLevel(Grid);

        top.Cursor = new Cursor(StandardCursorType.Wait);
        try
        {
            PivotData = PivotEngine.Execute(DataView, ViewDef);
            PivotGridRenderer.Show(Grid, PivotData, ViewDef);
        }
        finally
        {
            top.Cursor = new Cursor(StandardCursorType.Arrow);
        }
        
    }
    public void SetSource<T>(IEnumerable<T> Sequence)
    {
        if (Sequence == null)
            throw new ArgumentNullException(nameof(IEnumerable<T>));
        if (Grid == null)
            throw  new ApplicationException($"No {nameof(DataGrid)} defined");
        if (ViewDef == null)
            throw  new ApplicationException($"No {nameof(ViewDef)} defined");

        LastSourceType = typeof(T);
        LastSourceEnumerable = Sequence;
        LastSourceDataView = null;
        
        ViewDef.UpdateDataTypes(LastSourceType);
        
        var top = TopLevel.GetTopLevel(Grid);
        top.Cursor = new Cursor(StandardCursorType.Wait);
        try
        {
            PivotData = PivotEngine.Execute(Sequence, ViewDef);
            PivotGridRenderer.Show(Grid, PivotData, ViewDef);
        }
        finally
        {
            top.Cursor = new Cursor(StandardCursorType.Arrow);
        }

    }
    
    public DataGridColumn GetColumn(string FieldName) => Grid.Columns.FirstOrDefault(x => FieldName.IsSameText((x.Tag as PivotFieldDef).FieldName));
    public PivotFieldDef GetFieldDef(DataGridColumn Column) => Column.Tag as PivotFieldDef;

    public void UpdateDataTypes(PivotViewDef ViewDef)
    {
        if (LastSourceDataView != null)
            ViewDef.UpdateDataTypes(LastSourceDataView);
        else if (LastSourceType != null)
            ViewDef.UpdateDataTypes(LastSourceType);
    }

    public void Refresh()
    {
        if (Grid == null)
            throw new ApplicationException($"No {nameof(DataGrid)} defined");
        if (ViewDef == null)
            throw new ApplicationException($"No {nameof(ViewDef)} defined");

        if (LastSourceDataView != null)
        {
            ViewDef.UpdateDataTypes(LastSourceDataView);
            PivotData = PivotEngine.Execute(LastSourceDataView, ViewDef);
            PivotGridRenderer.Show(Grid, PivotData, ViewDef);
            return;
        }

        if (LastSourceEnumerable != null && LastSourceType != null)
        {
            ViewDef.UpdateDataTypes(LastSourceType);

            MethodInfo Method = typeof(PivotEngine)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == nameof(PivotEngine.Execute)
                            && x.IsGenericMethodDefinition
                            && x.GetParameters().Length == 2);

            MethodInfo GenericMethod = Method.MakeGenericMethod(LastSourceType);

            var top = TopLevel.GetTopLevel(Grid);

            top.Cursor = new Cursor(StandardCursorType.Wait);
            try
            {
                PivotData = (PivotData)GenericMethod.Invoke(null, new object[] { LastSourceEnumerable, ViewDef });
                PivotGridRenderer.Show(Grid, PivotData, ViewDef);
            }
            finally
            {
                top.Cursor = new Cursor(StandardCursorType.Arrow);
            }

            return;
        }

        throw new ApplicationException("No pivot source defined.");
    }
    
    public virtual async Task SaveViewDefs()
    {
        // TODO: SaveViewDefs όπως και η  AddItemAsync
    }
    public virtual async Task SaveViewDefsAs()
    {
        // TODO: SaveViewDefsAs όπως και η  AddItemAsync
    }

    public PivotViewDef CreateDefaultViewDef()
    {
        if (LastSourceDataView != null)
            return LastSourceDataView.CreateDefaultPivotDef();
        if (LastSourceType != null)
            LastSourceType.CreateDefaultPivotDef();
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
    public PivotViewDefs ViewDefs
    {
        get
        {
            if (fViewDefs == null)
                fViewDefs = new();
            
            return  fViewDefs;
        }
        set
        {
            if (fViewDefs != value)
            {
                if (value == null || value.DefList.Count == 0)
                    throw new ApplicationException($"No {nameof(ViewDefs)} defined");
                fViewDefs = value;
                if (fViewDefs.DefList.Count > 0)
                    ViewDef = fViewDefs.DefList[0];
            }
        }
    }
    public PivotViewDef ViewDef
    {
        get => fViewDef;
        set
        {
            if (fViewDef != value)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(ViewDef));
                if (!ViewDefs.DefList.Contains(value))
                    throw new ApplicationException($"{nameof(ViewDef)} not in {nameof(ViewDefs)} list");
                fViewDef = value;     
            }
        }
    }

    public PivotData PivotData { get; private set; }
    public PivotViewToolBar ToolBar { get; } = new();
    public PivotViewMenu Menu { get; } = new();
}