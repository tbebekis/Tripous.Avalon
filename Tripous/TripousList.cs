/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Represents an observable list that wraps a <see cref="List{T}"/>
/// and implements both generic and non-generic collection interfaces.
/// </summary>
public class TripousList<T> : IList<T>, IList, ICollection<T>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged
{
    // ● private fields
    /// <summary>
    /// Field
    /// </summary>
    protected List<T> Items = [];

    // ● overridables
    /// <summary>
    /// Checks whether an item can be added to the list.
    /// </summary>
    protected virtual void CheckAdding(T Item)
    {
        if (Item == null)
            throw new ArgumentNullException(nameof(Item));
        if (Items.Contains(Item))
            throw new Exception($"{nameof(Item)} instance '{Item}' is already in the collection.");
    }
    /// <summary>
    /// Checks whether an item can be removed from the list.
    /// </summary>
    protected virtual void CheckRemoving(T Item)
    {
    }
    /// <summary>
    /// Raises the CollectionChanged event for a single item change.
    /// </summary>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction Action, object Item, int Index)
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(Action, Item, Index));
    }
    /// <summary>
    /// Raises the CollectionChanged event for a replace operation.
    /// </summary>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction Action, object NewItem, object OldItem, int Index)
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(Action, NewItem, OldItem, Index));
    }
    /// <summary>
    /// Raises the CollectionChanged event.
    /// </summary>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs E)
    {
        CollectionChanged?.Invoke(this, E);
    }
    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public TripousList()
    {
    }

    // ● public 
    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    public void Add(T Item)
    {
        CheckAdding(Item);
        Items.Add(Item);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, Item, Items.Count - 1);
        OnPropertyChanged(nameof(Count));
    }
    /// <summary>
    /// Inserts an item at the specified index.
    /// </summary>
    public void Insert(int Index, T Item)
    {
        CheckAdding(Item);
        Items.Insert(Index, Item);
        OnCollectionChanged(NotifyCollectionChangedAction.Add, Item, Index);
        OnPropertyChanged(nameof(Count));
    }
    /// <summary>
    /// Removes an item from the list.
    /// </summary>
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
    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    public void RemoveAt(int Index)
    {
        T item = Items[Index];
        CheckRemoving(item);
        Items.RemoveAt(Index);
        OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, Index);
        OnPropertyChanged(nameof(Count));
    }
    /// <summary>
    /// Adds all items of a source collection to the list.
    /// </summary>
    public void AddRange(IEnumerable<T> Source)
    {
        foreach (T Item in Source)
            Add(Item);
    }
    /// <summary>
    /// Removes all items from the list.
    /// </summary>
    public void Clear()
    {
        if (Items.Count == 0)
            return;

        Items.Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(nameof(Count));
    }
    /// <summary>
    /// Returns the index of the specified item.
    /// </summary>
    public int IndexOf(T Item) => Items.IndexOf(Item);
    /// <summary>
    /// Returns true when the specified item exists in the list.
    /// </summary>
    public bool Contains(T Item) => Items.Contains(Item);
    /// <summary>
    /// Copies the list items to an array.
    /// </summary>
    public void CopyTo(T[] Array, int ArrayIndex) => Items.CopyTo(Array, ArrayIndex);

    // ● properties
    /// <summary>
    /// Gets the number of items in the list.
    /// </summary>
    public int Count => Items.Count;
    /// <summary>
    /// Gets a value indicating whether the list is read-only.
    /// </summary>
    public bool IsReadOnly => false;
    /// <summary>
    /// Gets or sets an item by index.
    /// </summary>
    [JsonIgnore]
    public T this[int Index]
    {
        get => Items[Index];
        set
        {
            T oldItem = Items[Index];
            if (object.ReferenceEquals(oldItem, value))
                return;

            CheckAdding(value);
            Items[Index] = value;
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, oldItem, Index);
        }
    }

    // ● interface implementation
    /// <summary>
    /// Adds an item through the non-generic IList interface.
    /// </summary>
    int IList.Add(object value) { Add((T)value); return Count - 1; }
    /// <summary>
    /// Returns true when an item exists through the non-generic IList interface.
    /// </summary>
    bool IList.Contains(object value) => Contains((T)value);
    /// <summary>
    /// Returns the index of an item through the non-generic IList interface.
    /// </summary>
    int IList.IndexOf(object value) => IndexOf((T)value);
    /// <summary>
    /// Inserts an item through the non-generic IList interface.
    /// </summary>
    void IList.Insert(int index, object value) => Insert(index, (T)value);
    /// <summary>
    /// Removes an item through the non-generic IList interface.
    /// </summary>
    void IList.Remove(object value) => Remove((T)value);
    /// <summary>
    /// Gets a value indicating whether the non-generic list has a fixed size.
    /// </summary>
    bool IList.IsFixedSize => false;
    /// <summary>
    /// Gets or sets an item through the non-generic IList interface.
    /// </summary>
    object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }
    /// <summary>
    /// Copies the list items to an array through the non-generic ICollection interface.
    /// </summary>
    void ICollection.CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);
    /// <summary>
    /// Gets a value indicating whether access to the collection is synchronized.
    /// </summary>
    bool ICollection.IsSynchronized => false;
    /// <summary>
    /// Gets an object that can be used to synchronize access to the collection.
    /// </summary>
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    /// <summary>
    /// Returns a generic enumerator for the list.
    /// </summary>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    /// <summary>
    /// Returns a non-generic enumerator for the list.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ● events
    /// <summary>
    /// Occurs when the list changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}
