/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.RegBuilder;

/// <summary>
/// Defines Registration Builder settings shared by the UI and console tools.
/// </summary>
public class RegBuilderSettings
{
    RegBuilderProject[] fProjects;
    RegBuilderOutput[] fTargets;
    string[] fBuildProjectFilePaths;
    string[] fAssemblyFilePaths;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public RegBuilderSettings()
    {
    }

    // ● static public
    /// <summary>
    /// Loads settings from a JSON file.
    /// </summary>
    static public RegBuilderSettings Load(string FilePath)
    {
        if (!File.Exists(FilePath))
            throw new FileNotFoundException("RegBuilder settings file was not found.", FilePath);

        RegBuilderSettings Result = Json.LoadFromFile(typeof(RegBuilderSettings), FilePath) as RegBuilderSettings;
        return Result ?? throw new InvalidOperationException("RegBuilder settings file is empty or invalid.");
    }

    // ● public
    /// <summary>
    /// Saves settings to a JSON file.
    /// </summary>
    public void Save(string FilePath) => Json.SaveToFile(this, FilePath);

    // ● public
    /// <summary>
    /// Returns the resolved outputs of a project.
    /// </summary>
    public RegBuilderOutput[] GetOutputs(RegBuilderProject Project)
    {
        if (Project == null)
            throw new TripousArgumentNullException(nameof(Project));

        string[] TargetNames = Project.TargetNames;
        if (TargetNames.Length == 0)
            return Project.Outputs;

        List<RegBuilderOutput> Result = [];
        foreach (string TargetName in TargetNames)
        {
            RegBuilderOutput Target = Targets.FirstOrDefault(Item => string.Equals(Item.TargetName, TargetName, StringComparison.OrdinalIgnoreCase));
            if (Target == null)
                throw new InvalidOperationException($"RegBuilder target was not found: {TargetName}");
            Result.Add(Target);
        }

        return Result.ToArray();
    }
    /// <summary>
    /// Resolves the outputs of a project from its target names.
    /// </summary>
    public void ResolveOutputs(RegBuilderProject Project)
    {
        if (Project != null)
            Project.Outputs = GetOutputs(Project);
    }
    /// <summary>
    /// Resolves all project outputs from project target names.
    /// </summary>
    public void ResolveOutputs()
    {
        foreach (RegBuilderProject Project in Projects)
            ResolveOutputs(Project);
    }

    // ● properties
    /// <summary>
    /// Gets or sets the projects that must be built before schema generation.
    /// </summary>
    public string[] BuildProjectFilePaths
    {
        get => fBuildProjectFilePaths != null ? fBuildProjectFilePaths : [];
        set => fBuildProjectFilePaths = value;
    }
    /// <summary>
    /// Gets or sets the assemblies that provide discoverable types such as enums.
    /// </summary>
    public string[] AssemblyFilePaths
    {
        get => fAssemblyFilePaths != null ? fAssemblyFilePaths : [];
        set => fAssemblyFilePaths = value;
    }
    /// <summary>
    /// Gets or sets the available output targets.
    /// </summary>
    public RegBuilderOutput[] Targets
    {
        get => fTargets != null ? fTargets : [];
        set => fTargets = value;
    }
    /// <summary>
    /// Gets or sets the schema generation projects.
    /// </summary>
    public RegBuilderProject[] Projects
    {
        get => fProjects != null ? fProjects : [];
        set => fProjects = value;
    }
}
