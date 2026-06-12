/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Represents the base abstract class for all database metadata schema objects.
/// </summary>
public abstract class DbMetaObject
{
    // ● properties
    /// <summary>
    /// Gets or sets the database object identifier name.
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the schema namespace owner of the object (e.g., dbo, public).
    /// </summary>
    public virtual string SchemaName { get; set; }
    /// <summary>
    /// Gets or sets the underlying DDL source code script of the object.
    /// </summary>
    public virtual string SourceCode { get; set; }
    /// <summary>
    /// Gets a descriptive, formatted string representation of the database object.
    /// </summary>
    public virtual string DisplayText => Name;
}