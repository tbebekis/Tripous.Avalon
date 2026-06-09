/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

static public class TripousAvalonExtensions
{
    // ● control text
    static public string GetText(this TextBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;

    static public int AsInt(this TextBox Box, int? Default = null)
    {
        string Text = GetText(Box);
        int value = Default.HasValue? Default.Value: 0;
        if (!int.TryParse(Box.Text, out value))
            value = 0;   // default

        return value;
    }
    static public string GetText(this TextEditor Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this AutoCompleteBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;
    static public string GetText(this ComboBox Box) => Box != null && !string.IsNullOrWhiteSpace(Box.Text) ? Box.Text.Trim() : string.Empty;

    static public bool GetValue(this CheckBox Box) => Box != null && Box.IsChecked.HasValue? Box.IsChecked.Value : false;

    static public TextAlignment GetTextAlignment(this Type T)
    {
        DataFieldType DataType = T.GetDataFieldType();
        return GetTextAlignment(DataType);
    }
    static public TextAlignment GetTextAlignment(this DataFieldType DataType)
    {
        TextAlignment Result = TextAlignment.Left;
 
        if (DataType.IsNumeric() || DataType.IsDateTime() || DataType.IsDateStrict() || DataType.IsDateTimeStrict() || DataType == DataFieldType.Boolean)
            Result = TextAlignment.Center;
        return Result;
    }

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
    static public void PerformClick(this Button Button)
    {
        if (Button != null)
        {
            var clickArgs = new RoutedEventArgs(Button.ClickEvent);
            Button.RaiseEvent(clickArgs);
        }
    }
    
    // ● TabItem
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

 