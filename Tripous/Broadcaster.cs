/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Sends named event notifications to registered listeners.
/// </summary>
static public class Broadcaster
{
    // ● private fields
    static readonly object fLock = new();
    static readonly List<IBroadcasterListener> fListeners = new();

    // ● private
    static bool CanCallListener(IBroadcasterListener Listener)
    {
        if (Listener == null)
            return false;

        PropertyInfo Property = Listener.GetType().GetProperty("IsDisposed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (Property != null && Property.PropertyType == typeof(bool))
            return !Convert.ToBoolean(Property.GetValue(Listener));

        return true;
    }
    static IBroadcasterListener[] GetListeners()
    {
        lock (fLock)
            return fListeners.ToArray();
    }

    // ● static public
    /// <summary>
    /// Registers a listener.
    /// </summary>
    /// <param name="Listener">The listener.</param>
    static public void Add(IBroadcasterListener Listener)
    {
        if (Listener == null)
            return;

        lock (fLock)
        {
            if (!fListeners.Contains(Listener))
                fListeners.Add(Listener);
        }
    }
    /// <summary>
    /// Unregisters a listener.
    /// </summary>
    /// <param name="Listener">The listener.</param>
    static public void Remove(IBroadcasterListener Listener)
    {
        if (Listener == null)
            return;

        lock (fLock)
            fListeners.Remove(Listener);
    }
    /// <summary>
    /// Sends a notification message to all listeners synchronously.
    /// </summary>
    /// <param name="EventName">The event name.</param>
    /// <param name="Sender">The sender.</param>
    /// <param name="Params">The parameter dictionary.</param>
    static public void Send(string EventName, object Sender, IDictionary<string, object> Params)
    {
        if (!Active)
            return;

        BroadcasterArgs Args = new(EventName, Sender, Params);
        foreach (IBroadcasterListener Listener in GetListeners())
        {
            if (!CanCallListener(Listener) || ReferenceEquals(Sender, Listener))
                continue;
            Listener.ProcessBroadcasterMessage(Args);
        }
    }
    /// <summary>
    /// Sends a notification message to all listeners synchronously.
    /// </summary>
    /// <param name="EventName">The event name.</param>
    /// <param name="Sender">The sender.</param>
    static public void Send(string EventName, object Sender)
    {
        Send(EventName, Sender, new Dictionary<string, object>());
    }
    /// <summary>
    /// Posts a notification message to all listeners asynchronously.
    /// </summary>
    /// <param name="EventName">The event name.</param>
    /// <param name="Sender">The sender.</param>
    /// <param name="Params">The parameter dictionary.</param>
    static public void Post(string EventName, object Sender, IDictionary<string, object> Params)
    {
        if (!Active)
            return;

        BroadcasterArgs Args = new(EventName, Sender, Params);
        foreach (IBroadcasterListener Listener in GetListeners())
        {
            if (!CanCallListener(Listener) || ReferenceEquals(Sender, Listener))
                continue;
            Task.Run(() => Listener.ProcessBroadcasterMessage(Args));
        }
    }
    /// <summary>
    /// Posts a notification message to all listeners asynchronously.
    /// </summary>
    /// <param name="EventName">The event name.</param>
    /// <param name="Sender">The sender.</param>
    static public void Post(string EventName, object Sender)
    {
        Post(EventName, Sender, new Dictionary<string, object>());
    }

    // ● properties
    /// <summary>
    /// When true, the broadcaster sends notifications.
    /// </summary>
    static public bool Active { get; set; } = true;
}
