// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a value-list filter dialog for a pivot grid source field.
/// </summary>
public class PivotGridFilterDialog: Window
{
    // ● private fields
    readonly List<PivotGridFilterValueItem> fItems;
    readonly TextBox edtSearch;
    readonly TextBlock lblCount;
    readonly StackPanel pnlItems;
    readonly Button btnSelectAll;
    readonly Button btnDeselectAll;
    readonly Button btnClear;
    readonly Button btnOk;
    readonly Button btnCancel;

    // ● private methods
    CheckBox CreateCheckBox(PivotGridFilterValueItem Item)
    {
        CheckBox Result = new()
        {
            Content = Item.Text,
            IsChecked = Item.IsChecked,
            Margin = new Thickness(0, 1),
        };
        Result.Click += (Sender, Args) =>
        {
            Item.IsChecked = Result.IsChecked == true;
            UpdateCount();
        };
        return Result;
    }
    bool MatchesSearch(PivotGridFilterValueItem Item)
    {
        string Text = edtSearch.Text ?? string.Empty;
        return string.IsNullOrWhiteSpace(Text)
               || (Item.Text ?? string.Empty).IndexOf(Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
    }
    void RebuildItems()
    {
        pnlItems.Children.Clear();
        foreach (PivotGridFilterValueItem Item in fItems.Where(MatchesSearch))
            pnlItems.Children.Add(CreateCheckBox(Item));

        UpdateCount();
    }
    void UpdateCount()
    {
        int SelectedCount = fItems.Count(Item => Item.IsChecked);
        lblCount.Text = $"Total: {fItems.Count}, Selected: {SelectedCount}";
    }
    void SetVisibleItemsChecked(bool Value)
    {
        foreach (PivotGridFilterValueItem Item in fItems.Where(MatchesSearch))
            Item.IsChecked = Value;

        RebuildItems();
    }
    void Search_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        RebuildItems();
    }
    void SelectAll_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetVisibleItemsChecked(true);
    }
    void DeselectAll_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        SetVisibleItemsChecked(false);
    }
    void Clear_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        edtSearch.Text = string.Empty;
    }
    void Ok_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Close(true);
    }
    void Cancel_Click(object Sender, Avalonia.Interactivity.RoutedEventArgs Args)
    {
        Close(false);
    }
    StackPanel CreateTopPanel()
    {
        return new StackPanel
        {
            Margin = new Thickness(12, 12, 12, 8),
            Spacing = 8,
            Children =
            {
                edtSearch,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        btnSelectAll,
                        btnDeselectAll,
                        btnClear,
                    },
                },
                lblCount,
            },
        };
    }
    StackPanel CreateButtonPanel()
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Thickness(12),
            Children =
            {
                btnCancel,
                btnOk,
            },
        };
    }
    Control CreateContent()
    {
        ScrollViewer ScrollViewer = new()
        {
            Content = pnlItems,
            VerticalScrollBarVisibility = Primitives.ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = Primitives.ScrollBarVisibility.Disabled,
            Margin = new Thickness(12, 0),
        };
        DockPanel.SetDock(ScrollViewer, Dock.Top);

        DockPanel Panel = new()
        {
            LastChildFill = true,
            Children =
            {
                CreateTopPanel(),
                CreateButtonPanel(),
                ScrollViewer,
            },
        };
        DockPanel.SetDock(Panel.Children[0], Dock.Top);
        DockPanel.SetDock(Panel.Children[1], Dock.Bottom);
        return Panel;
    }

    // ● protected methods
    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs Args)
    {
        base.OnKeyDown(Args);

        if (Args.Key == Key.Escape)
        {
            Close(false);
            Args.Handled = true;
        }
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PivotGridFilterDialog"/> class.
    /// </summary>
    /// <param name="FieldHeader">The field header text.</param>
    /// <param name="Items">The selectable filter values.</param>
    public PivotGridFilterDialog(string FieldHeader, IEnumerable<PivotGridFilterValueItem> Items)
    {
        fItems = Items == null ? new List<PivotGridFilterValueItem>() : Items.ToList();
        Title = "Filter " + (FieldHeader ?? string.Empty);
        Width = 360;
        Height = 460;
        MinWidth = 320;
        MinHeight = 360;
        CanMinimize = false;
        CanMaximize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        edtSearch = new TextBox
        {
            PlaceholderText = "Search",
        };
        lblCount = new TextBlock
        {
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };
        pnlItems = new StackPanel
        {
            Spacing = 2,
        };
        btnSelectAll = new Button
        {
            Content = "Select All",
            MinWidth = 82,
        };
        btnDeselectAll = new Button
        {
            Content = "Deselect All",
            MinWidth = 98,
        };
        btnClear = new Button
        {
            Content = "Clear",
            MinWidth = 70,
        };
        btnOk = new Button
        {
            Content = "OK",
            MinWidth = 80,
        };
        btnCancel = new Button
        {
            Content = "Cancel",
            MinWidth = 80,
        };

        edtSearch.TextChanged += Search_TextChanged;
        btnSelectAll.Click += SelectAll_Click;
        btnDeselectAll.Click += DeselectAll_Click;
        btnClear.Click += Clear_Click;
        btnOk.Click += Ok_Click;
        btnCancel.Click += Cancel_Click;
        Content = CreateContent();
        RebuildItems();
    }

    // ● properties
    /// <summary>
    /// Gets the selected values.
    /// </summary>
    public IReadOnlyList<object> SelectedValues => fItems.Where(Item => Item.IsChecked).Select(Item => Item.Value).ToList();
}
