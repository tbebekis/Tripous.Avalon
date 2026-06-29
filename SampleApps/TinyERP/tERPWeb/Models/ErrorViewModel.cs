/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.Models;

/// <summary>
/// Error view model.
/// </summary>
public class ErrorViewModel
{
    // ● properties
    /// <summary>
    /// Gets or sets the request id.
    /// </summary>
    public string RequestId { get; set; }
    /// <summary>
    /// True when the request id should be displayed.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
}
