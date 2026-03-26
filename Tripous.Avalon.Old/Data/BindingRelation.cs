namespace Tripous.Avalon.Data;

public class BindingRelation
{
    // ● construction
    public BindingRelation(string Name, BindingSource Parent, string ParentPropertyName, BindingSource Child, string ChildPropertyName)
        : this(Name, Parent, new[] { ParentPropertyName }, Child, new[] { ChildPropertyName })
    {
    }
    public BindingRelation(string Name, BindingSource Parent, string[] ParentPropertyNames, BindingSource Child, string[] ChildPropertyNames)
    {
        this.Name = Name;
        this.Parent = Parent;
        this.Child = Child;
        
        this.ParentPropertyNames = ParentPropertyNames;
        this.ChildPropertyNames = ChildPropertyNames;

        // connect to parent event
        if (this.Parent != null || this.Child != null)
        {
            this.Parent.OnCurrentPositionChanged += (s, e) =>
            {
                if (!Parent.DetailsActive)
                    return;

                object[] MasterValues = Parent.GetMasterValues(this);
                Child.VisibleRowsChanged(MasterValues);
            };
        }
    }

    // ● Properties
    public string Name { get; private set; }
    public BindingSource Parent { get; private set; }
    public BindingSource Child { get; private set; }
    public string[] ParentPropertyNames { get; }
    public string[] ChildPropertyNames { get; }
}