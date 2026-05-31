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

        Result["ASSET"] = "AST-XXXXXX";
        Result["BillOfMaterial"] = "BOM-XXXXXX";
        Result["CashAccount"] = "CASH-XXXXXX";
        Result["Company"] = "XXXXXX";
        Result["FixedAsset"] = "AST-XXXXXX";
        Result["JOURNAL_ENTRY"] = "JE-YYYY-XXXXXX";
        Result["PersonAddress"] = "ADR-XXXXXX";
        Result["Product"] = "XXXXXX";
        Result["Project"] = "YYYY-XXXX";
        Result["SalesPerson"] = "XXXX";
        Result["STOCK_COUNT"] = "SC-YYYY-XXXXXX";
        Result["STOCK_TRADE_DRAFT"] = "STK-DRAFT-YYYY-XXXXXX";
        Result["TRADE-DRAFT"] = "TR-DRAFT-YYYY-XXXXXX";
        Result["Warehouse"] = "WH-XXXXXX";
        Result["WarehouseLocation"] = "LOC-XXXXXX";

        return Result;
    }
}