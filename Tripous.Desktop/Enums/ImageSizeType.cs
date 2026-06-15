/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Defines common image size categories.
/// </summary>
public enum ImageSizeType
{
    /// <summary>
    /// The image size is undefined.
    /// </summary>
    Undefined,
    /// <summary>
    /// The image size is explicitly defined.
    /// </summary>
    Defined,
    /// <summary>
    /// A 16x16 icon image size.
    /// </summary>
    Icon16,
    /// <summary>
    /// A 32x32 icon image size.
    /// </summary>
    Icon32,
}
