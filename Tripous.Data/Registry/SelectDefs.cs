/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;


/// <summary>
/// A list of SELECT statements.
/// </summary>
public class SelectDefs : DefList<SelectDef>
{
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public SelectDefs()
    {
    }

    /// <summary>
    /// Adds a new SELECT statement.
    /// </summary>
    public SelectDef Add(string Name, string SqlText, string TitleKey = null)
    {
        SelectDef Result = new();
        Result.Name = Name;
        Result.SqlText = SqlText;
        Result.TitleKey = TitleKey;
        Add(Result);
        return Result;
    }
    
}