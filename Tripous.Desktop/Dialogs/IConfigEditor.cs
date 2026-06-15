/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Provides a desktop editor for a complex configuration property.
/// </summary>
public interface IConfigEditor
{
    // ● public
    /// <summary>
    /// Loads a configuration value into the editor.
    /// </summary>
    void LoadValue(ConfigPropertyDef Def, string Value);
    /// <summary>
    /// Returns the edited configuration value.
    /// </summary>
    string SaveValue();
}
