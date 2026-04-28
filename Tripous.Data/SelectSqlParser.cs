namespace Tripous.Data;

/// <summary>
/// A simple SELECT Sql parser. It parses a SELECT statement into its constituents parts.
/// The SELECT statement can contain nested sub-SELECTs or column names in double quotes or angle brackets.
/// </summary>
public class SelectSqlParser
{
    enum Token
    {
        None = 0,
        Select = 1,
        From = 2,
        Where = 3,
        GroupBy = 4,
        Having = 5,
        OrderBy = 6,
    }

    // ● private
    readonly string[] Tokens = { "select", "from", "where", "group by", "having", "order by" };
    string fText;
    string fSelect;
    string fFrom;
    string fWhere;
    string fGroupBy;
    string fHaving;
    string fOrderBy;
    Token fCurrentToken = Token.None;
    int fCurrentPos;
    int fLastPos;

    bool IsTokenBoundary(int Pos)
    {
        if (Pos < 0 || Pos >= fText.Length)
            return true;
        char C = fText[Pos];
        return !char.IsLetterOrDigit(C) && C != '_';
    }
    bool IsTokenAtCurrentPos(string TokenText)
    {
        if (fCurrentPos + TokenText.Length > fText.Length)
            return false;
        if (!IsTokenBoundary(fCurrentPos - 1))
            return false;
        if (!IsTokenBoundary(fCurrentPos + TokenText.Length))
            return false;
        return string.Compare(fText, fCurrentPos, TokenText, 0, TokenText.Length, true, CultureInfo.InvariantCulture) == 0;
    }
    int SkipToChar(char EndChar)
    {
        fCurrentPos++;
        while (fCurrentPos < fText.Length)
        {
            if (fText[fCurrentPos] == EndChar)
            {
                fCurrentPos++;
                break;
            }
            fCurrentPos++;
        }
        return fCurrentPos;
    }
    int SkipStringLiteral()
    {
        fCurrentPos++;
        while (fCurrentPos < fText.Length)
        {
            if (fText[fCurrentPos] == '\'')
            {
                if (fCurrentPos + 1 < fText.Length && fText[fCurrentPos + 1] == '\'')
                {
                    fCurrentPos += 2;
                    continue;
                }
                fCurrentPos++;
                break;
            }
            fCurrentPos++;
        }
        return fCurrentPos;
    }
    int SkipLineComment()
    {
        fCurrentPos += 2;
        while (fCurrentPos < fText.Length && fText[fCurrentPos] != '\r' && fText[fCurrentPos] != '\n')
            fCurrentPos++;
        return fCurrentPos;
    }
    int SkipBlockComment()
    {
        fCurrentPos += 2;
        while (fCurrentPos < fText.Length - 1)
        {
            if (fText[fCurrentPos] == '*' && fText[fCurrentPos + 1] == '/')
            {
                fCurrentPos += 2;
                break;
            }
            fCurrentPos++;
        }
        return fCurrentPos;
    }
    void SetClause()
    {
        string S = fText.Substring(fLastPos, fCurrentPos - fLastPos).Trim();

        switch (fCurrentToken)
        {
            case Token.Select:
                Select = S;
                break;
            case Token.From:
                From = S;
                break;
            case Token.Where:
                Where = S;
                break;
            case Token.GroupBy:
                GroupBy = S;
                break;
            case Token.Having:
                Having = S;
                break;
            case Token.OrderBy:
                OrderBy = S;
                break;
        }
    }
    void TokenChange(Token NewToken, string TokenText)
    {
        if (fCurrentToken != Token.None && fLastPos >= 0)
            SetClause();

        fCurrentPos += TokenText.Length;
        fLastPos = fCurrentPos;
        fCurrentToken = NewToken;
    }
    bool NextTokenEnd()
    {
        int ParenCount = 0;

        while (fCurrentPos < fText.Length)
        {
            char C = fText[fCurrentPos];

            if (C == '\'')
                SkipStringLiteral();
            else if (C == '"')
                SkipToChar('"');
            else if (C == '[')
                SkipToChar(']');
            else if (C == '`')
                SkipToChar('`');
            else if (C == '-' && fCurrentPos + 1 < fText.Length && fText[fCurrentPos + 1] == '-')
                SkipLineComment();
            else if (C == '/' && fCurrentPos + 1 < fText.Length && fText[fCurrentPos + 1] == '*')
                SkipBlockComment();
            else if (C == '(')
            {
                ParenCount++;
                fCurrentPos++;
            }
            else if (C == ')')
            {
                ParenCount--;
                if (ParenCount < 0)
                    throw new ApplicationException("SelectSqlParser: Wrong parentheses");
                fCurrentPos++;
            }
            else
            {
                if (ParenCount == 0)
                {
                    for (int i = Tokens.Length - 1; i >= 0; i--)
                    {
                        if (IsTokenAtCurrentPos(Tokens[i]))
                        {
                            TokenChange((Token)i + 1, Tokens[i]);
                            return true;
                        }
                    }
                }
                fCurrentPos++;
            }
        }

        return false;
    }
    void Parse()
    {
        fCurrentToken = Token.None;
        fCurrentPos = 0;
        fLastPos = -1;

        while (fCurrentPos < fText.Length)
        {
            if (!NextTokenEnd())
                break;
        }

        if (fCurrentToken != Token.None && fLastPos >= 0)
            SetClause();
    }

