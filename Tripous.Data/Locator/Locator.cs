namespace Tripous.Data;

/// <summary>
/// Runtime locator.
/// </summary>
[TypeStore]
public class Locator
{
    // ● protected methods
    /// <summary>
    /// Returns true when the specified source is a SELECT statement.
    /// </summary>
    protected virtual bool IsSelectSource(string Source) => !string.IsNullOrWhiteSpace(Source) && Source.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase);
    /// <summary>
    /// Returns the result field list as SQL text.
    /// </summary>
    protected virtual string GetSqlResultFields(LocatorDef LocatorDef)
    {
        List<string> FieldNames = LocatorDef.GetResultFields();
        return FieldNames.Count > 0 ? string.Join(", ", FieldNames) : "*";
    }
    /// <summary>
    /// Returns the base SELECT statement.
    /// </summary>
    protected virtual string GetBaseSql(LocatorDef LocatorDef)
    {
        string ResultFields = GetSqlResultFields(LocatorDef);
        return IsSelectSource(LocatorDef.Source) ? $"select {ResultFields} from ({LocatorDef.Source}) X" : $"select {ResultFields} from {LocatorDef.Source}";
    }
    /// <summary>
    /// Returns the search field names to use.
    /// </summary>
    protected virtual List<string> GetSearchFieldNames(LocatorDef LocatorDef, LocatorRequest Request)
    {
        if (!string.IsNullOrWhiteSpace(Request.SearchField))
            return [Request.SearchField];

        return LocatorDef.GetSearchFields(Request.IsMultiRow);
    }
    /// <summary>
    /// Returns the key field definition.
    /// </summary>
    protected virtual LocatorFieldDef GetKeyFieldDef(LocatorDef LocatorDef)
    {
        return LocatorDef.Fields.Find(LocatorDef.KeyField);
    }
    /// <summary>
    /// Returns a SQL value literal.
    /// </summary>
    protected virtual string GetSqlValue(object Value, DataFieldType DataType)
    {
        if (Sys.IsNull(Value))
            return "NULL";

        switch (DataType)
        {
            case DataFieldType.Integer:
            case DataFieldType.Double:
            case DataFieldType.Decimal:
            case DataFieldType.Decimal_:
                return Convert.ToString(Value, CultureInfo.InvariantCulture);
            case DataFieldType.Boolean:
                return Convert.ToBoolean(Value) ? "1" : "0";
            case DataFieldType.Date:
                return Convert.ToDateTime(Value).ToString("yyyy-MM-dd").QS();
            case DataFieldType.DateTime:
                return Convert.ToDateTime(Value).ToString("yyyy-MM-dd HH:mm:ss").QS();
            default:
                return Value.ToString().Replace("'", "''").QS();
        }
    }
    /// <summary>
    /// Returns the SQL WHERE clause for a key value.
    /// </summary>
    protected virtual string GetKeyWhereSql(LocatorDef LocatorDef, LocatorRequest Request)
    {
        LocatorFieldDef FieldDef = GetKeyFieldDef(LocatorDef);
        DataFieldType DataType = FieldDef != null ? FieldDef.DataType : DataFieldType.String;
        return $"{LocatorDef.KeyField} = {GetSqlValue(Request.KeyValue, DataType)}";
    }
    /// <summary>
    /// Returns the SQL WHERE clause for a search term.
    /// </summary>
    protected virtual string GetWhereSql(LocatorDef LocatorDef, LocatorRequest Request)
    {
        if (!Sys.IsNull(Request.KeyValue))
            return GetKeyWhereSql(LocatorDef, Request);

        if (string.IsNullOrWhiteSpace(Request.SearchTerm))
            return string.Empty;

        string Term = Request.SearchTerm.Replace("'", "''");
        List<string> Items = [];

        foreach (string FieldName in GetSearchFieldNames(LocatorDef, Request))
        {
            LocatorFieldDef FieldDef = LocatorDef.Fields.Find(FieldName);
            if (FieldDef != null && FieldDef.DataType == DataFieldType.String)
                Items.Add($"{FieldDef.Name} like '%{Term}%'");
        }

        return Items.Count > 0 ? string.Join(" or ", Items) : string.Empty;
    }
    /// <summary>
    /// Returns the SELECT statement to execute.
    /// </summary>
    protected virtual string GetSql(LocatorDef LocatorDef, LocatorRequest Request)
    {
        string Result = GetBaseSql(LocatorDef);
        string Where = GetWhereSql(LocatorDef, Request);

        if (!string.IsNullOrWhiteSpace(Where))
            Result = $"select * from ({Result}) X where {Where}";

        if (!string.IsNullOrWhiteSpace(LocatorDef.OrderBy))
            Result += $" order by {LocatorDef.OrderBy}";

        return Store.Provider.ApplyRowLimit(Result, LocatorDef.MaximumResultCount + 1);
    }
    /// <summary>
    /// Returns an invalid request result.
    /// </summary>
    protected virtual LocatorResult InvalidRequest(string Message)
    {
        return new LocatorResult()
        {
            Status = LocatorResultStatus.InvalidRequest,
            Message = Message,
        };
    }
    /// <summary>
    /// Returns the status for a row count.
    /// </summary>
    protected virtual LocatorResultStatus GetStatus(LocatorDef LocatorDef, int RowCount)
    {
        if (RowCount == 0)
            return LocatorResultStatus.NoResult;
        if (RowCount == 1)
            return LocatorResultStatus.SingleResult;
        if (RowCount > LocatorDef.MaximumResultCount)
            return LocatorResultStatus.TooManyResults;
        return LocatorResultStatus.MultipleResults;
    }
    /// <summary>
    /// Checks the specified request and returns an error result if it is invalid.
    /// </summary>
    protected virtual LocatorResult CheckRequest(LocatorDef LocatorDef, LocatorRequest Request)
    {
        if (Sys.IsNull(Request.KeyValue) && !string.IsNullOrWhiteSpace(Request.SearchTerm) && Request.SearchTerm.Length < LocatorDef.MinimumSearchLength)
            return InvalidRequest($"Locator search term must contain at least {LocatorDef.MinimumSearchLength} characters.");

        if (!string.IsNullOrWhiteSpace(Request.SearchField) && !LocatorDef.GetAllSearchFields().Any(x => x.IsSameText(Request.SearchField)))
            return InvalidRequest($"Locator search field not found: {Request.SearchField}");

        return null;
    }

    // ● public
    /// <summary>
    /// Executes a locator request.
    /// </summary>
    public virtual LocatorResult Execute(LocatorDef LocatorDef, LocatorRequest Request)
    {
        LocatorResult Result = CheckRequest(LocatorDef, Request);
        if (Result != null)
            return Result;

        MemTable Table = new(LocatorDef.Name);
        int RowCount = Store.SelectTo(Table, GetSql(LocatorDef, Request));
        LocatorResultStatus Status = GetStatus(LocatorDef, RowCount);

        return new LocatorResult()
        {
            Status = Status,
            Message = Status == LocatorResultStatus.TooManyResults ? "Too many results. Type more characters." : string.Empty,
            Table = Status == LocatorResultStatus.TooManyResults ? null : Table,
        };
    }

    // ● properties
    /// <summary>
    /// Returns the SQL store to use.
    /// </summary>
    protected virtual SqlStore Store => Db.DefaultStore;
}
