// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides source field validation helpers for <see cref="PivotGrid"/>.
/// </summary>
static public class PivotGridFieldRules
{
    // ● static public
    /// <summary>
    /// Returns true when a value type is numeric.
    /// </summary>
    /// <param name="ValueType">The value type.</param>
    /// <returns>True if the type is numeric; otherwise, false.</returns>
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
    /// Returns true when a value type can be used as a row or column axis field.
    /// </summary>
    /// <param name="ValueType">The value type.</param>
    /// <returns>True if the type can be used as an axis field; otherwise, false.</returns>
    static public bool CanUseAsAxis(Type ValueType)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        return Type == typeof(string)
               || Type == typeof(char)
               || Type == typeof(bool)
               || Type.IsEnum
               || IsNumericType(Type)
               || Type == typeof(DateTime)
               || Type == typeof(DateTimeOffset);
    }
    /// <summary>
    /// Returns true when a value type can be used as a measure field.
    /// </summary>
    /// <param name="ValueType">The value type.</param>
    /// <returns>True if the type can be used as a measure field; otherwise, false.</returns>
    static public bool CanUseAsMeasure(Type ValueType)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        return IsNumericType(Type);
    }
    /// <summary>
    /// Creates source field metadata.
    /// </summary>
    /// <param name="Name">The field name.</param>
    /// <param name="Header">The display header.</param>
    /// <param name="ValueType">The value type.</param>
    /// <returns>The source field metadata.</returns>
    static public PivotGridSourceField CreateSourceField(string Name, string Header, Type ValueType)
    {
        Type Type = Nullable.GetUnderlyingType(ValueType) ?? ValueType;
        bool IsNumeric = IsNumericType(Type);
        return new PivotGridSourceField
        {
            Name = Name ?? string.Empty,
            Header = string.IsNullOrWhiteSpace(Header) ? Name ?? string.Empty : Header,
            ValueType = Type,
            CanUseAsAxis = CanUseAsAxis(Type),
            CanUseAsMeasure = CanUseAsMeasure(Type),
            IsNumeric = IsNumeric,
        };
    }
}
