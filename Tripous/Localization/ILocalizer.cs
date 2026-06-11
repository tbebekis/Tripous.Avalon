/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;


/// <summary>
/// Represents a localizer.
/// </summary>
public interface ILocalizer
{
    /// <summary>
    /// Gets the localized text for the specified key.
    /// </summary>
    string GetText(string Key);
}