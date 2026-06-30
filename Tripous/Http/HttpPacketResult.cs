/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Http;

/// <summary>
/// Represents a JSON result packet returned by controller action methods.
/// </summary>
public class HttpPacketResult
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public HttpPacketResult()
    {
    }

    // ● static public
    /// <summary>
    /// Creates and returns a successful result with a serialized packet.
    /// </summary>
    static public HttpPacketResult SetPacket(object Packet, bool IsSuccess = true)
    {
        HttpPacketResult Result = new();
        Result.SerializePacket(Packet);
        Result.IsSuccess = IsSuccess;
        return Result;
    }
    /// <summary>
    /// Creates and returns a result with an entity serialized as part of the result object.
    /// </summary>
    static public HttpPacketResult SetEntity(object Entity, bool IsSuccess = true)
    {
        HttpPacketResult Result = new();
        Result.Entity = Entity;
        Result.IsSuccess = IsSuccess;
        return Result;
    }
    /// <summary>
    /// Creates and returns a failed result with an error message.
    /// </summary>
    static public HttpPacketResult Error(string ErrorText)
    {
        HttpPacketResult Result = new();
        Result.ErrorText = ErrorText;
        Result.IsSuccess = false;
        return Result;
    }

    // ● public
    /// <summary>
    /// Serializes a specified instance and assigns the <see cref="Packet"/> property.
    /// </summary>
    public void SerializePacket(object Packet)
    {
        if (Packet != null)
            this.Packet = Json.Serialize(Packet);
    }
    /// <summary>
    /// Deserializes the <see cref="Packet"/> property to an instance of a specified type.
    /// </summary>
    public T DeserializePacket<T>() => Json.Deserialize<T>(Packet);

    // ● properties
    /// <summary>
    /// Gets or sets the JSON text packet returned to the caller.
    /// </summary>
    public string Packet { get; set; }
    /// <summary>
    /// Gets or sets an entity serialized along with this result as a whole.
    /// </summary>
    public object Entity { get; set; }
    /// <summary>
    /// Gets or sets the error information, if any.
    /// </summary>
    public string ErrorText { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the call succeeded business-logic-wise.
    /// </summary>
    public bool IsSuccess { get; set; }
}
