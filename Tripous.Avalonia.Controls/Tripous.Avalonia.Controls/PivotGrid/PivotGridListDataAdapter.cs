// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Adapts a list of POCO objects to the <see cref="IPivotGridDataAdapter"/> contract.
/// </summary>
/// <typeparam name="T">The row type.</typeparam>
public class PivotGridListDataAdapter<T>: IPivotGridDataAdapter, IDisposable
{
    // ● private fields
    readonly IList<T> fItems;
    readonly Dictionary<string, PropertyInfo> fProperties;
    readonly List<PivotGridSourceField> fSourceFields;
    bool fDisposed;

    // ● private methods
    PropertyInfo FindProperty(string FieldName)
    {
        return string.IsNullOrWhiteSpace(FieldName) || !fProperties.TryGetValue(FieldName, out PropertyInfo Result)
            ? null
            : Result;
    }
    void Items_CollectionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
    {
        Changed?.Invoke(this, PivotGridDataChangedEventArgs.Reset());
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
    void Item_PropertyChanged(object Sender, PropertyChangedEventArgs Args)
    {
        Changed?.Invoke(this, PivotGridDataChangedEventArgs.Reset());
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridListDataAdapter{T}"/> class.
    /// </summary>
    /// <param name="Items">The source item list.</param>
    public PivotGridListDataAdapter(IList<T> Items)
    {
        fItems = Items ?? throw new ArgumentNullException(nameof(Items));
        fProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(Property => Property.GetIndexParameters().Length == 0)
            .ToDictionary(Property => Property.Name, StringComparer.OrdinalIgnoreCase);
        fSourceFields = fProperties.Values
            .Select(Property => PivotGridFieldRules.CreateSourceField(Property.Name, Property.Name, Property.PropertyType))
            .Where(Field => Field.CanUseAsAxis || Field.CanUseAsMeasure)
            .ToList();
        SubscribeRows(fItems);
        if (fItems is INotifyCollectionChanged Notifier)
            Notifier.CollectionChanged += Items_CollectionChanged;
    }

    // ● public methods
    /// <inheritdoc />
    public object GetRow(int RowIndex) => fItems[RowIndex];
    /// <inheritdoc />
    public object GetValue(int RowIndex, string FieldName)
    {
        PropertyInfo Property = FindProperty(FieldName);
        return Property == null ? null : Property.GetValue(fItems[RowIndex]);
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
    /// <inheritdoc />
    public IReadOnlyList<PivotGridSourceField> SourceFields => fSourceFields;

    // ● events
    /// <inheritdoc />
    public event EventHandler<PivotGridDataChangedEventArgs> Changed;
}
