namespace Tripous.Logging;

/// <summary>
/// The level of log info to issue, display, or persist
/// </summary>
[Flags]
public enum LogLevel
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Trace
    /// </summary>
    Trace = 1,
    /// <summary>
    /// Debug
    /// </summary>
    Debug = 2,
    /// <summary>
    /// Info
    /// </summary>
    Info = 4,
    /// <summary>
    /// Warn
    /// </summary>
    Warning = 8,
    /// <summary>
    /// Error
    /// </summary>
    Error = 0x10,
    /// <summary>
    /// Fatal
    /// </summary>
    Fatal = 0x20,

}
 
