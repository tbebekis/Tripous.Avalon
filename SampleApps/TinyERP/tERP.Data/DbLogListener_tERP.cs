namespace tERP.Data;

internal class DbLogListener_tERP: SyncedLogListener
{
    DataModule Module;
    
    public DbLogListener_tERP()
    {
        ModuleDef ModuleDef = DataRegistry.Modules.Get("Log");
        Module = ModuleDef.Create();
    }
    
    /// <summary>
    /// Called by the Logger to pass LogInfo to a log listener.
    /// <para>This is synchronized and safe as it is executed outside the <see cref="Logger"/> thread.</para>
    /// </summary>
    public override void ProcessLogSynced(LogEntry Entry)
    {
        LogRecord LogRec = new LogRecord(Entry);
        Module.Insert();
        DataRow Row = Module.tblItem.Rows[0];
        LogRec.AddToRow(Row);
        Module.Commit();
    }
}