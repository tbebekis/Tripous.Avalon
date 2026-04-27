namespace Tripous.Models;

public class DefList<T> where T: IDef
{
    // ● private
    private List<T> List = new();

    // ● overridables
    protected virtual void CheckAdding(T Def)
    {
        if (Def == null)
            throw new ArgumentNullException(nameof(Def));
        
        if (string.IsNullOrWhiteSpace(Def.Name))
            throw new ArgumentNullException(nameof(Def.Name));

        if (Contains(Def.Name))
            throw new ApplicationException($"{nameof(Def)} '{Def.Name}' is already registered.");
    }
    
    // ● public
    public T Add(T Def)
    {
        CheckAdding(Def);
        List.Add(Def);
        return Def;
    }
    public void Remove(T Def) => List.Remove(Def);

    public bool Contains(string Name) => List.FirstOrDefault(x => Sys.IsSameText(Name, x.Name)) != null;
    public T Find(string Name) => List.FirstOrDefault(x => Sys.IsSameText(Name, x.Name));
    public T Get(string Name)
    {
        T Result  = List.FirstOrDefault(x => Sys.IsSameText(Name, x.Name));
        if (Result == null)
            throw new ApplicationException($"{typeof(T)} not found: {Name}");
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
    
    // ● properties
    public ReadOnlyCollection<T> Items => List.AsReadOnly();
}