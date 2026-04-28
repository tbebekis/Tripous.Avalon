namespace Tripous.Data;

/// <summary>
/// Indicates the stage (phase) of a transaction operation
/// </summary>
public enum TransactionStage
{
    /// <summary>
    /// At the start transaction
    /// </summary>
    Start,
    /// <summary>
    /// At the commit transaction
    /// </summary>
    Commit,
    /// <summary>
    /// 
    /// </summary>
    Rollback,
}