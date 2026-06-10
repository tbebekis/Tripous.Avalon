/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Provides shared sizing helpers for locator controls.
/// </summary>
static public class LocatorControlHelper
{
    // ● public
    static public double GetFieldWidth(LocatorFieldDef FieldDef)
    {
        if (FieldDef.DisplayWidth > 0)
            return FieldDef.DisplayWidth;

        if (FieldDef.Name.IsSameText("Code"))
            return 120;

        if (FieldDef.Name.EndsWithText("Code"))
            return 140;

        if (FieldDef.Name.IsSameText("Name") || FieldDef.Name.EndsWithText("Name"))
            return 220;

        return FieldDef.DataType == DataFieldType.String ? 180 : 120;
    }
    static public double GetPopupWidth(IEnumerable<LocatorFieldDef> Fields, double MinimumWidth = 300, double MaximumWidth = 800)
    {
        double Width = Fields.Where(item => item.IsVisible).Sum(GetFieldWidth) + 30;
        return Math.Clamp(Width, MinimumWidth, MaximumWidth);
    }
}
