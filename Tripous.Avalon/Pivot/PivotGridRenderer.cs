namespace Tripous.Avalon;

/// <summary>
/// Renders PivotData to an Avalonia DataGrid / ProDataGrid.
/// </summary>
public static class PivotGridRenderer
{
    /// <summary>
    /// Shows pivot data in a grid.
    /// </summary>
    public static void Show(DataGrid grid, PivotData pivotData)
    {
        if (grid == null)
            throw new ArgumentNullException(nameof(grid));
        if (pivotData == null)
            throw new ArgumentNullException(nameof(pivotData));

        grid.Columns.Clear();
        grid.AutoGenerateColumns = false;
        grid.IsReadOnly = true;

        Dictionary<string, int> indexes = CreateIndexes(pivotData.Columns);
        List<PivotGridRow> rows = pivotData.Rows
            .Select(x => new PivotGridRow(x, indexes))
            .ToList();

        foreach (PivotDataColumn col in pivotData.Columns)
            grid.Columns.Add(CreateColumn(col));

        grid.ItemsSource = rows;
    }

    private static Dictionary<string, int> CreateIndexes(List<PivotDataColumn> columns)
    {
        Dictionary<string, int> result = new(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < columns.Count; i++)
            result[columns[i].Key] = i;

        return result;
    }

    private static DataGridColumn CreateColumn(PivotDataColumn col)
    {
        return new DataGridTemplateColumn
        {
            Header = CreateHeaderControl(col.Caption),
            IsReadOnly = true,
            CellTemplate = CreateCellTemplate(col)
        };
    }

    private static Control CreateHeaderControl(string caption)
    {
        return new Border
        {
            Padding = new Thickness(6, 4, 6, 4),
            Child = new TextBlock
            {
                Text = caption,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }

    private static IDataTemplate CreateCellTemplate(PivotDataColumn col)
    {
        bool isValueColumn = col.Kind == PivotDataColumnKind.Value;

        return new FuncDataTemplate<object>((item, _) =>
        {
            PivotGridRow row = item as PivotGridRow;

            Border border = new Border
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Padding = isValueColumn
                    ? new Thickness(4, 0, 6, 0)
                    : new Thickness(6, 0, 6, 0)
            };

            TextBlock textBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = isValueColumn ? TextAlignment.Right : TextAlignment.Left
            };

            ApplyRowStyle(row, border, textBlock);

            Binding binding = new Binding($"[{col.Key}]");
            if (!string.IsNullOrWhiteSpace(col.Format))
                binding.StringFormat = col.Format;

            textBlock.Bind(TextBlock.TextProperty, binding);

            border.Child = textBlock;
            return border;
        });
    }

    private static void ApplyRowStyle(PivotGridRow row, Border border, TextBlock textBlock)
    {
        if (row == null)
            return;

        switch (row.RowType)
        {
            case PivotDataRowType.Subtotal:
                textBlock.FontWeight = FontWeight.Bold;
                border.Background = new SolidColorBrush(Color.Parse("#FFF7F7F7"));
                break;

            case PivotDataRowType.GrandTotal:
                textBlock.FontWeight = FontWeight.Bold;
                border.Background = new SolidColorBrush(Color.Parse("#FFE6E6E6"));
                break;
        }
    }
}