/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Recalculates a document data module without committing it.
/// </summary>
[AjaxOperation("App.DocumentDataModule.Calculate")]
public class DocumentDataModuleCalculate: DataModuleAjaxOperation
{
    // ● private
    static string DescribeJsonValue(JsonElement Element)
    {
        string Text = Element.GetRawText();
        return Text.Length <= 160 ? Text : Text[..160] + "...";
    }
    static bool IsScalarJsonValue(JsonElement Element)
    {
        return Element.ValueKind == JsonValueKind.Null
               || Element.ValueKind == JsonValueKind.Undefined
               || Element.ValueKind == JsonValueKind.String
               || Element.ValueKind == JsonValueKind.Number
               || Element.ValueKind == JsonValueKind.True
               || Element.ValueKind == JsonValueKind.False;
    }
    static void ValidateDataModuleJsonText(string JsonText, DataModule Module)
    {
        if (string.IsNullOrWhiteSpace(JsonText) || Module == null)
            return;

        using JsonDocument Document = JsonDocument.Parse(JsonText);
        JsonElement Root = Document.RootElement;
        if (!Root.TryGetProperty("DataSet", out JsonElement DataSetElement)
            || !DataSetElement.TryGetProperty("Tables", out JsonElement TablesElement)
            || TablesElement.ValueKind != JsonValueKind.Array)
            return;

        foreach (JsonElement TableElement in TablesElement.EnumerateArray())
        {
            string TableName = TableElement.TryGetProperty("Name", out JsonElement TableNameElement) && TableNameElement.ValueKind == JsonValueKind.String
                ? TableNameElement.GetString()
                : string.Empty;
            MemTable Table = Module.FindTable(TableName);
            if (Table == null)
                continue;

            ValidateRows(TableElement, Table, "Rows");
            ValidateRows(TableElement, Table, "Deleted");
        }
    }
    static void ValidateRows(JsonElement TableElement, MemTable Table, string PropertyName)
    {
        if (!TableElement.TryGetProperty(PropertyName, out JsonElement RowsElement) || RowsElement.ValueKind != JsonValueKind.Array)
            return;

        int RowIndex = 0;
        foreach (JsonElement RowElement in RowsElement.EnumerateArray())
        {
            if (RowElement.TryGetProperty("Data", out JsonElement DataElement) && DataElement.ValueKind == JsonValueKind.Array)
            {
                int ColumnIndex = 0;
                foreach (JsonElement ValueElement in DataElement.EnumerateArray())
                {
                    if (ColumnIndex < Table.Columns.Count && !IsScalarJsonValue(ValueElement))
                    {
                        string ColumnName = Table.Columns[ColumnIndex].ColumnName;
                        Sys.Throw($"Invalid JSON value at {Table.TableName}.{ColumnName}, {PropertyName}[{RowIndex}], column index {ColumnIndex}. Expected scalar value, got {ValueElement.ValueKind}: {DescribeJsonValue(ValueElement)}");
                    }
                    ColumnIndex++;
                }
            }
            RowIndex++;
        }
    }
    JsonDataModule GetRequestPacket(AjaxRequest Request, DataModule Module)
    {
        string JsonText = GetStringParam(Request, "DataModuleJson");
        if (!string.IsNullOrWhiteSpace(JsonText))
        {
            ValidateDataModuleJsonText(JsonText, Module);
            return Json.Deserialize<JsonDataModule>(JsonText);
        }
        return GetDataModulePacket(Request);
    }
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetRequestPacket(Request, Module);
        string TableName = GetStringParam(Request, "TableName");
        string FieldName = GetStringParam(Request, "FieldName");
        string RowKey = GetStringParam(Request, "RowKey");

        if (Module is TradeDataModule TradeModule)
            return CreateDataModuleResponse(Request, TradeModule.JsonCalculate(Packet, TableName, FieldName));
        if (Module is StockTradeDataModule StockTradeModule)
            return CreateDataModuleResponse(Request, StockTradeModule.JsonCalculate(Packet, TableName, FieldName, RowKey));
        if (Module is StockCountDataModule StockCountModule)
            return CreateDataModuleResponse(Request, StockCountModule.JsonCalculate(Packet, TableName, FieldName, RowKey));
        if (Module is JournalEntryDataModule JournalEntryModule)
            return CreateDataModuleResponse(Request, JournalEntryModule.JsonCalculate(Packet, TableName, FieldName, RowKey));

        Sys.Throw($"DataModule is not a supported document calculation module: {Module.Name}");
        return null;
    }
}
