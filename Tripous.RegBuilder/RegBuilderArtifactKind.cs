/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.RegBuilder;

/// <summary>
/// Defines generated artifact kinds.
/// </summary>
[Flags]
public enum RegBuilderArtifactKind
{
    /// <summary>
    /// No artifact.
    /// </summary>
    None = 0,
    /// <summary>
    /// Schema version C# source file.
    /// </summary>
    Schema = 1,
    /// <summary>
    /// Registry version root C# source file.
    /// </summary>
    RegistryVersion = 2,
    /// <summary>
    /// Module registration C# source file.
    /// </summary>
    Modules = 4,
    /// <summary>
    /// Form registration C# source file.
    /// </summary>
    Forms = 8,
    /// <summary>
    /// Lookup registration C# source file.
    /// </summary>
    Lookups = 16,
    /// <summary>
    /// Locator registration C# source file.
    /// </summary>
    Locators = 32,
    /// <summary>
    /// Code provider registration C# source file.
    /// </summary>
    CodeProviders = 64,
    /// <summary>
    /// Ordered schema SQL file.
    /// </summary>
    SchemaSql = 128,
    /// <summary>
    /// All generated C# source files.
    /// </summary>
    CSharpSource = Schema | RegistryVersion | Modules | Forms | Lookups | Locators | CodeProviders,
    /// <summary>
    /// All generated artifacts.
    /// </summary>
    All = CSharpSource | SchemaSql
}
