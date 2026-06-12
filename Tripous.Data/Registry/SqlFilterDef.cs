/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Describes a filter applied to a Field.
/// <para>The field could be a <see cref="DataColumn"/> in a <see cref="DataView"/> or a field in a <c>SELECT</c> statement text.</para>
/// <para>It can be used to construct the <see cref="DataView.RowFilter"/> or to construct the <c>WHERE</c> clause of a <c>SELECT</c> statement. </para>
/// </summary>
public class SqlFilterDef : BaseDef
{
    // ● private fields
    BoolOp fBoolOp;
    ConditionOp fConditionOp;
    string fFieldName;
    DataFieldType fFilterDataType = DataFieldType.String;
    bool fCorrectingSerialization;
    string fValueText;
    string fValue2Text;
    string fValueType;
    string fValue2Type;
    object fValue;
    object fValue2;

    // ● private methods
    /// <summary>
    /// Synchronizes runtime object values and serialized text representations bi-directionally.
    /// </summary>
    void CorrectSerialization()
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

                        fValueType = ItemType.FullName;
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
                    fValueType = fValue.GetType().FullName;
                    fValueText = ConvertToText(fValue);

                    if (fValue2 != null)
                    {
                        fValue2Type = fValue2.GetType().FullName;
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
    /// <summary>
    /// Formats a primitive typed value into a culture-invariant text token string.
    /// </summary>
    static string ConvertToText(object Value)
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
    /// <summary>
    /// Parses a culture-invariant text token string into an instance object of the target definition type.
    /// </summary>
    static object ConvertFromText(string Text, Type T)
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

    // ● constructors
    /// <summary>
    /// Initializes a new instance of the SqlFilterDef class.
    /// </summary>
    public SqlFilterDef()
    {
    }

    // ● public methods
    /// <summary>
    /// Validates core structural configuration properties of this operational descriptor block.
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (ConditionOp == ConditionOp.None)
            throw new ApplicationException("A WHERE item must have a condition operator.");

        if (!Enum.IsDefined(typeof(ConditionOp), ConditionOp))
            throw new ApplicationException($"Invalid condition operator: {ConditionOp}");

        if (!Enum.IsDefined(typeof(BoolOp), BoolOp))
            throw new ApplicationException($"Invalid boolean operator: {BoolOp}");

        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ApplicationException("A WHERE item must have a field name");
    }
    /// <summary>
    /// Validates core properties structural attributes along with runtime value presence states matching operation tokens.
    /// </summary>
    public void CheckDescriptorWithValues()
    {
        CheckDescriptor();
        
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

    // ● properties
    /// <summary>
    /// Gets or sets the logical boolean operator linking this expression component to adjacent conditions.
    /// </summary>
    public BoolOp BoolOp
    {
        get => fBoolOp != BoolOp.None ? fBoolOp : BoolOp.Or;
        set { if (fBoolOp != value) { fBoolOp = value; NotifyPropertyChanged(nameof(BoolOp)); } }
    }
    /// <summary>
    /// Gets or sets the query conditional evaluation comparison operator indicator token.
    /// </summary>
    public ConditionOp ConditionOp 
    {
        get => fConditionOp != ConditionOp.None ? fConditionOp : ConditionOp.Equal;
        set { if (fConditionOp != value) { fConditionOp = value; NotifyPropertyChanged(nameof(ConditionOp)); } }
    }
    /// <summary>
    /// Gets or sets the literal target data column database identifier property field title string name.
    /// </summary>
    public string FieldName
    {
        get => !string.IsNullOrWhiteSpace(fFieldName) ? fFieldName : Name;
        set { if (fFieldName != value) { fFieldName = value; NotifyPropertyChanged(nameof(FieldName)); } }
    }
    /// <summary>
    /// Gets or sets the structural evaluation data classification rules defined for filter calculations.
    /// </summary>
    public DataFieldType FilterDataType
    {
        get => fFilterDataType;
        set
        {
            if (fFilterDataType != value)
            {
                if (!value.IsValidFilterType())
                    throw new TripousDataException($"{value} is invalid type for a filter");
                fFilterDataType = value; 
                NotifyPropertyChanged(nameof(FilterDataType)); 
            } 
        }
    }
    /// <summary>
    /// Gets the CLR type representation mapped directly from internal classification flags rules.
    /// </summary>
    [JsonIgnore] public Type DataType => FilterDataType.GetNetType();
    /// <summary>
    /// Gets or sets the primary raw condition argument data token instance applied to queries.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the secondary boundary condition argument data token instance applied to range queries.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the culture-invariant serialized text token mapping for the primary filtering argument value.
    /// </summary>
    public string ValueText
    {
        get => fValueText;
        set
        {
            fValueText = value;
            CorrectSerialization();
        }
    }
    /// <summary>
    /// Gets or sets the culture-invariant serialized text token mapping for the secondary boundary argument value.
    /// </summary>
    public string Value2Text
    {
        get => fValue2Text;
        set
        {
            fValue2Text = value;
            CorrectSerialization();
        }
    }
    /// <summary>
    /// Gets or sets the target assembly-qualified runtime CLR object identifier class name string for primary value parsing rules.
    /// </summary>
    public string ValueType
    {
        get => fValueType;
        set
        {
            fValueType = value;
            CorrectSerialization();
        }
    }
    /// <summary>
    /// Gets or sets the target assembly-qualified runtime CLR object identifier class name string for secondary boundary parsing rules.
    /// </summary>
    public string Value2Type
    {
        get => fValue2Type;
        set
        {
            fValue2Type = value;
            CorrectSerialization();
        }
    }
}