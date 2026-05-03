namespace Tripous.Data;

static public class SqlWhereFilterFormatter
{
    // ● private
    static string Field(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousArgumentNullException(nameof(Name));
        return Name;
    }
    static string Value(object V)
    {
        if (V == null || V == DBNull.Value)
            return "null";
        switch (V)
        {
            case string S:
                return $"'{S.Replace("'", "''")}'";
            case bool B:
                return B ? "1" : "0";
            case DateTime Dt:
                return $"'{Dt:yyyy-MM-dd HH:mm:ss}'";
            case DateTimeOffset Dto:
                return $"'{Dto:yyyy-MM-dd HH:mm:ss}'";
            case Guid:
                return $"'{V}'";
            case int or long or short or byte:
            case float or double or decimal:
                return Convert.ToString(V, CultureInfo.InvariantCulture);
            case IEnumerable E when V is not string:
                return string.Join(", ", E.Cast<object>().Select(SqlWhereFilterFormatter.Value));
            default:
                return $"'{V.ToString()?.Replace("'", "''")}'";
        }
    }
    static string LikePattern(ConditionOp ConditionOp, string Text)
    {
        Text ??= string.Empty;
        return ConditionOp switch
        {
            ConditionOp.Contains => $"%{Text}%",
            ConditionOp.StartsWith => $"{Text}%",
            ConditionOp.EndsWith => $"%{Text}",
            _ => Text,
        };
    }
    static string AddParameter(IDictionary<string, object> Params, string FieldName, object Value)
    {
        string BaseName = FieldName;
        foreach (char Ch in Path.GetInvalidFileNameChars())
            BaseName = BaseName.Replace(Ch, '_');
        BaseName = BaseName.Replace(".", "_").Replace(" ", "_");
        string ParamName = BaseName;
        int Index = 1;
        while (Params.ContainsKey(ParamName))
        {
            Index++;
            ParamName = $"{BaseName}_{Index}";
        }
        Params[ParamName] = Value ?? DBNull.Value;
        return ":" + ParamName;
    }
    static string FormatBool(BoolOp BoolOp, string Text)
    {
        return BoolOp switch
        {
            BoolOp.And => $"and ({Text}) ",
            BoolOp.Or => $"or ({Text}) ",
            BoolOp.AndNot => $"and (not {Text}) ",
            BoolOp.OrNot => $"or (not {Text}) ",
            _ => Text,
        };
    }
    static string RemoveFirstBool(string Text)
    {
        if (Text.StartsWith("and ", StringComparison.OrdinalIgnoreCase))
            return Text.Remove(0, "and ".Length);
        if (Text.StartsWith("or ", StringComparison.OrdinalIgnoreCase))
            return Text.Remove(0, "or ".Length);
        return Text;
    }

    // ● public
    /// <summary>
    /// Formats a SQL WHERE filter definition as inline SQL text.
    /// </summary>
    static public string FormatInline(SqlFilterDef Def)
    {
        Def.CheckDescriptorWithValues();
        string Text = string.Empty;
        string SFieldName = Field(Def.FieldName);
        string V1 = Value(Def.Value);
        string V2 = Value(Def.Value2);
        string S = Convert.ToString(Def.Value, CultureInfo.InvariantCulture) ?? string.Empty;
        switch (Def.ConditionOp)
        {
            case ConditionOp.Equal: Text = $"{SFieldName} = {V1}"; break;
            case ConditionOp.NotEqual: Text = $"{SFieldName} <> {V1}"; break;
            case ConditionOp.Greater: Text = $"{SFieldName} > {V1}"; break;
            case ConditionOp.GreaterOrEqual: Text = $"{SFieldName} >= {V1}"; break;
            case ConditionOp.Less: Text = $"{SFieldName} < {V1}"; break;
            case ConditionOp.LessOrEqual: Text = $"{SFieldName} <= {V1}"; break;
            case ConditionOp.Like: Text = $"{SFieldName} like {V1}"; break;
            case ConditionOp.Contains:
            case ConditionOp.StartsWith:
            case ConditionOp.EndsWith:
                Text = $"{SFieldName} like {Value(LikePattern(Def.ConditionOp, S))}";
                break;
            case ConditionOp.Between:
                Text = $"{SFieldName} between {V1} and {V2}";
                break;
            case ConditionOp.In:
                if (Def.Value is IEnumerable List && Def.Value is not string)
                    Text = $"{SFieldName} in ({string.Join(", ", List.Cast<object>().Select(Value))})";
                else
                    throw new ApplicationException("IN requires a collection value.");
                break;
            case ConditionOp.Null:
                Text = $"{SFieldName} is null";
                break;
        }
        return FormatBool(Def.BoolOp, Text);
    }
    /// <summary>
    /// Formats SQL WHERE filter definitions as inline SQL text.
    /// </summary>
    static public string FormatInline(SqlFilterDefs Defs)
    {
        Defs.CheckDescriptorsWithValues();
        StringBuilder SB = new();
        for (int i = 0; i < Defs.Count; i++)
        {
            string Text = FormatInline(Defs[i]);
            if (i == 0)
                Text = RemoveFirstBool(Text);
            SB.Append(Text);
        }
        return SB.ToString();
    }
    
