/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Registry version 2 FactBox registrations.
/// </summary>
public partial class RegistryVersion2
{
    // ● private
    /// <summary>
    /// Registers the Company summary FactBox.
    /// </summary>
    static void RegisterCompanySummaryFactBox()
    {
        ModuleDef Module = DataRegistry.Modules.Find("Company");
        if (Module == null || Module.FactBoxes.Contains("CompanySummary"))
            return;

        Module.FactBoxes.Add(new ItemFactBoxDef
        {
            Name = "CompanySummary",
            TitleKey = "Company Summary",
            ProviderClassName = typeof(CompanySummaryFactBoxProvider).FullName,
            DesktopControlClassName = "Tripous.Desktop.ItemInfoFactBoxControl",
            WebViewName = "FactBoxes/CompanySummary"
        });
    }

    // ● public
    /// <summary>
    /// Registers custom module FactBoxes.
    /// </summary>
    public override void RegisterFactBoxes()
    {
        RegisterCompanySummaryFactBox();
    }
}
