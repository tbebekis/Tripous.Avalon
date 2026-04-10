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
using Tripous.Data;

namespace Tripous.Avalon;

public class PivotView
{
    // ● private fields
    private DataGrid fGrid;
    private PivotDefs fPivotDefs;
    private PivotDef fPivotDef;
    
    // ● constructor
    public PivotView()
    {
    }
    
    // ● public
    static public PivotView Create(DataGrid Grid, DataView DataView, PivotDef PivotDef)
    {
        PivotDefs PivotDefs = new();
        PivotDefs.DefList.Add(PivotDef);
        return Create(Grid, DataView, PivotDefs);
    }
    static public PivotView Create(DataGrid Grid, DataView DataView, PivotDefs PivotDefs)
    {
        PivotView Result = new();
        Result.Grid = Grid;
        Result.PivotDefs = PivotDefs;
        Result.SetSource(DataView);
        return Result;
    }
    
    static public PivotView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, PivotDef PivotDef)
    {
        PivotDefs PivotDefs = new();
        PivotDefs.DefList.Add(PivotDef);
        return Create(Grid, Sequence, PivotDefs);
    }
    static public PivotView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, PivotDefs PivotDefs)
    {
        PivotView Result = new();
        Result.Grid = Grid;
        Result.PivotDefs = PivotDefs;
        Result.SetSource(Sequence);
        return Result;
    }
    
    public void SetSource(DataView DataView)
    {
        if (DataView == null)
            throw new ArgumentNullException(nameof(DataView));
        if (Grid == null)
            throw  new ApplicationException($"No {nameof(DataGrid)} defined");
        if (PivotDef == null)
            throw  new ApplicationException($"No {nameof(PivotDef)} defined");
        
        PivotData PivotData = PivotEngine.Execute(DataView, PivotDef);
        PivotGridRenderer.Show(Grid, PivotData, PivotDef);
    }
    public void SetSource<T>(IEnumerable<T> Sequence)
    {
        if (Sequence == null)
            throw new ArgumentNullException(nameof(IEnumerable<T>));
        if (Grid == null)
            throw  new ApplicationException($"No {nameof(DataGrid)} defined");
        if (PivotDef == null)
            throw  new ApplicationException($"No {nameof(PivotDef)} defined");
        
        PivotData PivotData = PivotEngine.Execute(Sequence, PivotDef);
        PivotGridRenderer.Show(Grid, PivotData, PivotDef);
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
    public PivotDefs PivotDefs
    {
        get
        {
            if (fPivotDefs == null)
                fPivotDefs = new();
            
            return  fPivotDefs;
        }
        set
        {
            if (fPivotDefs != value)
            {
                if (value == null || value.DefList.Count == 0)
                    throw new ApplicationException($"No {nameof(PivotDefs)} defined");
                fPivotDefs = value;
                if (fPivotDefs.DefList.Count > 0)
                    PivotDef = fPivotDefs.DefList[0];
            }
        }
    }
    public PivotDef PivotDef
    {
        get => fPivotDef;
        set
        {
            if (fPivotDef != value)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(PivotDef));
                if (!PivotDefs.DefList.Contains(value))
                    throw new ApplicationException($"{nameof(PivotDef)} not in {nameof(PivotDefs)} list");
                fPivotDef = value;     
            }
        }
    }
}