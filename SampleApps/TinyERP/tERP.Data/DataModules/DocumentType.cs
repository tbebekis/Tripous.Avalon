/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class DocumentType
{
    // ● private
    static SqlStore Store = Db.DefaultStore;

    // ● constructors
    public DocumentType(string ModuleName)
    {
        string SqlText = $"select * from DocumentType where ModuleName = '{ModuleName}'";
        DataRow Row = Store.SelectResults(SqlText);

        Id = Row.AsString("Id");
        Code = Row.AsString("Code");
        Name = Row.AsString("Name");
        TradeTypeId = Row.AsInteger("TradeTypeId");
        NumberSeriesId = Row.AsString("NumberSeriesId");
        this.ModuleName = Row.AsString("ModuleName");
        IsActive = Row.AsBoolean("IsActive");
        IsSystem = Row.AsBoolean("IsSystem");
        AllowManualNumber = Row.AsBoolean("AllowManualNumber");
        AutoComplete = Row.AsBoolean("AutoComplete");
        AffectsStock = Row.AsBoolean("AffectsStock");
        AffectsFinancial = Row.AsBoolean("AffectsFinancial");
        AffectsAccounting = Row.AsBoolean("AffectsAccounting");
        StockDirection = Row.AsInteger("StockDirection");
        FinancialDirection = Row.AsInteger("FinancialDirection");
        AccountingDirection = Row.AsInteger("AccountingDirection");
        IsCancellation = Row.AsBoolean("IsCancellation");
        CancellationTargetId = Row.AsString("CancellationTargetId");
        PrintTemplate = Row.AsString("PrintTemplate");
        ReportName = Row.AsString("ReportName");
        DisplayOrder = Row.AsInteger("DisplayOrder");
        Color = Row.AsString("Color");
        IconName = Row.AsString("IconName");
        Remarks = Row.BlobToString("Remarks");
    }
    
    // ● public
    override public string ToString() => Name;

    // ● properties
    public string Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public int TradeTypeId { get; private set; }
    public string NumberSeriesId { get; private set; }
    public string ModuleName { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystem { get; private set; }
    public bool AllowManualNumber { get; private set; }
    public bool AutoComplete { get; private set; }
    public bool AffectsStock { get; private set; }
    public bool AffectsFinancial { get; private set; }
    public bool AffectsAccounting { get; private set; }
    public int StockDirection { get; private set; }
    public int FinancialDirection { get; private set; }
    public int AccountingDirection { get; private set; }
    public bool IsCancellation { get; private set; }
    public string CancellationTargetId { get; private set; }
    public string PrintTemplate { get; private set; }
    public string ReportName { get; private set; }
    public int DisplayOrder { get; private set; }
    public string Color { get; private set; }
    public string IconName { get; private set; }
    public string Remarks { get; private set; }
}