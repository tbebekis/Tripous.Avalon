using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Tripous.Data;

public enum BoolOp
{
    None = 0,
    And = 1,
    Or = 2,
    AndNot = 4,
    OrNot = 8
}

public enum ConditionOp
{
    None = 0,
    Equal = 1,
    NotEqual = 2,
    Greater = 3,
    GreaterOrEqual = 4,
    Less = 5,
    LessOrEqual = 6,
    Like = 7,
    Contains = 8,
    StartsWith = 9,
    EndsWith = 10,
    Between = 11,
    In = 12,
    Null = 13
}

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

public class RowFilterItem
{
    private bool fCorrectingSerialization;
    private string fValueText;
    private string fValue2Text;
    private string fValueType;
    private string fValue2Type;
    private object fValue;
    private object fValue2;

    private void CorrectSerialization()
    {
        if (fCorrectingSerialization)
            return;

        try
        {
            fCorrectingSerialization = true;

            /* Runtime -> Serializable */
            if (fValue != null)
            {
                if (ConditionOp == ConditionOp.In)
                {
                    if (fValue is IEnumerable List && fValue is not string)
                    {
                        object[] Items = List.Cast<object>().ToArray();

                        Type ItemType = typeof(string);
                        object FirstNonNull = Items.FirstOrDefault(x => x != null);
                        if (FirstNonNull != null)
                            ItemType = FirstNonNull.GetType();

                        fValueType = ItemType.AssemblyQualifiedName;
                        fValueText = string.Join("\u001F", Items.Select(ConvertToText));

                        fValue2 = null;
                        fValue2Text = null;
                        fValue2Type = null;
                    }
                    else
                    {
                        throw new ApplicationException("IN requires an IEnumerable value.");
                    }
                }
                else
                {
                    fValueType = fValue.GetType().AssemblyQualifiedName;
                    fValueText = ConvertToText(fValue);

                    if (fValue2 != null)
                    {
                        fValue2Type = fValue2.GetType().AssemblyQualifiedName;
                        fValue2Text = ConvertToText(fValue2);
                    }
                    else
                    {
                        fValue2Type = null;
                        fValue2Text = null;
                    }
                }

                return;
            }

            /* Serializable -> Runtime */
            if (!string.IsNullOrWhiteSpace(fValueType))
            {
                Type T1 = Type.GetType(fValueType, throwOnError: false);
                if (T1 != null)
                {
                    if (ConditionOp == ConditionOp.In)
                    {
                        string[] Parts = string.IsNullOrWhiteSpace(fValueText)
                            ? Array.Empty<string>()
                            : fValueText.Split('\u001F');

                        fValue = Parts.Select(x => ConvertFromText(x, T1)).ToArray();
                        fValue2 = null;
                    }
                    else
                    {
                        fValue = fValueText != null ? ConvertFromText(fValueText, T1) : null;

                        Type T2 = !string.IsNullOrWhiteSpace(fValue2Type)
                            ? Type.GetType(fValue2Type, throwOnError: false)
                            : T1;

                        fValue2 = (T2 != null && fValue2Text != null)
                            ? ConvertFromText(fValue2Text, T2)
                            : null;
                    }
                }
            }
        }
        finally
        {
            fCorrectingSerialization = false;
        }
    }

