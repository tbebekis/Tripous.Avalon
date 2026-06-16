/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the data type of a product attribute value.
/// </summary>
[TypeStore]
public enum ProductAttributeType
{
    /// <summary>No attribute data type is specified.</summary>
    None = 0,
    /// <summary>A free-form text value.</summary>
    Text = 1, 
    /// <summary>A whole-number value.</summary>
    Integer = 2, 
    /// <summary>A decimal numeric value.</summary>
    Decimal = 3, 
    /// <summary>A value selected from predefined options.</summary>
    Option = 4,
}
