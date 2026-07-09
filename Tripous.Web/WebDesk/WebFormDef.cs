/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Describes a web form used by a desktop-like data entry web application.
/// </summary>
public class WebFormDef: BaseDef
{
    /// <summary>
    /// Default Razor view name for standard WebDesk data-entry forms.
    /// </summary>
    public const string DefaultViewName = "WebDataForm";
    /// <summary>
    /// Default Razor partial view name for standard WebDesk item pages.
    /// </summary>
    public const string DefaultItemViewName = "WebItemPage";

    // ● private fields
    string fViewName;
    string fItemViewName;
    string fModule;
    string fGroup;
    string fJsFormClassType;
    bool fIsReadOnly;
    bool fIsCustom;
    UserLevel fSecurityLevel;
    DefList<ItemFactBoxDef> fFactBoxes;

    // ● private
    bool IsAllowed(UserLevel UserLevel)
    {
        if (SecurityLevel == UserLevel.None)
            return true;
        if ((UserLevel & UserLevel.God) == UserLevel.God)
            return true;
        if ((UserLevel & UserLevel.Admin) == UserLevel.Admin)
            return SecurityLevel == UserLevel.Admin || SecurityLevel == UserLevel.User || SecurityLevel == UserLevel.Guest;
        if ((UserLevel & UserLevel.User) == UserLevel.User)
            return SecurityLevel == UserLevel.User || SecurityLevel == UserLevel.Guest;
        if ((UserLevel & UserLevel.Guest) == UserLevel.Guest)
            return SecurityLevel == UserLevel.Guest;
        return (UserLevel & SecurityLevel) == SecurityLevel;
    }

    // ● protected
    /// <summary>
    /// Returns the title key for this instance.
    /// </summary>
    protected override string GetTitleKey() => SplitTitleKeyToWordsWithPluralEnding();

    // ● public
    /// <summary>
    /// Returns true when the specified user may access this web form.
    /// </summary>
    public bool CanAccess(AppUser User)
    {
        UserLevel UserLevel = User != null ? User.UserLevel : UserLevel.None;
        return IsAllowed(UserLevel);
    }

    // ● properties
    /// <summary>
    /// The Razor view name or path of the main web form view.
    /// </summary>
    public string ViewName
    {
        get => !string.IsNullOrWhiteSpace(fViewName) ? fViewName : DefaultViewName;
        set { if (fViewName != value) { fViewName = value; NotifyPropertyChanged(nameof(ViewName)); } }
    }
    /// <summary>
    /// The optional Razor partial view name or path of the item page used by the main web form view.
    /// </summary>
    public string ItemViewName
    {
        get => !string.IsNullOrWhiteSpace(fItemViewName) ? fItemViewName : (IsCustom ? string.Empty : DefaultItemViewName);
        set { if (fItemViewName != value) { fItemViewName = value; NotifyPropertyChanged(nameof(ItemViewName)); } }
    }
    /// <summary>
    /// The registration name of the module this web form uses.
    /// </summary>
    public string Module
    {
        get => !string.IsNullOrWhiteSpace(fModule) ? fModule : (IsCustom ? string.Empty : Name);
        set { if (fModule != value) { fModule = value; NotifyPropertyChanged(nameof(Module)); } }
    }
    /// <summary>
    /// The group this web form belongs to.
    /// </summary>
    public string Group
    {
        get => !string.IsNullOrWhiteSpace(fGroup) ? fGroup : "General Forms";
        set { if (fGroup != value) { fGroup = value; NotifyPropertyChanged(nameof(Group)); } }
    }
    /// <summary>
    /// When true then no edits are allowed.
    /// </summary>
    public bool IsReadOnly
    {
        get => fIsReadOnly;
        set { if (fIsReadOnly != value) { fIsReadOnly = value; NotifyPropertyChanged(nameof(IsReadOnly)); } }
    }
    /// <summary>
    /// When true then this form is provided by custom markup and JavaScript.
    /// </summary>
    public bool IsCustom
    {
        get => fIsCustom;
        set { if (fIsCustom != value) { fIsCustom = value; NotifyPropertyChanged(nameof(IsCustom)); } }
    }
    /// <summary>
    /// Gets or sets the JavaScript class type that handles this form on the client.
    /// </summary>
    public string JsFormClassType
    {
        get => !string.IsNullOrWhiteSpace(fJsFormClassType) ? fJsFormClassType : string.Empty;
        set { if (fJsFormClassType != value) { fJsFormClassType = value; NotifyPropertyChanged(nameof(JsFormClassType)); } }
    }
    /// <summary>
    /// Gets or sets the minimum user level required to access this web form.
    /// </summary>
    public UserLevel SecurityLevel
    {
        get => fSecurityLevel;
        set { if (fSecurityLevel != value) { fSecurityLevel = value; NotifyPropertyChanged(nameof(SecurityLevel)); } }
    }
    /// <summary>
    /// Gets the extra CSS files this web form requires.
    /// </summary>
    public List<string> CssFiles { get; } = new();
    /// <summary>
    /// Gets the extra JavaScript files this web form requires.
    /// </summary>
    public List<string> JavaScriptFiles { get; } = new();
    /// <summary>
    /// The custom FactBoxes displayed by the item page of this web form.
    /// </summary>
    public DefList<ItemFactBoxDef> FactBoxes
    {
        get
        {
            fFactBoxes ??= new();
            return fFactBoxes;
        }
    }
}
