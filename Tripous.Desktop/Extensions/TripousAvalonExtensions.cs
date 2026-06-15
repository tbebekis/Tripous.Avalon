/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Provides extension methods for Avalonia controls.
/// </summary>
static public class TripousAvalonExtensions
{
    // ● control text
    /// <summary>
    /// Returns trimmed text from a text box.
    /// </summary>
    /// <param name="Box">The text box.</param>
    /// <returns>The trimmed text.</returns>
    static public string GetText(this TextBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;

    /// <summary>
    /// Returns the text box value as an integer.
    /// </summary>
    /// <param name="Box">The text box.</param>
    /// <param name="Default">The default value.</param>
    /// <returns>The integer value.</returns>
    static public int AsInt(this TextBox Box, int? Default = null)
    {
        string Text = GetText(Box);
        int value = Default.HasValue? Default.Value: 0;
        if (!int.TryParse(Box.Text, out value))
            value = 0;   // default

        return value;
    }
    /// <summary>
    /// Returns trimmed text from a text editor.
    /// </summary>
    /// <param name="Box">The text editor.</param>
    /// <returns>The trimmed text.</returns>
    static public string GetText(this TextEditor Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    /// <summary>
    /// Returns trimmed text from an auto-complete box.
    /// </summary>
    /// <param name="Box">The auto-complete box.</param>
    /// <returns>The trimmed text.</returns>
    static public string GetText(this AutoCompleteBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    /// <summary>
    /// Returns trimmed text from a combo box.
    /// </summary>
    /// <param name="Box">The combo box.</param>
    /// <returns>The trimmed text.</returns>
    static public string GetText(this ComboBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;

    /// <summary>
    /// Returns the checked value of a check box.
    /// </summary>
    /// <param name="Box">The check box.</param>
    /// <returns>True if the check box is checked; otherwise, false.</returns>
    static public bool GetValue(this CheckBox Box) => Box != null && Box.IsChecked.HasValue? Box.IsChecked.Value : false;

    /// <summary>
    /// Returns the text alignment for a type.
    /// </summary>
    /// <param name="T">The type.</param>
    /// <returns>The text alignment.</returns>
    static public TextAlignment GetTextAlignment(this Type T)
    {
        DataFieldType DataType = T.GetDataFieldType();
        return GetTextAlignment(DataType);
    }
    /// <summary>
    /// Returns the text alignment for a data field type.
    /// </summary>
    /// <param name="DataType">The data field type.</param>
    /// <returns>The text alignment.</returns>
    static public TextAlignment GetTextAlignment(this DataFieldType DataType)
    {
        TextAlignment Result = TextAlignment.Left;
 
        if (DataType.IsNumeric() || DataType.IsDateTime() || DataType.IsDateStrict() || DataType.IsDateTimeStrict() || DataType == DataFieldType.Boolean)
            Result = TextAlignment.Center;
        return Result;
    }

    /// <summary>
    /// Returns the text box text as trimmed non-empty lines.
    /// </summary>
    /// <param name="Box">The text box.</param>
    /// <returns>The text lines.</returns>
    static public string[] GetTextAsLines(this TextBox Box)
    {
        string[] Lines = Box.Text?
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(Line => Line.Trim())
            .Where(Line => !string.IsNullOrWhiteSpace(Line))
            .ToArray() ?? Array.Empty<string>();
        
        return Lines;
    }
    
    // ● Button
    /// <summary>
    /// Raises the click event of a button.
    /// </summary>
    /// <param name="Button">The button.</param>
    static public void PerformClick(this Button Button)
    {
        if (Button != null)
        {
            var clickArgs = new RoutedEventArgs(Button.ClickEvent);
            Button.RaiseEvent(clickArgs);
        }
    }
    
    // ● TabItem
    /// <summary>
    /// Handles middle-click closing for a tab item.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The pointer event arguments.</param>
    static public void TabItem_MiddleClick(object sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
        {
            if (sender is TabItem Page)
            {
                var Pager = Page.FindLogicalAncestorOfType<TabControl>();

                if (Pager != null)
                {
                    Pager.Items.Remove(Page);
                }
            }
        }
    }
    /// <summary>
    /// Closes a tab item by removing it from its parent tab control.
    /// </summary>
    /// <param name="Page">The tab item.</param>
    static public void Close(this TabItem Page)
    {
        if (Page != null)
        {
            var Pager = Page.FindLogicalAncestorOfType<TabControl>();

            if (Pager != null)
            {
                Pager.Items.Remove(Page);
            }
        }
    }
 
}

 
