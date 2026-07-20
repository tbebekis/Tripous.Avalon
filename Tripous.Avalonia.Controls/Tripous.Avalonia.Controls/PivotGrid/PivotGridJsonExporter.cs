// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Exports pivot grid matrix rows to JSON.
/// </summary>
public class PivotGridJsonExporter: PivotGridExporter
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
    PivotGridExportSnapshot GetSnapshot(PivotGrid Grid, PivotGridExportSnapshot Snapshot)
    {
        return Snapshot ?? Grid?.CreateExportSnapshot() ?? new PivotGridExportSnapshot(null, null, null, null, null);
    }

    // ● public methods
    /// <inheritdoc />
    public override void Export(PivotGrid Grid, PivotGridExportSnapshot Snapshot, string FilePath)
    {
        Snapshot = GetSnapshot(Grid, Snapshot);
        object Payload = new
        {
            RowFields = Snapshot.RowFields.Select(Field => new
            {
                Field.Name,
                Field.Header,
            }).ToList(),
            ColumnFields = Snapshot.ColumnFields.Select(Field => new
            {
                Field.Name,
                Field.Header,
            }).ToList(),
            Measures = Snapshot.Measures.Select(Measure => new
            {
                Measure.Name,
                Measure.Header,
                Measure.SourceFieldName,
                Measure.AggregateKind,
            }).ToList(),
            ValueColumns = Snapshot.ValueColumns.Select(Column => new
            {
                Column.ColumnIndex,
                Column.ColumnText,
                Column.MeasureIndex,
                MeasureName = Column.Measure?.Name ?? string.Empty,
                Column.Header,
                Column.IsTotal,
            }).ToList(),
            Rows = Snapshot.Rows.Select(Row => new
            {
                Row.RowIndex,
                Row.Level,
                Row.HeaderText,
                Row.RowTexts,
                Row.IsColumnTotal,
                Row.HasChildren,
                Row.IsExpanded,
                Values = Row.Cells.Select(Cell => new
                {
                    Cell.Column.ColumnIndex,
                    Cell.Column.MeasureIndex,
                    Cell.Text,
                    Value = NormalizeValue(Cell.Value),
                }).ToList(),
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
