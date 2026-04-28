namespace Tripous.Data;

public class SqlHelper
{
    /// <summary>
    /// LineBreak
    /// </summary>
    static private readonly string LB = Environment.NewLine; // Line Break
    
    /// <summary>
    /// WARNING: Not reliable
    /// </summary>
    static public string GetMainTableName(string SelectSqlText)
    {
        var Match = Regex.Match(SelectSqlText, @"FROM\s+([^\s,]+)", RegexOptions.IgnoreCase);
        return Match.Success ? Match.Groups[1].Value : string.Empty;
    }
    /// <summary>
    /// If the specified CreateTableSqlText is a CREATE TABLE TABLE_NAME (...) sql statement
    /// then this method returns the TABLE_NAME, else returns string.Empty
    /// </summary>
    static public string ExtractTableName(string CreateTableSqlText)
    {

        if (!string.IsNullOrWhiteSpace(CreateTableSqlText))
        {
            int Index = CreateTableSqlText.IndexOf('(');

            if (Index != -1)
            {
                string SqlText = CreateTableSqlText.Substring(0, Index);

                SqlText = SqlText.Trim();
                SqlText = SqlText.Replace("  ", " ");

                string[] Words = SqlText.Split(' ');

                if ((Words.Length >= 3) && Sys.IsSameText(Words[0], "create") && Sys.IsSameText(Words[1], "table"))
                {
                    return Words[2];
                }
            }
        }

        return string.Empty;
    }
    
    /// <summary>
    /// Transforms a string list into a field list by adding a <c>,</c> and a line break in every string item, except the last one.
    /// </summary>
    static public string TransformToFieldList(List<string> StringList)
    {
        string Result = string.Join(", " + Environment.NewLine, StringList.ToArray()).TrimEnd();
        return Result;
    }
        
    /// <summary>
    /// Converts the double Value to a float string, valid for S, i.e. ensures that the decimal
    /// seperator is the point.
    /// </summary>
    static public string FloatSQL(double Value)
    {
        return Value.ToString().Replace(',', '.');
    }
    /// <summary>
    /// Normalizes Value for use in a LIKE clause. It returns a string as <code>LIKE 'VALUE%'</code>
    /// <para>Value may or may not contain mask characters (%, ?, *)</para>
    /// </summary>
    static public string NormalizeMaskForce(string Value)
    {
        if (Value != null)
        {
            Value = Value.Trim();

            if (Value.Length > 0)
            {
                StringBuilder SB = new StringBuilder(Value);
                SB.Replace('*', '%');
                SB.Replace('?', '%');
                Value = SB.ToString();

                if (Value.IndexOf('%') != -1)
                    return string.Format(" like '{0}' ", Value);
                else
                    return string.Format(" like '{0}' ", Value + @"%");

            }
        }

        return " like '__not__existed__' ";
    }
    /// <summary>
    /// Normalizes Value for use in a LIKE clause. It returns a string as <code>LIKE 'VALUE%'</code>
    /// <para>Value may or may not contain mask characters (%, ?, *)</para>
    /// </summary>
    static public string NormalizeMask(string Value)
    {
        if (Value != null)
        {
            Value = Value.Trim();

            if (Value.Length > 0)
            {
                StringBuilder SB = new StringBuilder(Value);
                SB.Replace('*', '%');
                SB.Replace('?', '%');
                Value = SB.ToString();

                if (Value.IndexOf('%') != -1)
                    return string.Format(" like '{0}' ", Value);
                else
                    return string.Format(" like '{0}' ", Value + @"%");

            }
        }

        return string.Empty;
    }
    /// <summary>
    /// Normalizes Value for use in a LIKE clause. It returns a string as <code>LIKE 'VALUE%'</code>
    /// <para>Value may or may not contain mask characters (%, ?, *)</para>
    /// <para>WARNING: This version adds a % in front of Value, if not there</para>
    /// </summary>
    static public string NormalizeMask2(string Value)
    {
        if (Value != null)
        {
            Value = Value.Trim();

            if (Value.Length > 0)
            {
                StringBuilder SB = new StringBuilder(Value);
                SB.Replace('*', '%');
                SB.Replace('?', '%');
                Value = SB.ToString();

                if (Value.IndexOf('%') != -1)
                {
                    if (!string.IsNullOrWhiteSpace(Value) && (Value[0] != '%'))
                        Value = "%" + Value;

                    return string.Format(" like '{0}' ", Value);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(Value) && (Value[0] != '%'))
                        Value = "%" + Value;

                    return string.Format(" like '{0}' ", Value + @"%");
                }

            }
        }

