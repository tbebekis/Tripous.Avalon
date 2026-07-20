// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Defines the logical role of a pivot grid source field.
/// </summary>
public enum PivotGridFieldRole
{
    /// <summary>
    /// No field role.
    /// </summary>
    None,
    /// <summary>
    /// The field is available but not used.
    /// </summary>
    Available,
    /// <summary>
    /// The field is used in the row axis.
    /// </summary>
    Row,
    /// <summary>
    /// The field is used in the column axis.
    /// </summary>
    Column,
    /// <summary>
    /// The field is used as a value measure.
    /// </summary>
    Measure,
}
