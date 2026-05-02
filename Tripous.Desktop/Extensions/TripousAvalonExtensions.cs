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

    static public TextAlignment TextAlignmentOf(this Type T)
    {
        DataFieldType DataType = T.DataFieldTypeOf();
        return TextAlignmentOf(DataType);
    }
    static public TextAlignment TextAlignmentOf(this DataFieldType DataType)
    {
        TextAlignment Result = TextAlignment.Left;
 
        if (DataType.IsNumeric() || DataType.IsDateTime() || DataType.IsDateStrict() || DataType.IsDateTimeStrict() || DataType == DataFieldType.Boolean)
            Result = TextAlignment.Center;
        return Result;
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

    // ● miscs
    static public Type ToType(this SimpleType SimpleType)
    {
        switch (SimpleType)
        {
            case SimpleType.String : return typeof(string);
            case SimpleType.Integer: return  typeof(int);
            case SimpleType.Boolean: return typeof(bool);
            case SimpleType.Double: return typeof(double);
            case SimpleType.Decimal: return typeof(decimal);
            case SimpleType.DateTime: return typeof(DateTime);
            case SimpleType.Text: return typeof(string);
            case SimpleType.Graphic: return typeof(byte[]);
            case SimpleType.Blob: return typeof(byte[]);
        }
        
        return  null;
    }
    /// <summary>
    /// Returns true if a value exists inside a flags enum mask.
    /// </summary>
    static public bool In<T>(this T Value, T Mask) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long m = Convert.ToInt64(Mask);
        return (v & m) == v;
    }
}

 