/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Indicates the type of an Ajax request.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AjaxRequestType
{
    /// <summary>
    /// Requests user interface content or metadata.
    /// </summary>
    Ui = 0,
    /// <summary>
    /// Requests execution of a server procedure.
    /// </summary>
    Proc = 1,
}
