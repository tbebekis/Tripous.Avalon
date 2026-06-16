/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Owns a database connection and a single transaction for the lifetime of an operation.
/// </summary>
public class SqlTransactionContext : IDisposable
{
    // ● private
    DbConnection fConnection;
    DbTransaction fTransaction;
    bool fBeginCalled;
    bool fIsCompleted;
    bool fIsDisposed;

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public SqlTransactionContext(DbConnection Connection)
    {
        fConnection = Connection ?? throw new TripousArgumentNullException(nameof(Connection));
    }

    // ● public
    /// <summary>
    /// Opens the connection, if needed, and begins the transaction.
    /// </summary>
    public void BeginTransaction()
    {
        EnsureNotDisposed();

        if (fBeginCalled)
            throw new TripousDataException($"{nameof(SqlTransactionContext)} supports a single transaction only.");

        if (fConnection.State != ConnectionState.Open)
            fConnection.Open();

        fTransaction = fConnection.BeginTransaction();
        fBeginCalled = true;
    }
    /// <summary>
    /// Commits the transaction.
    /// </summary>
    public void Commit()
    {
        EnsureActiveTransaction();
        fTransaction.Commit();
        fIsCompleted = true;
    }
    /// <summary>
    /// Rolls back the transaction.
    /// </summary>
    public void Rollback()
    {
        EnsureActiveTransaction();
        fTransaction.Rollback();
        fIsCompleted = true;
    }
    /// <summary>
    /// Disposes the transaction and the connection.
    /// </summary>
    public void Dispose()
    {
        if (fIsDisposed)
            return;

        try
        {
            if (fTransaction != null && !fIsCompleted)
            {
                try
                {
                    fTransaction.Rollback();
                }
                catch
                {
                }
            }
        }
        finally
        {
            fTransaction?.Dispose();
            fConnection?.Dispose();
            fTransaction = null;
            fConnection = null;
            fIsDisposed = true;
        }
    }

    // ● private
    /// <summary>
    /// Throws an exception if this instance has been disposed.
    /// </summary>
    void EnsureNotDisposed()
    {
        if (fIsDisposed)
            throw new ObjectDisposedException(nameof(SqlTransactionContext));
    }
    /// <summary>
    /// Throws an exception if there is no active transaction.
    /// </summary>
    void EnsureActiveTransaction()
    {
        EnsureNotDisposed();

        if (fTransaction == null || fIsCompleted)
            throw new TripousDataException("No active transaction.");
    }

    // ● properties
    /// <summary>
    /// Returns the owned connection.
    /// </summary>
    public DbConnection Connection
    {
        get
        {
            EnsureNotDisposed();
            return fConnection;
        }
    }
    /// <summary>
    /// Returns the owned transaction.
    /// </summary>
    public DbTransaction Transaction
    {
        get
        {
            if (fTransaction == null)
                throw new TripousDataException("Transaction has not been started.");

            return fTransaction;
        }
    }
    /// <summary>
    /// Returns true when the transaction is active.
    /// </summary>
    public bool IsActive => !fIsDisposed && fTransaction != null && !fIsCompleted;
    /// <summary>
    /// Returns true when the transaction has been committed or rolled back.
    /// </summary>
    public bool IsCompleted => fIsCompleted;
}
