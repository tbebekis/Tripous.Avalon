/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Provides a <see cref="CurrentRow"/> property.
/// <para>Useful for single-row tables, and not only.</para>
/// </summary>
public interface IRowProvider
{
    /// <summary>
    /// The name of the table this provider refers to.
    /// </summary>
    string TableName { get; }
    /// <summary>
    /// The current row, if any, else null.
    /// </summary>
    DataRow CurrentRow { get; }

    /// <summary>
    /// Occurs when the <see cref="CurrentRow"/> changes.
    /// </summary>
    event EventHandler CurrentRowChanged;

    /// <summary>
    /// Updates the <see cref="CurrentRow"/>.
    /// </summary>
    void UpdateCurrentRow();
}

/// <summary>
/// Provides access to multiple <see cref="IRowProvider"/>.
/// <para>Useful when multiple tables are in an one-to-one relationship, such as a Trade, a StoreTrade and a FinTrade table.</para>
/// </summary>
public interface IRowProviderHost
{
    /// <summary>
    /// Returns true if a <see cref="IRowProvider"/> with the specified <paramref name="TableName"/> exists.
    /// </summary>
    bool RowProviderExists(string TableName);
    /// <summary>
    /// Returns the <see cref="IRowProvider"/> with the specified <paramref name="TableName"/>, if found, else null.
    /// </summary>
    IRowProvider FindRowProvider(string TableName);
    /// <summary>
    /// Returns the <see cref="IRowProvider"/> with the specified <paramref name="TableName"/>, if found, else throws an exception.
    /// </summary>
    IRowProvider GetRowProvider(string TableName);

    /// <summary>
    /// The list of <see cref="IRowProvider"/> instances hosted by this instance.
    /// </summary>
    ReadOnlyCollection<IRowProvider> RowProviders { get; }
}