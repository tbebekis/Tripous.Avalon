/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Represents this library.
/// </summary>
static public class DesktopLib
{
    // ● private
    static List<RegistryVersion> RegistryVersionList = [];

    // ● construction
    static DesktopLib()
    {
        RegistryVersionList.AddRange([
            new DesktopRegistryVersion1(),
            new DesktopRegistryVersion2()
        ]);
    }

    // ● public
    /// <summary>
    /// We need to call this first of all in order for .Net to load the assembly.
    /// <para>Otherwise is not "visible" to <see cref="TypeStore.RegisterLoadedAssemblies()"/> which registers types marked with the <see cref="TypeStoreAttribute"/>.</para>
    /// </summary>
    static public void Load()
    {
        // fake, must be called for the assembly to be loaded in the domain.
    }
    /// <summary>
    /// Initializes this library.
    /// </summary>
    static public void Initialize()
    {
        // nothing yet
    }
    /// <summary>
    /// Registers desktop descriptors.
    /// </summary>
    static public void RegisterDescriptors()
    {
        foreach (RegistryVersion Version in RegistryVersionList)
            Version.RegisterForms();

        UpdateForms();
        RegisterConfigProperties();
    }
    /// <summary>
    /// Definitions added by the registration builder may be incomplete.
    /// <para>This method provides a chance to complete those definitions.</para>
    /// </summary>
    static public void UpdateForms()
    {
        DesktopRegistry.Forms.Get("SalesDeliveryNote").ClassName = "SalesDeliveryNoteForm";

        FormDef AppUserForm = DesktopRegistry.Forms.Find("AppUser");
        if (AppUserForm != null)
            AppUserForm.SecurityLevel = UserLevel.Admin;

        FormDef CompanyForm = DesktopRegistry.Forms.Find("Company");
        if (CompanyForm != null && !CompanyForm.FactBoxes.Contains("CompanyDesktopFormInfo"))
        {
            CompanyForm.FactBoxes.Add(new ItemFactBoxDef
            {
                Name = "CompanyDesktopFormInfo",
                TitleKey = "Company Desktop Form Info",
                ProviderClassName = typeof(FormScopeFactBoxProvider).FullName,
                DesktopControlClassName = "Tripous.Desktop.ItemInfoFactBoxControl"
            });
        }
    }
    /// <summary>
    /// Registers desktop configuration properties.
    /// </summary>
    static public void RegisterConfigProperties()
    {
        string Name = Ui.SShowDataFormLog;
        string TitleKey = "Show DataForm Log";
        string GroupName = "Application";
        UserLevel SecurityLevel = UserLevel.User;
        ConfigValueKind Kind = ConfigValueKind.Boolean;
        string DefaultValue = "false";

        ConfigPropertyDef ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue);
        ConfigPropertyDef.ApplyValueFunc = (Def, S) =>
        {
            bool Value = Convert.ToBoolean(S);
            Ui.Settings.ShowDataFormLog = Value;
        };

        Name = Ui.SShowDataFormFactBoxPane;
        TitleKey = "Show DataForm FactBox Pane";
        DefaultValue = "true";

        ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue);
        ConfigPropertyDef.ApplyValueFunc = (Def, S) =>
        {
            bool Value = Convert.ToBoolean(S);
            Ui.Settings.ShowDataFormFactBoxPane = Value;
        };

        Name = Ui.STheme;
        TitleKey = "Theme";
        Kind = ConfigValueKind.String;
        DefaultValue = Ui.SDefaultTheme;

        ConfigPropertyDef = DataRegistry.AddOrUpdateConfigProperty(Name, TitleKey, GroupName, SecurityLevel, Kind, DefaultValue, Scopes: ConfigScopeFlags.User);
        ConfigPropertyDef.ApplyValueFunc = (Def, S) =>
        {
            Ui.ApplyTheme(S);
        };
    }
}
