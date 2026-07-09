/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Describes a readonly contextual FactBox for an item page.
/// </summary>
public class ItemFactBoxDef: BaseDef
{
    string fProviderClassName;
    string fDesktopControlClassName;
    string fWebViewName;
    bool fIsVisible = true;

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemFactBoxDef()
    {
    }

    // ● public
    /// <summary>
    /// Creates the provider described by this definition, if any.
    /// </summary>
    /// <returns>The provider instance or null.</returns>
    public ItemFactBoxProvider CreateProvider()
    {
        return string.IsNullOrWhiteSpace(ProviderClassName)
            ? null
            : TypeStore.CreateInstance<ItemFactBoxProvider>(ProviderClassName);
    }

    // ● properties
    /// <summary>
    /// The provider class name that gathers or calculates the FactBox data.
    /// </summary>
    public string ProviderClassName
    {
        get => fProviderClassName;
        set { if (fProviderClassName != value) { fProviderClassName = value; NotifyPropertyChanged(nameof(ProviderClassName)); } }
    }
    /// <summary>
    /// The desktop control class name that renders this FactBox.
    /// </summary>
    public string DesktopControlClassName
    {
        get => fDesktopControlClassName;
        set { if (fDesktopControlClassName != value) { fDesktopControlClassName = value; NotifyPropertyChanged(nameof(DesktopControlClassName)); } }
    }
    /// <summary>
    /// The WebDesk Razor partial view name or path that renders this FactBox.
    /// <para>When empty, the generic WebDesk server renderer is used.</para>
    /// </summary>
    public string WebViewName
    {
        get => fWebViewName;
        set { if (fWebViewName != value) { fWebViewName = value; NotifyPropertyChanged(nameof(WebViewName)); } }
    }
    /// <summary>
    /// Indicates whether this FactBox is visible by default.
    /// </summary>
    public bool IsVisible
    {
        get => fIsVisible;
        set { if (fIsVisible != value) { fIsVisible = value; NotifyPropertyChanged(nameof(IsVisible)); } }
    }
}
