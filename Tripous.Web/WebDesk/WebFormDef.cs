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
    string fModule;
    string fGroup;
    bool fIsReadOnly;
    UserLevel fSecurityLevel;

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
    /// The registration name of the module this web form uses.
    /// </summary>
    public string Module
    {
        get => !string.IsNullOrWhiteSpace(fModule) ? fModule : Name;
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
}
