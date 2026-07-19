/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Contains information about a posted document.
/// </summary>
public class DocumentPostedInfo: EventArgs
{
    // ● private
    static void AddId(List<string> List, string Id)
    {
        if (!string.IsNullOrWhiteSpace(Id) && !List.Any(Item => Item.IsSameText(Id)))
            List.Add(Id);
    }
    static string GetString(DataRow Row, string FieldName)
    {
        return Row != null && Row.Table.Columns.Contains(FieldName) ? Row.AsString(FieldName) : "";
    }
    static string GetParam(IDictionary<string, object> Params, string Name)
    {
        return Params.TryGetValue(Name, out object Value) && Value != null ? Convert.ToString(Value) : "";
    }
    static List<string> GetStringListParam(IDictionary<string, object> Params, string Name)
    {
        List<string> Result = [];
        if (Params == null || !Params.TryGetValue(Name, out object Value) || Value == null)
            return Result;

        if (Value is IEnumerable<string> StringList)
        {
            foreach (string Item in StringList)
                AddId(Result, Item);
            return Result;
        }

        if (Value is IEnumerable<object> ObjectList)
        {
            foreach (object Item in ObjectList)
                AddId(Result, Convert.ToString(Item));
        }
        return Result;
    }
    static void AddRelatedTradeIds(DocumentPostedInfo Info, DataModule Module)
    {
        MemTable LineTable = Module.FindTable("TradeLine");
        if (LineTable == null || Module.Store == null)
            return;

        foreach (DataRow Line in LineTable.Rows)
        {
            if (Line.RowState == DataRowState.Deleted || Line.RowState == DataRowState.Detached)
                continue;

            string SourceTradeLineId = GetString(Line, "SourceTradeLineId");
            if (string.IsNullOrWhiteSpace(SourceTradeLineId))
                continue;

            DataRow SourceTrade = Module.Store.SelectResults("""
                                                             select T.Id, T.SourceId, T.CancelsTradeId, T.CancelledByTradeId
                                                             from TradeLine TL
                                                               inner join Trade T on T.Id = TL.TradeId
                                                             where TL.Id = :Id
                                                             """, new Dictionary<string, object>()
            {
                ["Id"] = SourceTradeLineId,
            });
            if (SourceTrade == null)
                continue;

            AddId(Info.AffectedDocumentIds, SourceTrade.AsString("Id"));
            AddId(Info.AffectedDocumentIds, SourceTrade.AsString("SourceId"));
            AddId(Info.AffectedDocumentIds, SourceTrade.AsString("CancelsTradeId"));
            AddId(Info.AffectedDocumentIds, SourceTrade.AsString("CancelledByTradeId"));
        }
    }

    // ● static public
    /// <summary>
    /// Creates posted document information from a document row.
    /// </summary>
    /// <param name="ModuleName">The document module name.</param>
    /// <param name="Row">The document row.</param>
    /// <returns>The created information.</returns>
    static public DocumentPostedInfo FromRow(string ModuleName, DataRow Row)
    {
        DocumentPostedInfo Result = new()
        {
            ModuleName = ModuleName,
            DocumentId = GetString(Row, "Id"),
            SourceId = GetString(Row, "SourceId"),
            CancelsTradeId = GetString(Row, "CancelsTradeId"),
            CancelledByTradeId = GetString(Row, "CancelledByTradeId"),
            CancelsStockTradeId = GetString(Row, "CancelsStockTradeId"),
            CancelledByStockTradeId = GetString(Row, "CancelledByStockTradeId"),
            CancelledPaymentId = GetString(Row, "CancelledPaymentId"),
            CancellationPaymentId = GetString(Row, "CancellationPaymentId")
        };
        return Result;
    }
    /// <summary>
    /// Creates posted document information from a data module.
    /// </summary>
    /// <param name="ModuleName">The document module name.</param>
    /// <param name="Module">The document data module.</param>
    /// <returns>The created information.</returns>
    static public DocumentPostedInfo FromModule(string ModuleName, DataModule Module)
    {
        DocumentPostedInfo Result = FromRow(ModuleName, Module?.CurrentRow);
        if (Module != null)
            AddRelatedTradeIds(Result, Module);
        return Result;
    }
    /// <summary>
    /// Creates posted document information from a parameter dictionary.
    /// </summary>
    /// <param name="Params">The parameter dictionary.</param>
    /// <returns>The created information.</returns>
    static public DocumentPostedInfo FromParams(IDictionary<string, object> Params)
    {
        DocumentPostedInfo Result = new();
        if (Params == null)
            return Result;

        Result.ModuleName = GetParam(Params, "ModuleName");
        Result.DocumentId = GetParam(Params, "DocumentId");
        Result.SourceId = GetParam(Params, "SourceId");
        Result.CancelsTradeId = GetParam(Params, "CancelsTradeId");
        Result.CancelledByTradeId = GetParam(Params, "CancelledByTradeId");
        Result.CancelsStockTradeId = GetParam(Params, "CancelsStockTradeId");
        Result.CancelledByStockTradeId = GetParam(Params, "CancelledByStockTradeId");
        Result.CancelledPaymentId = GetParam(Params, "CancelledPaymentId");
        Result.CancellationPaymentId = GetParam(Params, "CancellationPaymentId");
        Result.AffectedDocumentIds.AddRange(GetStringListParam(Params, "AffectedDocumentIds"));
        return Result;
    }

