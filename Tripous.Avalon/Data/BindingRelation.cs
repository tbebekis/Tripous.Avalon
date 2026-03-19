namespace Tripous.Avalon.Data;

public class BindingRelation
{
    private List<string> fParentPropertyNames = new();
    private List<string> fChildPropertyNames = new();

    public BindingRelation(string Name, BindingSource Parent, string ParentPropertyName, BindingSource Child, string ChildPropertyName)
        : this(Name, Parent, new[] { ParentPropertyName }, Child, new[] { ChildPropertyName })
    {
    }

    public BindingRelation(string Name, BindingSource Parent, string[] ParentPropertyNames, BindingSource Child, string[] ChildPropertyNames)
    {
        this.Name = Name;
        this.Parent = Parent;
        this.Child = Child;
        
        this.fParentPropertyNames.AddRange(ParentPropertyNames);
        this.fChildPropertyNames.AddRange(ChildPropertyNames);

        // Σύνδεση με το event του Parent
        if (this.Parent != null)
        {
            this.Parent.OnCurrentPositionChanged += (s, e) => ChildSync();
        }
    }

    public void AddRelatedProperties(string ParentPropertyName, string ChildPropertyName)
    {
        fParentPropertyNames.Add(ParentPropertyName);
        fChildPropertyNames.Add(ChildPropertyName);
    }

    public void ChildSync()
    {
        var row = Parent.Current;
        if (row == null)
        {
            Child.DataSource.ApplyChildSync(null, null);
            return;
        }

        object[] values = ParentPropertyNames.Select(p => row[p]).ToArray();
        Child.DataSource.ApplyChildSync(ParentPropertyNames, values);
    }

    /* Properties */
    public string Name { get; private set; }
    public BindingSet BindingSet { get; set; }
    public BindingSource Parent { get; private set; }
    public BindingSource Child { get; private set; }
    public string[] ParentPropertyNames => fParentPropertyNames.ToArray();
    public string[] ChildPropertyNames => fChildPropertyNames.ToArray();
}