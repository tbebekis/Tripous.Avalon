namespace Tripous.Data;

/// <summary>
/// Provides values to Sql statements by replacing placeholders, such as AppData and AppUserName, with actual values.
/// <para>Provides values to DataRows when they have DataColumns named such as AppData and AppUserName. </para>  
/// </summary>
static public class SqlValueProviders
{
    static readonly System.Threading.Lock syncLock = new();
    static List<ISqlValueProvider> fList = new List<ISqlValueProvider>();

    /* construction */
    /// <summary>
    /// Constructor
    /// </summary>
    static SqlValueProviders()
    {
        Add(new SqlValueProviderInternal());
    }

    /* public */
    /// <summary>
    /// Registers a provider
    /// </summary>
    static public void Add(ISqlValueProvider Store)
    {
        lock (syncLock)
        {
            if ((Store != null) && !fList.Contains(Store))
                fList.Insert(0, Store);
        }
    }
    /// <summary>
    /// Unregisters a provider
    /// </summary>
    static public void Remove(ISqlValueProvider Store)
    {
        lock (syncLock)
        {
            fList.Remove(Store);
        }
    }


    /// <summary>
    /// Replaces certain placeholders in SqlText with actual values.
    /// </summary>
    static public void Process(ref string SqlText, SqlStore Store = null)
    {
        lock (syncLock)
        {
            for (int i = 0; i < fList.Count; i++)
            {
                fList[i].Process(ref SqlText, Store);
            }
        }
    }
    /// <summary>
    /// Set the value to a Column of a Row of a Table, based on a Keyword which may have values such as AppData or AppUserName.
    /// </summary>
    static public void Process(DataRow Row, DataColumn Column, string Keyword, SqlStore Store = null)
    {
        lock (syncLock)
        {
            for (int i = 0; i < fList.Count; i++)
            {
                fList[i].Process(Row, Column, Keyword, Store);
            }
        }
    }
    /// <summary>
    /// Sets values to Row for columns with names such as AppData or AppUserName.
    /// </summary>
    static public void Process(DataRow Row, SqlStore Store = null)
    {
        lock (syncLock)
        {
            for (int i = 0; i < fList.Count; i++)
            {
                fList[i].Process(Row, Store);
            }
        }
    }
    /// <summary>
    /// Sets values to all rows of the Table, for columns with names such as AppData or AppUserName.
    /// </summary>
    static public void Process(DataTable Table, SqlStore Store = null)
    {
        lock (syncLock)
        {
            for (int i = 0; i < fList.Count; i++)
            {
                fList[i].Process(Table, Store);
            }
        }
    }

}


/// <summary>
/// Provides values to Sql statements by replacing placeholders, such as AppData and AppUserName, with actual values.
/// <para>Provides values to DataRows when they have DataColumns named such as AppData and AppUserName. </para>  
/// </summary>
public interface ISqlValueProvider
{
    /// <summary>
    /// Replaces certain placeholders in SqlText with actual values.
    /// </summary>
    void Process(ref string SqlText, SqlStore Store = null);
    /// <summary>
    /// Set the value to a Column of a Row of a Table, based on a Keyword which may have values such as AppData or AppUserName.
    /// </summary>
    void Process(DataRow Row, DataColumn Column, string Keyword, SqlStore Store = null);
    /// <summary>
    /// Sets values to Row for columns with names such as AppData or AppUserName.
    /// </summary>
    void Process(DataRow Row, SqlStore Store = null);
    /// <summary>
    /// Sets values to all rows of the Table, for columns with names such as AppData or AppUserName.
    /// </summary>
    void Process(DataTable Table, SqlStore Store = null);
}


/// <summary>
/// Provides values to Sql statements by replacing placeholders, such as AppData and AppUserName, with actual values.
/// <para>Provides values to DataRows when they have DataColumns named such as AppData and AppUserName. </para>  
/// </summary>
internal class SqlValueProviderInternal : ISqlValueProvider
{
    string[] Trues = { "true", "yes", "1" };
    string[] Falses = { "false", "no", "0" };

