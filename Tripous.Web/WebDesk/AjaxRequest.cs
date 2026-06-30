/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Represents an Ajax request sent by a WebDesk client.
/// </summary>
public class AjaxRequest
{
    // ● public
    /// <summary>
    /// Returns true when the parameters dictionary contains a specified key with a non-null value.
    /// </summary>
    public bool ParamsContainsKey(string Key) => !string.IsNullOrWhiteSpace(Key) && Params.ContainsKey(Key) && Params[Key] != null;
    /// <summary>
    /// Returns a parameter value by key, if found; otherwise null.
    /// </summary>
    public object GetParam(string Key) => ParamsContainsKey(Key) ? Params[Key] : null;

    // ● properties
    /// <summary>
    /// Gets or sets the optional request id.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Gets or sets the required operation name to execute.
    /// </summary>
    public string OperationName { get; set; }
    /// <summary>
    /// Gets or sets the optional request parameters.
    /// </summary>
    public Dictionary<string, object> Params { get; set; } = new();
    /// <summary>
    /// Gets or sets the request type.
    /// </summary>
    public AjaxRequestType Type { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the requested UI may have a single client instance.
    /// </summary>
    public bool IsSingleInstance { get; set; }
    /// <summary>
    /// Gets or sets the optional command id that caused this request.
    /// </summary>
    public string CommandId { get; set; }
    /// <summary>
    /// Gets or sets the optional command name that caused this request.
    /// </summary>
    public string CommandName { get; set; }
}
