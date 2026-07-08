// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Exports group grid data rows to JSON.
/// </summary>
public class GroupGridJsonExporter: GroupGridExporter
{
    // ● private fields
    static readonly JsonSerializerOptions fJsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    // ● private methods
    object NormalizeValue(object Value)
    {
        return Value == DBNull.Value ? null : Value;
    }
    string GetColumnKey(GroupGridExportColumn Column, HashSet<string> UsedKeys)
    {
        string Result = string.IsNullOrWhiteSpace(Column.Name) ? Column.Header : Column.Name;
        if (string.IsNullOrWhiteSpace(Result))
            Result = "Column";

        string BaseName = Result;
        int Index = 2;
        while (!UsedKeys.Add(Result))
            Result = BaseName + Index++;

        return Result;
    }

    // ● public methods
    /// <inheritdoc />
    public override void Export(GroupGrid Grid, GroupGridExportSnapshot Snapshot, string FilePath)
    {
        Snapshot ??= Grid?.CreateExportSnapshot();
        if (Snapshot == null)
            Snapshot = new GroupGridExportSnapshot(Array.Empty<GroupGridExportColumn>(), Array.Empty<GroupGridExportRow>(), Array.Empty<GroupGridExportCell>());

        HashSet<string> UsedKeys = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<GroupGridExportColumn, string> ColumnKeys = Snapshot.Columns.ToDictionary(Column => Column, Column => GetColumnKey(Column, UsedKeys));
        object Payload = new
        {
            Columns = Snapshot.Columns.Select(Column => new
            {
                Column.Name,
                Column.Header,
                ValueType = Column.ValueType == null ? string.Empty : Column.ValueType.FullName,
            }).ToList(),
            Rows = Snapshot.GetDataRows().Select(Row =>
            {
                Dictionary<string, object> Values = new(StringComparer.OrdinalIgnoreCase);
                foreach (GroupGridExportCell Cell in Row.Cells)
                    Values[ColumnKeys[Cell.Column]] = NormalizeValue(Cell.Value);

                return Values;
            }).ToList(),
        };

        WriteText(FilePath, JsonSerializer.Serialize(Payload, fJsonOptions));
    }

    // ● properties
    /// <inheritdoc />
    public override string Name => "JSON";
    /// <inheritdoc />
    public override string DefaultExtension => "json";
}