    /// <summary>
    /// Replaces certain placeholders in SqlText with actual values.
    /// </summary>
    public void Process(ref string SqlText, SqlStore Store = null)
    {

        if (!string.IsNullOrWhiteSpace(SqlText))
        {
            string NewValue;
            string Prefix = SqlProvider.GlobalPrefix.ToString();
            //DateTime DT;

            /* replace any company field name place-holder, the SystemSchema tables or any other sql statement may contain */
            SqlText = SqlText.Replace("@COMPANY_ID", SysConfig.CompanyFieldName);

            /* @:CompanyId and :CompanyId */
            SqlText = SqlText.Replace(SysConfig.VariablesPrefix + SysConfig.CompanyFieldName, SysConfig.CompanyIdSql);
            SqlText = SqlText.Replace(Prefix + SysConfig.CompanyFieldName, SysConfig.CompanyIdSql);

            /* :@AppDate and :AppDate */
            NewValue = DateTime.Today.Date.ToString("yyyy-MM-dd").QS();
            SqlText = SqlText.Replace(SysConfig.VariablesPrefix + "AppDate", NewValue);
            SqlText = SqlText.Replace(Prefix + "AppDate", NewValue);

            /* :@SysDate and :SysDate */
            NewValue = DateTime.Today.ToString("yyyy-MM-dd").QS();
            SqlText = SqlText.Replace(SysConfig.VariablesPrefix + "SysDate", NewValue);
            SqlText = SqlText.Replace(Prefix + "SysDate", NewValue);

            /* :@SysTime and :SysTime */
            NewValue = DateTime.Now.ToString("HH:mm:ss").QS();
            SqlText = SqlText.Replace(SysConfig.VariablesPrefix + "SysTime", NewValue);
            SqlText = SqlText.Replace(Prefix + "SysTime", NewValue);

            /* :@DbServerTime and :DbServerTime  - WARNING: NO it goes to infinite loop
            DT = Store != null ? Store.GetServerDateTime() : DateTime.Now;
            NewValue = Store != null ? Store.QSDateTime(DT) : DT.ToString("yyyy-MM-dd HH:mm:ss").QS();

            SqlText = SqlText.Replace(Sys.VariablesPrefix + "DbServerTime", NewValue);
            SqlText = SqlText.Replace(":DbServerTime", NewValue);
            */

            // TODO: this should go to a IDataValueProvider in some of the application namespaces
            /* :@AppUserName and :AppUserName */
            /*
            if (User.IsReady)
            {
                SqlText = SqlText.Replace(SysConfig.VariablesPrefix + "AppUserName", User.UserName.QS());
                SqlText = SqlText.Replace(":AppUserName", User.UserName.QS());
            }
            */

            // DateRange
            if (Store != null)
                SelectSql.DateRangeReplaceWhereParams(ref SqlText, Store.Provider);
        }


    }
    /// <summary>
    /// Set the value to a Column of a Row of a Table, based on a Keyword which may have values such as AppData or AppUserName.
    /// </summary>
    public void Process(DataRow Row, DataColumn Column, string Keyword, SqlStore Store = null)
    {
        if (!Column.ReadOnly
            && Row.RowState != DataRowState.Deleted
            && (Sys.IsNull(Row[Column]) || (Simple.SimpleTypeOf(Column.DataType).IsString() && (Row[Column].ToString() == string.Empty)))
            )
        {

            if (!string.IsNullOrWhiteSpace(Keyword) && !Sys.IsSameText(Sys.NULL, Keyword))
            {
                if ((Sys.IsSameText(Keyword, "CompanyId")) || Sys.IsSameText(SysConfig.CompanyFieldName, Column.ColumnName))
                    Row[Column] = SysConfig.CompanyId;
                else if (Sys.IsSameText(Keyword, "EmptyString"))
                    Row[Column] = string.Empty;
                else if (Sys.IsSameText(Keyword, "AppDate"))
                    Row[Column] = DateTime.Today.Date;
                else if (Sys.IsSameText(Keyword, "SysDate"))
                    Row[Column] = DateTime.Now;
                else if (Sys.IsSameText(Keyword, "SysTime"))
                    Row[Column] = DateTime.Now.TimeOfDay;

                /* TODO: this should go to a IDataValueProvider in some of the application namespaces
                else if (Sys.IsSameText(Keyword, "AppUserName") && User.IsReady)
                    Row[Column] = User.UserName;
                else if (Sys.IsSameText(Keyword, "AppUserId") && User.IsReady)
                    Row[Column] = User.Id;
                */

                else if (Sys.IsSameText(Keyword, "NetUserName"))
                    Row[Column] = Environment.UserName;
                else if (Sys.IsSameText(Keyword, "Guid"))
                    Row[Column] = Sys.GenId();
                else if (Sys.IsSameText(Keyword, "DbServerTime"))
                    Row[Column] = Store != null ? Store.GetServerDateTime() : DateTime.Now;
                else
                {
                    object Value = Column.DataType.ImplementsInterface(typeof(IConvertible)) ? Convert.ChangeType(Keyword, Column.DataType, CultureInfo.InvariantCulture) : null;
                    if (!Sys.IsNull(Value))
                        Row[Column] = Value;
                }
            }


            /* if still is null */
            if (Sys.IsNull(Row[Column]) && Column.DataType == typeof(System.Boolean))
            {
                Row[Column] = (!string.IsNullOrWhiteSpace(Keyword) && Trues.ContainsText(Keyword)) ? true : false;
            }
            else if (Sys.IsNull(Row[Column]) && Simple.SimpleTypeOf(Column.DataType).IsInteger())
            {
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    if (Trues.ContainsText(Keyword))
                        Row[Column] = 1;
                    else if (Falses.ContainsText(Keyword))
                        Row[Column] = 0;
                }
            }
            else if (Sys.IsNull(Row[Column]) || Simple.SimpleTypeOf(Column.DataType).IsString() && (Row[Column].ToString() == string.Empty))
            {
                if (Sys.IsSameText(SysConfig.CompanyFieldName, Column.ColumnName)) // ColumnName is CompanyId
                    Row[Column] = SysConfig.CompanyId;
            }

        }
    }
    /// <summary>
    /// Sets values to Row for columns with names such as AppData or AppUserName.
    /// </summary>
    public void Process(DataRow Row, SqlStore Store = null)
    {
        if (Row.RowState == DataRowState.Deleted)
            return;

        // replacement code
        foreach (DataColumn Column in Row.Table.Columns)
        {
            if (Sys.IsNull(Row[Column]) || (Simple.SimpleTypeOf(Column.DataType).IsString() && (Row[Column].ToString() == string.Empty)))
            {
                if (Sys.IsSameText(SysConfig.CompanyFieldName, Column.ColumnName))
                    Row[Column] = SysConfig.CompanyId;
                else if (Sys.IsSameText("AppDate", Column.ColumnName))
                    Row[Column] =  DateTime.Today.Date;
                /*
                                     else if ((Sys.IsSameText("AppUserName", Column.ColumnName)) && User.IsReady)
                                        Row[Column] = User.UserName;
                                    else if (Sys.IsSameText("NetUserName", Column.ColumnName))
                                        Row[Column] = Sys.NetUserName;
                                     */
                else if (Sys.IsSameText("GuidStamp", Column.ColumnName))
                    Row[Column] = Sys.GenId();
                else if (Sys.IsSameText("DbServerTime", Column.ColumnName))
                    Row[Column] = Store != null ? Store.GetServerDateTime() : DateTime.Now;
            }
        }


    }
    /// <summary>
    /// Sets values to all rows of the Table, for columns with names such as AppData or AppUserName.
    /// </summary>
    public void Process(DataTable Table, SqlStore Store = null)
    {
        foreach (DataRow Row in Table.Rows)
            Process(Row, Store);
    }

}