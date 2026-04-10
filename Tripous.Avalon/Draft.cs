using System.Collections.ObjectModel;
using System.Data;

namespace Draft;

public interface IDataSource
{
    void ApplyFilter(string PropertyName, object Value);
    string Name { get;  }
}

public class BindingSourceRow  
{
    //public object this[string PropertyName] { get; set; }
    public BindingSource BindingSource { get; private set; }
    public object InnerObject { get; private set; }
    public DataRowState RowState { get; private set; }
}

public class BindingSource
{
    public void ApplyFilter(string PropertyName, object Value)
    {
        
    }
    
    public string Name { get; private set; }
    public IDataSource DataSource { get; protected set; }
    public ObservableCollection<BindingSourceRow> Rows { get; }
}

public class BindingRelation
{
 
    public BindingRelation(string Name, BindingSource Parent, string ParentPropertyName, BindingSource Child, string ChildPropertyName)
    {}
    public BindingRelation(string Name, BindingSource Parent, string[] ParentPropertyNames, BindingSource Child, string[] ChildPropertyNames)
    {}

    public void AddRelatedProperties(string ParentPropertyName, string ChildPropertyName)
    {}

    public void Sync()
    {
    }
    
    public string Name { get; private set; }
    public BindingSet BindingSet { get; set; }

    public BindingSource Parent { get; private set; }
    public BindingSource Child  { get; private set; }

    public string[] ParentPropertyNames { get; }
    public string[] ChildPropertyNames { get; }
}


public class BindingSet
{
    public BindingSource AddSource(IDataSource DataSource)
    {
        return null;
    }
    internal BindingRelation AddRelation(string Name, BindingSource Parent, string[] ParentPropertyNames, BindingSource Child, string[] ChildPropertyNames)
    {
        return null;
    }
    public void RemoveRelation(BindingSource BindingSource)
    {}
    
    public string Name { get; private set; }
    public BindingSource[] BindingSources { get; }
    public BindingRelation[] BindingRelations { get; }
}



/* Locator
 
 A bindable class, it binds just as a control using the Binding class.
 
 It can control either a LocatorBox control which displays its DisplayPropertyNames,
 or it can control DataGrid which displays Columns with its DisplayPropertyNames 
 */
public class Locator
{
    public Locator(string Name, string ValuePropertyName, string DisplayPropertyName)
    {}
    public Locator(string Name, string ValuePropertyName, string[] DisplayPropertyNames)
    {}
    
    public BindingSource DataSource { get; private set; } // the one that has the ValuePropertyName
    public string Name { get; private set; }
    public string ValuePropertyName { get; }
    public string[] DisplayPropertyNames { get; }
    public BindingSource LookupSource { get; }

    public int TermMinLength { get; set; } = 3;
    public char TermTerminator { get; set; } = '*';
    public bool RequiresEnterKey { get; set; } = true;
    public int MaxDropDownRows { get; set; } = 80;
    public string DataWindowName { get; set; }

    public BindingSource ApplyFilter(string PropertyName, object Value)
    {
        // return the LookUpSource
        return null;
    }
    
}
/*
public enum AggregateType
{
    None,
    Sum,
    Avg,
    Count,
    Min,
    Max, 
}
*/
 

// ---------------------------------------------

public enum DataGridExMode
{
    None, // normal
    Group,
    Pivot,
}

public class DataGridExDataSource
{
    private object fDataSource;
    
    // ● overridable
    protected virtual void DataSourceChanged()
    {
        // process fDataSource according to Grid.Mode and then assign the Grid.DataModel
        Grid.DataModel = this.DataModel;
    }
    
    // ● construction
    public DataGridExDataSource(DataGridEx Grid)
    {
        this.Grid = Grid;
    }

    // ● properties
    public DataGridEx Grid { get; }
    public object DataSource
    {
        get => fDataSource;
        set
        {
            if (fDataSource != value)
            {
                if (value == null || value is DataView || value is IEnumerable)
                {
                    fDataSource = null;
                }
                else if (value is DataTable)
                {
                    fDataSource = (value as DataTable).DefaultView;
                }
                else
                {
                    throw new ApplicationException($"DataSource not supported: {value.GetType()}");
                }

                DataSourceChanged();
            }
        }
    }
    public object DataModel { get; private set; }
}

public class DataGridExCustomButton
{
    public string ToolTip { get; set; }
    public string Image { get; set; } // δεν ξέρω τι τύπο να βάλω, ποιο είναι το σωστό εδώ
    public int OrdinalIndex { get; set; }

    //public event EventHandler<RoutedEventArgs> Click;
}

