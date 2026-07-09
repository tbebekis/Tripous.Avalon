/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Base class for desktop controls that render item FactBox data.
/// </summary>
public class ItemFactBoxControl: UserControl
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemFactBoxControl()
    {
    }

    // ● public
    /// <summary>
    /// Binds this control to FactBox data.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <param name="Data">The FactBox data.</param>
    public virtual void BindFactBox(ItemFactBoxContext Context, object Data)
    {
        this.Context = Context;
        this.Data = Data;
        DataContext = Data;
    }

    // ● properties
    /// <summary>
    /// The FactBox context.
    /// </summary>
    public ItemFactBoxContext Context { get; protected set; }
    /// <summary>
    /// The FactBox data.
    /// </summary>
    public object Data { get; protected set; }
}
