// ● broadcaster listener
/**
 * Represents a listener object subscribed to tp.Broadcaster events.
 */
tp.IBroadcasterListener = class {
    // ● public
    /**
     * Called by tp.Broadcaster to notify a listener about an event.
     * @param {tp.EventArgs} Args The event arguments.
     * @returns {*} Returns any listener result.
     */
    BroadcasterFunc(Args) {
        return null;
    }
};

// ● broadcaster
/**
 * Sends event notifications to subscribed listener objects.
 * A listener object must provide a BroadcasterFunc(Args) method.
 */
tp.Broadcaster = class {
    // ● protected
    /**
     * Prepares event arguments for broadcasting.
     * @param {string} EventName The event name.
     * @param {object|null|undefined} Sender The optional sender.
     * @param {object|tp.EventArgs|null|undefined} Args The optional event arguments.
     * @returns {tp.EventArgs} Returns the prepared event arguments.
     * @protected
     */
    static PrepareEventArgs(EventName, Sender, Args) {
        var Result = Args instanceof tp.EventArgs ? Args : new tp.EventArgs(Args || {});
        Result.Sender = Result.Sender || Sender || tp.Broadcaster;
        Result.EventName = EventName;
        Result.IsBroadcasterMessage = true;
        return Result;
    }

    // ● public
    /**
     * Type guard. Returns true when a value is a broadcaster listener.
     * @param {*} Value The value to check.
     * @returns {boolean} Returns true when the value is a broadcaster listener.
     */
    static IsBroadcasterListener(Value) {
        return !tp.IsNil(Value) && tp.IsFunction(Value.BroadcasterFunc);
    }
    /**
     * Adds a broadcaster listener.
     * @param {object} Listener An object that provides a BroadcasterFunc(Args) method.
     * @returns {void}
     */
    static Add(Listener) {
        if (this.IsBroadcasterListener(Listener) && this.fListeners.indexOf(Listener) < 0)
            this.fListeners.push(Listener);
    }
    /**
     * Removes a broadcaster listener.
     * @param {object} Listener The listener to remove.
     * @returns {void}
     */
    static Remove(Listener) {
        var Index = this.fListeners.indexOf(Listener);
        if (Index >= 0)
            this.fListeners.splice(Index, 1);
    }
    /**
     * Sends a notification message to all subscribers synchronously.
     * @param {string} EventName The event name.
     * @param {object|null|undefined} Sender The optional sender.
     * @param {object|tp.EventArgs|null|undefined} Args The optional event arguments.
     * @returns {void}
     */
    static Send(EventName, Sender, Args) {
        var EventArgs = this.PrepareEventArgs(EventName, Sender, Args);
        var Snapshot = this.fListeners.slice();
        var Listener;
        var Index;
        for (Index = 0; Index < Snapshot.length; Index++) {
            Listener = Snapshot[Index];
            if (this.IsBroadcasterListener(Listener))
                Listener.BroadcasterFunc.call(Listener, EventArgs);
        }
    }
};
/**
 * The broadcaster listener list.
 * @type {object[]}
 */
tp.Broadcaster.fListeners = [];
