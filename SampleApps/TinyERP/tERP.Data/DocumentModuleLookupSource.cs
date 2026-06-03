/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// A lookup source for document modules
/// </summary>
public class DocumentModuleLookupSource: LookupSource
{
    // ● construction 
    /// <summary>
    /// Constructor
    /// </summary>
    public DocumentModuleLookupSource()
    {
    }
    
    // ● public
    /// <summary>
    /// Fills the list using a SELECT statement
    /// </summary>
    public override void Select(string SqlText)
    {
    }
    /// <summary>
    /// Fills the list using a <see cref="DataTable"/>
    /// </summary>
    public override void LoadFrom(DataTable Table)
    {
    }
    /// <summary>
    /// Fills the list using an enum type.
    /// </summary>
    /// <param name="Enum"></param>
    public override void LoadFrom(Enum Enum)
    {
    }

    /// <summary>
    /// Returns the lookup list, full of items.
    /// </summary>
    public override List<LookupItem> GetList()
    {
        if (List == null)
            List = [];
        
        if (List.Count == 0)
        {
            LookupItem Item;
            foreach (DocumentHandlerDef HandlerDef in DataRegistry.DocumentHandlers)
            {
                Item = new(HandlerDef.Name, HandlerDef.Name);
                List.Add(Item);
            }
        }
            
        return List;
    }
 
}