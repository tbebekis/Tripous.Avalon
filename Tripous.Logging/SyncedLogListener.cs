namespace Tripous.Logging;


/// <summary>
/// A log listener that synchronizes the <see cref="ProcessLog(LogEntry)"/> call to any context.
/// <para>That is, it is safe to attach to the <see cref="EntryEvent"/> a GUI element such as a Form or a TextBox.</para>
/// <para>SEE: <see cref="SynchronizationContext.Post(SendOrPostCallback, object?)"/> which reverses the responsibility of thread synchronization. </para>
/// <para>Client code has just to link to <see cref="EntryEvent"/> and then process the passed <see cref="LogEntry"/> safely.</para>
/// <para>The <see cref="LogEntry"/> provides methods such as <see cref="LogEntry.AsJson()"/>, <see cref="LogEntry.AsList()"/>, etc. 
/// for getting a string representation of the entry.</para>
/// <para>SEE: https://lostechies.com/gabrielschenker/2009/01/23/synchronizing-calls-to-the-ui-in-a-multi-threaded-application/</para>
/// </summary>
public class SyncedLogListener : LogListener
{
    SynchronizationContext fSyncContext;

    void OnEntryEvent(LogEntry Entry)
    {
        ProcessLogSynced(Entry);
        EntryEvent?.Invoke(this, new LogEntryArgs(Entry));
    }

    // ● construction   
    /// <summary>
    /// Constructor.
    /// </summary>
    public SyncedLogListener()
        : base()
    {
        fSyncContext = AsyncOperationManager.SynchronizationContext; 
    }

    // ● public  
    /// <summary>
    /// Called by the Logger to pass LogInfo to a log listener.
    ///<para>
    /// CAUTION: The Logger calls its Listeners asynchronously, that is from inside a thread.
    /// Thus Listeners should synchronize the ProcessLogInfo() call. Controls need to check if InvokeRequired.
    /// <para>NOTE: This listener synchronizes this call to any context.</para>
    /// </para>
    /// </summary>
    public override void ProcessLog(LogEntry Entry)
    {
        fSyncContext.Post(e => OnEntryEvent(e as LogEntry), Entry);
    }
    /// <summary>
    /// Called by the Logger to pass LogInfo to a log listener.
    /// <para>This is synchronized and safe as it is executed outside the <see cref="Logger"/> thread.</para>
    /// </summary>
    public virtual void ProcessLogSynced(LogEntry Entry)
    {
    }

    // ● events  
    /// <summary>
    /// Occurs when a new <see cref="LogEntry"/> is available.>
    /// </summary>
    public event EventHandler<LogEntryArgs> EntryEvent;
}

