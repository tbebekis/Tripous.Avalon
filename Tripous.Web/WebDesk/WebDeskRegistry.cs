/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// A registry of <see cref="WebFormDef"/> instances.
/// </summary>
static public class WebDeskRegistry
{
    // ● private
    static WebFormDef AddFormInternal(string Name, string TitleKey = null, string Module = null, string ViewName = null, string ItemViewName = null, string Group = null, bool IsReadOnly = false, UserLevel SecurityLevel = UserLevel.None)
    {
        WebFormDef Result = new();

        Result.Name = Name;
        Result.TitleKey = TitleKey;
        Result.Module = !string.IsNullOrWhiteSpace(Module) ? Module : Name;
        Result.ViewName = ViewName;
        Result.ItemViewName = ItemViewName;
        Result.Group = Group;
        Result.IsReadOnly = IsReadOnly;
        Result.SecurityLevel = SecurityLevel;

        WebDeskRegistry.Forms.Add(Result);
        return Result;
    }
    static void CheckForm(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add a {nameof(WebFormDef)}. No '{nameof(Name)}' is provided.");
        if (Forms.Contains(Name))
            throw new TripousException($"Cannot add a {nameof(WebFormDef)}. '{Name}' is already registered.");
    }

    // ● forms
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public WebFormDef AddForm(string Name, string TitleKey = null, string Module = null, string ViewName = null, string ItemViewName = null, string Group = null, bool IsReadOnly = false, UserLevel SecurityLevel = UserLevel.None)
    {
        CheckForm(Name);
        WebFormDef Result = AddFormInternal(Name, TitleKey, Module, ViewName, ItemViewName, Group, IsReadOnly, SecurityLevel);
        return Result;
    }
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public WebFormDef AddForm(string Name, string Module, string Group) => AddForm(Name: Name, Module: Module, Group: Group);
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public WebFormDef AddForm(string Name, string Module, string ViewName, string Group) => AddForm(Name: Name, Module: Module, ViewName: ViewName, Group: Group);
    /// <summary>
    /// Adds a definition to the registry.
    /// <para>If the definition exists, an exception is thrown.</para>
    /// </summary>
    static public WebFormDef AddForm(string Name, string Module, string ViewName, string ItemViewName, string Group) => AddForm(Name: Name, Module: Module, ViewName: ViewName, ItemViewName: ItemViewName, Group: Group);
    /// <summary>
    /// Adds or updates a web form definition.
    /// <para><b>NOTE:</b> When the definition already exists, non-null parameters and nullable boolean parameters with a value update its scalar properties. The existing definition instance and its child collections are preserved.</para>
    /// </summary>
    static public WebFormDef AddOrUpdateForm(string Name, string TitleKey = null, string Module = null, string ViewName = null, string ItemViewName = null, string Group = null, bool? IsReadOnly = null, UserLevel? SecurityLevel = null)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousException($"Cannot add or update a {nameof(WebFormDef)}. No '{nameof(Name)}' is provided.");

        WebFormDef Result = Forms.Find(Name);
        if (Result == null)
            Result = AddFormInternal(Name, TitleKey, Module, ViewName, ItemViewName, Group, IsReadOnly ?? false, SecurityLevel ?? UserLevel.None);
        else
        {
            if (TitleKey != null)
                Result.TitleKey = TitleKey;
            if (Module != null)
                Result.Module = Module;
            if (ViewName != null)
                Result.ViewName = ViewName;
            if (ItemViewName != null)
                Result.ItemViewName = ItemViewName;
            if (Group != null)
                Result.Group = Group;
            if (IsReadOnly.HasValue)
                Result.IsReadOnly = IsReadOnly.Value;
            if (SecurityLevel.HasValue)
                Result.SecurityLevel = SecurityLevel.Value;
        }
        return Result;
    }
    /// <summary>
    /// Finds and returns a web form definition by name, if any; otherwise returns null.
    /// </summary>
    static public WebFormDef FindForm(string Name) => Forms.Find(Name);
    /// <summary>
    /// Returns a web form definition by name.
    /// Throws an exception when the definition is not found.
    /// </summary>
    static public WebFormDef GetForm(string Name) => Forms.Get(Name);

    // ● properties
    /// <summary>
    /// The list of registered web forms.
    /// </summary>
    static public DefList<WebFormDef> Forms { get; } = new();
}
