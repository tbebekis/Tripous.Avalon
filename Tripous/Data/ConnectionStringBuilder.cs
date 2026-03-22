namespace Tripous
{
    using System.Data;
    using System.Data.Common;
    
    /// <summary>
    /// A connection string builder
    /// </summary>
    public class ConnectionStringBuilder : DbConnectionStringBuilder
    {
        // HKEY_LOCAL_MACHINE\Software\Microsoft\Jet\4.0\ISAM Formats 
        // http://connectionstrings.com/?carrier=excel2007
        // HDR=YES; HDR=NO;      - set headers to NO even if the don't exist
        // to treat all data in the file as text, overriding Excels column type "General" to guess what type of data is in the column use IMEX
        // IMEX=1; IMEX=0;       - IMEX=1 simply tells the driver that data that's intermixed (numbers, dates, etc etc) should be treated as text. 

        // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Jet\4.0\ISAM Formats
        // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Jet\4.0\ISAM Formats
        // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Office\12.0\Access Connectivity Engine\ISAM Formats
        // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Office\14.0\Access\Access Connectivity Engine\ISAM Formats
        // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Office\14.0\Access Connectivity Engine\ISAM Formats

        /* private */
        bool IsOleDbFirebirdDatabaseName(string Entry)
        {
            return !string.IsNullOrEmpty(Entry) && (Entry.IndexOf(':') != -1) && (Entry.LastIndexOf(':') != Entry.IndexOf(':'));
        }
        void ExtractOleDbFirebirdServerAndDatabaseName(string Entry, out string Server, out string Database)
        {
            // ServerName:C:\Path\Database.FDB

            Server = string.Empty;
            Database = string.Empty;

            if (!string.IsNullOrEmpty(Entry))
            {
                int Index = Entry.IndexOf(':');
                if (Index > 1)
                {
                    Server = Entry.Substring(0, Index);
                    Database = Entry.Remove(0, Server.Length + 1);
                }
                else
                {
                    Server = "localhost";
                    Database = Entry;
                }
            }

        }

        /* constants */
        /// <summary>
        /// Constant
        /// </summary>
        public const string AliasKey = "Alias";

        /* construction */
        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionStringBuilder()
        {
        }
        /// <summary>
        /// Constructor
        /// <para>It assigns the Alias too, if the specified connection string contains an alias key.</para>
        /// </summary>
        public ConnectionStringBuilder(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        /// <summary>
        /// Constructor
        /// <para>Set UseOdbcRules to true to use {} to delimit fields. Set to false to use quotation marks.</para>
        /// </summary>
        public ConnectionStringBuilder(bool UseOdbcRules)
            : base(UseOdbcRules)
        {
        }
        /// <summary>
        /// Constructor
        /// <para>Set UseOdbcRules to true to use {} to delimit fields. Set to false to use quotation marks.</para>
        /// <para>It assigns the Alias too, if the specified connection string contains an alias key.</para>
        /// </summary>
        public ConnectionStringBuilder(bool UseOdbcRules, string ConnectionString)
            : base(UseOdbcRules)
        {
            this.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// Creates and returns a ConnectionStringBuilder. ConnectionString may be null or empty.
        /// </summary>
        public virtual ConnectionStringBuilder CreateConnectionStringBuilder(string ConnectionString)
        {
            ConnectionStringBuilder Result = new ConnectionStringBuilder(ConnectionString);
            if (string.IsNullOrEmpty(ConnectionString) || !Result.ContainsKey(ConnectionStringBuilder.AliasKey))
                Result.Alias = this.Alias;
            return Result;
        }

        /* connection string */
        /// <summary>
        /// Normalizes ConnectionString by replacing the following place-holders
        /// which may included in the database name path.
        /// <para>[AppPath]</para>
        /// <para>[Data]</para>
        /// <para>[BackUp]</para>
        /// </summary>
        static public string NormalizeConnectionString(string ConnectionString)
        {
            return ReplacePathPlaceholders(RemoveAliasEntry(ConnectionString));
        }
        /// <summary>
        /// Replaces path placeholders, such as [AppPath], [Data] and [BackUp], contained in ConnectionString
        /// </summary>
        static public string ReplacePathPlaceholders(string ConnectionString)
        {
            string AppDataFolder = Sys.RemoveTrailingSlash(SysConfig.AppDataFolderPath);      // C:\ProgramData\CompanyName\AppExeName\Data\
            string AppExeFolder = Sys.RemoveTrailingSlash(SysConfig.AppFolderPath);        // C:\MyFolder\MyApplication\

            string Result = ConnectionString;

            Result = Result.Replace("[BackUp]", AppDataFolder + "BackUp");
            Result = Result.Replace("[Data]", AppDataFolder);
            Result = Result.Replace("[AppPath]", AppExeFolder);

            return Result;
        }
        /// <summary>
        /// Extracts the <paramref name="Alias"/> and the <paramref name="ConnectionString"/> 
        /// from the <paramref name="Input"/> parameter.
        /// </summary>
        /// <remarks>The  <paramref name="Input"/> parameter is a ConnectionString that may contains a 
        /// "Alias=XXX" key-value pair. This static method parses the Input parameter
        /// and returns the contained information. The Alias entry is deleted from the ConnectionString if found </remarks>
        static public void ExtractAlias(string Input, ref string Alias, ref string ConnectionString)
        {

            if (!string.IsNullOrEmpty(Input))
            {
                Alias = string.Empty;
                ConnectionString = string.Empty;

                string[] Split = Input.Split(';');
                string[] KeyValue;
                string Key;
                string Value;
                string S;

                foreach (string item in Split)
                {
                    S = item.Trim();
                    KeyValue = S.Split('=');

                    if ((KeyValue != null) && (KeyValue.Length == 2))
                    {
                        Key = KeyValue[0].Trim();
                        Value = KeyValue[1].Trim();

                        if ((string.Compare(Key, AliasKey, StringComparison.InvariantCultureIgnoreCase) == 0))
                            Alias = Value;
                        else
                            ConnectionString = ConnectionString + S + ";";
                    }


                }

                ConnectionString = ConnectionString.TrimEnd(';');
            }
        }
        /// <summary>
        /// Extracts and returns the provider alias from the specified ConnectionString.
        /// </summary>
        static public string GetAlias(string ConnectionString)
        {
            string Input = ConnectionString;
            string Alias = string.Empty;

            ExtractAlias(Input, ref Alias, ref ConnectionString);
            return Alias;

        }
        /// <summary>
        /// Removes any provider Alias from ConnectionString and returns the ConnectionString
        /// without Alias.
        /// </summary>
        static public string RemoveAliasEntry(string ConnectionString)
        {
            if (ConnectionString.ContainsText(ConnectionStringBuilder.AliasKey))
            {
                string tempAlias = "";
                ExtractAlias(ConnectionString, ref tempAlias, ref ConnectionString);
            }
            return ConnectionString;
        }
 
        /* public */
        /// <summary>
        /// Sets the connection string. 
        /// <para>It assigns the Alias too, if the specified connection string contains an alias key.</para>
        /// </summary>
        public void SetConnectionString(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            if (string.IsNullOrEmpty(ConnectionString) || !this.ContainsKey(ConnectionStringBuilder.AliasKey))
                this.Alias = this.Alias; 
        }
        /// <summary>
        /// Returns true if Key exists in the connection string. When true the Value gets the value of the specified key.
        /// </summary>
        public bool TryGetValue(string Key, out string Value)
        {
            Value = string.Empty;

            if (!string.IsNullOrWhiteSpace(Key))
            {
                if (ContainsKey(Key))
                {
                    Value = this[Key].ToString();
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Returns the Value of the first Key in Keys that has a value, else empty string.
        /// </summary>
        public string GetFirst(string[] Keys)
        {
            foreach (string Key in Keys)
            {
                if (this.ContainsKey(Key))
                    return this[Key].ToString();
            }

            return string.Empty;
        }
        /// <summary>
        /// Removes the database entry from the connection string, and returns the connection string
        /// </summary>
        public string RemoveDatabaseEntry()
        {
            string[] Keys = { "Initial Catalog", "Database", "Data Source" };
            foreach (string Key in Keys)
            {
                if (ContainsKey(Key))
                    Remove(Key);
            }

            return this.ConnectionString;
        }

        /// <summary>
        /// Returns the ConnectionString as a DataTable with two fields: Key, Value
        /// </summary>
        public DataTable ToDataTable()
        {
            DataTable Table = new DataTable();

            Table.Columns.Add("Key");
            Table.Columns.Add("Value");

            Table.DefaultView.Sort = "Key";

            ToDataTable(Table);

            return Table;
        }
        /// <summary>
        /// Converts the ConnectionString to a DataTable with two fields: Key, Value
        /// </summary>
        public void ToDataTable(DataTable Table)
        {
            if (Table.Columns.Count == 0)
            {
                Table.Columns.Add("Key");
                Table.Columns.Add("Value");

                Table.DefaultView.Sort = "Key";
            }

            Table.DeleteRows();
            Table.AcceptChanges();

            string Value;
            foreach (string Key in this.Keys)
            {
                Value = this[Key].ToString();
                Table.Rows.Add(Key, Value);
            }
        }
        /// <summary>
        /// Resets the connection string from Table content.
        /// <para>NOTE: Table must have two fields: Key, Value</para>
        /// </summary>
        public void FromDataTable(DataTable Table)
        {
            this.Clear();
            foreach (DataRow Row in Table.Rows)
            {
                this[Row.AsString("Key")] = Row.AsString("Value");
            }
        }

        /*------------------------------------------------------------------------------------------------------------------------         
                    Ms Sql	            Firebird	    Oracle (Native)	    Oracle (Microsoft)	OleDb	        Odbc
        -------------------------------------------------------------------------------------------------------------------------
        Server	    Data Source	        DataSource	    Data Source	        Data Source			                DSN
                    Server			                                        Server			
                    Address						
                    Addr						
                    Network Address 						
        -------------------------------------------------------------------------------------------------------------------------
        Database	Initial Catalog	    Database			                                    Data Source	 
                    Database 						
        -------------------------------------------------------------------------------------------------------------------------
        UserName	User ID	            User	        User Id	            User ID			
                    UID						
        -------------------------------------------------------------------------------------------------------------------------
        Password	Password	        Password	    Password	        Password			
                    PWD 
        -------------------------------------------------------------------------------------------------------------------------*/


        /* properties */
        /// <summary>
        ///  Gets or sets the value associated with the specified Key.
        /// </summary>
        public override object this[string Key]
        {
            get
            {
                if (!ContainsKey(Key))
                    return string.Empty;

                return base[Key];
            }
            set
            {
                base[Key] = value;
            }
        }
        /// <summary>
        /// Gets or sets the value of the Provider Alias key in the connection string
        /// </summary>
        public string Alias
        {
            get
            {
                string S = string.Empty;
                TryGetValue(AliasKey, out S);
                return S;
            }
            set { this[AliasKey] = value; }
        }
        /// <summary>
        /// Gets the value of the User key in the connection string
        /// </summary>
        public string User
        {
            get
            {
                string S = string.Empty;

                if (!TryGetValue("User", out S))
                    if (!TryGetValue("UserId", out S))
                        if (!TryGetValue("User ID", out S))
                            if (!TryGetValue("UID", out S))
                                S = string.Empty;

                return S;
            }
        }
        /// <summary>
        /// Gets the value of the Password key in the connection string
        /// </summary>
        public string Password
        {
            get
            {
                string S = string.Empty;
                if (!TryGetValue("Password", out S))
                    if (!TryGetValue("Psw", out S))
                        S = string.Empty;

                return S;
            }
        }
        /// <summary>
        /// Gets the value of the Database key in the connection string
        /// </summary>
        public string Database
        {
            get
            {
                string S = string.Empty;

                /* MsSql, SQLite or Firebird */
                if (TryGetValue("Initial Catalog", out S) || TryGetValue("Database", out S))
                    return S;

                /* Oracle, OleDbFirebird or any of the rest OLEDB providers  */
                if (TryGetValue("Data Source", out S))     /* it is not ADO.NET Firebird. It may be OleDbFirebird though */
                {
                    return S;
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// Gets the value of the Server key in the connection string
        /// </summary>
        public string Server
        {
            get
            {
                string S = string.Empty;

                /* Ms Sql or SQLite */
                if (TryGetValue("Initial Catalog", out S))
                {
                    if (TryGetValue("Data Source", out S))
                        return S;
                    else
                        return "localhost";
                }
                /* ADO.NET Firebird */
                else if (TryGetValue("Database", out S))
                {
                    if (TryGetValue("DataSource", out S) || TryGetValue("Data Source", out S))
                        return S;
                    else
                        return "localhost";
                }

                /* Oracle or an OLEDB provider */
                else if (TryGetValue("Data Source", out S) || TryGetValue("Server ", out S))
                {
                    return S;
                }

                return string.Empty;
            }
        }
        /// <summary>
        /// Gets or sets the value of the Provider key in the connection string for the OleDb ADO.NET Provider
        /// </summary>
        public string OleDbProvider
        {
            get
            {
                string S = string.Empty;
                TryGetValue("Provider", out S);
                return S;
            }
            set { this["Provider"] = value; }
        }
        /// <summary>
        /// Gets or sets the value of the Extended Properties key in the connection string for the OleDb ADO.NET Provider
        /// </summary>
        public string ExtendedProperties
        {
            get
            {
                string S = string.Empty;
                TryGetValue("Extended Properties", out S);
                return S;
            }

            set { this["Extended Properties"] = value; }
        }
    }
}




