/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.RegBuilder;

/// <summary>
/// Defines generated duplicate registry checks.
/// </summary>
[Flags]
public enum DuplicateCheck
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Lookup
    /// </summary>
    Lookup = 1,
    /// <summary>
    /// Enum
    /// </summary>
    Enum = 2,
    /// <summary>
    /// Form
    /// </summary>
    Form = 4,
    /// <summary>
    /// Module
    /// </summary>
    Module = 8,
    /// <summary>
    /// Locator
    /// </summary>
    Locator = 16,
    /// <summary>
    /// CodeProvider
    /// </summary>
    CodeProvider = 32
}
