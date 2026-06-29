/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.RegBuilder;

/// <summary>
/// Represents a registration builder project
/// </summary>
public class RegBuilderProject
{
    string[] fReferenceFilePaths;
    RegBuilderOutput[] fOutputs;

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public RegBuilderProject()
    {
    }

    // ● public
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => Name;

    // ● properties
    /// <summary>
    /// The name of the project
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The path to the schema file.
    /// </summary>
    public string SchemaFilePath { get; set; }
    /// <summary>
    /// The namespace name of the project.
    /// </summary>
    public string NamespaceName { get; set; }
    /// <summary>
    /// The schema version.
    /// </summary>
    public int SchemaVersion { get; set; }
    /// <summary>
    /// The checks to be performed in the generated registry in order to avoid duplicate definitions.
    /// </summary>
    public DuplicateCheck DuplicateChecks { get; set; } = DuplicateCheck.None;
    /// <summary>
    /// A list of schema file paths that are referenced by the schema file. 
    /// </summary>
    public string[] ReferenceFilePaths
    {
        get => fReferenceFilePaths != null ? fReferenceFilePaths : [];
        set => fReferenceFilePaths = value;
    }
    /// <summary>
    /// Output targets for generated artifacts.
    /// </summary>
    public RegBuilderOutput[] Outputs
    {
        get => fOutputs != null ? fOutputs : [];
        set => fOutputs = value;
    }
}
