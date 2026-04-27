namespace Tripous.Data;

/// <summary>
/// Used with transaction event
/// </summary>
public class TransactionStageEventArgs : EventArgs
{
 

    /// <summary>
    /// Constructor.
    /// </summary>
    public TransactionStageEventArgs(SqlStore Store, DbTransaction Transaction, TransactionStage Stage, ExecTime ExecTime, object RowId)
    {
        this.Store = Store;
        this.Transaction = Transaction;
        this.Stage = Stage;
        this.ExecTime = ExecTime;
        this.RowId = RowId;
    }

    /// <summary>
    /// Gets the current Executor
    /// </summary>
    public SqlStore Store { get; private set; }
    /// <summary>
    /// Gets the current transaction
    /// </summary>
    public DbTransaction Transaction { get; private set; }
    /// <summary>
    /// Gets the stage of the call (before or after a start, commit or rollback transaction)
    /// </summary>
    public TransactionStage Stage { get; private set; }
    /// <summary>
    /// Gets the ExecTime
    /// </summary>
    public ExecTime ExecTime { get; private set; }
    /// <summary>
    /// Gets the Path (could be integer or string) of the current row.
    /// </summary>
    public object RowId { get; private set; }
}