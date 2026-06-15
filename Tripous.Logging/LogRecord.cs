/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Logging;

/// <summary>
/// Represents a log entry in a tabular form.
/// </summary>
public class LogRecord
{
 
    string PropertiesText;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LogRecord"/> class.
    /// </summary>
    /// <param name="Entry">The log entry to copy values from.</param>
    public LogRecord(LogEntry Entry)
    {
        Id = Entry.Id;
        TimeStamp = Entry.TimeStamp;
        Date = Entry.Date;
        Time = Entry.Time;
        User = Entry.User;
        Host = Entry.Host;
        Level = Entry.LevelText;
        Source = Entry.Source;
        Scope = Entry.ScopeId;
        EventId = Entry.EventId;
        Message = Entry.Text;
 

        if (Entry.Properties != null && Entry.Properties.Count > 0)
        {
            PropertiesText = Entry.GetPropertiesAsTextList();
            Properties = Entry.GetPropertiesAsSingleLine();
        }

        if (!string.IsNullOrWhiteSpace(Entry.ExceptionData))
            Stack = Entry.ExceptionData;
    }

    /// <summary>
    /// Returns the full log message, including properties and stack information.
    /// </summary>
    public string MessageFull()
    {
        StringBuilder SB = new();

        SB.AppendLine(Message);
        if (!string.IsNullOrWhiteSpace(PropertiesText))
        {
            SB.AppendLine("Properties");
            SB.AppendLine(PropertiesText);
        }

        if (!string.IsNullOrWhiteSpace(Stack))
        {
            SB.AppendLine("Stack");
            SB.AppendLine(Stack);
        }
        
        string Result = SB.ToString();
        return Result;
    }

    /// <summary>
    /// Adds this log record to a data row.
    /// </summary>
    /// <param name="Row">The data row to populate.</param>
    public void AddToRow(DataRow Row)
    {
        Row["Id"] = Id;
        Row["Year"] = TimeStamp.Year;
        Row["Month"] = TimeStamp.Month;
        Row["DayOfMonth"] = TimeStamp.Day;
        Row["LogTime"] = TimeStamp.ToString("yy-MM-dd HH:mm:ss");
        Row["User"] = User;
        Row["Host"] = Host;
        Row["Level"] = Level;
        Row["Source"] = Source;
        Row["Scope"] = Scope;
        Row["EventId"] = EventId;
        Row["Message"] = MessageFull();
    }

    /// <summary>
    /// Gets the log entry identifier.
    /// </summary>
    public string Id { get; }
    /// <summary>
    /// Gets the timestamp of the log entry.
    /// </summary>
    public DateTime TimeStamp { get; }
    /// <summary>
    /// Gets the date text of the log entry.
    /// </summary>
    public string Date { get; }
    /// <summary>
    /// Gets the time text of the log entry.
    /// </summary>
    public string Time { get; }
    /// <summary>
    /// Gets the user name associated with the log entry.
    /// </summary>
    public string User { get; }
    /// <summary>
    /// Gets the host name associated with the log entry.
    /// </summary>
    public string Host { get; }
    /// <summary>
    /// Gets the log level text.
    /// </summary>
    public string Level { get; }
    /// <summary>
    /// Gets the source of the log entry.
    /// </summary>
    public string Source { get; }
    /// <summary>
    /// Gets the scope identifier of the log entry.
    /// </summary>
    public string Scope { get; }
    /// <summary>
    /// Gets the event identifier of the log entry.
    /// </summary>
    public string EventId { get; }
    /// <summary>
    /// Gets the log message.
    /// </summary>
    public string Message { get; }
    /// <summary>
    /// Gets the log entry properties in a single-line text format.
    /// </summary>
    public string Properties { get; }
    /// <summary>
    /// Gets the stack or exception information of the log entry.
    /// </summary>
    public string Stack { get; }
}
 
