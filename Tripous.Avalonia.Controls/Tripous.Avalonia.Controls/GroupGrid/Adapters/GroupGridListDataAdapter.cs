// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Adapts a list of POCO objects to the <see cref="IGroupGridDataAdapter"/> contract.
/// </summary>
/// <typeparam name="T">The row type.</typeparam>
public class GroupGridListDataAdapter<T>: IGroupGridDataAdapter, IDisposable
{
    // ● private fields
    readonly IList<T> fItems;
    readonly Dictionary<string, PropertyInfo> fProperties;
    bool fDisposed;

    // ● private methods
    PropertyInfo FindProperty(GroupGridColumn Column)
    {
        string Name = Column == null ? string.Empty : Column.Name;
        if (string.IsNullOrWhiteSpace(Name))
            return null;

        fProperties.TryGetValue(Name, out PropertyInfo Result);
        return Result;
    }
    void SubscribeRows(IEnumerable<T> Items)
    {
        foreach (T Item in Items)
            if (Item is INotifyPropertyChanged Notifier)
                Notifier.PropertyChanged += Item_PropertyChanged;
    }
    void UnsubscribeRows(IEnumerable<T> Items)
    {
        foreach (T Item in Items)
            if (Item is INotifyPropertyChanged Notifier)
                Notifier.PropertyChanged -= Item_PropertyChanged;
    }
    void Items_CollectionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
    {
        if (Args.OldItems != null)
            UnsubscribeRows(Args.OldItems.Cast<T>());
        if (Args.NewItems != null)
            SubscribeRows(Args.NewItems.Cast<T>());

        if (Args.Action == NotifyCollectionChangedAction.Add && Args.NewStartingIndex >= 0)
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowAdded(Args.NewStartingIndex));
        else if (Args.Action == NotifyCollectionChangedAction.Remove && Args.OldStartingIndex >= 0)
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowRemoved(Args.OldStartingIndex));
        else if (Args.Action == NotifyCollectionChangedAction.Move && Args.NewStartingIndex >= 0 && Args.OldStartingIndex >= 0)
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowMoved(Args.OldStartingIndex, Args.NewStartingIndex));
        else
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.Reset());
    }
    void Item_PropertyChanged(object Sender, PropertyChangedEventArgs Args)
    {
        if (Sender is not T Item)
            return;

        int RowIndex = fItems.IndexOf(Item);
        if (RowIndex < 0)
            return;

        if (string.IsNullOrWhiteSpace(Args.PropertyName))
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowChanged(RowIndex));
        else
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.CellChanged(RowIndex, Args.PropertyName));
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridListDataAdapter{T}"/> class.
    /// </summary>
    /// <param name="Items">The source item list.</param>
    public GroupGridListDataAdapter(IList<T> Items)
    {
        fItems = Items ?? throw new ArgumentNullException(nameof(Items));
        fProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(Property => Property.GetIndexParameters().Length == 0)
            .ToDictionary(Property => Property.Name, StringComparer.OrdinalIgnoreCase);

        SubscribeRows(fItems);

        if (fItems is INotifyCollectionChanged Notifier)
            Notifier.CollectionChanged += Items_CollectionChanged;
    }

    // ● public methods
    /// <inheritdoc />
    public object GetRow(int RowIndex) => fItems[RowIndex];
    /// <inheritdoc />
    public object GetValue(int RowIndex, GroupGridColumn Column)
    {
        PropertyInfo Property = FindProperty(Column);
        return Property == null ? null : Property.GetValue(fItems[RowIndex]);
    }
    /// <inheritdoc />
    public void SetValue(int RowIndex, GroupGridColumn Column, object Value)
    {
        PropertyInfo Property = FindProperty(Column);
        if (Property == null || !Property.CanWrite || Column == null || Column.IsReadOnly)
            return;

        object OldValue = Property.GetValue(fItems[RowIndex]);
        object ParsedValue = Column.ParseValue(Value);
        if (Equals(OldValue, ParsedValue))
            return;

        bool RaisesPropertyChanged = fItems[RowIndex] is INotifyPropertyChanged;
        Property.SetValue(fItems[RowIndex], ParsedValue);
        if (!RaisesPropertyChanged)
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.CellChanged(RowIndex, Column.Name));
    }
    /// <inheritdoc />
    public bool CanSetValue(int RowIndex, GroupGridColumn Column)
    {
        PropertyInfo Property = FindProperty(Column);
        return Property != null && Property.CanWrite && Column != null && !Column.IsReadOnly;
    }
    /// <inheritdoc />
    public bool CanInsertRow(int RowIndex)
    {
        return !typeof(T).IsAbstract && typeof(T).GetConstructor(Type.EmptyTypes) != null;
    }
    /// <inheritdoc />
    public int InsertRow(int RowIndex)
    {
        if (!CanInsertRow(RowIndex))
            return -1;

        int NewIndex = Math.Clamp(RowIndex + 1, 0, fItems.Count);
        T Item = (T)Activator.CreateInstance(typeof(T));
        fItems.Insert(NewIndex, Item);
        if (fItems is not INotifyCollectionChanged)
        {
            SubscribeRows(new[] { Item });
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowAdded(NewIndex));
        }

        return NewIndex;
    }
    /// <inheritdoc />
    public bool CanDeleteRow(int RowIndex)
    {
        return RowIndex >= 0 && RowIndex < fItems.Count;
    }
    /// <inheritdoc />
    public bool DeleteRow(int RowIndex)
    {
        if (!CanDeleteRow(RowIndex))
            return false;

        T Item = fItems[RowIndex];
        fItems.RemoveAt(RowIndex);
        if (fItems is not INotifyCollectionChanged)
        {
            UnsubscribeRows(new[] { Item });
            Changed?.Invoke(this, GroupGridDataChangedEventArgs.RowRemoved(RowIndex));
        }

        return true;
    }
    /// <summary>
    /// Releases subscriptions held by this adapter.
    /// </summary>
    public void Dispose()
    {
        if (fDisposed)
            return;

        if (fItems is INotifyCollectionChanged Notifier)
            Notifier.CollectionChanged -= Items_CollectionChanged;

        UnsubscribeRows(fItems);
        fDisposed = true;
    }

    // ● properties
    /// <inheritdoc />
    public int RowCount => fItems.Count;

    // ● events
    /// <inheritdoc />
    public event EventHandler<GroupGridDataChangedEventArgs> Changed;
}
