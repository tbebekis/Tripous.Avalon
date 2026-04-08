using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using Avalonia.Controls;
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
    //private ContextMenu fMenu;
 
    // ● overridables
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
    
    // ● constructors
    public GridView()
    {
        ToolBar.GridView = this;
    }
    
    // ● public methods
    /*
    public void Open(DataView Source, GridViewDef Def = null)
    {
        Controller.Open(Source, Def);
    }
    public void Open<T>(IEnumerable<T> Source, GridViewDef Def = null)
    {
        Controller.Open(Source, Def);
    }
    public void Open(GridViewSource Source, GridViewDef Def = null)
    {
        Controller.Open(Source, Def);
    }
    public void ApplyViewDef(GridViewDef Def)
    {
        if (Controller == null)
            throw new ApplicationException("GridView has no controller.");

        Controller.ApplyViewDef(Def);
    }
    */
    public void SetSource(DataView DataViewSource)
    {
        this.DataView = DataViewSource;
    }
    public void SetSource<T>(IEnumerable<T> SequenceSource)
    {
        this.Controller.SetSource(SequenceSource);
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
                    GridViewGridBinder.Bind(fGrid, Controller);
                    fGrid.SelectionChanged += GridSelectionChanged;
                }
            }
        }
    }
    public DataView DataView
    {
        get => Controller.DataView ;
        set => Controller.DataView = value;
    }
    public GridViewDefs ViewDefs
    {
        get => fViewDefs;
        set
        {
            if (fViewDefs != value)
            {
                if (value == null)
                    throw new ApplicationException(nameof(GridViewDefs));
                fViewDefs = value;
                if (fViewDefs.DefList.Count > 0)
                    ViewDef = fViewDefs.DefList[0];
            }
        }
    }
    public GridViewDef ViewDef
    {
        get => Controller != null ? Controller.ViewDef : null;
        set => Controller.ViewDef = value;
    }
    public GridViewToolBar ToolBar { get; } = new();
    //public ContextMenu Menu => fMenu; // EDW

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
    
    // ● events
    public event EventHandler<GridViewDataChangedEventArgs> DataChanged;
    public event EventHandler PositionChanged;
    public event EventHandler<GridViewItemEventArgs> DataItemAction;
}