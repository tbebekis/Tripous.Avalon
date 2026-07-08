// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Grid.Tests;

/// <summary>
/// Tests group grid column formatting and parsing behavior.
/// </summary>
public class GroupGridColumnTests
{
    // ● private types
    class LookupItem
    {
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the item name.
        /// </summary>
        public string Name { get; set; }
    }

    // ● tests
    /// <summary>
    /// Verifies base column display formatting.
    /// </summary>
    [Fact]
    public void FormatValue_WithDisplayFormat_UsesCurrentCultureFormat()
    {
        GroupGridColumn Column = new() { DisplayFormat = "0.00" };

        Assert.Equal("12.35", Column.FormatValue(12.345m));
        Assert.Equal(string.Empty, Column.FormatValue(null));
    }
    /// <summary>
    /// Verifies numeric parsing and numeric alignment defaults.
    /// </summary>
    [Fact]
    public void NumberColumn_ParseValue_WithIntValueType_ReturnsInt()
    {
        GroupGridNumberColumn Column = new() { ValueType = typeof(int) };

        object Value = Column.ParseValue("42");

        Assert.IsType<int>(Value);
        Assert.Equal(42, Value);
        Assert.Equal(GroupGridCellHorizontalAlignment.Right, Column.HorizontalAlignment);
    }
    /// <summary>
    /// Verifies date edit formatting and parsing.
    /// </summary>
    [Fact]
    public void DateColumn_FormatEditValueAndParseValue_UseEditAndDateParsing()
    {
        GroupGridDateColumn Column = new() { EditFormat = "yyyy-MM-dd" };

        string Text = Column.FormatEditValue(new DateTime(2026, 7, 7));
        object Value = Column.ParseValue("2026-07-07");

        Assert.Equal("2026-07-07", Text);
        Assert.Equal(new DateTime(2026, 7, 7), Value);
    }
    /// <summary>
    /// Verifies checkbox formatting and parsing.
    /// </summary>
    [Fact]
    public void CheckBoxColumn_FormatValueAndParseValue_UseBooleanSemantics()
    {
        GroupGridCheckBoxColumn Column = new();

        Assert.Equal("x", Column.FormatValue(true));
        Assert.Equal(string.Empty, Column.FormatValue(false));
        Assert.Equal(false, Column.ParseValue(null));
        Assert.Equal(true, Column.ParseValue("true"));
    }
    /// <summary>
    /// Verifies lookup display formatting and value parsing.
    /// </summary>
    [Fact]
    public void LookupColumn_FormatValueAndParseValue_UseLookupMembers()
    {
        LookupItem Item = new() { Id = 2, Name = "Beta" };
        GroupGridLookupColumn Column = new()
        {
            ValueType = typeof(int),
            LookupItemsSource = new[]
            {
                new LookupItem { Id = 1, Name = "Alpha" },
                Item,
            },
            ValueMember = nameof(LookupItem.Id),
            DisplayMember = nameof(LookupItem.Name),
        };

        Assert.Equal("Beta", Column.FormatValue(2));
        Assert.Equal(2, Column.ParseValue(Item));
        Assert.Equal(3, Column.ParseValue("3"));
    }
}
