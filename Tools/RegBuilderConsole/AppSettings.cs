/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

using Tripous.Data;

namespace RegBuilderConsole;
/// <summary>
/// Defines the console RegBuilder configuration.
/// </summary>
public class AppSettings
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public AppSettings()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the projects that must be built before schema generation.
    /// </summary>
    public string[] BuildProjectFilePaths { get; set; } = [];
    /// <summary>
    /// Gets or sets the assemblies that provide discoverable types such as enums.
    /// </summary>
    public string[] AssemblyFilePaths { get; set; } = [];
    /// <summary>
    /// Gets or sets the schema generation projects.
    /// </summary>
    public RegBuilderConsoleProject[] Projects { get; set; } = [];
}

/// <summary>
/// Defines a schema generation project and its destination folder.
/// </summary>
public class RegBuilderConsoleProject : RegBuilderProject
{
    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public RegBuilderConsoleProject()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the destination folder for generated source files.
    /// </summary>
    public string OutputFolderPath { get; set; }
}
