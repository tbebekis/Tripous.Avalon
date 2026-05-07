namespace Tripous.Logging;

public class LogRecord
{
 
    string PropertiesText;
    
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

    public string Id { get; }
    public DateTime TimeStamp { get; }
    public string Date { get; }
    public string Time { get; }
    public string User { get; }
    public string Host { get; }
    public string Level { get; }
    public string Source { get; }
    public string Scope { get; }
    public string EventId { get; }
    public string Message { get; }
    public string Properties { get; }
    public string Stack { get; }
}
 
