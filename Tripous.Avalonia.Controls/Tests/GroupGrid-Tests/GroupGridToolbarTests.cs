// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Grid.Tests;

/// <summary>
/// Tests group grid toolbar public API.
/// </summary>
public class GroupGridToolbarTests
{
    // ● tests
    /// <summary>
    /// Verifies default toolbar button state.
    /// </summary>
    [Fact]
    public void Constructor_CreatesDefaultToolbarButtons()
    {
        GroupGrid Grid = new();

        Assert.NotNull(Grid.GetToolButton("Insert"));
        Assert.NotNull(Grid.GetToolButton("Delete"));
        Assert.NotNull(Grid.GetToolButton("Edit"));
        Assert.True(Grid.IsInsertButtonVisible);
        Assert.True(Grid.IsDeleteButtonVisible);
        Assert.False(Grid.IsEditButtonVisible);
    }
    /// <summary>
    /// Verifies default toolbar button visibility properties.
    /// </summary>
    [Fact]
    public void DefaultButtonVisibilityProperties_ChangeDefaultButtonVisibility()
    {
        GroupGrid Grid = new();

        Grid.IsInsertButtonVisible = false;
        Grid.IsDeleteButtonVisible = false;
        Grid.IsEditButtonVisible = true;

        Assert.False(Grid.GetToolButton("Insert").IsVisible);
        Assert.False(Grid.GetToolButton("Delete").IsVisible);
        Assert.True(Grid.GetToolButton("Edit").IsVisible);
    }
    /// <summary>
    /// Verifies adding a custom toolbar button.
    /// </summary>
    [Fact]
    public void AddButton_WithRightAlignment_AddsCustomButton()
    {
        GroupGrid Grid = new();

        GroupGridToolButton Button = Grid.AddButton(GroupGridToolButtonAlignment.Right, "Refresh", "R", "Refresh data");

        Assert.Same(Button, Grid.GetToolButton("refresh"));
        Assert.Equal(GroupGridToolButtonAlignment.Right, Button.Alignment);
        Assert.Equal("R", Button.Text);
        Assert.Equal("Refresh data", Button.ToolTip);
    }
    /// <summary>
    /// Verifies inserting custom buttons around an existing button.
    /// </summary>
    [Fact]
    public void InsertButtonBeforeAndAfter_WithExistingButton_InsertInOrder()
    {
        GroupGrid Grid = new();

        GroupGridToolButton Before = Grid.InsertButtonBefore("Delete", "BeforeDelete", "BD");
        GroupGridToolButton After = Grid.InsertButtonAfter("Delete", "AfterDelete", "AD");

        int DeleteIndex = Grid.ToolButtons.IndexOf(Grid.GetToolButton("Delete"));
        Assert.NotNull(Before);
        Assert.NotNull(After);
        Assert.Equal(DeleteIndex - 1, Grid.ToolButtons.IndexOf(Before));
        Assert.Equal(DeleteIndex + 1, Grid.ToolButtons.IndexOf(After));
        Assert.Equal(Grid.GetToolButton("Delete").Alignment, Before.Alignment);
        Assert.Equal(Grid.GetToolButton("Delete").Alignment, After.Alignment);
    }
    /// <summary>
    /// Verifies insert helpers return null when the anchor button does not exist.
    /// </summary>
    [Fact]
    public void InsertButtonBeforeAndAfter_WithMissingButton_ReturnNull()
    {
        GroupGrid Grid = new();

        Assert.Null(Grid.InsertButtonBefore("Missing", "Before", "B"));
        Assert.Null(Grid.InsertButtonAfter("Missing", "After", "A"));
    }
}
