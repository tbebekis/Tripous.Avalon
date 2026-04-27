namespace Tripous.Data;

public class SelectSql : BaseDef
{
    // ● private
    string fConnectionName;


    // ● static public
    /// <summary>
    /// LineBreak
    /// </summary>
    static public readonly string LB = Environment.NewLine;
    /// <summary>
    /// Space
    /// </summary>
    static public readonly string SPACE = " ";
    /// <summary>
    /// Spaces
    /// </summary>
    static public readonly string SPACES = "  ";
    /// <summary>
    /// Constant
    /// </summary>
    static public readonly string DefaultName = "SelectSql";

    // ● private
    static string StripClauseKeyword(string Text, string Keyword)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return string.Empty;

        Text = Text.Trim();
        Keyword = Keyword.Trim();

        if (Text.StartsWith(Keyword, StringComparison.InvariantCultureIgnoreCase))
            return Text.Substring(Keyword.Length).Trim();

        return Text;
    }
    static string FirstFromToken(string FromClause)
    {
        if (string.IsNullOrWhiteSpace(FromClause))
            return string.Empty;

        string S = StripClauseKeyword(FromClause, "from").Trim();

        if (S.StartsWith("("))
            return string.Empty;

        StringBuilder SB = new StringBuilder();
        bool InQuotedName = false;
        char QuoteEnd = '\0';

        for (int i = 0; i < S.Length; i++)
        {
            char C = S[i];

            if (InQuotedName)
            {
                SB.Append(C);
                if (C == QuoteEnd)
                    InQuotedName = false;
                continue;
            }

            if (C == '"' || C == '`')
            {
                InQuotedName = true;
                QuoteEnd = C;
                SB.Append(C);
                continue;
            }

            if (C == '[')
            {
                InQuotedName = true;
                QuoteEnd = ']';
                SB.Append(C);
                continue;
            }

            if (char.IsWhiteSpace(C) || C == ',')
                break;

            SB.Append(C);
        }

        return SB.ToString().Trim();
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public SelectSql()
    {
        this.Name = DefaultName;
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public SelectSql(string StatementText)
        : this()
    {
        this.Text = StatementText;
    }

    // ● static public
    /// <summary>
    /// Adds a Carriage Return (LineBreak) after S
    /// </summary>
    static public string CR(string S)
    {
        if (S != null)
            S = S.TrimEnd() + LB;

        return S;
    }
    /// <summary>
    /// Replaces any trailing comma with space in S, ignoring trailing spaces.
    /// </summary>
    static public void ReplaceLastComma(ref string S)
    {
        if (!string.IsNullOrWhiteSpace(S))
        {
            StringBuilder SB = new StringBuilder(S);
            int i = SB.Length;
            char C;

            while (true)
            {
                i--;
                if (i < 0)
                    break;

                C = SB[i];

                if (C == ',')
                {
                    SB[i] = ' ';
                    break;
                }
                else if (!char.IsWhiteSpace(C))
                    break;
            }

            S = SB.ToString();
        }
    }
    /// <summary>
    /// Concatenates Clause + Delimiter + Plus
    /// </summary>
    static public string AddTo(string Clause, string Delimiter, string Plus)
    {
        if (string.IsNullOrWhiteSpace(Plus))
            Plus = string.Empty;

        if (!string.IsNullOrWhiteSpace(Clause) && !string.IsNullOrWhiteSpace(Delimiter))
        {
            Clause = Clause.TrimEnd();
            Delimiter = Delimiter.Trim();
            Plus = Plus.Trim();

            if (Clause.EndsWith(Delimiter, StringComparison.InvariantCultureIgnoreCase)
                || Plus.StartsWith(Delimiter, StringComparison.InvariantCultureIgnoreCase))
                return CR(Clause) + SPACES + Plus.Trim();
            else
                return CR(Clause) + SPACES + Delimiter + SPACE + Plus;
        }

        return SPACES + Plus.Trim();
    }
    /// <summary>
    /// Concatenates Keyword + Clause
    /// </summary>
    static public string NormalizeClause(string Clause, string Keyword)
    {
        if (!string.IsNullOrWhiteSpace(Clause) && !string.IsNullOrWhiteSpace(Keyword))
        {
            Clause = Clause.Trim();
            Keyword = Keyword.Trim();

            if (!Clause.StartsWith(Keyword, StringComparison.InvariantCultureIgnoreCase))
                return CR(Keyword) + SPACES + Clause;
        }

        return !string.IsNullOrWhiteSpace(Clause) ? Clause : string.Empty;
    }
    /// <summary>
    /// Returns true if Value contains any of the mask characters (%, ?, *)
    /// </summary>
    static public bool IsMasked(string Value)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return false;

        if (Value.IndexOf('*') != -1)
            return true;

        if (Value.IndexOf('?') != -1)
            return true;

        if (Value.IndexOf('%') != -1)
            return true;

        return false;
    }
    /// <summary>
    /// Constructs a DateTime param pair (date range params) suitable for thw WHERE part in a SelectSql
    /// </summary>
    static public string DateRangeConstructWhereParams(DateRange Range, string FieldName)
    {
        string sFrom = ":" + DateRanges.PrefixFrom + Range.ToString();
        string sTo = ":" + DateRanges.PrefixTo + Range.ToString();
        string S = string.Format(" (({0} >= {1}) and ({0} <= {2})) ", FieldName, sFrom, sTo);
        return S;
    }
    /// <summary>
    /// Replaces any DateTime param pair (date range params) found in SqlText with actual values.
    /// </summary>
    static public void DateRangeReplaceWhereParams(ref string SqlText, SqlProvider Provider)
    {
        if (!SqlText.Contains(DateRanges.PrefixFrom))
            return;

        string sFrom = string.Empty;
        string sTo = string.Empty;

        Func<DateRange, string, string> Replace = delegate (DateRange Range, string S)
        {
            DateTime FromDate = DateTime.Now;
            DateTime ToDate = DateTime.Now;

            Range.ToDates(DateTime.Now, ref FromDate, ref ToDate);

            string sFromValue = Provider.QSDateTime(FromDate.StartOfDay());
            string sToValue = Provider.QSDateTime(ToDate.EndOfDay());

            S = S.Replace(sFrom, sFromValue);
            S = S.Replace(sTo, sToValue);

            return S;
        };

        string Prefix = SqlProvider.GlobalPrefix.ToString();
        foreach (DateRange Range in DateRanges.WhereRanges)
        {
            sFrom = Prefix + DateRanges.PrefixFrom + Range.ToString();
            if (SqlText.Contains(sFrom))
            {
                sTo = Prefix + DateRanges.PrefixTo + Range.ToString();
                SqlText = Replace(Range, SqlText);
                return;
            }
        }
    }

    // ● public
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        //base.CheckDescriptor();
        
        if (string.IsNullOrWhiteSpace(this.Name) || string.IsNullOrWhiteSpace(this.Select) || string.IsNullOrWhiteSpace(this.From))
            Sys.Throw(Texts.GS($"E_{typeof(SelectSql)}_NotFullyDefined", $"{typeof(SelectSql)} Name or SQL statement is empty"));
    }
    /// <summary>
    /// Clears the content of the clause properties
    /// </summary>
    public override void Clear()
    {
        Select = string.Empty;
        From = string.Empty;
        Where = string.Empty;
        WhereUser = string.Empty;
        GroupBy = string.Empty;
        Having = string.Empty;
        OrderBy = string.Empty;

        CompanyAware = false;
        ConnectionName = string.Empty;

        DateRangeColumn = string.Empty;
        DateRange = Tripous.DateRange.Custom;
    }
    /// <summary>
    /// Assigns this properties from Source.
    /// <para>WARNING: We need this because an automated Assign() calls the Text property
    /// which in turn calls the SelectSqlParser which has no ability to handle fields
    /// surrounded by double quotes or [] or something.</para>
    /// </summary>
    public void Assign(object Source)
    {
        if (Source is SelectSql)
        {
            SelectSql Src = Source as SelectSql;

            this.Name = Src.Name;
            this.TitleKey = Src.TitleKey;

            Select = Src.Select;
            From = Src.From;
            Where = Src.Where;
            WhereUser = Src.WhereUser;
            GroupBy = Src.GroupBy;
            Having = Src.Having;
            OrderBy = Src.OrderBy;

            CompanyAware = Src.CompanyAware;
            ConnectionName = Src.ConnectionName;

            DateRangeColumn = Src.DateRangeColumn;
            DateRange = Src.DateRange;
        }
        else
        {
            Clear();
        }
    }
    /// <summary>
    /// Returns a clone of this instance.
    /// </summary>
    public override BaseDef Clone()
    {
        SelectSql Result = new SelectSql();
        Result.Assign(this);
        return Result;
    }
    /// <summary>
    /// Concatenates Keyword + Clause for all clauses
    /// </summary>
    public string GetSqlText()
    {
        string sSelect = NormalizeClause(Select, "select") + SPACES;
        string sFrom = NormalizeClause(From, "from") + SPACES;
        string sWhere = string.IsNullOrWhiteSpace(Where) ? string.Empty : Where.Trim();

        if (CompanyAware)
        {
            string sCompany = SysConfig.CompanyFieldName + string.Format(" = {0}{1}", SysConfig.VariablesPrefix, SysConfig.CompanyFieldName);

            if (!sWhere.Contains(sCompany.Trim()))
                sWhere = sWhere.Length == 0 ? sCompany : AddTo(sWhere, "and", sCompany);
        }

        if (!string.IsNullOrWhiteSpace(DateRangeColumn) && this.DateRange.IsPast())
        {
            string Range = SelectSql.DateRangeConstructWhereParams(this.DateRange, DateRangeColumn);

            if (!sWhere.Contains(Range.Trim()))
                sWhere = sWhere.Length == 0 ? Range : AddTo(sWhere, "and", Range);
        }

        if (!string.IsNullOrWhiteSpace(WhereUser) && WhereUser.Trim().Length > 0)
        {
            if (!sWhere.Contains(WhereUser.Trim()))
                sWhere = sWhere.Length == 0 ? WhereUser : AddTo(sWhere, "and", WhereUser);
        }

        sWhere = NormalizeClause(sWhere, "where");

        string sGroupBy = NormalizeClause(GroupBy, "group by");
        string sHaving = NormalizeClause(Having, "having");
        string sOrderBy = NormalizeClause(OrderBy, "order by");

        string Result = sSelect.TrimEnd() + LB + sFrom.TrimEnd();

        if (sWhere.Trim().Length > 0)
            Result += LB + sWhere.TrimEnd() + SPACES;
        if (sGroupBy.Trim().Length > 0)
            Result += LB + sGroupBy.TrimEnd() + SPACES;
        if (sHaving.Trim().Length > 0)
            Result += LB + sHaving.TrimEnd() + SPACES;
        if (sOrderBy.Trim().Length > 0)
            Result += LB + sOrderBy.TrimEnd() + SPACES;

        return Result;
    }
    
    /// <summary>
    /// Concatenates WHERE + and + Plus
    /// </summary>
    public void AddToWhere(string Plus)
    {
        Where = AddTo(Where, "and", Plus);
    }
    /// <summary>
    /// Concatenates WHERE + or + Plus
    /// </summary>
    public void OrToWhere(string Plus)
    {
        Where = AddTo(Where, "or", Plus);
    }
    /// <summary>
    /// Concatenates WHERE + Delimiter + Plus
    /// </summary>
    public void AddToWhere(string Plus, string Delimiter)
    {
        Where = AddTo(Where, Delimiter, Plus);
    }
    /// <summary>
    /// Concatenates GROUP BY + , + Plus
    /// </summary>
    public void AddToGroupBy(string Plus)
    {
        GroupBy = AddTo(GroupBy, ",", Plus);
    }
    /// <summary>
    /// Concatenates HAVING + and + Plus
    /// </summary>
    public void AddToHaving(string Plus)
    {
        Having = AddTo(Having, "and", Plus);
    }
    /// <summary>
    /// Concatenates ORDER BY + , + Plus
    /// </summary>
    public void AddToOrderBy(string Plus)
    {
        OrderBy = AddTo(OrderBy, ",", Plus);
    }
    
    /// <summary>
    /// Removes the ORDER BY clause.
    /// </summary>
    public void RemoveOrderBy()
    {
        OrderBy = string.Empty;
    }
    
    /// <summary>
    /// Returns concatenated the SELECT and FROM clauses only.
    /// </summary>
    public string SelectFromToString()
    {
        return NormalizeClause(Select, "select") + LB + NormalizeClause(From, "from");
    }
    /// <summary>
    /// Parses StatementText and assigns its clause properties.
    /// </summary>
    public void Parse(string StatementText)
    {
        Select = string.Empty;
        From = string.Empty;
        Where = string.Empty;
        WhereUser = string.Empty;
        GroupBy = string.Empty;
        Having = string.Empty;
        OrderBy = string.Empty;

        if (!string.IsNullOrWhiteSpace(StatementText))
        {
            SelectSqlParser Parser = SelectSqlParser.Execute(StatementText);

            this.Select = Parser.Select;
            this.From = Parser.From;
            this.Where = Parser.Where;
            this.GroupBy = Parser.GroupBy;
            this.Having = Parser.Having;
            this.OrderBy = Parser.OrderBy;
        }
    }
    /// <summary>
    /// Creates a select * from TableName statement and then calls Parse()
    /// </summary>
    public void ParseFromTableName(string TableName)
    {
        Parse("select * from " + TableName);
    }
    /// <summary>
    /// Tries to get the main table name from the statement
    /// </summary>
    public string GetMainTableName()
    {
        return FirstFromToken(this.From);
    }

    // ● properties
    /// <summary>
    /// Gets the statement as a whole.
    /// <para>Sets clause properties by parsing the passed string value.</para>
    /// </summary>
    public string Text
    {
        get { return string.IsNullOrWhiteSpace(Select) ? string.Empty : GetSqlText(); }
        set { Parse(value); }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this object
    /// <para>uses the CompanyFieldName when preparing the statement as a whole</para>
    /// </summary>
    public bool CompanyAware { get; set; }
    /// <summary>
    /// Gets or sets the database connection name
    /// </summary>
    public string ConnectionName
    {
        get { return string.IsNullOrWhiteSpace(fConnectionName) ? SysConfig.DefaultConnectionName : fConnectionName; }
        set { fConnectionName = value; }
    }
 
    /// <summary>
    /// A fully qualified (i.e. TABLE_NAME.FIELD_NAME) column of type date or datetime.
    /// <para>It works in conjuction with DateRange property in order to produce
    /// a fixed part in the WHERE clause of this select statement.</para>
    /// </summary>
    public string DateRangeColumn { get; set; } = "";
    /// <summary>
    /// A DateRange.
    /// <para>It works in conjuction with DateRangeColumn property in order to produce
    /// a fixed part in the WHERE clause of this select statement.</para>
    /// </summary>
    public DateRange DateRange { get; set; } = DateRange.LastWeek;
    
    /// <summary>
    /// Gets or sets the SELECT clause.
    /// </summary>
    [JsonIgnore]
    public string Select { get; set; }
    /// <summary>
    /// Gets or sets the FROM clause.
    /// </summary>
    [JsonIgnore]
    public string From { get; set; }
    /// <summary>
    /// Gets or sets the WHERE clause.
    /// </summary>
    [JsonIgnore]
    public string Where { get; set; }
    /// <summary>
    /// Gets or sets the WHERE clause, that part that is considered user where.
    /// </summary>
    [JsonIgnore]
    public string WhereUser { get; set; }
    /// <summary>
    /// Gets or sets the GROUP BY clause.
    /// </summary>
    [JsonIgnore]
    public string GroupBy { get; set; }
    /// <summary>
    /// Gets or sets the HAVING clause.
    /// </summary>
    [JsonIgnore]
    public string Having { get; set; }
    /// <summary>
    /// Gets or sets the ORDER BY clause.
    /// </summary>
    [JsonIgnore]
    public string OrderBy { get; set; }
    /// <summary>
    /// Returns true if the statement is empty
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty { get { return string.IsNullOrWhiteSpace(Text); } }
    /// <summary>
    /// The DataTable that results after the select execution
    /// </summary>
    [JsonIgnore]
    public MemTable Table { get; set; }
    /// <summary>
    /// A user defined value.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}