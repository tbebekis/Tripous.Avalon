// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Adapts a list of POCO objects to the <see cref="IChartDataAdapter"/> contract.
/// </summary>
public class ChartListDataAdapter: IChartDataAdapter, IDisposable
{
    // ● private fields
    readonly IList fItems;
    readonly Dictionary<string, PropertyInfo> fProperties;
    readonly List<ChartSourceField> fSourceFields;
    bool fDisposed;

    // ● private methods
    Type ResolveItemType(IList Items)
    {
        Type ListType = Items.GetType();
        Type ItemType = ListType.IsGenericType ? ListType.GetGenericArguments().FirstOrDefault() : null;
        if (ItemType != null && ItemType != typeof(object))
            return ItemType;

        foreach (object Item in Items)
            if (Item != null)
                return Item.GetType();

        return typeof(object);
    }
    PropertyInfo FindProperty(string FieldName)
    {
        return string.IsNullOrWhiteSpace(FieldName) || !fProperties.TryGetValue(FieldName, out PropertyInfo Result)
            ? null
            : Result;
    }
    void Items_CollectionChanged(object Sender, NotifyCollectionChangedEventArgs Args)
    {
        Changed?.Invoke(this, new ChartDataChangedEventArgs());
    }
    void SubscribeRows(IEnumerable Items)
    {
        foreach (object Item in Items)
            if (Item is INotifyPropertyChanged Notifier)
                Notifier.PropertyChanged += Item_PropertyChanged;
    }
    void UnsubscribeRows(IEnumerable Items)
    {
        foreach (object Item in Items)
            if (Item is INotifyPropertyChanged Notifier)
                Notifier.PropertyChanged -= Item_PropertyChanged;
    }
    void Item_PropertyChanged(object Sender, PropertyChangedEventArgs Args)
    {
        Changed?.Invoke(this, new ChartDataChangedEventArgs());
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartListDataAdapter"/> class.
    /// </summary>
    /// <param name="Items">The source item list.</param>
    public ChartListDataAdapter(IList Items)
    {
        fItems = Items ?? throw new ArgumentNullException(nameof(Items));
        Type ItemType = ResolveItemType(fItems);
        fProperties = ItemType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(Property => Property.GetIndexParameters().Length == 0)
            .ToDictionary(Property => Property.Name, StringComparer.OrdinalIgnoreCase);
        fSourceFields = fProperties.Values
            .Select(Property => ChartFieldRules.CreateSourceField(Property.Name, Property.Name, Property.PropertyType))
            .Where(Field => Field.CanUseAsDimension || Field.CanUseAsMeasure)
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
        object Row = RowIndex >= 0 && RowIndex < fItems.Count ? fItems[RowIndex] : null;
        return Property == null || Row == null ? null : Property.GetValue(Row);
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
    public IReadOnlyList<ChartSourceField> SourceFields => fSourceFields;

    // ● events
    /// <inheritdoc />
    public event EventHandler<ChartDataChangedEventArgs> Changed;
}
