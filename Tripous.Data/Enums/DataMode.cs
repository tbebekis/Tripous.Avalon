/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Indicates the mode of a form or business object
/// </summary>
[Flags]
public enum DataMode
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// List
    /// </summary>
    List = 1,
    /// <summary>
    /// Insert
    /// </summary>
    Insert = 2,
    /// <summary>
    /// Edit
    /// </summary>
    Edit = 4,
    /// <summary>
    /// Delete
    /// </summary>
    Delete = 8,
    /// <summary>
    /// Save
    /// </summary>
    Save = 0x10,
    /// <summary>
    /// Cancel
    /// </summary>
    Cancel = 0x20,
}