    // ● construction
    /// <summary>
    /// Constructor. Text can not be null or empty.
    /// </summary>
    public SelectSqlParser(string Text)
    {
        Parse(Text);
    }

    // ● static public
    /// <summary>
    /// Parses Text into the constituent parts of a SELECT S statement.
    /// <para>Example call: SelectSqlParser.Parse("select * from CUSTOMER").ToString();</para>
    /// </summary>
    static public SelectSqlParser Execute(string Text)
    {
        return new SelectSqlParser(Text);
    }

    // ● public
    /// <summary>
    /// Parses Text into the constituent parts of a SELECT S statement.
    /// </summary>
    public void Parse(string Text)
    {
        Clear();

        if (!string.IsNullOrWhiteSpace(Text))
        {
            fText = Text;
            Parse();
        }
    }
    /// <summary>
    /// Clears the resulting clauses
    /// </summary>
    public void Clear()
    {
        Select = string.Empty;
        From = string.Empty;
        Where = string.Empty;
        GroupBy = string.Empty;
        Having = string.Empty;
        OrderBy = string.Empty;
    }
    /// <summary>
    /// Returns a string that represents the current object
    /// </summary>
    public override string ToString()
    {
        StringBuilder SB = new StringBuilder();

        SB.AppendLine(string.Format("select : {0}", Select));
        SB.AppendLine(string.Format("from : {0}", From));
        SB.AppendLine(string.Format("where : {0}", Where));
        SB.AppendLine(string.Format("group by : {0}", GroupBy));
        SB.AppendLine(string.Format("having : {0}", Having));
        SB.AppendLine(string.Format("order by : {0}", OrderBy));

        return SB.ToString();
    }

    // ● properties
    /// <summary>
    /// Returns the SELECT clause of the parsed statement
    /// </summary>
    public string Select
    {
        get { return !string.IsNullOrWhiteSpace(fSelect) ? fSelect : string.Empty; }
        set { fSelect = value; }
    }
    /// <summary>
    /// Returns the FROM clause of the parsed statement
    /// </summary>
    public string From
    {
        get { return !string.IsNullOrWhiteSpace(fFrom) ? fFrom : string.Empty; }
        set { fFrom = value; }
    }
    /// <summary>
    /// Returns the WHERE clause of the parsed statement
    /// </summary>
    public string Where
    {
        get { return !string.IsNullOrWhiteSpace(fWhere) ? fWhere : string.Empty; }
        set { fWhere = value; }
    }
    /// <summary>
    /// Returns the GROUP BY clause of the parsed statement
    /// </summary>
    public string GroupBy
    {
        get { return !string.IsNullOrWhiteSpace(fGroupBy) ? fGroupBy : string.Empty; }
        set { fGroupBy = value; }
    }
    /// <summary>
    /// Returns the HAVING clause of the parsed statement
    /// </summary>
    public string Having
    {
        get { return !string.IsNullOrWhiteSpace(fHaving) ? fHaving : string.Empty; }
        set { fHaving = value; }
    }
    /// <summary>
    /// Returns the ORDER BY clause of the parsed statement
    /// </summary>
    public string OrderBy
    {
        get { return !string.IsNullOrWhiteSpace(fOrderBy) ? fOrderBy : string.Empty; }
        set { fOrderBy = value; }
    }
}