    private static string ConvertToText(object Value)
    {
        if (Value == null)
            return null;

        Type T = Value.GetType();

        if (T == typeof(DateTime))
            return ((DateTime)Value).ToString("O", CultureInfo.InvariantCulture);

        if (T == typeof(DateTimeOffset))
            return ((DateTimeOffset)Value).ToString("O", CultureInfo.InvariantCulture);

        if (T == typeof(decimal))
            return ((decimal)Value).ToString(CultureInfo.InvariantCulture);

        if (T == typeof(double))
            return ((double)Value).ToString(CultureInfo.InvariantCulture);

        if (T == typeof(float))
            return ((float)Value).ToString(CultureInfo.InvariantCulture);

        if (T == typeof(Guid))
            return Value.ToString();

        if (T.IsEnum)
            return Convert.ToInt32(Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

        return Convert.ToString(Value, CultureInfo.InvariantCulture);
    }

    private static object ConvertFromText(string Text, Type T)
    {
        if (Text == null)
            return null;

        if (T == typeof(string))
            return Text;

        if (T == typeof(int))
            return int.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(long))
            return long.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(short))
            return short.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(byte))
            return byte.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(decimal))
            return decimal.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(double))
            return double.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(float))
            return float.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(bool))
            return bool.Parse(Text);

        if (T == typeof(DateTime))
            return DateTime.Parse(Text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        if (T == typeof(DateTimeOffset))
            return DateTimeOffset.Parse(Text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        if (T == typeof(Guid))
            return Guid.Parse(Text);

        if (T.IsEnum)
            return Enum.ToObject(T, int.Parse(Text, CultureInfo.InvariantCulture));

        return Convert.ChangeType(Text, T, CultureInfo.InvariantCulture);
    }

    public RowFilterItem()
    {
    }

    /// <summary>
    /// Constructor
    /// <para>With <see cref="ConditionOp.In"/> a value could be an <see cref="IEnumerable"/>, e.g. new[] { "Open", "Closed", "Pending" }</para>
    /// </summary>
    public RowFilterItem(BoolOp BoolOp, ConditionOp ConditionOp, string FieldName, object Value, object Value2 = null)
        : this()
    {
        this.BoolOp = BoolOp;
        this.ConditionOp = ConditionOp;
        this.FieldName = FieldName;
        this.Value = Value;
        this.Value2 = Value2;
    }

    public override string ToString() => this.Text;

    public void Check()
    {
        if (ConditionOp == ConditionOp.None)
            throw new ApplicationException("A WHERE item must have a condition operator.");

        if (!Enum.IsDefined(typeof(ConditionOp), ConditionOp))
            throw new ApplicationException($"Invalid condition operator: {ConditionOp}");

        if (!Enum.IsDefined(typeof(BoolOp), BoolOp))
            throw new ApplicationException($"Invalid boolean operator: {BoolOp}");

        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ApplicationException("A WHERE item must have a field name");

        switch (ConditionOp)
        {
            case ConditionOp.Equal:
            case ConditionOp.NotEqual:
            case ConditionOp.Greater:
            case ConditionOp.GreaterOrEqual:
            case ConditionOp.Less:
            case ConditionOp.LessOrEqual:
            case ConditionOp.Like:
            case ConditionOp.Contains:
            case ConditionOp.StartsWith:
            case ConditionOp.EndsWith:
            case ConditionOp.In:
                if (Value == null)
                    throw new ApplicationException($"Operator {ConditionOp} requires a value: {FieldName}");
                break;

            case ConditionOp.Between:
                if (Value == null || Value2 == null)
                    throw new ApplicationException($"A BETWEEN expression requires two values: {FieldName}");
                break;
        }
    }

    public BoolOp BoolOp { get; set; }
    public ConditionOp ConditionOp { get; set; }
    public string FieldName { get; set; }

    [JsonIgnore]
    public object Value
    {
        get => fValue;
        set
        {
            fValue = value;
            CorrectSerialization();
        }
    }

    [JsonIgnore]
    public object Value2
    {
        get => fValue2;
        set
        {
            fValue2 = value;
            CorrectSerialization();
        }
    }

    public string ValueText
    {
        get => fValueText;
        set
        {
            fValueText = value;
            CorrectSerialization();
        }
    }

    public string Value2Text
    {
        get => fValue2Text;
        set
        {
            fValue2Text = value;
            CorrectSerialization();
        }
    }

    public string ValueType
    {
        get => fValueType;
        set
        {
            fValueType = value;
            CorrectSerialization();
        }
    }

    public string Value2Type
    {
        get => fValue2Type;
        set
        {
            fValue2Type = value;
            CorrectSerialization();
        }
    }

    [JsonIgnore]
    public string Text
    {
        get
        {
            Check();
            string Text = string.Empty;

            string SFieldName = RowFilterFormatter.Field(FieldName);
            string V1 = RowFilterFormatter.Value(Value);
            string V2 = RowFilterFormatter.Value(Value2);

            string S = Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;

            switch (ConditionOp)
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
                    Text += $"{SFieldName} like {RowFilterFormatter.Value(RowFilterFormatter.LikePattern(ConditionOp, S))}";
                    break;

                case ConditionOp.Between:
                    Text += $"{SFieldName} between {V1} and {V2}";
                    break;

                case ConditionOp.In:
                    if (Value is IEnumerable List && Value is not string)
                    {
                        var Items = List.Cast<object>().Select(RowFilterFormatter.Value);
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

            switch (BoolOp)
            {
                case BoolOp.And: Text = $"and ({Text}) "; break;
                case BoolOp.Or: Text = $"or ({Text}) "; break;
                case BoolOp.AndNot: Text = $"and (not {Text}) "; break;
                case BoolOp.OrNot: Text = $"or (not {Text}) "; break;
            }

            return Text;
        }
    }

    [JsonIgnore]
    public object Tag { get; set; }
}

public class RowFilterItemList : List<RowFilterItem>
{
    public RowFilterItemList()
    {
    }

    public override string ToString() => this.Text;
    public void Check() => ForEach(x => x.Check());

    public RowFilterItem Add(BoolOp BoolOp, ConditionOp ConditionOp, string FieldName, object Value, object Value2 = null)
    {
        RowFilterItem Result = new RowFilterItem(BoolOp, ConditionOp, FieldName, Value, Value2);
        this.Add(Result);
        return Result;
    }

    [JsonIgnore]
    public string Text
    {
        get
        {
            Check();
            StringBuilder SB = new();

            string Text;

            for (int i = 0; i < Count; i++)
            {
                RowFilterItem Item = this[i];
                Text = Item.Text;

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
}