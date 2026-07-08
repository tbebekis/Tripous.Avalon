// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides the global group grid exporter registry.
/// </summary>
public static class GroupGridExporters
{
    // ● private fields
    static readonly List<GroupGridExporter> fExporters = new();
    static readonly List<Func<GroupGridExporter>> fFactories = new();

    // ● constructor
    static GroupGridExporters()
    {
        Register(new GroupGridCsvExporter());
        Register(new GroupGridJsonExporter());
        Register(new GroupGridHtmlExporter());
    }

    // ● static public
    /// <summary>
    /// Registers an exporter instance.
    /// </summary>
    /// <param name="Exporter">The exporter instance.</param>
    static public void Register(GroupGridExporter Exporter)
    {
        if (Exporter == null)
            throw new ArgumentNullException(nameof(Exporter));

        fExporters.Add(Exporter);
    }
    /// <summary>
    /// Registers an exporter factory.
    /// </summary>
    /// <param name="Factory">The exporter factory.</param>
    static public void Register(Func<GroupGridExporter> Factory)
    {
        if (Factory == null)
            throw new ArgumentNullException(nameof(Factory));

        fFactories.Add(Factory);
    }
    /// <summary>
    /// Creates exporter instances from the registry.
    /// </summary>
    /// <returns>The registered exporters.</returns>
    static public IReadOnlyList<GroupGridExporter> CreateExporters()
    {
        List<GroupGridExporter> Result = new();
        Result.AddRange(fExporters);
        foreach (Func<GroupGridExporter> Factory in fFactories)
        {
            GroupGridExporter Exporter = Factory();
            if (Exporter != null)
                Result.Add(Exporter);
        }

        return Result;
    }
}
