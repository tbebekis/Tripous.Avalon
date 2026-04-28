namespace Tripous.Data;

/// <summary>
/// Simulates an .ini file using a table in a database
/// </summary>
public class DbIni
{
    /* fields */
    SqlStore Store = null;




    /// <summary>
    /// Ensures that the ini table exists in database.
    /// </summary>
    private void EnsureTableExists()
    {
        if (!Store.TableExists(TableName, UseSelect: true))
        {
            string CreateTableSql = $@"
create table {TableName} (                   
{EntryField}    @VARCHAR(512)  @NOT_NULL primary key
,{ValueField}    @VARCHAR(2048)    
,{BlobField}     @BLOB             
) 
";

            Store.CreateTable(CreateTableSql);
        }
    }

    /* construction */ 
    /// <summary>
    /// Constructor
    /// </summary>
    public DbIni(DbConnectionInfo ConnectionInfo)
    {
        this.Store = new SqlStore(ConnectionInfo);
        EnsureTableExists();
    }


    /* public */
    /// <summary>
    /// Returns true if Entry exists in table
    /// </summary>
    public bool Exists(string Entry)
    {
        string SqlText = string.Format("select {0} from {1} where {0} = '{2}' ", EntryField, TableName, Entry);
        DataRow Row = Store.SelectResults(SqlText);
        return (Row != null);
    }
    /// <summary>
    /// Deletes the Entry from table
    /// </summary>
    public void Delete(string Entry)
    {
        string SqlText = string.Format("delete from {0} where {1} = '{2}' ", TableName, EntryField, Entry);
        Store.ExecSql(SqlText);
    }

    /// <summary>
    /// Writes the Value to the Entry. If the Entry does not exist, it created.
    /// </summary>
    public void WriteString(string Entry, string Value)
    {
        string SqlText;

        if (Exists(Entry))
            SqlText = string.Format("update {0} set {1} = '{2}'  where {3} = '{4}'  ", TableName, ValueField, Value, EntryField, Entry);
        else
            SqlText = string.Format("insert into {0} ({1}, {2}) values ('{3}', '{4}')", TableName, EntryField, ValueField, Entry, Value);

        Store.ExecSql(SqlText);
    }
    /// <summary>
    /// Returns the Value of the Entry, if exists, else Default
    /// </summary>
    public string ReadString(string Entry, string Default)
    {
        string SqlText = string.Format("select {0} from {1} where {2} = '{3}' ", ValueField, TableName, EntryField, Entry);
        DataRow Row = Store.SelectResults(SqlText);
        if ((Row != null) && (!Row.IsNull(ValueField)))
            return Row[ValueField].ToString();

        return Default;
    }

    /// <summary>
    /// Writes the Value to the Entry. If the Entry does not exist, it created.
    /// </summary>
    public void WriteInteger(string Entry, int Value)
    {
        WriteString(Entry, Value.ToString());
    }
    /// <summary>
    /// Returns the Value of the Entry, if exists, else Default
    /// </summary>
    public int ReadInteger(string Entry, int Default)
    {
        return Sys.AsInteger(ReadString(Entry, Default.ToString()), Default);
    }

    /// <summary>
    /// Writes the Value to the Entry. If the Entry does not exist, it created.
    /// </summary>
    public void WriteFloat(string Entry, double Value)
    {
        WriteString(Entry, Sys.DoubleToStr(Value));
    }
    /// <summary>
    /// Returns the Value of the Entry, if exists, else Default
    /// </summary>
    public double ReadFloat(string Entry, double Default)
    {
        return Sys.AsDouble(ReadString(Entry, Default.ToString()), Default);
    }

    /// <summary>
    /// Writes the Value to the Entry. If the Entry does not exist, it created.
    /// </summary>
    public void WriteBool(string Entry, bool Value)
    {
        WriteString(Entry, Value.ToString());
    }
    /// <summary>
    /// Returns the Value of the Entry, if exists, else Default
    /// </summary>
    public bool ReadBool(string Entry, bool Default)
    {
        return Sys.AsBoolean(ReadString(Entry, Default.ToString()), Default);
    }

