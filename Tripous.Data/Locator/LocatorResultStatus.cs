namespace Tripous.Data;

/// <summary>
/// Status of a locator resolution operation.
/// </summary>
public enum LocatorResultStatus
{
    /// <summary>
    /// No status has been assigned.
    /// </summary>
    None,
    /// <summary>
    /// The locator request is invalid.
    /// </summary>
    InvalidRequest,
    /// <summary>
    /// The locator context is invalid.
    /// </summary>
    InvalidContext,
    /// <summary>
    /// No result was found.
    /// </summary>
    NoResult,
    /// <summary>
    /// A single result was found.
    /// </summary>
    SingleResult,
    /// <summary>
    /// Multiple results were found.
    /// </summary>
    MultipleResults,
    /// <summary>
    /// Too many results were found.
    /// </summary>
    TooManyResults,
    /// <summary>
    /// An error occurred.
    /// </summary>
    Error,
}
