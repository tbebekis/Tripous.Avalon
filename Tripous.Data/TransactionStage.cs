namespace Tripous.Data;

/// <summary>
/// Indicates the stage (phase) of a transaction operation
/// </summary>
public enum TransactionStage
{
    /// <summary>
    /// At the transaction start 
    /// </summary>
    Start,
    /// <summary>
    /// At <see cref="TableSet.PostChanges()"/>
    /// </summary>
    Post,
    /// <summary>
    /// At the transaction commit 
    /// </summary>
    Commit,
    /// <summary>
    /// At the transaction rollback 
    /// </summary>
    Rollback,
}