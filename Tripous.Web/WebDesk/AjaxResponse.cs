/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Represents a response produced for an <see cref="AjaxRequest"/>.
/// </summary>
public class AjaxResponse
{
    Dictionary<string, object> fProperties = new();

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public AjaxResponse()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public AjaxResponse(string OperationName)
    {
        this.OperationName = OperationName;
    }

    // ● public
    /// <summary>
    /// Returns the packet object returned to the caller.
    /// </summary>
    public object GetPacketObject()
    {
        JsonObject Result = new();

        if (!string.IsNullOrWhiteSpace(OperationName))
            Result["OperationName"] = OperationName;

        foreach (var Entry in fProperties)
        {
            string JsonText = Json.Serialize(Entry.Value);
            Result[Entry.Key] = JsonNode.Parse(JsonText);
        }

        return Result;
    }
    /// <summary>
    /// Returns true when the response contains a specified property key.
    /// </summary>
    public bool ContainsKey(string Key) => fProperties.ContainsKey(Key);

    // ● properties
    /// <summary>
    /// Gets or sets the optional request/response operation name.
    /// </summary>
    public string OperationName { get; set; }
    /// <summary>
    /// Gets or sets a response property value by key.
    /// </summary>
    public object this[string Key]
    {
        get => fProperties.ContainsKey(Key) ? fProperties[Key] : null;
        set => fProperties[Key] = value;
    }
}
