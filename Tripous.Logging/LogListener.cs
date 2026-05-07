namespace Tripous.Logging;

/// <summary>
/// A listener for <see cref="LogEntry"/> messages.
/// <para>A listener registers itself automatically with <see cref="Logger"/>, upon construction.</para>
/// <para>This class provides retain policy related properties to inheritors.</para>
/// </summary>
public abstract class LogListener
{
    int fRetainPolicyCounter;
    int fRetainDays;
    int fMaxSizeKiloBytes;

    // ● construction  
    /// <summary>
    /// Constructor
    /// </summary>
    public LogListener()
    {
        Register();
    }

    // ● public  
    /// <summary>
    /// Called by the Logger to pass <see cref="LogEntry"/> to a log listener.
    ///<para>
    /// CAUTION: The Logger calls its Listeners asynchronously, that is from inside a thread.
    /// Thus Listeners should synchronize the ProcessLog() call. Controls need to check if InvokeRequired.
    /// </para>
    /// </summary>
    public abstract void ProcessLog(LogEntry Entry);

    /// <summary>
    /// Registers this listener to <see cref="Logger"/>
    /// </summary>
    public void Register()
    {
        Logger.Add(this);
    }
    /// <summary>
    /// Unregisters this listener to <see cref="Logger"/>
    /// </summary>
    public void Unregister()
    {
        Logger.Remove(this);
    }

    // ● properties  
    /// <summary>
    /// After how many writes to check whether it is time to apply the retain policy. Defaults to 100
    /// </summary>
    public virtual int RetainPolicyCounter
    {
        get { return fRetainPolicyCounter >= Logger.RetainPolicyCounter ? fRetainPolicyCounter : Logger.RetainPolicyCounter; }
        set { fRetainPolicyCounter = value; }
    }
    /// <summary>
    /// Retain policy. How many days to retain in the storage medium. Defaults to 7
    /// </summary>
    public virtual int RetainDays
    {
        get { return fRetainDays >= Logger.RetainDays ? fRetainDays : Logger.RetainDays; }
        set { fRetainDays = value; }
    }
    /// <summary>
    /// Retain policy. How many KB to allow a single log file to grow. Defaults to 512 KB
    /// </summary>
    public virtual int MaxSizeKiloBytes
    {
        get { return fMaxSizeKiloBytes >= Logger.MaxSizeKiloBytes ? fMaxSizeKiloBytes : Logger.MaxSizeKiloBytes; }
        set { fMaxSizeKiloBytes = value; }
    }
}