    // ● public
    /// <summary>
    /// Converts this instance to a parameter dictionary.
    /// </summary>
    /// <returns>The parameter dictionary.</returns>
    public IDictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>()
        {
            ["ModuleName"] = ModuleName,
            ["DocumentId"] = DocumentId,
            ["SourceId"] = SourceId,
            ["CancelsTradeId"] = CancelsTradeId,
            ["CancelledByTradeId"] = CancelledByTradeId,
            ["CancelsStockTradeId"] = CancelsStockTradeId,
            ["CancelledByStockTradeId"] = CancelledByStockTradeId,
            ["CancelledPaymentId"] = CancelledPaymentId,
            ["CancellationPaymentId"] = CancellationPaymentId,
            ["AffectedDocumentIds"] = AffectedDocumentIds,
        };
    }
    /// <summary>
    /// Returns true when the specified document id is affected by this notification.
    /// </summary>
    /// <param name="DocumentId">The document id to check.</param>
    /// <returns>True when the document is affected.</returns>
    public bool AffectsDocument(string DocumentId)
    {
        return !string.IsNullOrWhiteSpace(DocumentId)
               && (DocumentId.IsSameText(this.DocumentId)
                   || DocumentId.IsSameText(SourceId)
                   || DocumentId.IsSameText(CancelsTradeId)
                   || DocumentId.IsSameText(CancelledByTradeId)
                   || DocumentId.IsSameText(CancelsStockTradeId)
                   || DocumentId.IsSameText(CancelledByStockTradeId)
                   || DocumentId.IsSameText(CancelledPaymentId)
                   || DocumentId.IsSameText(CancellationPaymentId)
                   || AffectedDocumentIds.Any(Item => DocumentId.IsSameText(Item)));
    }

    // ● properties
    /// <summary>
    /// The posted document module name.
    /// </summary>
    public string ModuleName { get; set; } = "";
    /// <summary>
    /// The posted document id.
    /// </summary>
    public string DocumentId { get; set; } = "";
    /// <summary>
    /// The source document id, when the posted document was transformed from another document.
    /// </summary>
    public string SourceId { get; set; } = "";
    /// <summary>
    /// The cancelled document id, when the posted document cancels another document.
    /// </summary>
    public string CancelsTradeId { get; set; } = "";
    /// <summary>
    /// The cancellation document id, when present in the posted row.
    /// </summary>
    public string CancelledByTradeId { get; set; } = "";
    /// <summary>
    /// The cancelled Stock Transaction id, when the posted document cancels another Stock Transaction.
    /// </summary>
    public string CancelsStockTradeId { get; set; } = "";
    /// <summary>
    /// The Stock Transaction cancellation id, when present in the posted row.
    /// </summary>
    public string CancelledByStockTradeId { get; set; } = "";
    /// <summary>
    /// The cancelled payment id, when the posted payment cancels another payment.
    /// </summary>
    public string CancelledPaymentId { get; set; } = "";
    /// <summary>
    /// The payment cancellation id, when present in the posted row.
    /// </summary>
    public string CancellationPaymentId { get; set; } = "";
    /// <summary>
    /// Additional document ids affected by posting this document.
    /// </summary>
    public List<string> AffectedDocumentIds { get; } = [];
}
