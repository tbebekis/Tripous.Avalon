// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Base class for group grid exporters.
/// </summary>
public abstract class GroupGridExporter
{
    // ● protected methods
    /// <summary>
    /// Ensures that the export file path is a full file path.
    /// </summary>
    /// <param name="FilePath">The export file path.</param>
    protected void ValidateFilePath(string FilePath)
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            throw new ArgumentException("An export file path is required.", nameof(FilePath));
        if (!Path.IsPathFullyQualified(FilePath))
            throw new ArgumentException("The export file path must be a full file path.", nameof(FilePath));
    }
    /// <summary>
    /// Writes export text to a file.
    /// </summary>
    /// <param name="FilePath">The export file path.</param>
    /// <param name="Text">The text to write.</param>
    protected void WriteText(string FilePath, string Text)
    {
        ValidateFilePath(FilePath);
        string DirectoryPath = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        File.WriteAllText(FilePath, Text ?? string.Empty, Encoding.UTF8);
    }

    // ● public methods
    /// <summary>
    /// Exports a group grid snapshot.
    /// </summary>
    /// <param name="Grid">The source group grid.</param>
    /// <param name="Snapshot">The export snapshot.</param>
    /// <param name="FilePath">The export file path.</param>
    public abstract void Export(GroupGrid Grid, GroupGridExportSnapshot Snapshot, string FilePath);

    // ● properties
    /// <summary>
    /// Gets the exporter display name.
    /// </summary>
    public abstract string Name { get; }
    /// <summary>
    /// Gets the default file extension without a leading dot.
    /// </summary>
    public abstract string DefaultExtension { get; }
}
