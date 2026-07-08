// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a date in-place editor with a text box and calendar drop-down button.
/// </summary>
public class GroupGridDateInplaceEditor: GroupGridInplaceEditorBase
{
    // ● private fields
    readonly TextBox fTextBox;
    readonly Button fDropDownButton;
    readonly GroupGridDateColumn fColumn;

    // ● private methods
    void TextBox_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        RaiseValueChanged();
    }
    void TextBox_TemplateApplied(object Sender, TemplateAppliedEventArgs Args)
    {
        CleanTextBoxChrome();
    }
    void DropDownButton_PointerPressed(object Sender, PointerPressedEventArgs Args)
    {
        RaiseDropDownRequested();
        FocusEditor();
        Args.Handled = true;
    }
    void Editor_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.F4)
        {
            RaiseDropDownRequested();
            Args.Handled = true;
        }
    }
    void CleanTextBoxChrome()
    {
        fTextBox.BorderThickness = new Thickness(0);
        fTextBox.FocusAdorner = null;
        foreach (Border Border in fTextBox.GetVisualDescendants().OfType<Border>())
        {
            Border.BorderThickness = new Thickness(0);
            Border.CornerRadius = new CornerRadius(0);
        }
    }
    DateTime? GetDateValue()
    {
        if (GroupGridDateTextNormalizer.TryParse(fTextBox.Text, CultureInfo.CurrentCulture, out DateTime Date))
            return Date;
        return null;
    }
    TextBox CreateTextBox()
    {
        return new TextBox
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            FocusAdorner = null,
            FontSize = 12,
            Padding = new Thickness(2, 0, 2, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            MinHeight = 0,
        };
    }
    Button CreateDropDownButton()
    {
        return new Button
        {
            Content = "...",
            Width = 24,
            FontSize = 10,
            Padding = new Thickness(0),
            BorderThickness = new Thickness(0),
            Background = Brushes.Transparent,
            Focusable = false,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
    }
    Control CreateEditorPanel()
    {
        Grid Panel = new()
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Background = Brushes.Transparent,
        };
        Grid.SetColumn(fTextBox, 0);
        Grid.SetColumn(fDropDownButton, 1);
        Panel.Children.Add(fTextBox);
        Panel.Children.Add(fDropDownButton);
        return Panel;
    }

    // ● protected methods
    /// <inheritdoc />
    protected override void OnPointerPressed(PointerPressedEventArgs Args)
    {
        base.OnPointerPressed(Args);

        Point Point = Args.GetPosition(this);
        if (Point.X < Math.Max(0, Bounds.Width - 22))
            return;

        RaiseDropDownRequested();
        FocusEditor();
        Args.Handled = true;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridDateInplaceEditor"/> class.
    /// </summary>
    /// <param name="Column">The date column.</param>
    public GroupGridDateInplaceEditor(GroupGridDateColumn Column)
    {
        fColumn = Column;
        fTextBox = CreateTextBox();
        fDropDownButton = CreateDropDownButton();
        fDropDownButton.AddHandler(InputElement.PointerPressedEvent, DropDownButton_PointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
        fTextBox.TemplateApplied += TextBox_TemplateApplied;
        fTextBox.TextChanged += TextBox_TextChanged;
        fTextBox.KeyDown += Editor_KeyDown;
        SetEditorControl(CreateEditorPanel());
        Dispatcher.UIThread.Post(CleanTextBoxChrome, DispatcherPriority.Background);
    }

    // ● public methods
    /// <inheritdoc />
    public override void FocusEditor()
    {
        fTextBox.Focus();
    }
    /// <inheritdoc />
    public override void SelectAll()
    {
        fTextBox.SelectAll();
    }
    /// <inheritdoc />
    public override void Cleanup()
    {
        fDropDownButton.RemoveHandler(InputElement.PointerPressedEvent, DropDownButton_PointerPressed);
        fTextBox.TemplateApplied -= TextBox_TemplateApplied;
        fTextBox.TextChanged -= TextBox_TextChanged;
        fTextBox.KeyDown -= Editor_KeyDown;
    }
    /// <inheritdoc />
    public override Control CreateDropDownControl()
    {
        DateTime? Date = GetDateValue();
        Calendar Calendar = new()
        {
            SelectionMode = CalendarSelectionMode.SingleDate,
            IsTodayHighlighted = true,
            SelectedDate = Date,
            DisplayDate = Date ?? DateTime.Today,
            Focusable = true,
        };
        return Calendar;
    }
    /// <inheritdoc />
    public override void DropDownOpened(Control DropDownControl)
    {
        DropDownControl?.Focus(NavigationMethod.Pointer, KeyModifiers.None);
    }
    /// <inheritdoc />
    public override void SelectDropDownItem(object Item)
    {
        Value = Item;
    }

    // ● properties
    /// <inheritdoc />
    public override object Value
    {
        get => fTextBox.Text ?? string.Empty;
        set => fTextBox.Text = fColumn == null ? Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty : fColumn.FormatEditValue(value);
    }
    /// <inheritdoc />
    public override bool HasDropDown => true;
    /// <inheritdoc />
    public override double DropDownHeight => 260;
    /// <inheritdoc />
    public override double DropDownWidth => 260;
    /// <summary>
    /// Gets or sets the text foreground brush.
    /// </summary>
    public IBrush Foreground
    {
        get => fTextBox.Foreground;
        set => fTextBox.Foreground = value;
    }
}
