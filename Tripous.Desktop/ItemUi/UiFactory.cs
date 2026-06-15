/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Creates reusable controls used by item page layouts and field editors.
/// </summary>
static public class UiFactory
{
    // ● common controls
    /// <summary>
    /// Adds a child control to a panel or assigns it as the content of a content control.
    /// </summary>
    static public void AddChild(Control ParentControl, Control Child)
    {
        if (ParentControl is Panel Panel)
        {
            Panel.Children.Add(Child);
            return;
        }
        if (ParentControl is ContentControl ContentControl)
        {
            ContentControl.Content = Child;
            return;
        }
        throw new ApplicationException("Invalid layout parent.");
    }
    /// <summary>
    /// Creates the root scroll viewer.
    /// </summary>
    static public ScrollViewer CreateScrollViewer()
    {
        return new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
    }
    /// <summary>
    /// Creates a vertical stack panel.
    /// </summary>
    static public StackPanel CreateStackPanel()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
    }
    /// <summary>
    /// Creates a horizontal stack panel.
    /// </summary>
    static public StackPanel CreateToolBarPanel()
    {
        StackPanel Result = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 32,
        };

        Result.Classes.Add("ToolBar");
        return Result;
    }
    /// <summary>
    /// Creates a border for a toolbar.
    /// </summary>
    static public Border CreateToolBarBorder()
    {
        Border Result = new();
        Result.Classes.Add("ToolbarContainer");
        return Result;
    }
    /// <summary>
    /// Creates an expander.
    /// </summary>
    static public Expander CreateExpander(Control ParentControl, string Caption)
    {
        Expander Result = new()
        {
            Header = Caption,
            IsExpanded = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 8)
        };
        AddChild(ParentControl, Result);
        return Result;
    }
    /// <summary>
    /// Creates a tab control.
    /// </summary>
    static public TabControl CreateTabControl()
    {
        return new TabControl
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 8, 0, 0)
        };
    }
    
    // ● item field controls
    /// <summary>
    /// Creates the label used next to a field editor.
    /// <para>Lookup labels ending in "Id" are displayed without that suffix.</para>
    /// </summary>
    static public TextBlock CreateFieldLabel(FieldDef Field)
    {
        string Title = Field.Title;
        if (Field.IsLookup && Title.EndsWith(" Id", StringComparison.OrdinalIgnoreCase))
            Title = Title.Substring(0, Title.Length - 3);
        bool IsRequired = Field.IsRequired && !Field.IsBoolean;
        if (IsRequired)
            Title += " *";

        TextBlock Result = new()
        {
            Text = Title,
            FontWeight = IsRequired ? FontWeight.SemiBold : FontWeight.Normal,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 6, 6)
        };
        ToolTip.SetTip(Result, Field.ToolTip);
        return Result;
    }
    /// <summary>
    /// Creates and binds the editor used for a large memo field.
    /// </summary>
    static public Control CreateLargeMemoEditor(FieldDef Field, ItemBinder Binder)
    {
        TextBox Result = new();
        Result.AcceptsReturn = true;
        Result.TextWrapping = TextWrapping.NoWrap;
        Result.FontFamily = new FontFamily("Consolas");
        Result.MinHeight = 280;
        Result.MaxHeight = 500;
        Result.HorizontalAlignment = HorizontalAlignment.Stretch;
        Result.Margin = new Thickness(0, 8, 0, 8);
        DataColumn DataColumn = Binder.TableInfo.Table.FindColumn(Field.Name);
        Binder.BindMemo(Result, Field.Name, DataColumn, Field);
        return Result;
    }
    /// <summary>
    /// Creates the placeholder control used for image fields.
    /// </summary>
    static public Control CreateImageControl(FieldDef Field, ItemBinder Binder)
    {
        StackPanel Result = new();
        TextBlock Label = new()
        {
            Text = Field.Title,
            Margin = new Thickness(0, 0, 0, 4)
        };
        Border Border = new()
        {
            Height = Ui.Settings.FormImageHeight,
            BorderThickness = new Thickness(1),
            Padding = new Thickness(4),
            Child = new TextBlock
            {
                Text = "No Image",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Result.Margin = new Thickness(0, 0, 0, 6);
        Result.Children.Add(Label);
        Result.Children.Add(Border);
        return Result;
    }
}
