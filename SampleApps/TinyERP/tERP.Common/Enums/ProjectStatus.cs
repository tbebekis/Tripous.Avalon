/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Common;

/// <summary>
/// Defines the lifecycle status of a project.
/// </summary>
[TypeStore]
public enum ProjectStatus
{
    /// <summary>No project status is specified.</summary>
    None = 0,
    /// <summary>The project is being prepared and has not started.</summary>
    Draft = 1,
    /// <summary>The project is currently active.</summary>
    Active = 2,
    /// <summary>The project is temporarily paused.</summary>
    Suspended = 3,
    /// <summary>The project has finished successfully.</summary>
    Completed = 4,
    /// <summary>The project was cancelled before completion.</summary>
    Cancelled = 5,
}
