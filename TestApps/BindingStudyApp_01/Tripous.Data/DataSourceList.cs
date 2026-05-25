namespace Tripous.Data;

/// <summary>
/// Represents a named DataSource collection.
/// </summary>
public class DataSourceList: Collection<DataSource>
{
    // ● private fields
    object fOwner;

    // ● private
    void ValidateItem(DataSource Item)
    {
        ValidateItem(Item, null);
    }
    void ValidateItem(DataSource Item, DataSource ExistingItem)
    {
        if (Item == null)
            throw new ArgumentNullException(nameof(Item));
        if (string.IsNullOrWhiteSpace(Item.Name))
            throw new ApplicationException("DataSource name is not specified.");

        foreach (DataSource Source in this)
        {
            if (!ReferenceEquals(Source, ExistingItem) && string.Equals(Source.Name, Item.Name, StringComparison.OrdinalIgnoreCase))
                throw new ApplicationException($"DataSource already exists: {Item.Name}");
        }
    }
    void SetOwner(DataSource Item)
    {
        if (Item != null)
            Item.Owner = fOwner;
    }
    void ClearOwner(DataSource Item)
    {
        if (Item != null && ReferenceEquals(Item.Owner, fOwner))
            Item.Owner = null;
    }
    void ChangeOwner(object Value)
    {
        object OldOwner = fOwner;

        if (ReferenceEquals(OldOwner, Value))
            return;

        fOwner = Value;

        foreach (DataSource Item in this)
        {
            if (ReferenceEquals(Item.Owner, OldOwner))
                Item.Owner = Value;
        }
    }

    // ● protected
    /// <summary>
    /// Inserts a DataSource.
    /// </summary>
    protected override void InsertItem(int Index, DataSource Item)
    {
        ValidateItem(Item);
        base.InsertItem(Index, Item);
        SetOwner(Item);
    }
    /// <summary>
    /// Replaces a DataSource.
    /// </summary>
    protected override void SetItem(int Index, DataSource Item)
    {
        DataSource OldItem = this[Index];
        ValidateItem(Item, this[Index]);
        ClearOwner(OldItem);
        base.SetItem(Index, Item);
        SetOwner(Item);
    }
    /// <summary>
    /// Removes a DataSource.
    /// </summary>
    protected override void RemoveItem(int Index)
    {
        DataSource Item = this[Index];
        base.RemoveItem(Index);
        ClearOwner(Item);
    }
    /// <summary>
    /// Clears all DataSources.
    /// </summary>
    protected override void ClearItems()
    {
        foreach (DataSource Item in this)
            ClearOwner(Item);

        base.ClearItems();
    }

    // ● public
    /// <summary>
    /// Finds a DataSource by name, or returns null.
    /// </summary>
    public DataSource Find(string Name)
    {
        return this.FirstOrDefault(Source => string.Equals(Source.Name, Name, StringComparison.OrdinalIgnoreCase));
    }
    /// <summary>
    /// Gets a DataSource by name, or throws an exception.
    /// </summary>
    public DataSource Get(string Name)
    {
        DataSource Result = Find(Name);

        if (Result == null)
            throw new ApplicationException($"DataSource not found: {Name}");

        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the owner context.
    /// </summary>
    public object Owner
    {
        get => fOwner;
        set => ChangeOwner(value);
    }
}
