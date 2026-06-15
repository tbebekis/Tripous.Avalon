/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;


/// <summary>
/// Describes a form
/// </summary>
public class FormDef: BaseDef
{
    string fClassName;
    string fItemClassName;
    string fReferenceMenuClassName;
    string fModule;
    bool fIsReadOnly;
    string fGroup;
    UserLevel fSecurityLevel;

    // ● private methods
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

    // ● protected methods
    /// <summary>
    /// Returns the title key for this instance.
    /// </summary>
    protected override string GetTitleKey() => SplitTitleKeyToWordsWithPluralEnding();

    // ● public
    /// <summary>
    /// Creates a form instance as described by this instance.
    /// </summary>
    /// <returns></returns>
    public DataForm Create() => TypeStore.CreateInstance<DataForm>(ClassName);
    /// <summary>
    /// Creates a reference context menu.
    /// </summary>
    public ReferenceContextMenu CreateReferenceContextMenu() => TypeStore.CreateInstance<ReferenceContextMenu>(ReferenceMenuClassName);
    /// <summary>
    /// Creates a command that displays the form.
    /// </summary>
    public Command CreateShowCommand(Func<Command, object> ExecuteFunc = null)
    {
        Command Result = new(Name) { TitleKey = TitleKey, Form = Name, SecurityLevel = SecurityLevel };
        Result.ExecuteFunc = ExecuteFunc;
        return Result;
    }
    /// <summary>
    /// Returns true when the specified user may access this form.
    /// </summary>
    public bool CanAccess(AppUser User)
    {
        UserLevel UserLevel = User != null ? User.UserLevel : UserLevel.None;
        return IsAllowed(UserLevel);
    }

    // ● properties
    /// <summary>
    /// The class name of the <see cref="System.Type"/> this descriptor describes.
    /// <para>NOTE: The value of this property may be a string returned by the <see cref="Type.AssemblyQualifiedName"/> property of the type. </para>
    /// <para>In that case, it consists of the type name, including its namespace, followed by a comma, followed by the display name of the assembly
    /// the type belongs to. It might looks like the following</para>
    /// <para><c>Tripous.Data.DataModule, Tripous, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</c></para>
    /// <para>Otherwise it can be a full type name <see cref="Type.FullName"/>, e.g. </para>
    /// <para><c>Tripous.Data.DataModule</c></para>
    /// </summary>
    public string ClassName
    {
        get => !string.IsNullOrWhiteSpace(fClassName)? fClassName: typeof(DataForm).FullName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
    /// <summary>
    /// The class name of the <see cref="System.Type"/> of the item part user control.
    /// <para>NOTE: The value of this property may be a string returned by the <see cref="Type.AssemblyQualifiedName"/> property of the type. </para>
    /// <para>In that case, it consists of the type name, including its namespace, followed by a comma, followed by the display name of the assembly
    /// the type belongs to. It might looks like the following</para>
    /// <para><c>Tripous.Data.DataModule, Tripous, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</c></para>
    /// <para>Otherwise it can be a full type name <see cref="Type.FullName"/>, e.g. </para>
    /// <para><c>Tripous.Data.DataModule</c></para>
    /// </summary>
    public string ItemClassName
    {
        get => !string.IsNullOrWhiteSpace(fItemClassName)? fItemClassName: typeof(ItemPage).FullName;
        set { if (fItemClassName != value) { fItemClassName = value; NotifyPropertyChanged(nameof(ItemClassName)); } }
    }
    /// <summary>
    /// Common context menu for controls that edit reference values, such as lookup and locator controls.
    /// </summary>
    public string ReferenceMenuClassName
    {
        get => !string.IsNullOrWhiteSpace(fReferenceMenuClassName)? fReferenceMenuClassName: typeof(ReferenceContextMenu).FullName;
        set { if (fReferenceMenuClassName != value) { fReferenceMenuClassName = value; NotifyPropertyChanged(nameof(ReferenceMenuClassName)); } }
    }
    /// <summary>
    /// The registration name of the module this form uses.
    /// </summary>
    public string Module
    {
        get => !string.IsNullOrWhiteSpace(fModule) ? fModule : Name;
        set { if (fModule != value) { fModule = value; NotifyPropertyChanged(nameof(Module)); } }
    }
    /// <summary>
    /// The group this form belongs to.
    /// <para>Used in creating groups of <see cref="Command"/> lists.</para>
    /// </summary>
    public string Group
    {
        get => !string.IsNullOrWhiteSpace(fGroup) ? fGroup : "General Forms";
        set { if (fGroup != value) { fGroup = value; NotifyPropertyChanged(nameof(Module)); } }
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
    /// Gets or sets the minimum user level required to access this form.
    /// </summary>
public UserLevel SecurityLevel
    {
        get => fSecurityLevel;
        set { if (fSecurityLevel != value) { fSecurityLevel = value; NotifyPropertyChanged(nameof(SecurityLevel)); } }
    }
}
