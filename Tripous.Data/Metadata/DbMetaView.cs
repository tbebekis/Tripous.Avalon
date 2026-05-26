/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

public class DbMetaView : DbMetaObject
{
    public string GetFieldNameList() => string.Join($", {Environment.NewLine}", Columns.Select(x => x.Name));
    
    public List<DbMetaColumn> Columns { get; } = new();
    
    
    
}