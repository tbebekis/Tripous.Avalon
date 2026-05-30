/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class Registry
{
    /// <summary>
    /// Can be used when seeding sample data to the database.
    /// </summary>
    static public Dictionary<string, string> GetCodeProviderPatterns()
    {
        Dictionary<string, string> Result = [];

        Result["BillOfMaterial"] = "BOM-XXXXXX";
        Result["CashAccount"] = "CASH-XXXXXX";
        Result["Company"] = "XXXXXX";
        Result["FixedAsset"] = "AST-XXXXXX";
        Result["PersonAddress"] = "ADR-XXXXXX";
        Result["Product"] = "XXXXXX";
        Result["Project"] = "XXXXXX";
        Result["SalesPerson"] = "XXXXXX";
        Result["Warehouse"] = "XXXXXX";
        Result["WarehouseLocation"] = "LOC-XXXXXX";

        return Result;
    }
}