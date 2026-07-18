/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Base class for objects that subscribe to <see cref="Broadcaster"/> messages.
/// </summary>
public abstract class BroadcasterListener: IBroadcasterListener
{
    // ● protected
    /// <summary>
    /// Raises the <see cref="MessageEvent"/> event.
    /// </summary>
    /// <param name="Args">The broadcaster event arguments.</param>
    protected void OnMessage(BroadcasterArgs Args)
    {
        if (Active && MessageEvent != null)
            MessageEvent(this, Args);
    }

    // ● construction
    /// <summary>
    /// Creates a new instance and registers it to the <see cref="Broadcaster"/>.
    /// </summary>
    public BroadcasterListener()
    {
        Register();
    }

    // ● public
    /// <summary>
    /// Registers this listener to the <see cref="Broadcaster"/>.
    /// </summary>
    public void Register()
    {
        Broadcaster.Add(this);
    }
    /// <summary>
    /// Unregisters this listener from the <see cref="Broadcaster"/>.
    /// </summary>
    public void Unregister()
    {
        Broadcaster.Remove(this);
    }
    /// <summary>
    /// Processes a broadcaster message.
    /// </summary>
    /// <param name="Args">The broadcaster event arguments.</param>
    public abstract void ProcessBroadcasterMessage(BroadcasterArgs Args);

    // ● properties
    /// <summary>
    /// When false, broadcaster listener instances should not process incoming events.
    /// </summary>
    static public bool Active { get; set; } = true;

    // ● events
    /// <summary>
    /// Occurs when a broadcaster message is available.
    /// </summary>
    public event EventHandler<BroadcasterArgs> MessageEvent;
}
