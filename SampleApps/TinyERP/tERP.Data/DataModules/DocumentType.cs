/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Represents a document type definition.
/// </summary>
public class DocumentType
{
    // ● private
    static SqlStore Store = Db.DefaultStore;

    // ● constructors
    /// <summary>
    /// Loads a document type by module name.
    /// </summary>
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
    /// <summary>
    /// Returns the document type name.
    /// </summary>
    override public string ToString() => Name;

    // ● properties
    /// <summary>
    /// Document type identifier.
    /// </summary>
    public string Id { get; private set; }
    /// <summary>
    /// Document type code.
    /// </summary>
    public string Code { get; private set; }
    /// <summary>
    /// Document type name.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// Trade type identifier.
    /// </summary>
    public int TradeTypeId { get; private set; }
    /// <summary>
    /// Number series identifier.
    /// </summary>
    public string NumberSeriesId { get; private set; }
    /// <summary>
    /// Module name associated with this document type.
    /// </summary>
    public string ModuleName { get; private set; }
    /// <summary>
    /// Indicates whether the document type is active.
    /// </summary>
    public bool IsActive { get; private set; }
    /// <summary>
    /// Indicates whether the document type is system-defined.
    /// </summary>
    public bool IsSystem { get; private set; }
    /// <summary>
    /// Indicates whether manual document numbers are allowed.
    /// </summary>
    public bool AllowManualNumber { get; private set; }
    /// <summary>
    /// Indicates whether documents of this type are completed automatically.
    /// </summary>
    public bool AutoComplete { get; private set; }
    /// <summary>
    /// Indicates whether documents of this type affect stock.
    /// </summary>
    public bool AffectsStock { get; private set; }
    /// <summary>
    /// Indicates whether documents of this type affect financial balances.
    /// </summary>
    public bool AffectsFinancial { get; private set; }
    /// <summary>
    /// Indicates whether documents of this type affect accounting.
    /// </summary>
    public bool AffectsAccounting { get; private set; }
    /// <summary>
    /// Stock movement direction.
    /// </summary>
    public int StockDirection { get; private set; }
    /// <summary>
    /// Financial movement direction.
    /// </summary>
    public int FinancialDirection { get; private set; }
    /// <summary>
    /// Accounting movement direction.
    /// </summary>
    public int AccountingDirection { get; private set; }
    /// <summary>
    /// Indicates whether this document type is a cancellation type.
    /// </summary>
    public bool IsCancellation { get; private set; }
    /// <summary>
    /// Target document type identifier for cancellations.
    /// </summary>
    public string CancellationTargetId { get; private set; }
    /// <summary>
    /// Print template name.
    /// </summary>
    public string PrintTemplate { get; private set; }
    /// <summary>
    /// Report name.
    /// </summary>
    public string ReportName { get; private set; }
    /// <summary>
    /// Display order.
    /// </summary>
    public int DisplayOrder { get; private set; }
    /// <summary>
    /// Display color.
    /// </summary>
    public string Color { get; private set; }
    /// <summary>
    /// Icon name.
    /// </summary>
    public string IconName { get; private set; }
    /// <summary>
    /// Remarks.
    /// </summary>
    public string Remarks { get; private set; }
}
