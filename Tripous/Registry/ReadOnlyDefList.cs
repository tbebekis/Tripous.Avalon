namespace Tripous;

public class ReadOnlyDefList<T> where T: IDef
{
    private readonly DefList<T> InternalDefList;

    // ● construction
    public ReadOnlyDefList(DefList<T> DefList)
    {
        this.InternalDefList = DefList;
    }
 
    // ● public
    public bool Contains(string Name) => InternalDefList.Contains(Name);
    public T Find(string Name) => InternalDefList.Find(Name);
    public T Get(string Name) => InternalDefList.Get(Name);

    public IEnumerator<T> GetEnumerator() => InternalDefList.GetEnumerator();
    
    // ● properties
    public T this[string Name] => Get(Name);
    public int Count => InternalDefList.Count;
}