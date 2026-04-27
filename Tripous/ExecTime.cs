namespace Tripous;

/// <summary>
/// Indicates the time of execution of a sub-operation relative to a main operation
/// </summary>
[Flags]
public enum ExecTime
{
    /// <summary>
    /// None of the flags
    /// </summary>
    None = 0,
    /// <summary>
    /// Before the execution of the main operation
    /// </summary>
    Before = 1,
    /// <summary>
    /// During the execution of the main operation
    /// </summary>
    During = 2,
    /// <summary>
    /// After the execution of the main operation
    /// </summary>
    After = 4,
}