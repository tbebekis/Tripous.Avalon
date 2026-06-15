/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;
 
/// <summary>
/// A registry of <see cref="FormDef"/> instances.
/// </summary>
static public class DesktopRegistry
{
    // ● private
    static FormDef AddFormInternal(string Name, string TitleKey = null, string Module = null, string ClassName = null, string Group = null, string ItemClassName = null, bool IsReadOnly = false, UserLevel SecurityLevel = UserLevel.None)
    {
        FormDef Result = new();
        
        Result.Name = Name;
        Result.TitleKey = TitleKey;
        Result.Module = !string.IsNullOrWhiteSpace(Module) ? Module : Name;
        Result.ClassName = ClassName;
        Result.Group = Group;
        Result.ItemClassName = ItemClassName;
        Result.IsReadOnly = IsReadOnly;
        Result.SecurityLevel = SecurityLevel;
        
        DesktopRegistry.Forms.Add(Result);
        return Result;
    }
    static void CheckForm(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(FormDef)}. No '{nameof(Name)}' is provided.");
        if (Forms.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(FormDef)}. '{Name}' is already registered.");
    }
    
    // ● forms
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string TitleKey = null, string Module = null, string ClassName = null, string Group = null, string ItemClassName = null, bool IsReadOnly = false, UserLevel SecurityLevel = UserLevel.None)
    {
        CheckForm(Name);
        FormDef Result = AddFormInternal(Name, TitleKey, Module, ClassName, Group, ItemClassName, IsReadOnly, SecurityLevel);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string Module, string Group) => AddForm(Name: Name, Module: Module, Group: Group);
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string Module, string ClassName, string Group) => AddForm(Name: Name, Module: Module, ClassName: ClassName, Group: Group);
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public FormDef AddForm(string Name, string Module, string ClassName, string TitleKey, string Group) => AddForm(Name: Name, Module: Module, ClassName: ClassName, TitleKey: TitleKey, Group: Group);
    /// <summary>
    /// Adds or updates a form definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public FormDef AddOrUpdateForm(string Name, string TitleKey = null, string Module = null, string ClassName = null, string Group = null, string ItemClassName = null, bool? IsReadOnly = null, UserLevel? SecurityLevel = null)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(FormDef)}. No '{nameof(Name)}' is provided.");

        FormDef Result = Forms.Find(Name);
        if (Result == null)
            Result = AddFormInternal(Name, TitleKey, Module, ClassName, Group, ItemClassName, IsReadOnly ?? false, SecurityLevel ?? UserLevel.None);
        else
        {
            if (TitleKey != null)
                Result.TitleKey = TitleKey;
            if (Module != null)
                Result.Module = Module;
            if (ClassName != null)
                Result.ClassName = ClassName;
            if (Group != null)
                Result.Group = Group;
            if (ItemClassName != null)
                Result.ItemClassName = ItemClassName;
            if (IsReadOnly.HasValue)
                Result.IsReadOnly = IsReadOnly.Value;
            if (SecurityLevel.HasValue)
                Result.SecurityLevel = SecurityLevel.Value;
        }
        return Result;
    }
    
    // ● create form
    /// <summary>
    /// Creates a <see cref="DataForm"/> instance based on the name of a definition.
    /// </summary>
    static public DataForm CreateDataForm(string Name) => Forms.Get(Name).Create();
    
    // ● properties
    /// <summary>
    /// The list of registered forms.
    /// </summary>
    static public DefList<FormDef> Forms { get; } = new();
    
}
