/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Base class
/// </summary>
public abstract class DbMetaObject
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string SchemaName { get; set; }               // dbo, public, etc (if exists)
    public virtual string SourceCode { get; set; }
    
    public virtual string DisplayText => Name;
}

 