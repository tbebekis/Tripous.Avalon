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
    private GridViewController fController;
    private GridViewToolBar fToolBar;
    //private ContextMenu fMenu;
    
    // ● protected methods
    
    
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
    protected virtual void Initialize()
    {
        fController = new GridViewController();
        fController.DataChanged += ControllerDataChanged;
        fController.PositionChanged += ControllerPositionChanged;

        fGrid.SelectionChanged += GridSelectionChanged;
    }
    protected virtual void Bind()
    {
        GridViewGridBinder.Bind(fGrid, fController);
    }
    protected virtual void Unbind()
    {
        GridViewGridBinder.Unbind(fGrid);
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
        if (DataItem == null || Controller == null || Controller.Source == null)
            return false;

        if (Controller.Source is DataViewGridViewSource DataViewSource)
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

        IList ListSource = Controller.Source.ListSource;
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
        if (DataItem == null || Controller == null || Controller.Source == null)
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

        IList ListSource = Controller.Source.ListSource;
        if (ListSource != null)
        {
            ListSource.Remove(DataItem);
            Refresh();
            return true;
        }

        return false;
    }
    
    // ● constructors
    public GridView(DataGrid Grid, StackPanel ToolBarPanel = null)
    {
        fGrid = Grid ?? throw new ArgumentNullException(nameof(Grid));
        if (ToolBarPanel != null)
            fToolBar = new GridViewToolBar(ToolBarPanel, this);
        Initialize();
        Bind();
    }

    // ● public methods
    public void Open(DataView Source, GridViewDef Def = null)
    {
        fController.Open(Source, Def);
    }
    public void Open<T>(IEnumerable<T> Source, GridViewDef Def = null)
    {
        fController.Open(Source, Def);
    }
    public void Open(GridViewSource Source, GridViewDef Def = null)
    {
        fController.Open(Source, Def);
    }
    public void Close()
    {
        fController.Close();
        GridViewGridBinder.Refresh(fGrid);
    }
    public void Refresh()
    {
        fController.Refresh();
    }

    public bool MoveTo(int DataIndex)
    {
        return fController != null && fController.MoveTo(DataIndex);
    }
    public bool MoveFirst()
    {
        return fController != null && fController.MoveFirst();
    }
    public bool MoveLast()
    {
        return fController != null && fController.MoveLast();
    }
    public bool MoveNext()
    {
        return fController != null && fController.MoveNext();
    }
    public bool MovePrior()
    {
        return fController != null && fController.MovePrior();
    }

    public bool MoveToAll(int Index)
    {
        return fController != null && fController.MoveToAll(Index);
    }
    public bool MoveFirstAll()
    {
        return fController != null && fController.MoveFirstAll();
    }
    public bool MoveLastAll()
    {
        return fController != null && fController.MoveLastAll();
    }
    public bool MoveNextAll()
    {
        return fController != null && fController.MoveNextAll();
    }
    public bool MovePriorAll()
    {
        return fController != null && fController.MovePriorAll();
    }
    
    public bool ExpandAll()
    {
        return fController != null && fController.ExpandAll();
    }
    public bool CollapseAll()
    {
        return fController != null && fController.CollapseAll();
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

        if (Controller != null && Controller.Source is DataViewGridViewSource DataViewSource)
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
    public GridViewController Controller => fController;
    public GridDataRow Current => fController != null ? fController.Current : null;
    public DataGrid Grid => fGrid;
    //public ContextMenu Menu => fMenu;
    public int Position
    {
        get => fController != null ? fController.Position : -1;
        set
        {
            if (fController != null)
                fController.Position = value;
        }
    }
    public int PositionAll
    {
        get => fController != null ? fController.PositionAll : -1;
        set
        {
            if (fController != null)
                fController.PositionAll = value;
        }
    }
    public ObservableCollection<GridDataRow> Rows => fController != null ? fController.Rows : null;
    public GridViewToolBar ToolBar => fToolBar;
    public GridViewDef ViewDef
    {
        get => fController != null ? fController.ViewDef : null;
    }
 
    
    // ● events
    public event EventHandler<GridViewDataChangedEventArgs> DataChanged;
    public event EventHandler PositionChanged;
    public event EventHandler<GridViewItemEventArgs> DataItemAction;
}