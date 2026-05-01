namespace Tripous;

public class DefList<T>: IEnumerable<T>, IEnumerable where T: IDef
{
    // ● private
    private List<T> List = new();

    // ● overridables
    protected virtual void CheckAdding(T Def)
    {
        if (Def == null)
            throw new TripousArgumentNullException(nameof(Def));
        
        if (string.IsNullOrWhiteSpace(Def.Name))
            throw new TripousArgumentNullException(nameof(Def.Name));

        if (Contains(Def.Name))
            throw new TripousException($"{nameof(Def)} '{Def.Name}' is already registered.");
        
        if (List.Contains(Def))
            throw new TripousException($"{nameof(Def)} instance '{Def}' is already registered.");
    }
    
    // ● public
    public DefList()
    {
    }

    // ● public
    public T Add(T Def)
    {
        CheckAdding(Def);
        List.Add(Def);
        return Def;
    }

    public void AddRange(IEnumerable<T> Items)
    {
        foreach (T Item in Items)
            Add(Item);
    }
    public void Remove(T Def) => List.Remove(Def);
    

    public bool Contains(string Name) => List.FirstOrDefault(x => Sys.IsSameText(Name, x.Name)) != null;
    public T Find(string Name) => List.FirstOrDefault(x => Sys.IsSameText(Name, x.Name));
    public T Get(string Name)
    {
        T Result  = List.FirstOrDefault(x => Sys.IsSameText(Name, x.Name));
        if (Result == null)
            throw new TripousException($"{typeof(T)} not found: {Name}");
        return Result;
    }

    public int IndexOf(string Name)
    {
        T Def = Find(Name);
        if (Def != null)
            return List.IndexOf(Def);
        return -1;
    }
    public void InsertBefore(string BeforeName, T Def)
    {
        T Other = Find(BeforeName);
        if (Other != null)
        {
            int Index = List.IndexOf(Other);
            if (Index != -1)
            {
                Index--;
                if (Index < 0)
                    Index = 0;
                List.Insert(Index, Def);
            }
        }
    }
    public void InsertAfter(string AfterName, T Def)
    {
        T Other = Find(AfterName);
        if (Other != null)
        {
            int Index = List.IndexOf(Other);
            if (Index != -1)
            {
                Index++;
                if (Index > List.Count - 1)
                    List.Add(Def);
                else
                    List.Insert(Index, Def);
            }
        }
    }
    
    public IEnumerator<T> GetEnumerator() => List.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();
    
    // ● properties
    public T this[string Name] => Get(Name);
    public int Count => List.Count;
    public ReadOnlyCollection<T> Items => List.AsReadOnly();
    
    
 
}

