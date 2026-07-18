/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Provides data for a <see cref="Broadcaster"/> message.
/// </summary>
public class BroadcasterArgs: EventArgs
{
    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="EventName">The event name.</param>
    /// <param name="Sender">The sender.</param>
    /// <param name="Params">The parameter dictionary.</param>
    public BroadcasterArgs(string EventName, object Sender, IDictionary<string, object> Params)
    {
        this.EventName = EventName ?? "";
        this.Sender = Sender;
        this.Params = Params ?? new Dictionary<string, object>();
    }
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="EventName">The event name.</param>
    /// <param name="Sender">The sender.</param>
    public BroadcasterArgs(string EventName, object Sender) : this(EventName, Sender, null)
    {
    }
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="EventName">The event name.</param>
    public BroadcasterArgs(string EventName) : this(EventName, null, null)
    {
    }

    // ● properties
    /// <summary>
    /// The event name.
    /// </summary>
    public string EventName { get; }
    /// <summary>
    /// The sender.
    /// </summary>
    public object Sender { get; }
    /// <summary>
    /// The parameter dictionary.
    /// </summary>
    public IDictionary<string, object> Params { get; }
}
