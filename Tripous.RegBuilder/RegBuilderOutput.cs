/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.RegBuilder;

/// <summary>
/// Defines a generated output target.
/// </summary>
public class RegBuilderOutput
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public RegBuilderOutput()
    {
    }

    // ● properties
    /// <summary>
    /// Target name, e.g. Data, Desktop, Web, Wasm, Android.
    /// </summary>
    public string TargetName { get; set; }
    /// <summary>
    /// Destination folder for generated artifacts.
    /// </summary>
    public string OutputFolderPath { get; set; }
    /// <summary>
    /// Generated artifact kinds written to the destination folder.
    /// </summary>
    public RegBuilderArtifactKind Artifacts { get; set; } = RegBuilderArtifactKind.CSharpSource;
}
