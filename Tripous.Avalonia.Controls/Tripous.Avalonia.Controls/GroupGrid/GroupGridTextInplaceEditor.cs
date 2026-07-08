// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a text box based in-place editor for a group grid cell.
/// </summary>
public class GroupGridTextInplaceEditor: GroupGridInplaceEditorBase
{
    // ● private fields
    readonly TextBox fTextBox;

    // ● private methods
    void TextBox_TextChanged(object Sender, TextChangedEventArgs Args)
    {
        RaiseValueChanged();
    }
    void TextBox_TemplateApplied(object Sender, TemplateAppliedEventArgs Args)
    {
        CleanEditorChrome();
    }
    void CleanEditorChrome()
    {
        fTextBox.BorderThickness = new Thickness(0);
        fTextBox.FocusAdorner = null;
        foreach (Border Border in fTextBox.GetVisualDescendants().OfType<Border>())
        {
            Border.BorderThickness = new Thickness(0);
            Border.CornerRadius = new CornerRadius(0);
        }
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridTextInplaceEditor"/> class.
    /// </summary>
    public GroupGridTextInplaceEditor()
    {
        fTextBox = new TextBox
        {
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            FocusAdorner = null,
            FontSize = 12,
            Padding = new Thickness(2, 0, 2, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            MinHeight = 0,
        };
        fTextBox.TemplateApplied += TextBox_TemplateApplied;
        fTextBox.TextChanged += TextBox_TextChanged;
        SetEditorControl(fTextBox);
        Dispatcher.UIThread.Post(CleanEditorChrome, DispatcherPriority.Background);
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
        fTextBox.TemplateApplied -= TextBox_TemplateApplied;
        fTextBox.TextChanged -= TextBox_TextChanged;
    }

    // ● properties
    /// <inheritdoc />
    public override object Value
    {
        get => fTextBox.Text ?? string.Empty;
        set => fTextBox.Text = Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
    }
    /// <summary>
    /// Gets or sets the text foreground brush.
    /// </summary>
    public IBrush Foreground
    {
        get => fTextBox.Foreground;
        set => fTextBox.Foreground = value;
    }
    /// <summary>
    /// Gets or sets the text alignment.
    /// </summary>
    public TextAlignment TextAlignment
    {
        get => fTextBox.TextAlignment;
        set => fTextBox.TextAlignment = value;
    }
}
