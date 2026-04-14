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
using Org.BouncyCastle.Asn1.Cmp;
using Tripous.Data;

namespace Tripous.Avalon;

public enum GridViewAction
{
    Add,
    Edit,
    Delete
}

public class GridViewItemEventArgs: EventArgs
{
    public GridViewAction Action { get; set; }
    public object DataItem { get; set; }
    public GridDataRow Row { get; set; }
    public bool Handled { get; set; }
    public bool Cancel { get; set; }
}

public class GridView
{
    // ● private fields
    private DataGrid fGrid;
    private GridViewDefs fViewDefs;

 
    // ● overridables
    protected virtual void GridViewSourceChanged()
    {
       // Menu.IsEnabled = Controller.ViewSource != null;
    }
    protected virtual void OnDataChanged(GridViewDataChangedEventArgs e)
    {
        DataChanged?.Invoke(this, e);
    }
    protected virtual void OnPositionChanged()
    {
        PositionChanged?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void ControllerDataChanged(object Sender, GridViewDataChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            UpdateColumnTitles();
        }, DispatcherPriority.Input);
        
        OnDataChanged(e);
    }
    protected virtual void ControllerPositionChanged(object Sender, EventArgs e)
    {
        OnPositionChanged();
    }
    protected virtual void GridSelectionChanged(object Sender, SelectionChangedEventArgs e)
    {
    }
 
    protected virtual GridViewItemEventArgs CreateArgs(GridViewAction Action)
    {
        return new GridViewItemEventArgs()
        {
            Action = Action,
            Row = Current,
            DataItem = Current != null ? Current.DataItem : null
        };
    }
    protected bool HasDataItemActionHandler => DataItemAction != null;
    protected virtual void OnItemAction(GridViewItemEventArgs e)
    {
        DataItemAction?.Invoke(this, e);
    }
    
    protected virtual bool AddDataItem(object DataItem)
    {
        if (DataItem == null || Controller == null || Controller.ViewSource == null)
            return false;

        if (Controller.ViewSource is DataViewGridViewSource DataViewSource)
        {
            if (DataItem is DataRowView RowView)
            {
                DataViewSource.Source.Table.Rows.Add(RowView.Row);
                Refresh();
                return true;
            }

            if (DataItem is DataRow Row)
            {
                DataViewSource.Source.Table.Rows.Add(Row);
                Refresh();
                return true;
            }

            return false;
        }

        IList ListSource = Controller.ViewSource.ListSource;
        if (ListSource != null)
        {
            ListSource.Add(DataItem);
            Refresh();
            return true;
        }

        return false;
    }
    protected virtual bool DeleteDataItem(object DataItem)
    {
        if (DataItem == null || Controller == null || Controller.ViewSource == null)
            return false;

        if (DataItem is DataRowView RowView)
        {
            RowView.Delete();
            Refresh();
            return true;
        }

        if (DataItem is DataRow Row)
        {
            Row.Delete();
            Refresh();
            return true;
        }

        IList ListSource = Controller.ViewSource.ListSource;
        if (ListSource != null)
        {
            ListSource.Remove(DataItem);
            Refresh();
            return true;
        }

        return false;
    }
    
    // ● constructor
    public GridView()
    {
        ToolBar.GridView = this;
        Menu.GridView = this;
    }

    static public GridView Create(DataGrid Grid, DataView DataView, StackPanel ToolBarPanel = null, ILookupSource[] LookupSources = null)
    {
        GridViewDef ViewDef = GridViewDef.Create(DataView);
        return Create(Grid, DataView, ViewDef, ToolBarPanel, LookupSources);
    }
    static public GridView Create(DataGrid Grid, DataView DataView, GridViewDef ViewDef, StackPanel ToolBarPanel = null, ILookupSource[] LookupSources = null)
    {
        GridViewDefs ViewDefs = new();
        ViewDefs.DefList.Add(ViewDef);
        return Create(Grid, DataView, ViewDefs, ToolBarPanel, LookupSources);
    }
    static public GridView Create(DataGrid Grid, DataView DataView, GridViewDefs ViewDefs = null, StackPanel ToolBarPanel = null, ILookupSource[] LookupSources = null)
    {
        GridView Result = new();
        
        if (LookupSources != null && LookupSources.Length > 0)
            Result.LookupRegistry.AddRange(LookupSources);
        
        Result.Grid = Grid;
        Result.SetSource(DataView);

        if (ViewDefs != null)
        {
            if (ViewDefs.DefList.Count == 0)
            {
                GridViewDef ViewDef = GridViewDef.Create(DataView);
                ViewDefs.DefList.Add(ViewDef);
            }
            
            Result.ViewDefs = ViewDefs;
        }
        else
        {
            GridViewDef ViewDef = GridViewDef.Create(DataView);
            Result.ViewDefs.DefList.Add(ViewDef);
            Result.ViewDef = ViewDef;
        }

        if (ToolBarPanel != null)
            Result.ToolBar.Panel = ToolBarPanel;

        return Result;
    }
    
    static public GridView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, StackPanel ToolBarPanel = null, ILookupSource[] LookupSources = null)
    {
        GridViewDef ViewDef = GridViewDef.Create(typeof(T));
        return Create(Grid, Sequence, ViewDef, ToolBarPanel, LookupSources);
    }
    static public GridView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, GridViewDef ViewDef, StackPanel ToolBarPanel = null, ILookupSource[] LookupSources = null)
    {
        GridViewDefs ViewDefs = new();
        ViewDefs.DefList.Add(ViewDef);
        return Create(Grid, Sequence, ViewDefs, ToolBarPanel, LookupSources);
    }
    static public GridView Create<T>(DataGrid Grid, IEnumerable<T> Sequence, GridViewDefs ViewDefs = null, StackPanel ToolBarPanel = null, ILookupSource[] LookupSources = null)
    {
        GridView Result = new();
        
        if (LookupSources != null && LookupSources.Length > 0)
            Result.LookupRegistry.AddRange(LookupSources);
        
        Result.Grid = Grid;
        Result.SetSource(Sequence);
        
        if (ViewDefs != null)
        {
            if (ViewDefs.DefList.Count == 0)
            {
                GridViewDef ViewDef = GridViewDef.Create(typeof(T));
                ViewDefs.DefList.Add(ViewDef);
            }
            
            Result.ViewDefs = ViewDefs;
        }
        else
        {
            GridViewDef ViewDef = GridViewDef.Create(typeof(T));
            Result.ViewDefs.DefList.Add(ViewDef);
            Result.ViewDef = ViewDef;
        }

        if (ToolBarPanel != null)
            Result.ToolBar.Panel = ToolBarPanel;

        return Result;
    }
    
    // ● public methods
    public void SetSource(DataView DataViewSource)
    {
        this.Controller.SetSource(DataViewSource);
        GridViewSourceChanged();
    }
    public void SetSource<T>(IEnumerable<T> SequenceSource)
    {
        this.Controller.SetSource(SequenceSource);
        GridViewSourceChanged();
    }
    
    public void Close()
    {
        Controller.Close();
        GridViewGridBinder.Refresh(fGrid);
    }
    public void Refresh()
    {
        Controller.Refresh();
    }
 
    public bool MoveTo(int DataIndex)
    {
        return Controller != null && Controller.MoveTo(DataIndex);
    }
    public bool MoveFirst()
    {
        return Controller != null && Controller.MoveFirst();
    }
    public bool MoveLast()
    {
        return Controller != null && Controller.MoveLast();
    }
    public bool MoveNext()
    {
        return Controller != null && Controller.MoveNext();
    }
    public bool MovePrior()
    {
        return Controller != null && Controller.MovePrior();
    }

    public bool MoveToAll(int Index)
    {
        return Controller != null && Controller.MoveToAll(Index);
    }
    public bool MoveFirstAll()
    {
        return Controller != null && Controller.MoveFirstAll();
    }
    public bool MoveLastAll()
    {
        return Controller != null && Controller.MoveLastAll();
    }
    public bool MoveNextAll()
    {
        return Controller != null && Controller.MoveNextAll();
    }
    public bool MovePriorAll()
    {
        return Controller != null && Controller.MovePriorAll();
    }
    
    public bool ExpandAll()
    {
        return Controller != null && Controller.ExpandAll();
    }
    public bool CollapseAll()
    {
        return Controller != null && Controller.CollapseAll();
    }

    public virtual async Task AddItemAsync()
    {
        GridViewItemEventArgs e = CreateArgs(GridViewAction.Add);
        e.Row = null;
        e.DataItem = null;

        if (HasDataItemActionHandler)
        {
            OnItemAction(e);

            if (e.Cancel)
                return;

            if (e.DataItem != null)
                AddDataItem(e.DataItem);

            return;
        }

        if (Controller != null && Controller.ViewSource is DataViewGridViewSource DataViewSource)
        {
            DataRow Row = DataViewSource.Source.Table.NewRow();
            DataViewSource.Source.Table.Rows.Add(Row);
            Refresh();
        }
    }
    public virtual async Task EditItemAsync()
    {
        if (Current == null || !Current.IsData)
            return;

        if (!HasDataItemActionHandler)
            return;

        GridViewItemEventArgs e = CreateArgs(GridViewAction.Edit);

        OnItemAction(e);

        if (e.Cancel)
            return;

        if (e.Handled)
            Refresh();
    }
    public virtual async Task DeleteItemAsync()
    {
        if (Current == null || !Current.IsData || Current.DataItem == null)
            return;

        bool Flag = await MessageBox.YesNo("Delete selected row?");
        if (!Flag)
            return;

        GridViewItemEventArgs e = CreateArgs(GridViewAction.Delete);

        if (HasDataItemActionHandler)
        {
            OnItemAction(e);

            if (e.Cancel)
                return;

            if (e.Handled)
                DeleteDataItem(e.DataItem);

            return;
        }

        DeleteDataItem(e.DataItem);
    }

    public virtual async Task SaveViewDefs()
    {
        // TODO: SaveViewDefs όπως και η  AddItemAsync
    }
    public virtual async Task SaveViewDefsAs()
    {
        // TODO: SaveViewDefsAs όπως και η  AddItemAsync
    }
    
    public DataGridColumn GetColumn(string FieldName) => Grid.Columns.FirstOrDefault(x => FieldName.IsSameText((x.Tag as GridViewColumnDef).FieldName));
    public GridViewColumnDef GetColumnDef(DataGridColumn Column) => Column.Tag as GridViewColumnDef;

    public void UpdateColumnTitles()
    {
        foreach (DataGridColumn Column in Grid.Columns)
            UpdateColumnTitle(Column);
    }
    public void UpdateColumnTitle(DataGridColumn Column)
    {
        if (ViewDef == null)
            return;
        
        GridViewColumnDef ColumnDef = GetColumnDef(Column);
        if (ColumnDef != null)
        {
            string S = ColumnDef.Caption;
            if (ColumnDef.SortIndex >= 0)
            S += ColumnDef.SortDirection == ListSortDirection.Ascending ? $" ↓" : $" ↑";
            Column.Header = S;
        }
    }
    
    public GridViewDef CreateDefaultViewDef() => Controller.ViewSource.CreateDefaultViewDef();
    
    // ● properties
    public DataGrid Grid
    {
        get => fGrid;
        set
        {
            if (fGrid != value)
            {
                if (fGrid != null)
                {
                    fGrid.SelectionChanged -= GridSelectionChanged;
                    
                    Controller.DataChanged -= ControllerDataChanged;
                    Controller.PositionChanged -= ControllerPositionChanged;
 
                    Controller = null;
                    GridViewGridBinder.Unbind(fGrid);
                }

                fGrid = value;

                if (fGrid != null)
                {
                    Controller = new();
                    Controller.DataChanged += ControllerDataChanged;
                    Controller.PositionChanged += ControllerPositionChanged;
                    Controller.Context = this.Context;
                    
                    GridViewGridBinder.Bind(fGrid, Controller);
                    fGrid.SelectionChanged += GridSelectionChanged;
                }
            }
        }
    }
    public DataView DataView
    {
        get => Controller.DataView ;
        set => SetSource(value);
    }
    public GridViewDefs ViewDefs
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
                    throw new ApplicationException($"No {nameof(GridViewDefs)} defined");
                fViewDefs = value;
                if (fViewDefs.DefList.Count > 0)
                    ViewDef = fViewDefs.DefList[0];
            }
        }
    }
    public GridViewDef ViewDef
    {
        get => Controller != null ? Controller.ViewDef : null;
        set
        {
            if (ViewDef != value)
            {
                
                if (value == null)
                    throw new ArgumentNullException(nameof(GridViewDef));
                if (!ViewDefs.DefList.Contains(value))
                    throw new ApplicationException($"{nameof(GridViewDef)} not in {nameof(GridViewDefs)} list");
                Controller.ViewDef = value;
            }
        }
    }
    public GridViewToolBar ToolBar { get; } = new();
    public GridViewMenu Menu { get; } = new();

    public int Position
    {
        get => Controller != null ? Controller.Position : -1;
        set
        {
            if (Controller != null)
                Controller.Position = value;
        }
    }
    public int PositionAll
    {
        get => Controller != null ? Controller.PositionAll : -1;
        set
        {
            if (Controller != null)
                Controller.PositionAll = value;
        }
    }
    public ObservableCollection<GridDataRow> Rows => Controller != null ? Controller.Rows : null;
    public bool IsEmpty => Rows == null || Rows.Count == 0;
    
    public GridViewController Controller { get; private set; } = new();
    public GridDataRow Current => Controller != null ? Controller.Current : null;
    public GridViewSource ViewSource => Controller != null ? Controller.ViewSource : null;
     
    public GridViewContext Context { get; } = new();
    public LookupRegistry LookupRegistry => Context.LookupRegistry;
    
    // ● events
    public event EventHandler<GridViewDataChangedEventArgs> DataChanged;
    public event EventHandler PositionChanged;
    public event EventHandler<GridViewItemEventArgs> DataItemAction;
}