namespace Tripous.Data;

/// <summary>
/// Provides a <see cref="CurrentRow"/> property.
/// <para>Useful for single-row tables, and not only.</para>
/// </summary>
public interface IRowProvider
{
    string TableName { get; }
    DataRow CurrentRow { get; }

    event EventHandler CurrentRowChanged;

    void UpdateCurrentRow();
}

/// <summary>
/// Provides access to multiple <see cref="IRowProvider"/>.
/// <para>Useful when multiple tables are in an one-to-one relationship, such as a Trade, a StoreTrade and a FinTrade table.</para>
/// </summary>
public interface IRowProviderHost
{
    bool RowProviderExists(string TableName);
    IRowProvider FindRowProvider(string TableName);
    IRowProvider GetRowProvider(string TableName);

    ReadOnlyCollection<IRowProvider> RowProviders { get; }
}