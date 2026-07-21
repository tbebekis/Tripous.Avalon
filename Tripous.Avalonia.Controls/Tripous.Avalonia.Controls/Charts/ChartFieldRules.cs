// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides source field rules used by chart adapters.
/// </summary>
static public class ChartFieldRules
{
    // ● static public
    /// <summary>
    /// Returns true when a type can be treated as numeric.
    /// </summary>
    /// <param name="ValueType">The value type.</param>
    /// <returns>True if numeric; otherwise, false.</returns>
    static public bool IsNumericType(Type ValueType)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        return Type == typeof(byte)
            || Type == typeof(sbyte)
            || Type == typeof(short)
            || Type == typeof(ushort)
            || Type == typeof(int)
            || Type == typeof(uint)
            || Type == typeof(long)
            || Type == typeof(ulong)
            || Type == typeof(float)
            || Type == typeof(double)
            || Type == typeof(decimal);
    }
    /// <summary>
    /// Returns true when a type can be used as a chart dimension.
    /// </summary>
    /// <param name="ValueType">The value type.</param>
    /// <returns>True if the type is supported; otherwise, false.</returns>
    static public bool IsDimensionType(Type ValueType)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        return Type.IsEnum
            || Type == typeof(string)
            || Type == typeof(char)
            || Type == typeof(bool)
            || Type == typeof(DateTime)
            || Type == typeof(DateTimeOffset)
            || Type == typeof(Guid)
            || IsNumericType(Type);
    }
    /// <summary>
    /// Creates a source field descriptor.
    /// </summary>
    /// <param name="Name">The field name.</param>
    /// <param name="Header">The field header.</param>
    /// <param name="ValueType">The field value type.</param>
    /// <returns>The source field descriptor.</returns>
    static public ChartSourceField CreateSourceField(string Name, string Header, Type ValueType)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        bool IsNumeric = IsNumericType(Type);
        return new ChartSourceField
        {
            Name = Name ?? string.Empty,
            Header = string.IsNullOrWhiteSpace(Header) ? Name ?? string.Empty : Header,
            ValueType = Type,
            CanUseAsDimension = IsDimensionType(Type),
            CanUseAsMeasure = IsNumeric || Type == typeof(string) || Type == typeof(bool) || Type.IsEnum || Type == typeof(Guid) || Type == typeof(DateTime),
            IsNumeric = IsNumeric,
        };
    }
}
