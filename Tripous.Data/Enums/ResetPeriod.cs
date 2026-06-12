/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Indicates the period after which a value is reset.
/// </summary>
[TypeStore]
public enum ResetPeriod
{
    /// <summary>
    /// No reset period.
    /// </summary>
    None = 0,
    /// <summary>
    /// Resets once a year.
    /// </summary>
    Year = 1,
    /// <summary>
    /// Resets once a semester.
    /// </summary>
    Semester = 2,
    /// <summary>
    /// Resets once a quarter.
    /// </summary>
    Quarter = 3,
    /// <summary>
    /// Resets once a month.
    /// </summary>
    Month = 4,
    /// <summary>
    /// Resets once a week.
    /// </summary>
    Week = 5,
    /// <summary>
    /// Resets once a day.
    /// </summary>
    Day = 6,
}