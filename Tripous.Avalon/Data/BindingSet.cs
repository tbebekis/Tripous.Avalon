namespace Tripous.Avalon.Data;

public class BindingSet
{
    private List<BindingSource> fSources = new();
    private List<BindingRelation> fRelations = new();

    public BindingSet(string fName)
    {
        this.Name = fName;
    }

    public BindingSource AddSource(IDataSource fDataSource)
    {
        var source = new BindingSource(fDataSource);
        fSources.Add(source);
        return source;
    }

    internal BindingRelation AddRelation(string fName, BindingSource fParent, string[] fParentPropertyNames, BindingSource fChild, string[] fChildPropertyNames)
    {
        var relation = new BindingRelation(fName, fParent, fParentPropertyNames, fChild, fChildPropertyNames);
        relation.BindingSet = this;
        fRelations.Add(relation);
        
        // Εκτελούμε ένα αρχικό Sync
        relation.ChildSync();
        
        return relation;
    }

    public void RemoveRelation(BindingRelation fRelation)
    {
        if (fRelations.Contains(fRelation))
        {
            fRelation.BindingSet = null;
            fRelations.Remove(fRelation);
        }
    }

    /* Properties */
    public string Name { get; private set; }
    public BindingSource[] BindingSources => fSources.ToArray();
    public BindingRelation[] BindingRelations => fRelations.ToArray();
}