namespace Tripous.Logging;

public class LogEntryArgs : EventArgs
{
    public LogEntryArgs(LogEntry Entry)
    {
        this.Entry = Entry;
    }

    public LogEntry Entry { get; }
}
 
