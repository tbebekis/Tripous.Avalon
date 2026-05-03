namespace Tripous;

/// <summary>
/// A robust Tripous observable list that wraps a List&lt;T&gt; and implements all collection interfaces
/// </summary>
public class TripousList<T> : IList<T>, IList, ICollection<T>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged
{
    // ● private fields
    protected List<T> Items = [];

    // ● overridables
    protected virtual void CheckAdding(T Item)
    {
        if (Item == null)
            throw new ArgumentNullException(nameof(Item));
        if (Items.Contains(Item))
            throw new Exception($"{nameof(Item)} instance '{Item}' is already in the collection.");
    }
    protected virtual void CheckRemoving(T Item)
    {
    }
    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction Action, object Item, int Index)
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(Action, Item, Index));
    }
    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction Action, object NewItem, object OldItem, int Index)
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(Action, NewItem, OldItem, Index));
    }
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs E)
    {
        CollectionChanged?.Invoke(this, E);
    }
    protected virtual void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    // ● constructors
    public TripousList()
    {
    }

    // ● public 
    public void Add(T Item)
    {
        CheckAdding(Item);
        Items.Add(Item);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, Item, Items.Count - 1);
        OnPropertyChanged(nameof(Count));
    }
    public void Insert(int Index, T Item)
    {
        CheckAdding(Item);
        Items.Insert(Index, Item);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, Item, Index);
        OnPropertyChanged(nameof(Count));
    }
    public bool Remove(T Item)
    {
        int index = Items.IndexOf(Item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }
    public void RemoveAt(int Index)
    {
        T item = Items[Index];
        CheckRemoving(item);
        Items.RemoveAt(Index);
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, Index);
        OnPropertyChanged(nameof(Count));
    }
    public void AddRange(IEnumerable<T> Source)
    {
        foreach (T Item in Source)
            Add(Item);
    }
    public void Clear()
    {
        Items.Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(nameof(Count));
    }
    public int IndexOf(T Item) => Items.IndexOf(Item);
    public bool Contains(T Item) => Items.Contains(Item);
    public void CopyTo(T[] Array, int ArrayIndex) => Items.CopyTo(Array, ArrayIndex);

    // ● properties
    public int Count => Items.Count;
    public bool IsReadOnly => false;
    [JsonIgnore]
    public T this[int Index]
    {
        get => Items[Index];
        set
        {
            T oldItem = Items[Index];
            CheckAdding(value);
            Items[Index] = value;
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, oldItem, Index);
        }
    }

    // ● interface implementation (IList, ICollection)
    int IList.Add(object value) { Add((T)value); return Count - 1; }
    bool IList.Contains(object value) => Contains((T)value);
    int IList.IndexOf(object value) => IndexOf((T)value);
    void IList.Insert(int index, object value) => Insert(index, (T)value);
    void IList.Remove(object value) => Remove((T)value);
    bool IList.IsFixedSize => false;
    object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }
    void ICollection.CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ● events
    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;
}