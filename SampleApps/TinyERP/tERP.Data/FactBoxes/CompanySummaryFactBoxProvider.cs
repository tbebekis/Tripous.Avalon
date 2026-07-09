/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Provides summary information for the Company item FactBox.
/// </summary>
public class CompanySummaryFactBoxProvider: ItemFactBoxProvider
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public CompanySummaryFactBoxProvider()
    {
    }

    // ● private
    /// <summary>
    /// Returns the number of non-deleted rows in a table.
    /// </summary>
    /// <param name="Table">The table.</param>
    /// <returns>The row count.</returns>
    static int GetRowCount(MemTable Table) => Table != null ? Table.GetRowCount() : 0;
    /// <summary>
    /// Finds the first row matching a boolean field.
    /// </summary>
    /// <param name="Table">The table.</param>
    /// <param name="FieldName">The boolean field name.</param>
    /// <returns>The matching row or null.</returns>
    static DataRow FindBooleanRow(MemTable Table, string FieldName)
    {
        if (Table == null || string.IsNullOrWhiteSpace(FieldName) || !Table.Columns.Contains(FieldName))
            return null;

        foreach (DataRow Row in Table.Rows)
        {
            if (Row.RowState != DataRowState.Deleted && Row.AsBoolean(FieldName))
                return Row;
        }

        return null;
    }
    /// <summary>
    /// Returns a row display name.
    /// </summary>
    /// <param name="Row">The row.</param>
    /// <returns>The display name.</returns>
    static string GetRowName(DataRow Row)
    {
        if (Row == null)
            return string.Empty;
        if (Row.Table.Columns.Contains("Name"))
            return Row.AsString("Name");
        if (Row.Table.Columns.Contains("Title"))
            return Row.AsString("Title");
        if (Row.Table.Columns.Contains("Code"))
            return Row.AsString("Code");
        return string.Empty;
    }

    // ● public
    /// <summary>
    /// Creates serializable data for a FactBox.
    /// </summary>
    /// <param name="Context">The FactBox context.</param>
    /// <returns>The serializable FactBox data.</returns>
    public override object GetData(ItemFactBoxContext Context)
    {
        DataModule Module = Context?.Module;
        DataRow CompanyRow = Context?.Row ?? Module?.tblItem?.CurrentRow;
        MemTable BranchTable = Module?.FindTable("CompanyBranch");
        MemTable BankAccountTable = Module?.FindTable("CompanyBankAccount");
        DataRow PrimaryBranch = FindBooleanRow(BranchTable, "IsPrimary");
        DataRow DefaultBankAccount = FindBooleanRow(BankAccountTable, "IsDefault");

        return new Dictionary<string, object>
        {
            ["Company"] = GetRowName(CompanyRow),
            ["Tax Number"] = CompanyRow != null && CompanyRow.Table.Columns.Contains("TaxNumber") ? CompanyRow.AsString("TaxNumber") : string.Empty,
            ["Branches"] = GetRowCount(BranchTable),
            ["Primary Branch"] = GetRowName(PrimaryBranch),
            ["Bank Accounts"] = GetRowCount(BankAccountTable),
            ["Default Bank Account"] = GetRowName(DefaultBankAccount)
        };
    }
}
