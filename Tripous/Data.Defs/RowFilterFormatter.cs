namespace Tripous.Data;

static class RowFilterFormatter
{
    public static string Field(string Name)
        => $"[{Name.Replace("]", "]]")}]";

    public static string Value(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return "null";

        switch (Value)
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
                return Convert.ToString(Value, CultureInfo.InvariantCulture);

            case IEnumerable E when Value is not string:
                return string.Join(", ", E.Cast<object>().Select(RowFilterFormatter.Value));

            default:
                return $"'{Value.ToString()?.Replace("'", "''")}'";
        }
    }

    public static string LikePattern(ConditionOp ConditionOp, string Text)
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
}