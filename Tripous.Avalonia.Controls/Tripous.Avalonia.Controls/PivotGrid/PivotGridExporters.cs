// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides the global pivot grid exporter registry.
/// </summary>
public static class PivotGridExporters
{
    // ● private fields
    static readonly List<PivotGridExporter> fExporters = new();
    static readonly List<Func<PivotGridExporter>> fFactories = new();

    // ● constructor
    static PivotGridExporters()
    {
        Register(new PivotGridCsvExporter());
        Register(new PivotGridJsonExporter());
        Register(new PivotGridHtmlExporter());
    }

    // ● static public
    /// <summary>
    /// Registers an exporter instance.
    /// </summary>
    /// <param name="Exporter">The exporter instance.</param>
    static public void Register(PivotGridExporter Exporter)
    {
        if (Exporter == null)
            throw new ArgumentNullException(nameof(Exporter));

        fExporters.Add(Exporter);
    }
    /// <summary>
    /// Registers an exporter factory.
    /// </summary>
    /// <param name="Factory">The exporter factory.</param>
    static public void Register(Func<PivotGridExporter> Factory)
    {
        if (Factory == null)
            throw new ArgumentNullException(nameof(Factory));

        fFactories.Add(Factory);
    }
    /// <summary>
    /// Creates exporter instances from the registry.
    /// </summary>
    /// <returns>The registered exporters.</returns>
    static public IReadOnlyList<PivotGridExporter> CreateExporters()
    {
        List<PivotGridExporter> Result = new();
        Result.AddRange(fExporters);
        foreach (Func<PivotGridExporter> Factory in fFactories)
        {
            PivotGridExporter Exporter = Factory();
            if (Exporter != null)
                Result.Add(Exporter);
        }

        return Result;
    }
}
