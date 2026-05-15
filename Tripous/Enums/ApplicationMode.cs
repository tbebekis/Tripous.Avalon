namespace Tripous;

/// <summary>
/// The mode of an application or external plugin
/// </summary>
[Flags]
public enum ApplicationMode
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Console
    /// </summary>
    Console = 1,
    /// <summary>
    /// Desktop
    /// </summary>
    Desktop = 2,
    /// <summary>
    /// Web
    /// </summary>
    Web = 4,
    /// <summary>
    /// Service
    /// </summary>
    Service = 8,
    /// <summary>
    /// ServicePanel
    /// </summary>
    ServicePanel = 0x10,
    /// <summary>
    /// All
    /// </summary>
    All = 0xFFFFFFF,
}