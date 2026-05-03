namespace Tripous.Data;

static class DataViewRowFilterFormatter
{
    // ● private
    static string Field(string Name) => $"[{Name.Replace("]", "]]")}]";
    static string Value(object V)
    {
        if (V == null || V == DBNull.Value)
            return "null";

        switch (V)
        {
            case string S:
                return $"'{S.Replace("'", "''")}'";

            case bool B:
                return B ? "true" : "false";

            case DateTime Dt:
                return $"#{Dt:MM/dd/yyyy HH:mm:ss}#";

            case DateTimeOffset Dto:
                return $"#{Dto.DateTime:MM/dd/yyyy HH:mm:ss}#";

            case int or long or short or byte:
            case float or double or decimal:
                return Convert.ToString(V, CultureInfo.InvariantCulture);

            case IEnumerable E when V is not string:
                return string.Join(", ", E.Cast<object>().Select(DataViewRowFilterFormatter.Value));

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

    // ● public
    /// <summary>
    /// Formats a filter definition for the <see cref="DataView.RowFilter"/>.
    /// </summary>
    static public string FormatText(SqlFilterDef Def)
    {
        Def.CheckDescriptorWithValues();
        string Text = string.Empty;

        string SFieldName = DataViewRowFilterFormatter.Field(Def.FieldName);
        string V1 = DataViewRowFilterFormatter.Value(Def.Value);
        string V2 = DataViewRowFilterFormatter.Value(Def.Value2);

        string S = Convert.ToString(Def.Value, CultureInfo.InvariantCulture) ?? string.Empty;

        switch (Def.ConditionOp)
        {
            case ConditionOp.Equal: Text += $"{SFieldName} = {V1}"; break;
            case ConditionOp.NotEqual: Text += $"{SFieldName} <> {V1}"; break;
            case ConditionOp.Greater: Text += $"{SFieldName} > {V1}"; break;
            case ConditionOp.GreaterOrEqual: Text += $"{SFieldName} >= {V1}"; break;
            case ConditionOp.Less: Text += $"{SFieldName} < {V1}"; break;
            case ConditionOp.LessOrEqual: Text += $"{SFieldName} <= {V1}"; break;
            case ConditionOp.Like: Text += $"{SFieldName} like {V1}"; break;

            case ConditionOp.Contains:
            case ConditionOp.StartsWith:
            case ConditionOp.EndsWith:
                Text += $"{SFieldName} like {DataViewRowFilterFormatter.Value(DataViewRowFilterFormatter.LikePattern(Def.ConditionOp, S))}";
                break;

            case ConditionOp.Between:
                Text += $"{SFieldName} between {V1} and {V2}";
                break;

            case ConditionOp.In:
                if (Def.Value is IEnumerable List && Def.Value is not string)
                {
                    var Items = List.Cast<object>().Select(DataViewRowFilterFormatter.Value);
                    Text += $"{SFieldName} in ({string.Join(", ", Items)})";
                }
                else
                {
                    throw new ApplicationException("IN requires a collection value.");
                }
                break;

            case ConditionOp.Null:
                Text += $"{SFieldName} is null";
                break;
        }

        switch (Def.BoolOp)
        {
            case BoolOp.And: Text = $"and ({Text}) "; break;
            case BoolOp.Or: Text = $"or ({Text}) "; break;
            case BoolOp.AndNot: Text = $"and (not {Text}) "; break;
            case BoolOp.OrNot: Text = $"or (not {Text}) "; break;
        }

        return Text;
    }
    /// <summary>
    /// Formats a filter definition for the <see cref="DataView.RowFilter"/>.
    /// </summary>
    static public string FormatText(SqlFilterDefs Defs)
    {
        Defs.CheckDescriptorsWithValues();
        StringBuilder SB = new();

        string Text;

        for (int i = 0; i < Defs.Count; i++)
        {
            SqlFilterDef def = Defs[i];
            Text = FormatText(def);

            if (i == 0)
            {
                if (Text.StartsWith("and "))
                    Text = Text.Remove(0, "and ".Length);
                else if (Text.StartsWith("or "))
                    Text = Text.Remove(0, "or ".Length);
            }

            SB.Append(Text);
        }

        return SB.ToString();
    }
}