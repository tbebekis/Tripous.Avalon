// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a reusable text-box and button base for group grid drop-down in-place editors.
/// </summary>
public abstract class GroupGridDropDownInplaceEditorBase: GroupGridInplaceEditorBase
{
    // ● private methods
    void TextBox_TemplateApplied(object Sender, TemplateAppliedEventArgs Args)
    {
        CleanTextBoxChrome();
    }
    void TextBox_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        RaiseValueChanged();
    }
    void TextBox_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.F4 || (Args.Key == Key.Down && Args.KeyModifiers.HasFlag(KeyModifiers.Alt)))
        {
            ToggleDropDown();
            Args.Handled = true;
        }
    }
    void DropDownButton_PointerPressed(object Sender, PointerPressedEventArgs Args)
    {
        ToggleDropDown();
        FocusEditor();
        Args.Handled = true;
    }
    void CleanTextBoxChrome()
    {
        TextBox.BorderThickness = new Thickness(0);
        TextBox.FocusAdorner = null;
        foreach (Border Border in TextBox.GetVisualDescendants().OfType<Border>())
        {
            Border.BorderThickness = new Thickness(0);
            Border.CornerRadius = new CornerRadius(0);
        }
    }

    // ● protected methods
    /// <summary>
    /// Creates the editor panel.
    /// </summary>
    /// <returns>The editor panel.</returns>
    protected virtual Control CreateEditorPanel()
    {
        Grid Panel = new()
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Background = Brushes.Transparent,
        };
        Grid.SetColumn(TextBox, 0);
        Grid.SetColumn(DropDownButton, 1);
        Panel.Children.Add(TextBox);
        Panel.Children.Add(DropDownButton);
        return Panel;
    }
    /// <summary>
    /// Creates the text box.
    /// </summary>
    /// <returns>The text box.</returns>
    protected virtual TextBox CreateTextBox()
    {
        return new TextBox
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            FocusAdorner = null,
            IsReadOnly = IsTextReadOnly,
            FontSize = 12,
            Padding = new Thickness(2, 0, 2, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            MinHeight = 0,
        };
    }
    /// <summary>
    /// Creates the drop-down button.
    /// </summary>
    /// <returns>The drop-down button.</returns>
    protected virtual Button CreateDropDownButton()
    {
        return new Button
        {
            Content = DropDownButtonText,
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
    /// <summary>
    /// Requests opening the drop-down.
    /// </summary>
    protected virtual void OpenDropDown()
    {
        RaiseDropDownRequested();
    }
    /// <summary>
    /// Requests closing the drop-down.
    /// </summary>
    protected virtual void CloseDropDown()
    {
        DropDownHost?.CloseDropDown();
    }
    /// <summary>
    /// Requests toggling the drop-down.
    /// </summary>
    protected virtual void ToggleDropDown()
    {
        RaiseDropDownRequested();
    }
    /// <summary>
    /// Commits the currently selected drop-down value.
    /// </summary>
    protected virtual void CommitDropDownSelection()
    {
        DropDownHost?.CommitDropDownValue(GetDropDownSelectedValue());
    }
    /// <summary>
    /// Cancels the active drop-down.
    /// </summary>
    protected virtual void CancelDropDown()
    {
        DropDownHost?.CancelDropDown();
    }
    /// <summary>
    /// Returns the currently selected drop-down value.
    /// </summary>
    /// <returns>The selected value.</returns>
    protected abstract object GetDropDownSelectedValue();
    /// <summary>
    /// Called when the editor is clicked outside the inner button.
    /// </summary>
    /// <param name="Args">The pointer event arguments.</param>
    protected override void OnPointerPressed(PointerPressedEventArgs Args)
    {
        base.OnPointerPressed(Args);

        Point Point = Args.GetPosition(this);
        if (Point.X < Math.Max(0, Bounds.Width - DropDownButton.Bounds.Width - 4))
            return;

        ToggleDropDown();
        FocusEditor();
        Args.Handled = true;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridDropDownInplaceEditorBase"/> class.
    /// </summary>
    protected GroupGridDropDownInplaceEditorBase()
    {
        TextBox = CreateTextBox();
        DropDownButton = CreateDropDownButton();
        TextBox.TemplateApplied += TextBox_TemplateApplied;
        TextBox.TextChanged += TextBox_TextChanged;
        TextBox.KeyDown += TextBox_KeyDown;
        DropDownButton.AddHandler(InputElement.PointerPressedEvent, DropDownButton_PointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
        SetEditorControl(CreateEditorPanel());
        Dispatcher.UIThread.Post(CleanTextBoxChrome, DispatcherPriority.Background);
    }

    // ● public methods
    /// <inheritdoc />
    public override void FocusEditor()
    {
        TextBox.Focus();
    }
    /// <inheritdoc />
    public override void SelectAll()
    {
        TextBox.SelectAll();
    }
    /// <inheritdoc />
    public override void Cleanup()
    {
        DropDownButton.RemoveHandler(InputElement.PointerPressedEvent, DropDownButton_PointerPressed);
        TextBox.TemplateApplied -= TextBox_TemplateApplied;
        TextBox.TextChanged -= TextBox_TextChanged;
        TextBox.KeyDown -= TextBox_KeyDown;
    }

    // ● properties
    /// <summary>
    /// Gets the editor text box.
    /// </summary>
    protected TextBox TextBox { get; }
    /// <summary>
    /// Gets the drop-down button.
    /// </summary>
    protected Button DropDownButton { get; }
    /// <summary>
    /// Gets the host used by this drop-down editor.
    /// </summary>
    public IGroupGridDropDownEditorHost DropDownHost { get; internal set; }
    /// <inheritdoc />
    public override bool HasDropDown => true;
    /// <summary>
    /// Gets the drop-down button text.
    /// </summary>
    protected virtual string DropDownButtonText => "...";
    /// <summary>
    /// Gets a value indicating whether the text box is read-only.
    /// </summary>
    protected virtual bool IsTextReadOnly => false;
    /// <inheritdoc />
    public override object Value
    {
        get => TextBox.Text ?? string.Empty;
        set => TextBox.Text = Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
    }
}
