// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a BI-friendly chart palette.
/// </summary>
public class ChartPalette
{
    // ● private fields
    readonly List<Color> fColors = new();

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPalette"/> class.
    /// </summary>
    public ChartPalette()
    {
    }

    // ● static public methods
    /// <summary>
    /// Returns a built-in palette by name.
    /// </summary>
    /// <param name="Name">The palette name.</param>
    /// <returns>The palette.</returns>
    static public ChartPalette Get(string Name)
    {
        string PaletteName = string.IsNullOrWhiteSpace(Name) ? "Business" : Name.Trim();
        if (string.Equals(PaletteName, "Muted", StringComparison.OrdinalIgnoreCase))
            return Create("Muted", "#607D8B", "#90A4AE", "#78909C", "#546E7A", "#455A64", "#B0BEC5");
        if (string.Equals(PaletteName, "Signal", StringComparison.OrdinalIgnoreCase))
            return Create("Signal", "#2E7D32", "#1565C0", "#C62828", "#EF6C00", "#6A1B9A", "#00838F");

        return Create("Business", "#2563EB", "#059669", "#D97706", "#DC2626", "#7C3AED", "#0891B2", "#4B5563", "#84CC16");
    }
    /// <summary>
    /// Creates a palette from color strings.
    /// </summary>
    /// <param name="Name">The palette name.</param>
    /// <param name="Colors">The palette colors.</param>
    /// <returns>The palette.</returns>
    static public ChartPalette Create(string Name, params string[] Colors)
    {
        ChartPalette Result = new() { Name = Name ?? string.Empty };
        foreach (string ColorText in Colors)
            Result.Colors.Add(Color.Parse(ColorText));

        return Result;
    }

    // ● public methods
    /// <summary>
    /// Returns a color by index.
    /// </summary>
    /// <param name="Index">The color index.</param>
    /// <returns>The color.</returns>
    public Color GetColor(int Index)
    {
        if (fColors.Count == 0)
            return Color.Parse("#2563EB");

        int NormalizedIndex = Math.Abs(Index) % fColors.Count;
        return fColors[NormalizedIndex];
    }

    // ● properties
    /// <summary>
    /// Gets or sets the palette name.
    /// </summary>
    public string Name { get; set; } = "Business";
    /// <summary>
    /// Gets the palette colors.
    /// </summary>
    public IList<Color> Colors => fColors;
}
