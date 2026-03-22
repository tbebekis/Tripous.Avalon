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