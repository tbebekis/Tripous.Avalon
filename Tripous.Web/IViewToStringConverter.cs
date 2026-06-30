/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Web;

/// <summary>
/// Represents an object that renders a Razor view to a string.
/// </summary>
public interface IViewToStringConverter
{
    // ● public
    /// <summary>
    /// Renders a view to a string.
    /// </summary>
    string ViewToString(string ViewName, object Model, IDictionary<string, object> PlusViewData = null);
    /// <summary>
    /// Renders a view to a string.
    /// </summary>
    string ViewToString(string ViewName, IDictionary<string, object> PlusViewData = null);
}
