// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a projected pivot axis item.
/// </summary>
public class PivotGridAxisItem
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridAxisItem"/> class.
    /// </summary>
    /// <param name="Key">The internal key.</param>
    /// <param name="Text">The display text.</param>
    /// <param name="Values">The axis field values.</param>
    public PivotGridAxisItem(string Key, string Text, IReadOnlyList<object> Values)
    {
        this.Key = Key ?? string.Empty;
        this.Text = Text ?? string.Empty;
        this.Values = Values ?? Array.Empty<object>();
    }

    // ● properties
    /// <summary>
    /// Gets the internal key.
    /// </summary>
    public string Key { get; }
    /// <summary>
    /// Gets the display text.
    /// </summary>
    public string Text { get; }
    /// <summary>
    /// Gets the axis field values.
    /// </summary>
    public IReadOnlyList<object> Values { get; }
}