    /// <summary>
    /// Formats a SQL WHERE filter definition as parameterized SQL text.
    /// <para>NOTE: The <see cref="Params"/> should be an empty dictionary. The formatter adds Parameter Names and Values to it.</para>
    /// </summary>
    static public string FormatParameterized(SqlFilterDef Def, IDictionary<string, object> Params)
    {
        if (Params == null)
            throw new TripousArgumentNullException(nameof(Params));
        Def.CheckDescriptorWithValues();
        string Text = string.Empty;
        string SFieldName = Field(Def.FieldName);
        string P1 = AddParameter(Params, Def.FieldName, Def.Value);
        string S = Convert.ToString(Def.Value, CultureInfo.InvariantCulture) ?? string.Empty;
        switch (Def.ConditionOp)
        {
            case ConditionOp.Equal: Text = $"{SFieldName} = {P1}"; break;
            case ConditionOp.NotEqual: Text = $"{SFieldName} <> {P1}"; break;
            case ConditionOp.Greater: Text = $"{SFieldName} > {P1}"; break;
            case ConditionOp.GreaterOrEqual: Text = $"{SFieldName} >= {P1}"; break;
            case ConditionOp.Less: Text = $"{SFieldName} < {P1}"; break;
            case ConditionOp.LessOrEqual: Text = $"{SFieldName} <= {P1}"; break;
            case ConditionOp.Like: Text = $"{SFieldName} like {P1}"; break;
            case ConditionOp.Contains:
            case ConditionOp.StartsWith:
            case ConditionOp.EndsWith:
                Params[P1.TrimStart(':')] = LikePattern(Def.ConditionOp, S);
                Text = $"{SFieldName} like {P1}";
                break;
            case ConditionOp.Between:
                string P2 = AddParameter(Params, Def.FieldName + "2", Def.Value2);
                Text = $"{SFieldName} between {P1} and {P2}";
                break;
            case ConditionOp.In:
                if (Def.Value is IEnumerable List && Def.Value is not string)
                {
                    List<string> ParamNames = new();
                    foreach (object Item in List)
                        ParamNames.Add(AddParameter(Params, Def.FieldName, Item));
                    Text = $"{SFieldName} in ({string.Join(", ", ParamNames)})";
                }
                else
                {
                    throw new ApplicationException("IN requires a collection value.");
                }
                break;
            case ConditionOp.Null:
                Text = $"{SFieldName} is null";
                break;
        }
        return FormatBool(Def.BoolOp, Text);
    }
    /// <summary>
    /// Formats SQL WHERE filter definitions as parameterized SQL text.
    /// <para>NOTE: The <see cref="Params"/> should be an empty dictionary. The formatter adds Parameter Names and Values to it.</para>
    /// </summary>
    static public string FormatParameterized(SqlFilterDefs Defs, IDictionary<string, object> Params)
    {
        if (Params == null)
            throw new TripousArgumentNullException(nameof(Params));
        Defs.CheckDescriptorsWithValues();
        StringBuilder SB = new();
        for (int i = 0; i < Defs.Count; i++)
        {
            string Text = FormatParameterized(Defs[i], Params);
            if (i == 0)
                Text = RemoveFirstBool(Text);
            SB.Append(Text);
        }
        return SB.ToString();
    }
}