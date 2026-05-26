/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

public class DbMetaProcedure : DbMetaObject
{
    public string ProcedureType { get; set; }              // procedure vs function
    
    public override string DisplayText
    {
        get
        {
            string Result = Name;
            
            if (!string.IsNullOrWhiteSpace(ProcedureType))
                Result += $" ({ProcedureType})";
 
            return Result;
        }
    }
}