        return string.Empty;

        /*  
                    string Result = Value;

                    if (Result.Length > 0)
                    {
                        StringBuilder SB = new StringBuilder(Result);
                        SB.Replace('*', '%');
                        SB.Replace('?', '%');
                        Result = SB.ToString();

                        if (Result.IndexOf('%') != -1)
                        {
                            if (!string.IsNullOrWhiteSpace(Result) && (Result[0] != '%'))
                                Result = "%" + Result;

                            Result = string.Format(" like '{0}' ", Result);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(Result) && (Result[0] != '%'))
                                Result = "%" + Result;

                            Result = string.Format(" like '{0}' ", Result + @"%");
                        }

                    }

                    return Result; 
         */
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
    /// Returns a string such as  
    /// <para><c>TableNameOrAlias.FieldName as Alias</c></para>
    /// </summary>
    static public string FormatFieldNameAlias(string TableNameOrAlias, string FieldName, string Alias, int Spaces)
    {
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException("TableNameOrAlias");

        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException("FieldName");

        return FormatFieldNameAlias(TableNameOrAlias + "." + FieldName, Alias, Spaces);
    }
    /// <summary>
    /// Returns a string such as 
    /// <para><c>FieldName as Alias</c></para>
    /// </summary>
    static public string FormatFieldNameAlias(string FieldName, string Alias, int Spaces)
    {
        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException("FieldName");

        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ArgumentNullException("Alias");

        return FieldName.PadRight(Spaces, ' ') + " as " + Alias + ", " + Environment.NewLine;
    }
    /// <summary>
    /// Returns a string such as 
    /// <para><c>TableName  Alias</c></para>
    /// </summary>
    static public string FormatTableNameAlias(string TableName, string Alias)
    {
        if (string.IsNullOrWhiteSpace(TableName))
            throw new ArgumentNullException("TableName");

        if (string.IsNullOrWhiteSpace(Alias) || Sys.IsSameText(TableName, Alias))
            return TableName;
        else
            return TableName + " " + Alias;
    }
    
    /// <summary>
    /// Formats Value. Quotes the result if Quoted is true.
    /// </summary>
    static public string DateToStr(DateTime Value, bool Quoted)
    {
        if (Quoted)
            return Value.ToString("yyyy-MM-dd").QS();
        else
            return Value.ToString("yyyy-MM-dd");
    }
    /// <summary>
    /// Formats Value. Quotes the result if Quoted is true.
    /// </summary>
    static public string TimeToStr(DateTime Value, bool Quoted)
    {
        if (Quoted)
            return Value.ToString("HH:mm").QS();
        else
            return Value.ToString("HH:mm");
    }
    /// <summary>
    /// Formats Value. Quotes the result if Quoted is true.
    /// </summary>
    static public string DateTimeToStr(DateTime Value, bool Quoted)
    {
        if (Quoted)
            return Value.ToString("yyyy-MM-dd HH:mm").QS();
        else
            return Value.ToString("yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// Returns the value of the Value as a string for constructing Sql statements.
    /// <para>In case of a string or DateTime it surrounds the result with single quotes.</para>
    /// </summary>
    static public string Format(object Value)
    {
        if (Value != null)
        {
            SimpleType simpleType = Simple.SimpleTypeOf(Value);

            if (simpleType.IsString())
                return Value.ToString().QS();

            if (simpleType.IsInteger())
                return Value.ToString();

            if (simpleType.IsFloat())
                return FloatSQL((double)Value);

            if (simpleType.IsDateTime())
                return DateTimeToStr((DateTime)Value, true);

            if (simpleType.IsDateTime())
                return DateToStr((DateTime)Value, true);

            return Value.ToString();
        }

        return string.Empty;
    }
    /// <summary>
    /// Returns the value of the Id as a string for constructing Sql statements.
    /// <para>Id could be either a guid string or a 32-bit integer.</para>
    /// </summary>
    static public string FormatId(object Id)
    {
        /* null */
        if (Sys.IsNull(Id))
        {
            return "null";
        }

        string S = Id.ToString();

        /* int */
        int Num;
        if (int.TryParse(S, out Num))
        {
            return S;
        }

        /* string */
        return S.QS();

    }    
    
    /// <summary>
    /// The spaces to be used when generating Sql statements
    /// </summary>
    public const int StatementDefaultSpaces = 30;
}