    /// <summary>
    /// Writes the Value to the Entry. If the Entry does not exist, it created.
    /// </summary>
    public void WriteDateTime(string Entry, DateTime Value)
    {
        WriteString(Entry, Sys.DateTimeToStr(Value, false));
    }
    /// <summary>
    /// Returns the Value of the Entry, if exists, else Default
    /// </summary>
    public DateTime ReadDateTime(string Entry, DateTime Default)
    {
        return Sys.AsDateTime(ReadString(Entry, Default.ToString()), Default);
    }

    /// <summary>
    /// Writes the Value and Stream to the Entry. If the Entry does not exist, it created.
    /// </summary>
    public void WriteBlob(string Entry, Stream Stream, string Value)
    {
        DataTable Table = new DataTable();
        Table.Columns.Add(BlobField, typeof(byte[]));

        byte[] Bytes = new byte[Stream.Length];
        Stream.Position = 0;
        Stream.ReadExactly(Bytes, 0, Convert.ToInt32(Stream.Length));

        Table.Rows.Add(Bytes);

        string SqlText;


        if (Exists(Entry))
        {
            SqlText = string.Format("update {0} set {1} = '{2}', {3} = :{3} where {4} = '{5}' ",
                TableName, ValueField, Value, BlobField, EntryField, Entry);
        }
        else
        {
            SqlText = string.Format("insert into {0} ({1}, {2}, {3}) values ('{4}', '{5}', :{3})",
                TableName, EntryField, ValueField, BlobField, Entry, Value);
        }


        Store.ExecSql(SqlText, Table.Rows[0]);
    }
    /// <summary>
    /// Writes the Stream to the Entry. If the Entry does not exist, it created.
    /// </summary>
    public void WriteBlob(string Entry, Stream Stream)
    {
        WriteBlob(Entry, Stream, "");
    }
    /// <summary>
    /// Reads the Entry to the Stream and Value. Returns true if the Entry exists.
    /// </summary>
    public bool ReadBlob(string Entry, Stream Stream, ref string Value)
    {
        bool Result = false;

        string SqlText = string.Format("select * from {0} where {1} = '{2}' ", TableName, EntryField, Entry);
        DataRow Row = Store.SelectResults(SqlText);
        if (Row != null)
        {
            Result = true;

            if (!Row.IsNull(BlobField))
            {
                byte[] Bytes = (byte[])Row[BlobField];
                if (Bytes.Length > 0)
                {
                    Stream.Write(Bytes, 0, Bytes.Length);
                    Stream.Position = 0;
                }
            }

            if (!Row.IsNull(EntryField))
                Value = Row[EntryField].ToString();

        }

        return Result;

    }
    /// <summary>
    /// Reads the Entry to the Stream. Returns true if the Entry exists.
    /// </summary>
    public bool ReadBlob(string Entry, Stream Stream)
    {
        string Value = "";
        return ReadBlob(Entry, Stream, ref Value);
    }

    /// <summary>
    /// Loads Instance from a DbIni blob
    /// </summary>
    public bool ReadInstance(string Entry, object Instance)
    {
        string JsonText = string.Empty;

        // load from db ini
        using (MemoryStream MS = new MemoryStream())
        {
            ReadBlob(Entry, MS);
            MS.Position = 0;
            
            using (StreamReader reader = new StreamReader(MS, Encoding.UTF8))
            {
                JsonText = reader.ReadToEnd();
            }
        }

        if (!string.IsNullOrWhiteSpace(JsonText))
        {
            Json.PopulateObject(Instance, JsonText);
            return true;
        }

        return false;
    }
    /// <summary>
    /// Saves Instance to a DbIni blob
    /// </summary>
    public void WriteInstance(string Entry, object Instance)
    {
        // save to db ini
        string JsonText = Json.Serialize(Instance);
        byte[] Buffer = Encoding.UTF8.GetBytes(JsonText);

        using (MemoryStream MS = new MemoryStream())
        {
            MS.Write(Buffer, 0, Buffer.Length);
            MS.Position = 0;
            WriteBlob(Entry, MS);
        }
    }


    /// <summary>
    /// Constant
    /// </summary>
    static public string TableName { get; set; } = "SYS_INI";
    /// <summary>
    /// Constant
    /// </summary>
    static public string EntryField { get; set; }  = "EntryKey";
    /// <summary>
    /// Constant
    /// </summary>
    static public string ValueField { get; set; }  = "EntryValue";
    /// <summary>
    /// Constant
    /// </summary>
    static public string BlobField { get; set; }  = "EntryData";

}