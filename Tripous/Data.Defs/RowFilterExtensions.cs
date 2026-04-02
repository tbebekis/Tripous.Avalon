namespace Tripous.Data;

static public class RowFilterExtensions
{
    static public string GetText(this BoolOp Op)
    {
        switch (Op)
        {
            case BoolOp.And: return "and ";
            case BoolOp.Or: return "or ";
            case BoolOp.AndNot: return "and not ";
            case BoolOp.OrNot: return "or not ";
        }

        return string.Empty;
    }

    static public string GetText(this ConditionOp Op)
    {
        switch (Op)
        {
            case ConditionOp.Equal: return " = ";
            case ConditionOp.NotEqual: return " <> ";
            case ConditionOp.Greater: return " > ";
            case ConditionOp.GreaterOrEqual: return " >= ";
            case ConditionOp.Less: return " < ";
            case ConditionOp.LessOrEqual: return " <= ";
            case ConditionOp.Like:
            case ConditionOp.Contains:
            case ConditionOp.StartsWith:
            case ConditionOp.EndsWith:
                return " like ";
            case ConditionOp.Between: return " between ";
            case ConditionOp.In: return " in ";
            case ConditionOp.Null: return " is null ";
        }

        return string.Empty;
    }
}