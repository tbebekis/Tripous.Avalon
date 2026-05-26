/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

[TypeStore]
public enum ResetPeriod
{
    None = 0,
    Year = 1,
    Semester = 2,
    Quarter = 3,
    Month = 4,
    Week = 5,
    Day = 6,
}