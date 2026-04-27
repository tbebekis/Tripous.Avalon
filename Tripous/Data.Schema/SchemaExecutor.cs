namespace Tripous.Data
{
    /// <summary>
    /// The schema execution engine. Creates database tables, etc. 
    /// </summary>
    internal class SchemaExecutor
    {
        SchemaVersion Schema;
        DbConnectionInfo ConnectionInfo;
        SqlStore Store;
        DbTransaction transaction;

        List<string> TableNamesList;
        List<string> IndexNamesList;

        /* construction */
        private SchemaExecutor(DbConnectionInfo ConnectionInfo, SchemaVersion SchemaVersion)
        {
            this.ConnectionInfo = ConnectionInfo;
            this.Store = SqlStores.CreateSqlStore(ConnectionInfo);
            this.Schema = SchemaVersion;
        }

        /* execution */
        void Execute()
        {
 
            TableNamesList = new List<string>(Store.GetTableNames());
            IndexNamesList = new List<string>(Store.GetIndexNames());

            /* start a transaction */
            using (transaction = Store.BeginTransaction())
            {
                try
                {
                    /* database statements -before */
                    foreach (string SqlText in Schema.StatementsBefore)
                        DoStatement(SqlText);

                    /* tables */
                    foreach (SchemaItem Table in Schema.Tables)
                        DoTable(Table);

                    /* views */
                    foreach (SchemaItem View in Schema.Views)
                        DoView(View);

                    /* commit the transaction */
                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }


            /* inserts, alter table add-drop-rename column, etc. etc. after the database is created */
            /* start a transaction */
            using (transaction = Store.BeginTransaction())
            {
                try
                {
                    /* database statements -after */
                    foreach (string SqlText in Schema.StatementsAfter)
                        DoStatement(SqlText);

                    /* commit the transaction */
                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }



        }
        void Process(string SqlText)
        {
            if (!string.IsNullOrEmpty(SqlText))
            {
                SqlText = Store.Provider.ReplaceDataTypePlaceholders(SqlText);
                Store.ExecSql(transaction, SqlText);
            }
        }

        void DoTable(SchemaItem Table)
        {
            /* create table */
            if (!TableNamesList.ContainsText(Table.Name))
            {
                Process(Table.SqlText);

                /* generator */
                if (Store.Provider.SupportsGenerators && Store.ConnectionInfo.AutoCreateGenerators && !Store.Provider.GeneratorExists(Store.ConnectionInfo.ConnectionString, "G_" + Table.Name))
                {
                    Store.Provider.CreateGenerator(Store.ConnectionInfo.ConnectionString, "G_" + Table.Name);
                }

            }

        }
        void DoView(SchemaItem View)
        {
            if (!TableNamesList.ContainsText(View.Name))
                Process(View.SqlText);
        }
        void DoStatement(string SqlText)
        {
            if (!string.IsNullOrWhiteSpace(SqlText))
            {
                SqlText = SqlText.Trim();
                while (SqlText.Contains("  "))
                    SqlText = SqlText.Replace("  ", " ");

                if (SqlText.StartsWith("create index", StringComparison.InvariantCultureIgnoreCase)
                    || SqlText.StartsWith("create unique index", StringComparison.InvariantCultureIgnoreCase))
                {
                    /* extract index name */
                    string IndexName = ExtractIndexName("create index ", SqlText);
                    if (string.IsNullOrWhiteSpace(IndexName))
                        IndexName = ExtractIndexName("create unique index ", SqlText);

                    /* create only if not exists */
                    if (IndexNamesList.ContainsText(IndexName))
                        return;
                }
                /*
                 else if (SqlText.StartsWith("alter table", StringComparison.InvariantCultureIgnoreCase)
                    && SqlText.ToLower().Contains("column"))
                {
                    SqlText = Store.Provider.NormalizeAlterTableColumnSql(SqlText);
                }                
                 */


                Process(SqlText);
            }


        }

        /* utils */
        string ExtractIndexName(string CreateSql, string SqlText)
        {
            if (SqlText.StartsWith(CreateSql, StringComparison.InvariantCultureIgnoreCase))
            {
                SqlText = SqlText.Remove(0, CreateSql.Length).TrimStart();
                string[] Words = SqlText.Split(' ');
                return Words[0];
            }
            return string.Empty;
        }

        /// <summary>
        /// Executes the schema. If no connection info is specified then the default connection is used.
        /// </summary>
        static internal void Execute(SchemaVersion SchemaVersion, DbConnectionInfo ConnectionInfo = null)
        {
            if (ConnectionInfo == null)
                ConnectionInfo = Db.GetDefaultConnectionInfo();

            new SchemaExecutor(ConnectionInfo, SchemaVersion).Execute();
        }
    }

}