public class DataGridExColumnInfo
{
    internal DataGridExColumnInfo(DataGridColumn GridColumn, DataColumn TableColumn)
    {
        this.TableColumn = TableColumn;
        this.GridColumn = GridColumn;

        FieldName = TableColumn.ColumnName;
        DataType = TableColumn.DataType;
        UnderlyingType = Nullable.GetUnderlyingType(DataType) ?? DataType;
        
        //GridColumn.ColumnKey = FieldName;
    }
    internal DataGridExColumnInfo(DataGridColumn GridColumn, PropertyInfo PropertyInfo)
    {
        this.PropertyInfo = PropertyInfo;
        this.GridColumn = GridColumn;

        FieldName = PropertyInfo.Name;
        DataType = PropertyInfo.PropertyType;
        UnderlyingType = Nullable.GetUnderlyingType(DataType) ?? DataType;
        
        //GridColumn.ColumnKey = FieldName;
    }
    
    public DataGridColumn GridColumn { get; }
    public DataColumn TableColumn { get; }
    public PropertyInfo PropertyInfo { get; }
    public string FieldName { get; }
    public Type DataType  { get; }
    public Type UnderlyingType { get; }
    public bool IsString => UnderlyingType.IsString();  
    public bool IsDate => UnderlyingType.IsDateTime(); 
    public bool IsNumeric => UnderlyingType.IsNumeric();
    public bool IsRowFilterSupportedColumn => IsString || IsNumeric || IsDate;

    public AggregateType[] ValidAggregates => UnderlyingType.GetValidAggregates();
}

/// <summary>
/// No complex binding path for columns. We just use either the FieldName (DataView) or the PropertyName (Poco) 
/// </summary>
public class DataGridEx
{
    // ● private
    private DataGridExMode fMode;
    private DataGridExDataSource fDataSource;
    private object fDataModel;
    private GridViewDef fViewDef;
    // CHECK private PivotDef fPivotDef;
    private List<DataGridExCustomButton> Buttons = new();
    private List<DataGridExColumnInfo> ColumnInfoListInternal = new();
    
    // ● overridable
    protected virtual void ModeChanged()
    {
        // whatever
    }
    protected virtual void DataSourceChanged()
    {
        // whatever
    }
    protected virtual void DataModelChanged()
    {
        // use DataModel to fill the ColumnInfoListInternal
        // use DataModel according to Grid.Mode in order to bind to grid and render data using the proper renderer
    }
    protected virtual void ViewDefChanged()
    {
    }
    protected virtual void PivotDefChanged()
    {
    }
    
    // ● construction
    public DataGridEx()
    {
    }
    
    // ● public
    public DataGridExCustomButton AddButton(string ToolTip, string Image)
    {
        DataGridExCustomButton Result = new() { ToolTip = ToolTip, Image = Image };
        Buttons.Add(Result);
        Result.OrdinalIndex = Buttons.IndexOf(Result);
        // TODO: Adjust UI
        return Result;
    }
    public void RemoveButton(DataGridExCustomButton CustomButton)
    {
        Buttons.Remove(CustomButton);
        // TODO: Adjust UI
    }
    
    // ● properties
    public DataGridExMode Mode     {
        get => fMode;
        set
        {
            if (fMode != value)
            {
                fMode = value;
                ModeChanged();
            }
        }
    }
    public DataGridExDataSource DataSource
    {
        get => fDataSource;
        set
        {
            if (fDataSource != value)
            {
                fDataSource = value;
                DataSourceChanged();
            }
        }
    }
    public object DataModel
    {
        get => fDataModel;
        internal set
        {
            if (fDataModel != value)
            {
                fDataModel = value;
                DataModelChanged();
            }
        }
    }
    public GridViewDef ViewDef
    {
        get => fViewDef;
        set
        {
            if (fViewDef != value)
            {
                fViewDef = value;
                ViewDefChanged();
            }
        }
    }
    // CHECK
    // public PivotDef PivotDef
    // {
    //     get => fPivotDef;
    //     set
    //     {
    //         if (fPivotDef != value)
    //         {
    //             fPivotDef = value;
    //             PivotDefChanged();
    //         }
    //     }
    // }
    
    public bool IsGroupPanelVisible { get; set; }   // top panel where the user drags n drops column headers for grouping
    public bool IsBottomPanelVisible { get; set; }  // bottom summary panel (grand totals, etc)
    public bool IsToolBarVisible { get; set; }
    public bool IsConfigButtonVisible { get; set; } // displays the proper configuration dialog, i.e. GroupSumDefDialog or PivotDefDialog
    public bool IsAddButtonVisible { get; set; }
    public bool IsEditButtonVisible { get; set; }
    public bool IsDeleteButtonVisible { get; set; }

    public IReadOnlyList<DataGridExColumnInfo> ColumnInfoList => ColumnInfoListInternal;
}