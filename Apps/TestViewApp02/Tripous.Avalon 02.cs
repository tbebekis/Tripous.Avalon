using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Tripous.Data;

namespace Tripous.Avalon;

internal class GridViewInplaceEditorContext
{
    // ● constructors
    public GridViewInplaceEditorContext()
    {
    }

    // ● properties
    public DataGrid Grid { get; set; }
    public GridViewController Controller { get; set; }
    public GridViewData Data { get; set; }
    public GridViewRenderRow Row { get; set; }
    public GridViewColumnDef ColumnDef { get; set; }
    public int ColumnIndex { get; set; }
}
 
internal enum GridViewEditCommitReason
{
    None,
    Enter,
    Tab,
    Arrow,
    LostFocus,
    Pointer,
}
 
internal abstract class GridViewInplaceEditor
{
    // ● private fields
    private bool fCanceled;
    private bool fCompleted;
    private GridViewInplaceEditorContext fContext;

    // ● protected methods
    protected object GetCellValue()
    {
        if (Context?.Row?.Values == null)
            return null;

        if (Context.ColumnIndex < 0 || Context.ColumnIndex >= Context.Row.Values.Length)
            return null;

        return Context.Row.Values[Context.ColumnIndex];
    }
    protected Type GetCoreDataType()
    {
        if (Context?.ColumnDef?.DataType == null)
            return null;

        return Nullable.GetUnderlyingType(Context.ColumnDef.DataType) ?? Context.ColumnDef.DataType;
    }
    protected static IBrush GetRowBackground(GridViewRenderRow Row)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal))
            return Brushes.Gainsboro;

        return Brushes.Transparent;
    }
    protected void RestoreCellFocus()
    {
        if (Context?.Grid == null)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            if (Context?.Grid == null)
                return;

            Context.Grid.Focus();

            if (Context.ColumnIndex >= 0 && Context.ColumnIndex < Context.Grid.Columns.Count)
                Context.Grid.CurrentColumn = Context.Grid.Columns[Context.ColumnIndex];
        }, DispatcherPriority.Input);
    }
    protected void HandleFocusAfterCommit(GridViewEditCommitReason Reason)
    {
        if (Context?.Grid == null)
            return;

        switch (Reason)
        {
            case GridViewEditCommitReason.Enter:
            case GridViewEditCommitReason.Arrow:
            case GridViewEditCommitReason.LostFocus:
            case GridViewEditCommitReason.Pointer:
                RestoreCellFocus();
                break;
        }
    }
    protected void Complete(GridViewEditCommitReason Reason = GridViewEditCommitReason.None)
    {
        if (fCompleted || fCanceled)
            return;

        fCompleted = true;
        HandleFocusAfterCommit(Reason);
    }
    protected void Commit(object Value, GridViewEditCommitReason Reason = GridViewEditCommitReason.None)
    {
        if (fCompleted || fCanceled)
            return;

        fCompleted = true;

        if (Context?.Row == null || Context.Row.DataRow == null || Context.ColumnDef == null)
            return;

        if (Context.Controller != null)
        {
            Context.Controller.SetValue(Context.Row.DataRow, Context.ColumnDef.FieldName, Value);
            HandleFocusAfterCommit(Reason);
            return;
        }

        if (Context.Row.DataRow.SetValue(Context.ColumnDef.FieldName, Value) && Context.Data != null)
        {
            GridViewGridBinder.Refresh(Context.Grid);
            HandleFocusAfterCommit(Reason);
        }
    }
    protected void Cancel()
    {
        if (fCompleted)
            return;

        fCanceled = true;
        fCompleted = true;

        if (Context?.Grid != null)
            GridViewGridBinder.Refresh(Context.Grid);

        RestoreCellFocus();
    }

    // ● constructors
    protected GridViewInplaceEditor(GridViewInplaceEditorContext Context)
    {
        fContext = Context ?? throw new ArgumentNullException(nameof(Context));
    }

    // ● public methods
    public abstract Control Create();

    // ● properties
    public bool Canceled => fCanceled;
    public bool Completed => fCompleted;
    public GridViewInplaceEditorContext Context => fContext;
}

internal class GridViewTextInplaceEditor: GridViewInplaceEditor
{
    // ● private methods
    private static HorizontalAlignment GetHorizontalAlignment(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal) && Row.LabelColumnIndex == ColumnIndex)
            return HorizontalAlignment.Left;

        if (ColumnDef != null && (ColumnDef.IsNumeric || ColumnDef.IsDateTime))
            return HorizontalAlignment.Right;

        return HorizontalAlignment.Left;
    }
    private static string FormatValue(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        Type CoreType = ColumnDef?.DataType != null
            ? Nullable.GetUnderlyingType(ColumnDef.DataType) ?? ColumnDef.DataType
            : null;

        if (CoreType != null && CoreType.IsEnum)
        {
            try
            {
                object EnumValue = Value.GetType().IsEnum
                    ? Value
                    : Enum.ToObject(CoreType, Value);

                return EnumValue.ToString();
            }
            catch
            {
            }
        }

        return Convert.ToString(Value) ?? string.Empty;
    }

    // ● constructors
    public GridViewTextInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }

    // ● public methods
    public override Control Create()
    {
        string OriginalText = FormatValue(Context.Row, Context.ColumnDef, Context.ColumnIndex);

        TextBox Editor = new()
        {
            Text = OriginalText,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = GetHorizontalAlignment(Context.Row, Context.ColumnDef, Context.ColumnIndex),
            IsReadOnly = false,
        };

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.SelectAll();
                }
            }, DispatcherPriority.Input);
        };

        void CommitEdit(GridViewEditCommitReason Reason)
        {
            string NewText = Editor.Text ?? string.Empty;
            if (string.Equals(NewText, OriginalText, StringComparison.Ordinal))
            {
                Complete(Reason);
                return;
            }

            Commit(NewText, Reason);
        }

        Editor.LostFocus += (s, e) => CommitEdit(GridViewEditCommitReason.LostFocus);
        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    CommitEdit(GridViewEditCommitReason.Enter);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    e.Handled = true;
                    break;

                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Context.Row),
            Child = Editor,
        };

        return Result;
    }
}

internal class GridViewBoolInplaceEditor: GridViewInplaceEditor
{
    // ● private methods
    private static bool? GetEditorValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        if (Value is bool B)
            return B;

        try
        {
            return Convert.ToBoolean(Value);
        }
        catch
        {
            return null;
        }
    }

    // ● constructors
    public GridViewBoolInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }

    // ● public methods
    public override Control Create()
    {
        bool? OriginalValue = GetEditorValue(GetCellValue());

        CheckBox Editor = new()
        {
            IsChecked = OriginalValue,
            Margin = new Thickness(6, 1, 6, 1),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsThreeState = Context.ColumnDef?.DataType != typeof(bool),
        };

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                    Editor.Focus();
            }, DispatcherPriority.Input);
        };

        void CommitEdit(GridViewEditCommitReason Reason)
        {
            bool? NewValue = Editor.IsChecked;
            if (Nullable.Equals(NewValue, OriginalValue))
            {
                Complete(Reason);
                return;
            }

            if (Context.ColumnDef?.DataType == typeof(bool))
                Commit(NewValue == true, Reason);
            else
                Commit(NewValue, Reason);
        }

        Editor.IsCheckedChanged += (s, e) =>
        {
            if (!Canceled && !Completed)
                CommitEdit(GridViewEditCommitReason.Pointer);
        };

        Editor.LostFocus += (s, e) =>
        {
            if (!Canceled && !Completed)
                CommitEdit(GridViewEditCommitReason.LostFocus);
        };

        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    CommitEdit(GridViewEditCommitReason.Enter);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    e.Handled = true;
                    break;

                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Context.Row),
            Child = Editor,
        };

        return Result;
    }
}

internal class GridViewNumericInplaceEditor: GridViewInplaceEditor
{
    // ● private methods
    private string GetText(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        string EditFormat = Context?.ColumnDef?.EditFormat;
        if (!string.IsNullOrWhiteSpace(EditFormat) && Value is IFormattable Formattable)
            return Formattable.ToString(EditFormat, System.Globalization.CultureInfo.InvariantCulture);

        return Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }
    private bool TryGetTypedValue(string Text, out object Result)
    {
        Result = null;

        Type DataType = Context?.ColumnDef?.DataType;
        if (DataType == null)
            return false;

        Type CoreType = Nullable.GetUnderlyingType(DataType) ?? DataType;
        bool IsNullable = Context.ColumnDef.IsNullable; //Nullable.GetUnderlyingType(DataType) != null;

        if (string.IsNullOrWhiteSpace(Text))
        {
            if (IsNullable)
            {
                Result = null;
                return true;
            }

            return false;
        }

        try
        {
            if (CoreType == typeof(byte))
                Result = byte.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            else if (CoreType == typeof(short))
                Result = short.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            else if (CoreType == typeof(int))
                Result = int.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            else if (CoreType == typeof(long))
                Result = long.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            else if (CoreType == typeof(float))
                Result = float.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            else if (CoreType == typeof(double))
                Result = double.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            else if (CoreType == typeof(decimal))
                Result = decimal.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
            else
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    // ● constructors
    public GridViewNumericInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }

    // ● public methods
    public override Control Create()
    {
        string OriginalText = GetText(GetCellValue());

        TextBox Editor = new()
        {
            Text = OriginalText,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            IsReadOnly = false,
        };

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.SelectAll();
                }
            }, DispatcherPriority.Input);
        };

        void CommitEdit(GridViewEditCommitReason Reason)
        {
            string NewText = Editor.Text ?? string.Empty;
            if (string.Equals(NewText, OriginalText, StringComparison.Ordinal))
            {
                Complete(Reason);
                return;
            }

            if (!TryGetTypedValue(NewText, out object Value))
                return;

            Commit(Value, Reason);
        }

        Editor.LostFocus += (s, e) => CommitEdit(GridViewEditCommitReason.LostFocus);
        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    CommitEdit(GridViewEditCommitReason.Enter);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    e.Handled = true;
                    break;

                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Context.Row),
            Child = Editor,
        };

        return Result;
    }
}

internal class GridViewDateInplaceEditor: GridViewInplaceEditor
{
    // ● private methods
    private string GetText(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        string EditFormat = Context?.ColumnDef?.EditFormat;
        if (!string.IsNullOrWhiteSpace(EditFormat) && Value is IFormattable Formattable)
            return Formattable.ToString(EditFormat, System.Globalization.CultureInfo.InvariantCulture);

        return Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }
    private bool TryGetTypedValue(string Text, out object Result)
    {
        Result = null;

        Type DataType = Context?.ColumnDef?.DataType;
        if (DataType == null)
            return false;

        Type CoreType = Nullable.GetUnderlyingType(DataType) ?? DataType;
        bool IsNullable = Context.ColumnDef.IsNullable; //Nullable.GetUnderlyingType(DataType) != null;

        if (string.IsNullOrWhiteSpace(Text))
        {
            if (IsNullable)
            {
                Result = null;
                return true;
            }

            return false;
        }

        try
        {
            if (CoreType == typeof(DateTime))
            {
                Result = DateTime.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }

            if (CoreType == typeof(DateTimeOffset))
            {
                Result = DateTimeOffset.Parse(Text, System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    // ● constructors
    public GridViewDateInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }

    // ● public methods
    public override Control Create()
    {
        string OriginalText = GetText(GetCellValue());
 
        TextBox Editor = new()
        {
            Text = OriginalText,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Right,
            IsReadOnly = false,
        };

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.SelectAll();
                }
            }, DispatcherPriority.Input);
        };

        void CommitEdit(GridViewEditCommitReason Reason)
        {
            string NewText = Editor.Text ?? string.Empty;
            if (string.Equals(NewText, OriginalText, StringComparison.Ordinal))
            {
                Complete(Reason);
                return;
            }

            if (!TryGetTypedValue(NewText, out object Value))
                return;

            Commit(Value, Reason);
        }

        Editor.LostFocus += (s, e) => CommitEdit(GridViewEditCommitReason.LostFocus);
        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    CommitEdit(GridViewEditCommitReason.Enter);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    e.Handled = true;
                    break;

                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Context.Row),
            Child = Editor,
        };

        return Result;
    }
}

static internal class GridViewLookupHelper
{
    // ● private methods
    static private bool AreEqual(object A, object B)
    {
        if (A == null && B == null)
            return true;

        if (A == null || B == null)
            return false;

        if (Equals(A, B))
            return true;

        try
        {
            string SA = Convert.ToString(A, CultureInfo.InvariantCulture);
            string SB = Convert.ToString(B, CultureInfo.InvariantCulture);
            return string.Equals(SA, SB, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
        }

        return false;
    }
    static private object GetMemberValue(object Item, string MemberName)
    {
        if (Item == null)
            return null;

        if (string.IsNullOrWhiteSpace(MemberName))
            return Item;

        return GridViewEngine.GetValue(Item, MemberName);
    }

    // ● static public methods
    static public IEnumerable GetItems(GridViewColumnDef ColumnDef)
    {
        if (ColumnDef == null)
            return null;

        if (ColumnDef.LookupItemsSource != null)
            return ColumnDef.LookupItemsSource;

        if (ColumnDef.LookupItemsProvider != null)
            return ColumnDef.LookupItemsProvider();

        return null;
    }
    static public object GetItemValue(object Item, GridViewColumnDef ColumnDef)
    {
        if (Item == null || ColumnDef == null)
            return null;

        return GetMemberValue(Item, ColumnDef.ValueMember);
    }
    static public string GetItemDisplayText(object Item, GridViewColumnDef ColumnDef)
    {
        if (Item == null || ColumnDef == null)
            return string.Empty;

        object Value = GetMemberValue(Item, ColumnDef.DisplayMember);
        return Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
    static public object FindItemByValue(GridViewColumnDef ColumnDef, object Value)
    {
        IEnumerable Items = GetItems(ColumnDef);
        if (Items == null)
            return null;

        foreach (object Item in Items)
        {
            object ItemValue = GetItemValue(Item, ColumnDef);
            if (AreEqual(ItemValue, Value))
                return Item;
        }

        return null;
    }
    static public string GetDisplayTextByValue(GridViewColumnDef ColumnDef, object Value)
    {
        if (ColumnDef == null)
            return string.Empty;

        object Item = FindItemByValue(ColumnDef, Value);
        if (Item != null)
            return GetItemDisplayText(Item, ColumnDef);

        return Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
}

/*
internal class GridViewLookupInplaceEditor: GridViewInplaceEditor
{
    private IEnumerable GetItems()
    {
        List<object> Result = new();

        if (Context.ColumnDef != null && Context.ColumnDef.IsNullable)
            Result.Add(null);

        IEnumerable Source = GridViewLookupHelper.GetItems(Context.ColumnDef);
        if (Source != null)
        {
            foreach (object Item in Source)
                Result.Add(Item);
        }

        return Result;
    }
    
    // ● constructors
    public GridViewLookupInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }

    // ● public methods
    public override Control Create()
    {
        IEnumerable Items = GetItems(); // IEnumerable Items = GridViewLookupHelper.GetItems(Context.ColumnDef);
        object OriginalValue = GetCellValue();
        object OriginalItem = GridViewLookupHelper.FindItemByValue(Context.ColumnDef, OriginalValue);

        ComboBox Editor = new()
        {
            ItemsSource = Items,
            SelectedItem = OriginalItem,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            IsEnabled = true,
        };

        Editor.ItemTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBlock Result = new()
            {
                Text = Item != null ? GridViewLookupHelper.GetItemDisplayText(Item, Context.ColumnDef) : string.Empty,
                Margin = new Thickness(4, 2, 4, 2),
                VerticalAlignment = VerticalAlignment.Center,
            };

            return Result;
        });

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.IsDropDownOpen = true;
                }
            }, DispatcherPriority.Input);
        };

        void CommitEdit(GridViewEditCommitReason Reason)
        {
            object Item = Editor.SelectedItem;
            object NewValue = GridViewLookupHelper.GetItemValue(Item, Context.ColumnDef);

            if (Equals(NewValue, OriginalValue))
            {
                Complete(Reason);
                return;
            }

            Commit(NewValue, Reason);
        }

        Editor.DropDownClosed += (s, e) =>
        {
            if (!Canceled && !Completed)
                CommitEdit(GridViewEditCommitReason.Pointer);
        };

        Editor.LostFocus += (s, e) =>
        {
            if (!Canceled && !Completed && !Editor.IsDropDownOpen)
                CommitEdit(GridViewEditCommitReason.LostFocus);
        };

        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            if (Editor.IsDropDownOpen)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Editor.IsDropDownOpen = false;
                        e.Handled = true;
                        break;

                    case Key.Enter:
                        CommitEdit(GridViewEditCommitReason.Enter);
                        e.Handled = true;
                        break;

                    case Key.Tab:
                        CommitEdit(GridViewEditCommitReason.Arrow);
                        e.Handled = true;
                        break;
                }

                return;
            }

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    CommitEdit(GridViewEditCommitReason.Enter);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    e.Handled = true;
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Context.Row),
            Child = Editor,
        };

        return Result;
    }
}

internal class GridViewEnumInplaceEditor: GridViewInplaceEditor
{
    // ● private methods
    private object GetEditorValue(object Value)
    {
        Type EnumType = GetCoreDataType();

        if (EnumType == null || !EnumType.IsEnum || Value == null || Value == DBNull.Value)
            return null;

        if (Value.GetType().IsEnum)
            return Value;

        try
        {
            if (Value is string S && !string.IsNullOrWhiteSpace(S))
                return Enum.Parse(EnumType, S, true);

            return Enum.ToObject(EnumType, Value);
        }
        catch
        {
            return null;
        }
    }
    private IEnumerable GetItems()
    {
        List<object> Result = new();

        if (Context.ColumnDef != null && Context.ColumnDef.IsNullable)
            Result.Add(null);

        Type EnumType = GetCoreDataType();
        if (EnumType != null)
        {
            foreach (object Item in Enum.GetValues(EnumType))
                Result.Add(Item);
        }

        return Result;
    }
    
    // ● constructors
    public GridViewEnumInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }

    // ● public methods
    public override Control Create()
    {
        Type EnumType = GetCoreDataType();
        object OriginalValue = GetEditorValue(GetCellValue());

        ComboBox Editor = new()
        {
            ItemsSource = GetItems(), //Enum.GetValues(EnumType),
            SelectedItem = OriginalValue,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            IsEnabled = true,
        };
        
        Editor.ItemTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBlock Result = new()
            {
                Text = Item != null ? Item.ToString() : string.Empty,
                Margin = new Thickness(4, 2, 4, 2),
                VerticalAlignment = VerticalAlignment.Center,
            };

            return Result;
        });

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.IsDropDownOpen = true;
                }
            }, DispatcherPriority.Input);
        };

        void CommitEdit(GridViewEditCommitReason Reason)
        {
            object NewValue = Editor.SelectedItem;
            if (Equals(NewValue, OriginalValue))
            {
                Complete(Reason);
                return;
            }

            Commit(NewValue, Reason);
        }

        Editor.DropDownClosed += (s, e) =>
        {
            if (!Canceled && !Completed)
                CommitEdit(GridViewEditCommitReason.Pointer);
        };

        Editor.LostFocus += (s, e) =>
        {
            if (!Canceled && !Completed && !Editor.IsDropDownOpen)
                CommitEdit(GridViewEditCommitReason.LostFocus);
        };

        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            if (Editor.IsDropDownOpen)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Editor.IsDropDownOpen = false;
                        e.Handled = true;
                        break;

                    case Key.Enter:
                        CommitEdit(GridViewEditCommitReason.Enter);
                        e.Handled = true;
                        break;

                    case Key.Tab:
                        CommitEdit(GridViewEditCommitReason.Arrow);
                        e.Handled = true;
                        break;

                    case Key.Left:
                    case Key.Right:
                    case Key.Up:
                    case Key.Down:
                        break;
                }

                return;
            }

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    CommitEdit(GridViewEditCommitReason.Enter);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    e.Handled = true;
                    break;

                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Context.Row),
            Child = Editor,
        };

        return Result;
    }
}
*/

internal abstract class GridViewComboBoxInplaceEditor: GridViewInplaceEditor
{
    // ● protected methods
    protected abstract IEnumerable GetItems();
    protected abstract object GetOriginalItem();
    protected abstract string GetItemText(object Item);
    protected abstract object GetCommittedValue(object Item);

    // ● constructors
    protected GridViewComboBoxInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }

    // ● public methods
    public override Control Create()
    {
        IEnumerable Items = GetItems();
        object OriginalItem = GetOriginalItem();

        ComboBox Editor = new()
        {
            ItemsSource = Items,
            SelectedItem = OriginalItem,
            Margin = new Thickness(4, 1, 4, 1),
            VerticalAlignment = VerticalAlignment.Center,
            IsEnabled = true,
        };

        Editor.ItemTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBlock Result = new()
            {
                Text = GetItemText(Item),
                Margin = new Thickness(4, 2, 4, 2),
                VerticalAlignment = VerticalAlignment.Center,
            };

            return Result;
        });

        Editor.AttachedToVisualTree += (s, e) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (Editor.IsVisible && Editor.IsEffectivelyEnabled)
                {
                    Editor.Focus();
                    Editor.IsDropDownOpen = true;
                }
            }, DispatcherPriority.Input);
        };

        void CommitEdit(GridViewEditCommitReason Reason)
        {
            object NewItem = Editor.SelectedItem;

            object OriginalValue = GetCommittedValue(OriginalItem);
            object NewValue = GetCommittedValue(NewItem);

            if (Equals(NewValue, OriginalValue))
            {
                Complete(Reason);
                return;
            }

            Commit(NewValue, Reason);
        }

        Editor.DropDownClosed += (s, e) =>
        {
            if (!Canceled && !Completed)
                CommitEdit(GridViewEditCommitReason.Pointer);
        };

        Editor.LostFocus += (s, e) =>
        {
            if (!Canceled && !Completed && !Editor.IsDropDownOpen)
                CommitEdit(GridViewEditCommitReason.LostFocus);
        };

        Editor.KeyDown += (s, e) =>
        {
            if (e.Handled)
                return;

            if (Editor.IsDropDownOpen)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        Editor.IsDropDownOpen = false;
                        e.Handled = true;
                        break;

                    case Key.Enter:
                        CommitEdit(GridViewEditCommitReason.Enter);
                        e.Handled = true;
                        break;

                    case Key.Tab:
                        CommitEdit(GridViewEditCommitReason.Arrow);
                        e.Handled = true;
                        break;
                }

                return;
            }

            switch (e.Key)
            {
                case Key.Escape:
                    Cancel();
                    e.Handled = true;
                    break;

                case Key.Enter:
                    CommitEdit(GridViewEditCommitReason.Enter);
                    e.Handled = true;
                    break;

                case Key.Tab:
                    CommitEdit(GridViewEditCommitReason.Arrow);
                    e.Handled = true;
                    break;
            }
        };

        Border Result = new()
        {
            Background = GetRowBackground(Context.Row),
            Child = Editor,
        };

        return Result;
    }
}

internal class GridViewEnumInplaceEditor: GridViewComboBoxInplaceEditor
{
    // ● private methods
    private object GetEditorValue(object Value)
    {
        Type EnumType = GetCoreDataType();

        if (EnumType == null || !EnumType.IsEnum || Value == null || Value == DBNull.Value)
            return null;

        if (Value.GetType().IsEnum)
            return Value;

        try
        {
            if (Value is string S && !string.IsNullOrWhiteSpace(S))
                return Enum.Parse(EnumType, S, true);

            return Enum.ToObject(EnumType, Value);
        }
        catch
        {
            return null;
        }
    }

    // ● protected methods
    protected override IEnumerable GetItems()
    {
        List<object> Result = new();

        if (Context.ColumnDef != null && Context.ColumnDef.IsNullable)
            Result.Add(null);

        Type EnumType = GetCoreDataType();
        if (EnumType != null)
        {
            foreach (object Item in Enum.GetValues(EnumType))
                Result.Add(Item);
        }

        return Result;
    }
    protected override object GetOriginalItem()
    {
        return GetEditorValue(GetCellValue());
    }
    protected override string GetItemText(object Item)
    {
        return Item != null ? Item.ToString() : string.Empty;
    }
    protected override object GetCommittedValue(object Item)
    {
        return Item;
    }

    // ● constructors
    public GridViewEnumInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }
}

internal class GridViewLookupInplaceEditor: GridViewComboBoxInplaceEditor
{
    // ● protected methods
    protected override IEnumerable GetItems()
    {
        List<object> Result = new();

        if (Context.ColumnDef != null && Context.ColumnDef.IsNullable)
            Result.Add(null);

        IEnumerable Source = GridViewLookupHelper.GetItems(Context.ColumnDef);
        if (Source != null)
        {
            foreach (object Item in Source)
                Result.Add(Item);
        }

        return Result;
    }
    protected override object GetOriginalItem()
    {
        object OriginalValue = GetCellValue();
        return GridViewLookupHelper.FindItemByValue(Context.ColumnDef, OriginalValue);
    }
    protected override string GetItemText(object Item)
    {
        return Item != null
            ? GridViewLookupHelper.GetItemDisplayText(Item, Context.ColumnDef)
            : string.Empty;
    }
    protected override object GetCommittedValue(object Item)
    {
        return GridViewLookupHelper.GetItemValue(Item, Context.ColumnDef);
    }

    // ● constructors
    public GridViewLookupInplaceEditor(GridViewInplaceEditorContext Context)
        : base(Context)
    {
    }
}

static internal class GridViewInplaceEditorFactory
{
    // ● static public methods
    static public GridViewInplaceEditor Create(GridViewInplaceEditorContext Context)
    {
        if (Context == null)
            throw new ArgumentNullException(nameof(Context));

        Type CoreType = Context.ColumnDef?.DataType != null
            ? Nullable.GetUnderlyingType(Context.ColumnDef.DataType) ?? Context.ColumnDef.DataType
            : null;
        
        if (Context.ColumnDef != null && Context.ColumnDef.IsLookup)
            return new GridViewLookupInplaceEditor(Context);

        if (CoreType == typeof(bool))
            return new GridViewBoolInplaceEditor(Context);

        if (CoreType == typeof(byte) || 
            CoreType == typeof(short) || 
            CoreType == typeof(int) || 
            CoreType == typeof(long) || 
            CoreType == typeof(float) || 
            CoreType == typeof(double) || 
            CoreType == typeof(decimal))
            return new GridViewNumericInplaceEditor(Context);

        if (CoreType == typeof(DateTime) || CoreType == typeof(DateTimeOffset))
            return new GridViewDateInplaceEditor(Context);

        if (CoreType != null && CoreType.IsEnum)
            return new GridViewEnumInplaceEditor(Context);

        return new GridViewTextInplaceEditor(Context);
    }
}
 
/*
GRID VIEW NAVIGATION RULES
==========================

DISPLAY MODE
------------

General
- Navigation is data-row centric.
- Group, footer and grand total rows are skipped by cell navigation.
- Only data rows participate in cell-to-cell navigation.

Horizontal Navigation
- Left       → moves to previous editable cell in the same data row
- Right      → moves to next editable cell in the same data row
- Tab        → same as Right (data-entry forward navigation)
- Shift+Tab  → same as Left (data-entry backward navigation)

Vertical Navigation
- Up         → moves to previous visible row (including group/footer/grand total)
- Down       → moves to next visible row (including group/footer/grand total)

Tab Sequence (Data Entry Flow)
- Tab moves only across editable cells of data rows
- Group, footer and grand total rows are skipped
- If last editable cell of a data row:
    → moves to first editable cell of next data row
- If first editable cell and Shift+Tab:
    → moves to last editable cell of previous data row
- If no next/previous data row:
    → navigation stops

Editing
- Enter      → moves to next row (same column) [DataGrid default behavior]
- F2         → begins edit mode
- Space      → toggles boolean value (data rows only)
- Esc        → no action

EDIT MODE
---------

General
- Editors handle their own input and commit/cancel logic
- Navigator is not responsible for editor internals

Editing Behavior
- Enter      → commit and stay on the same cell
- Esc        → cancel and stay on the same cell
- Tab        → commit (navigation handled externally if any)
- Shift+Tab  → commit (navigation handled externally if any)

Arrow Keys
- Text editor: arrows move caret inside editor
- Enum editor: arrows control dropdown selection
- No forced grid navigation while editing

NON-DATA ROWS
-------------

Group Rows
- Not part of cell navigation
- Can be reached only via Up/Down
- Space      → expand/collapse group
- Enter      → no action
- Not editable

Footer / Grand Total Rows
- Not part of cell navigation
- Can be reached only via Up/Down
- No editing or actions

DESIGN PRINCIPLES
-----------------

- Clear separation between navigation and editing
- Navigator controls movement between cells
- Editors control value editing and commit lifecycle
- Data-entry flow (Tab) is independent from visual rows
- Predictable and stable behavior is preferred over complex automation
*/

internal class GridViewGridNavigator
{
    // ● private fields
    private GridViewController fController;
    private DataGrid fGrid;
    private GridViewRenderData fRenderData;
    private int fColumnIndex = -1;
    private int fRowIndex = -1;
    private object fRowItem;

    // ● private methods
    private bool IsValid()
    {
        return fGrid != null && fController != null && fRenderData != null;
    }
    private bool IsDataRow(object Item)
    {
        return Item is GridViewRenderRow Row && Row.IsData && Row.DataRow != null;
    }
    private bool IsValidColumnIndex(int ColumnIndex)
    {
        return fGrid != null && ColumnIndex >= 0 && ColumnIndex < fGrid.Columns.Count;
    }
    private object GetRowItem(int RowIndex)
    {
        if (fGrid == null || fGrid.ItemsSource is not IEnumerable List)
            return null;

        int Index = 0;
        foreach (object Item in List)
        {
            if (Index == RowIndex)
                return Item;

            Index++;
        }

        return null;
    }
    private int GetRowIndex(object RowItem)
    {
        if (fGrid == null || fGrid.ItemsSource is not IEnumerable List)
            return -1;

        int Index = 0;
        foreach (object Item in List)
        {
            if (ReferenceEquals(Item, RowItem))
                return Index;

            Index++;
        }

        return -1;
    }
    private bool IsEditableColumn(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null || Row.DataRow == null)
            return false;

        if (!IsValidColumnIndex(ColumnIndex))
            return false;

        if (fRenderData.Columns == null || ColumnIndex < 0 || ColumnIndex >= fRenderData.Columns.Count)
            return false;

        GridViewColumnDef ColumnDef = fRenderData.Columns[ColumnIndex];
        if (ColumnDef == null)
            return false;

        return fController.CanEdit(Row.DataRow, ColumnDef.FieldName);
    }
    private bool TryGetCurrentCell()
    {
        fRowItem = fGrid != null ? fGrid.SelectedItem : null;
        fRowIndex = GetRowIndex(fRowItem);
        fColumnIndex = fGrid != null && fGrid.CurrentColumn != null
            ? fGrid.Columns.IndexOf(fGrid.CurrentColumn)
            : -1;

        return IsDataRow(fRowItem) && fRowIndex >= 0 && fColumnIndex >= 0;
    }
    private int FindNextEditableColumn(int RowIndex, int StartColumnIndex)
    {
        object RowItem = GetRowItem(RowIndex);
        if (RowItem is not GridViewRenderRow Row || !Row.IsData || Row.DataRow == null)
            return -1;

        for (int i = StartColumnIndex + 1; i < fGrid.Columns.Count; i++)
        {
            if (IsEditableColumn(Row, i))
                return i;
        }

        return -1;
    }
    private int FindPreviousEditableColumn(int RowIndex, int StartColumnIndex)
    {
        object RowItem = GetRowItem(RowIndex);
        if (RowItem is not GridViewRenderRow Row || !Row.IsData || Row.DataRow == null)
            return -1;

        for (int i = StartColumnIndex - 1; i >= 0; i--)
        {
            if (IsEditableColumn(Row, i))
                return i;
        }

        return -1;
    }
    private int FindFirstEditableColumn(int RowIndex)
    {
        object RowItem = GetRowItem(RowIndex);
        if (RowItem is not GridViewRenderRow Row || !Row.IsData || Row.DataRow == null)
            return -1;

        for (int i = 0; i < fGrid.Columns.Count; i++)
        {
            if (IsEditableColumn(Row, i))
                return i;
        }

        return -1;
    }
    private int FindLastEditableColumn(int RowIndex)
    {
        object RowItem = GetRowItem(RowIndex);
        if (RowItem is not GridViewRenderRow Row || !Row.IsData || Row.DataRow == null)
            return -1;

        for (int i = fGrid.Columns.Count - 1; i >= 0; i--)
        {
            if (IsEditableColumn(Row, i))
                return i;
        }

        return -1;
    }
    private int FindNextDataRow(int StartRowIndex)
    {
        for (int i = StartRowIndex + 1; ; i++)
        {
            object RowItem = GetRowItem(i);
            if (RowItem == null)
                return -1;

            if (IsDataRow(RowItem))
                return i;
        }
    }
    private int FindPreviousDataRow(int StartRowIndex)
    {
        for (int i = StartRowIndex - 1; i >= 0; i--)
        {
            object RowItem = GetRowItem(i);
            if (IsDataRow(RowItem))
                return i;
        }

        return -1;
    }
    private bool TrySetCurrentCell(int RowIndex, int ColumnIndex)
    {
        object RowItem = GetRowItem(RowIndex);
        if (RowItem is not GridViewRenderRow Row || !Row.IsData || Row.DataRow == null)
            return false;

        int TargetColumnIndex = ColumnIndex;

        if (!IsEditableColumn(Row, TargetColumnIndex))
            TargetColumnIndex = FindFirstEditableColumn(RowIndex);

        if (!IsValidColumnIndex(TargetColumnIndex))
            return false;

        fGrid.Focus();
        fGrid.SelectedItem = RowItem;
        fGrid.CurrentColumn = fGrid.Columns[TargetColumnIndex];
        return true;
    }

    // ● constructors
    public GridViewGridNavigator(DataGrid Grid, GridViewController Controller, GridViewRenderData RenderData)
    {
        fGrid = Grid ?? throw new ArgumentNullException(nameof(Grid));
        fController = Controller ?? throw new ArgumentNullException(nameof(Controller));
        fRenderData = RenderData ?? throw new ArgumentNullException(nameof(RenderData));
    }

    // ● public methods
    public bool TryMoveLeft()
    {
        if (!IsValid() || !TryGetCurrentCell())
            return false;

        int TargetColumnIndex = FindPreviousEditableColumn(fRowIndex, fColumnIndex);
        if (TargetColumnIndex < 0)
            return false;

        return TrySetCurrentCell(fRowIndex, TargetColumnIndex);
    }
    public bool TryMoveRight()
    {
        if (!IsValid() || !TryGetCurrentCell())
            return false;

        int TargetColumnIndex = FindNextEditableColumn(fRowIndex, fColumnIndex);
        if (TargetColumnIndex < 0)
            return false;

        return TrySetCurrentCell(fRowIndex, TargetColumnIndex);
    }
    public bool TryMoveUp()
    {
        if (!IsValid() || !TryGetCurrentCell())
            return false;

        int TargetRowIndex = FindPreviousDataRow(fRowIndex);
        if (TargetRowIndex < 0)
            return false;

        return TrySetCurrentCell(TargetRowIndex, fColumnIndex);
    }
    public bool TryMoveDown()
    {
        if (!IsValid() || !TryGetCurrentCell())
            return false;

        int TargetRowIndex = FindNextDataRow(fRowIndex);
        if (TargetRowIndex < 0)
            return false;

        return TrySetCurrentCell(TargetRowIndex, fColumnIndex);
    }
    public bool TryMoveNextCell()
    {
        if (!IsValid() || !TryGetCurrentCell())
            return false;

        int TargetColumnIndex = FindNextEditableColumn(fRowIndex, fColumnIndex);
        if (TargetColumnIndex >= 0)
            return TrySetCurrentCell(fRowIndex, TargetColumnIndex);

        int TargetRowIndex = FindNextDataRow(fRowIndex);
        if (TargetRowIndex < 0)
            return false;

        TargetColumnIndex = FindFirstEditableColumn(TargetRowIndex);
        if (TargetColumnIndex < 0)
            return false;

        return TrySetCurrentCell(TargetRowIndex, TargetColumnIndex);
    }
    public bool TryMovePreviousCell()
    {
        if (!IsValid() || !TryGetCurrentCell())
            return false;

        int TargetColumnIndex = FindPreviousEditableColumn(fRowIndex, fColumnIndex);
        if (TargetColumnIndex >= 0)
            return TrySetCurrentCell(fRowIndex, TargetColumnIndex);

        int TargetRowIndex = FindPreviousDataRow(fRowIndex);
        if (TargetRowIndex < 0)
            return false;

        TargetColumnIndex = FindLastEditableColumn(TargetRowIndex);
        if (TargetColumnIndex < 0)
            return false;

        return TrySetCurrentCell(TargetRowIndex, TargetColumnIndex);
    }
    public bool TryRestoreCurrentCell()
    {
        if (!IsValid() || !TryGetCurrentCell())
            return false;

        return TrySetCurrentCell(fRowIndex, fColumnIndex);
    }

    // ● properties
    public GridViewController Controller => fController;
    public DataGrid Grid => fGrid;
    public GridViewRenderData RenderData => fRenderData;
}

internal class GridViewRenderRow
{
    // ● constructors
    public GridViewRenderRow()
    {
    }

    // ● public methods
    public override string ToString() => RowType.ToString();

    // ● properties
    public GridDataRow DataRow { get; set; }
    public int LabelColumnIndex { get; set; } = -1;
    public GridViewNode Node => DataRow != null ? DataRow.Node : null;
    public int Level => DataRow != null ? DataRow.Level : 0;
    public GridDataRowType RowType => DataRow != null ? DataRow.RowType : GridDataRowType.Data;
    public object[] Values { get; set; }
    public bool IsData => DataRow != null && DataRow.IsData;
    public bool IsGroup => DataRow != null && DataRow.IsGroup;
    public bool IsFooter => DataRow != null && DataRow.IsFooter;
    public bool IsGrandTotal => DataRow != null && DataRow.IsGrandTotal;
}

internal class GridViewRenderData
{
    // ● constructors
    public GridViewRenderData()
    {
    }

    // ● properties
    public List<GridViewColumnDef> Columns { get; set; } = new();
    public List<GridViewRenderRow> Rows { get; set; } = new();
}
 
internal class GridViewGridLink: IDisposable
{
    // ● private fields
    private GridViewController fController;
    private GridViewData fData;
    private bool fDisposed;
    private DataGrid fGrid;
    private bool fSyncingPosition;

    // ● private methods
    private void AttachData(GridViewData Data)
    {
        if (ReferenceEquals(fData, Data))
            return;

        DetachData();

        fData = Data;

        if (fData != null)
            fData.PositionChanged += Data_PositionChanged;
    }
    private void AttachGrid(DataGrid Grid)
    {
        if (Grid != null)
            Grid.SelectionChanged += Grid_SelectionChanged;
    }
    private void Controller_DataChanged(object Sender, GridViewDataChangedEventArgs e)
    {
        AttachData(e != null ? e.Data : null);
        Refresh();
    }
    private void Data_PositionChanged(object Sender, EventArgs e)
    {
        SyncDataToGrid();
    }
    private void DetachController()
    {
        if (fController != null)
            fController.DataChanged -= Controller_DataChanged;
    }
    private void DetachData()
    {
        if (fData != null)
            fData.PositionChanged -= Data_PositionChanged;

        fData = null;
    }
    private void DetachGrid(DataGrid Grid)
    {
        if (Grid != null)
            Grid.SelectionChanged -= Grid_SelectionChanged;
    }
    private int GetSelectedPosition()
    {
        return fGrid != null ? fGrid.SelectedIndex : -1;
    }
    private object GetSelectedRowItemByPosition(int Position)
    {
        if (fGrid == null || Position < 0 || fGrid.ItemsSource is not IEnumerable List)
            return null;

        int Index = 0;
        foreach (object Item in List)
        {
            if (Index == Position)
                return Item;

            Index++;
        }

        return null;
    }
    private void Grid_SelectionChanged(object Sender, SelectionChangedEventArgs e)
    {
        SyncGridToData();
    }
    private void OnBoundChanged()
    {
        AttachData(fController != null ? fController.Data : null);
        Refresh();
    }
    private void SyncDataToGrid()
    {
        if (fSyncingPosition || fGrid == null || fData == null)
            return;

        object SelectedItem = GetSelectedRowItemByPosition(fData.Position);

        if (!ReferenceEquals(fGrid.SelectedItem, SelectedItem))
        {
            try
            {
                fSyncingPosition = true;
                fGrid.SelectedItem = SelectedItem;
            }
            finally
            {
                fSyncingPosition = false;
            }
        }
    }
    private void SyncGridToData()
    {
        if (fSyncingPosition || fData == null || fGrid == null)
            return;

        int Position = GetSelectedPosition();

        try
        {
            fSyncingPosition = true;
            fData.MoveTo(Position);
        }
        finally
        {
            fSyncingPosition = false;
        }
    }

    // ● constructors
    public GridViewGridLink()
    {
    }
    public GridViewGridLink(DataGrid Grid, GridViewController Controller)
    {
        Bind(Grid, Controller);
    }

    // ● public methods
    public void Bind(DataGrid Grid, GridViewController Controller)
    {
        if (ReferenceEquals(fGrid, Grid) && ReferenceEquals(fController, Controller))
            return;

        DetachController();
        DetachData();
        DetachGrid(fGrid);

        fGrid = Grid;
        fController = Controller;

        AttachGrid(fGrid);

        if (fController != null)
            fController.DataChanged += Controller_DataChanged;

        OnBoundChanged();
    }
    public void Dispose()
    {
        if (!fDisposed)
        {
            DetachController();
            DetachData();
            DetachGrid(fGrid);

            fGrid = null;
            fController = null;
            fDisposed = true;
        }
    }
    public void Refresh()
    {
        if (fGrid == null)
            return;

        AttachData(fController != null ? fController.Data : null);

        try
        {
            fSyncingPosition = true;
            GridViewGridBinder.Apply(fGrid, fController);
        }
        finally
        {
            fSyncingPosition = false;
        }

        SyncDataToGrid();
    }

    // ● properties
    public GridViewController Controller => fController;
    public GridViewData Data => fData;
    public bool IsDisposed => fDisposed;
    public DataGrid Grid => fGrid;
}
 
static internal class GridViewRenderer
{
    // ● private methods
    static private List<GridViewColumnDef> GetDisplayColumns(GridViewDef Def)
    {
        List<GridViewColumnDef> GroupColumns = Def.GetGroupColumns().OrderBy(x => x.GroupIndex).ToList();
        List<GridViewColumnDef> VisibleColumns = Def.GetVisibleColumns().ToList();
        List<GridViewColumnDef> NonGroupColumns = VisibleColumns.Where(x => x.GroupIndex < 0).OrderBy(x => x.VisibleIndex).ToList();
        return GroupColumns.Concat(NonGroupColumns).ToList();
    }
    static private string GetFooterLabel(GridViewNode Node)
    {
        if (Node == null || Node.OwnerGroup == null || Node.OwnerGroup.IsRoot)
            return "Grand Total";

        return $"Total ({Node.OwnerGroup.FieldName} = {Node.OwnerGroup.Key})";
    }
    static private int GetGroupLabelColumnIndex(GridViewNode Node, int ColumnCount)
    {
        if (ColumnCount <= 0)
            return -1;

        return Math.Min(Math.Max(Node != null ? Node.Level : 0, 0), ColumnCount - 1);
    }
    static private object GetRowValue(GridDataRow Row, string FieldName)
    {
        return Row != null ? Row.GetValue(FieldName) : null;
    }

    // ● static public methods
    static public GridViewRenderData Render(GridViewData Data)
    {
        if (Data == null)
            throw new ArgumentNullException(nameof(Data));

        if (Data.ViewDef == null)
            throw new ArgumentNullException(nameof(Data.ViewDef));

        GridViewRenderData Result = new();
        List<GridViewColumnDef> Columns = GetDisplayColumns(Data.ViewDef);

        Result.Columns.AddRange(Columns);

        foreach (GridDataRow DataRow in Data.Rows)
        {
            GridViewRenderRow Row = new()
            {
                DataRow = DataRow,
                Values = new object[Columns.Count],
            };

            if (DataRow.IsGroup)
            {
                int ColumnIndex = GetGroupLabelColumnIndex(DataRow.Node, Columns.Count);
                Row.LabelColumnIndex = ColumnIndex;

                if (ColumnIndex >= 0)
                    Row.Values[ColumnIndex] = $"{DataRow.Node.FieldName} = {DataRow.Node.Key}";

                if (DataRow.Node != null && !DataRow.Node.IsExpanded)
                {
                    foreach (GridViewSummary Summary in DataRow.Node.Summaries)
                    {
                        int SummaryColumnIndex = Columns.FindIndex(x => string.Equals(x.FieldName, Summary.FieldName, StringComparison.OrdinalIgnoreCase));
                        if (SummaryColumnIndex >= 0 && SummaryColumnIndex != Row.LabelColumnIndex)
                            Row.Values[SummaryColumnIndex] = Summary.Value;
                    }
                }
            }
            else if (DataRow.IsFooter || DataRow.IsGrandTotal)
            {
                if (Columns.Count > 0)
                {
                    int LabelColumnIndex = 0;

                    if (DataRow.IsFooter && DataRow.Node?.OwnerGroup != null && !DataRow.Node.OwnerGroup.IsRoot)
                    {
                        LabelColumnIndex = Columns.FindIndex(x =>
                            string.Equals(x.FieldName, DataRow.Node.OwnerGroup.FieldName, StringComparison.OrdinalIgnoreCase));

                        if (LabelColumnIndex < 0)
                            LabelColumnIndex = 0;
                    }

                    Row.LabelColumnIndex = LabelColumnIndex;
                    Row.Values[LabelColumnIndex] = GetFooterLabel(DataRow.Node);
                }

                if (DataRow.Node != null)
                {
                    foreach (GridViewSummary Summary in DataRow.Node.Summaries)
                    {
                        int ColumnIndex = Columns.FindIndex(x => string.Equals(x.FieldName, Summary.FieldName, StringComparison.OrdinalIgnoreCase));
                        if (ColumnIndex >= 0)
                            Row.Values[ColumnIndex] = Summary.Value;
                    }
                }
            }
            else if (DataRow.IsData)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    GridViewColumnDef ColumnDef = Columns[i];

                    if (!Data.ViewDef.ShowGroupColumnsAsDataColumns && ColumnDef.GroupIndex >= 0)
                        Row.Values[i] = null;
                    else
                        Row.Values[i] = GetRowValue(DataRow, ColumnDef.FieldName);
                }
            }

            Result.Rows.Add(Row);
        }

        return Result;
    }
}

static public class GridViewGridBinder
{
    // ● private fields
    static private ConditionalWeakTable<DataGrid, GridViewGridLink> fLinks = new();
    static private ConditionalWeakTable<DataGrid, GridState> fStates = new();

    // ● private methods
    static private Type GetCoreDataType(GridViewColumnDef ColumnDef)
    {
        if (ColumnDef?.DataType == null)
            return null;

        return Nullable.GetUnderlyingType(ColumnDef.DataType) ?? ColumnDef.DataType;
    }
    static private void AttachEditHandlers(DataGrid Grid, GridState State)
    {
        if (Grid == null || State == null)
            return;

        if (State.EditHandlersAttached)
            return;

        Grid.KeyDown += Grid_KeyDown;
        State.EditHandlersAttached = true;
    }
    static private bool IsEditStartKey(KeyEventArgs e)
    {
        if (e == null || e.Handled)
            return false;

        Key Key = e.Key;

        if (Key >= Key.A && Key <= Key.Z)
            return true;

        if (Key >= Key.D0 && Key <= Key.D9)
            return true;

        if (Key >= Key.NumPad0 && Key <= Key.NumPad9)
            return true;

        return Key switch
        {
            Key.Space => true,
            Key.OemPlus => true,
            Key.OemMinus => true,
            Key.OemComma => true,
            Key.OemPeriod => true,
            _ => false,
        };
    }
 
    /*
    static private void Grid_KeyDown(object Sender, KeyEventArgs e)
    {
        if (Sender is not DataGrid Grid)
            return;

        GridState State = GetState(Grid);
        if (State == null || State.Controller == null)
            return;

        if (!e.Handled && e.Key == Key.Space)
        {
            if (!Grid.IsReadOnly &&
                Grid.SelectedItem is GridViewRenderRow Row &&
                Row.IsData &&
                Row.DataRow != null &&
                Grid.CurrentColumn != null)
            {
                DataGridColumn GridColumn = Grid.CurrentColumn;
                int ColumnIndex = Grid.Columns.IndexOf(GridColumn);
                if (ColumnIndex >= 0 &&
                    State.RenderData != null &&
                    ColumnIndex < State.RenderData.Columns.Count)
                {
                    GridViewColumnDef ColumnDef = State.RenderData.Columns[ColumnIndex];
                    Type CoreType = GetCoreDataType(ColumnDef);

                    if (CoreType == typeof(bool) && State.Controller.CanEdit(Row.DataRow, ColumnDef.FieldName))
                    {
                        object Value = Row.Values != null && ColumnIndex < Row.Values.Length
                            ? Row.Values[ColumnIndex]
                            : null;

                        bool CurrentValue = false;

                        if (Value != null && Value != DBNull.Value)
                        {
                            try
                            {
                                CurrentValue = Convert.ToBoolean(Value);
                            }
                            catch
                            {
                            }
                        }

                        if (State.Controller.SetValue(Row.DataRow, ColumnDef.FieldName, !CurrentValue))
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                if (Grid.IsVisible && Grid.IsEffectivelyEnabled)
                                {
                                    Grid.Focus();

                                    if (ColumnIndex >= 0 && ColumnIndex < Grid.Columns.Count)
                                        Grid.CurrentColumn = Grid.Columns[ColumnIndex];

                                    if (Grid.SelectedItem != null)
                                        Grid.SelectedItem = Grid.SelectedItem;
                                }
                            }, DispatcherPriority.Input);
                        }

                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        if (!IsEditStartKey(e))
            return;

        if (!CanBeginEditFromKeyboard(Grid, State))
            return;

        Grid.BeginEdit();
    }
    */
    
    static private void Grid_KeyDown(object Sender, KeyEventArgs e)
    {
        if (Sender is not DataGrid Grid)
            return;

        GridState State = GetState(Grid);
        if (State == null || State.Controller == null)
            return;

        GridViewGridNavigator Navigator = State.RenderData != null
            ? new GridViewGridNavigator(Grid, State.Controller, State.RenderData)
            : null;

        if (!e.Handled && Navigator != null)
        {
            bool Handled = false;

            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && e.Key == Key.Tab)
                Handled = Navigator.TryMovePreviousCell();
            else
            {
                switch (e.Key)
                {
                    case Key.Left:
                        Handled = Navigator.TryMoveLeft();
                        break;

                    case Key.Right:
                        Handled = Navigator.TryMoveRight();
                        break;

                    case Key.Tab:
                        Handled = Navigator.TryMoveNextCell();
                        break;
                }
            }

            if (Handled)
            {
                e.Handled = true;
                return;
            }
        }

        if (!e.Handled && e.Key == Key.Space)
        {
            if (!Grid.IsReadOnly &&
                Grid.SelectedItem is GridViewRenderRow Row &&
                Row.IsData &&
                Row.DataRow != null &&
                Grid.CurrentColumn != null)
            {
                DataGridColumn GridColumn = Grid.CurrentColumn;
                int ColumnIndex = Grid.Columns.IndexOf(GridColumn);
                if (ColumnIndex >= 0 &&
                    State.RenderData != null &&
                    ColumnIndex < State.RenderData.Columns.Count)
                {
                    GridViewColumnDef ColumnDef = State.RenderData.Columns[ColumnIndex];
                    Type CoreType = GetCoreDataType(ColumnDef);

                    if (CoreType == typeof(bool) && State.Controller.CanEdit(Row.DataRow, ColumnDef.FieldName))
                    {
                        object Value = Row.Values != null && ColumnIndex < Row.Values.Length
                            ? Row.Values[ColumnIndex]
                            : null;

                        bool CurrentValue = false;

                        if (Value != null && Value != DBNull.Value)
                        {
                            try
                            {
                                CurrentValue = Convert.ToBoolean(Value);
                            }
                            catch
                            {
                            }
                        }

                        if (State.Controller.SetValue(Row.DataRow, ColumnDef.FieldName, !CurrentValue))
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                if (Grid.IsVisible && Grid.IsEffectivelyEnabled)
                                {
                                    Grid.Focus();

                                    if (ColumnIndex >= 0 && ColumnIndex < Grid.Columns.Count)
                                        Grid.CurrentColumn = Grid.Columns[ColumnIndex];
                                }
                            }, DispatcherPriority.Input);
                        }

                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        if (!IsEditStartKey(e))
            return;

        if (!CanBeginEditFromKeyboard(Grid, State))
            return;

        Grid.BeginEdit();
    }
 
    static private bool CanBeginEditFromKeyboard(DataGrid Grid, GridState State)
    {
        if (Grid == null || State == null || State.Controller == null)
            return false;

        if (Grid.IsReadOnly)
            return false;

        if (Grid.SelectedItem is not GridViewRenderRow Row)
            return false;

        if (!Row.IsData || Row.DataRow == null)
            return false;

        DataGridColumn GridColumn = Grid.CurrentColumn;
        if (GridColumn == null)
            return false;

        int ColumnIndex = Grid.Columns.IndexOf(GridColumn);
        if (ColumnIndex < 0)
            return false;

        GridViewRenderData RenderData = State.RenderData;
        if (RenderData == null || ColumnIndex >= RenderData.Columns.Count)
            return false;

        GridViewColumnDef ColumnDef = RenderData.Columns[ColumnIndex];
        if (ColumnDef == null)
            return false;

        return State.Controller.CanEdit(Row.DataRow, ColumnDef.FieldName);
    }
    static private void AddOrReplaceLink(DataGrid Grid, GridViewGridLink Link)
    {
        if (Grid == null)
            return;

        if (fLinks.TryGetValue(Grid, out GridViewGridLink OldLink))
        {
            OldLink.Dispose();
            fLinks.Remove(Grid);
        }

        if (Link != null)
            fLinks.Add(Grid, Link);
    }
    static private void AddRows(ObservableCollection<GridViewRenderRow> TargetRows, IList<GridViewRenderRow> SourceRows, int StartIndex)
    {
        for (int i = StartIndex; i < SourceRows.Count; i++)
            TargetRows.Add(SourceRows[i]);
    }
    static private void BindRows(DataGrid Grid, GridViewRenderData RenderData, GridState State)
    {
        if (Grid == null)
            return;

        if (State == null)
        {
            Grid.ItemsSource = RenderData != null ? RenderData.Rows : null;
            return;
        }

        if (State.Rows == null)
            State.Rows = new ObservableCollection<GridViewRenderRow>();

        UpdateRows(State.Rows, RenderData != null ? RenderData.Rows : null);

        if (!ReferenceEquals(Grid.ItemsSource, State.Rows))
            Grid.ItemsSource = State.Rows;
    }
    static private void BuildStructure(DataGrid Grid, GridViewRenderData RenderData, GridState State)
    {
        Grid.AutoGenerateColumns = false;
        Grid.Columns.Clear();

        for (int i = 0; i < RenderData.Columns.Count; i++)
        {
            int ColumnIndex = i;
            GridViewColumnDef ColumnDef = RenderData.Columns[i];

            DataGridTemplateColumn Column = new()
            {
                Header = !string.IsNullOrWhiteSpace(ColumnDef.Title) ? ColumnDef.Title : ColumnDef.FieldName,
                Width = DataGridLength.Auto,
                IsReadOnly = ColumnDef.IsReadOnly,
                CellTemplate = new FuncDataTemplate<GridViewRenderRow>((Row, _) =>
                {
                    return CreateCell(Grid, State, Row, ColumnDef, ColumnIndex);
                }),
                CellEditingTemplate = new FuncDataTemplate<GridViewRenderRow>((Row, _) =>
                {
                    return CreateEditingCell(Grid, State, Row, ColumnDef, ColumnIndex);
                }),
                SortMemberPath = ColumnDef.FieldName,
            };

            Grid.Columns.Add(Column);
        }

        if (State != null)
            State.StructureKey = GetStructureKey(RenderData);
        
        if (State != null)
        {
            State.StructureKey = GetStructureKey(RenderData);
            State.RenderData = RenderData;
            AttachEditHandlers(Grid, State);
        }
    }
    static private bool CanEditCell(GridViewController Controller, GridViewRenderRow Row, GridViewColumnDef ColumnDef)
    {
        if (Controller == null || Row == null || Row.DataRow == null || ColumnDef == null)
            return false;

        return Controller.CanEdit(Row.DataRow, ColumnDef.FieldName);
    }
    static private void ClearState(GridState State)
    {
        if (State == null)
            return;

        State.Controller = null;
        State.Data = null;
        State.StructureKey = null;

        if (State.Rows != null)
            State.Rows.Clear();
    }
    
    static private Control CreateCell(DataGrid Grid, GridState State, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && Row.IsGroup && IsLabelCell(Row, ColumnIndex))
            return CreateGroupCell(Grid, State, Row, ColumnIndex);

        Type CoreType = GetCoreDataType(ColumnDef);
        bool IsBool = CoreType == typeof(bool);

        string TextValue;
        if (IsBool)
        {
            object Value = Row != null && Row.Values != null && ColumnIndex >= 0 && ColumnIndex < Row.Values.Length
                ? Row.Values[ColumnIndex]
                : null;

            bool BoolValue = false;

            if (Value != null && Value != DBNull.Value)
            {
                try
                {
                    BoolValue = Convert.ToBoolean(Value);
                }
                catch
                {
                }
            }

            TextValue = BoolValue ? "x" : string.Empty;
        }
        else
        {
            TextValue = FormatValue(Row, ColumnDef, ColumnIndex);
        }

        TextBlock Text = new()
        {
            Text = TextValue,
            Margin = new Thickness(6, 2, 6, 2),
            HorizontalAlignment = IsBool ? HorizontalAlignment.Center : GetHorizontalAlignment(Row, ColumnDef, ColumnIndex),
            TextAlignment = IsBool ? TextAlignment.Center : TextAlignment.Left,
            FontWeight = GetFontWeight(Row, ColumnIndex),
            VerticalAlignment = VerticalAlignment.Center,
        };

        Border Result = new()
        {
            Background = GetRowBackground(Row),
            Child = Text,
        };

        return Result;
    }
    
    static private Control CreateEditingCell(DataGrid Grid, GridState State, GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        GridViewController Controller = State != null ? State.Controller : null;

        if (!CanEditCell(Controller, Row, ColumnDef))
            return CreateCell(Grid, State, Row, ColumnDef, ColumnIndex);

        GridViewInplaceEditorContext Context = new()
        {
            Grid = Grid,
            Controller = State != null ? State.Controller : null,
            Data = State != null ? State.Data : null,
            Row = Row,
            ColumnDef = ColumnDef,
            ColumnIndex = ColumnIndex,
        };

        GridViewInplaceEditor Editor = GridViewInplaceEditorFactory.Create(Context);
        return Editor.Create();
    }
    static private Control CreateGroupCell(DataGrid Grid, GridState State, GridViewRenderRow Row, int ColumnIndex)
    {
        string Glyph = Row != null && Row.Node != null && Row.Node.IsExpanded ? "▼ " : "▶ ";

        TextBlock Text = new()
        {
            Text = Glyph + FormatValue(Row, null, ColumnIndex),
            Margin = new Thickness(6, 2, 6, 2),
            HorizontalAlignment = HorizontalAlignment.Left,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
        };

        Border Result = new()
        {
            Background = GetRowBackground(Row),
            Child = Text,
            Cursor = new Cursor(StandardCursorType.Hand),
        };

        Result.PointerPressed += (s, e) =>
        {
            if (Row == null || Row.Node == null)
                return;

            if (Row.Node.Toggle() && State != null && State.Data != null)
                Apply(Grid, State.Data, State.Controller);
        };

        return Result;
    }
    /*
    static private string FormatValue(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        Type CoreType = GetCoreDataType(ColumnDef);

        if (CoreType != null && CoreType.IsEnum)
        {
            try
            {
                object EnumValue = Value.GetType().IsEnum
                    ? Value
                    : Enum.ToObject(CoreType, Value);

                return EnumValue.ToString();
            }
            catch
            {
            }
        }

        return Convert.ToString(Value) ?? string.Empty;
    }
    */
    
    /*
    static private string FormatValue(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        Type CoreType = GetCoreDataType(ColumnDef);

        if (CoreType != null && CoreType.IsEnum)
        {
            try
            {
                object EnumValue = Value.GetType().IsEnum
                    ? Value
                    : Enum.ToObject(CoreType, Value);

                return EnumValue.ToString();
            }
            catch
            {
            }
        }

        string DisplayFormat = ColumnDef != null ? ColumnDef.DisplayFormat : string.Empty;
        if (!string.IsNullOrWhiteSpace(DisplayFormat) && Value is IFormattable Formattable)
            return Formattable.ToString(DisplayFormat, System.Globalization.CultureInfo.InvariantCulture);

        return Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }
    */
    
    /*
    static private string FormatValue(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        Type CoreType = GetCoreDataType(ColumnDef);

        if (CoreType != null && CoreType.IsEnum)
        {
            try
            {
                object EnumValue = Value.GetType().IsEnum
                    ? Value
                    : Enum.ToObject(CoreType, Value);

                return EnumValue.ToString();
            }
            catch
            {
            }
        }

        string DisplayFormat = ColumnDef != null ? ColumnDef.DisplayFormat : string.Empty;
        if (!string.IsNullOrWhiteSpace(DisplayFormat) && Value is IFormattable Formattable)
            return Formattable.ToString(DisplayFormat, System.Globalization.CultureInfo.InvariantCulture);

        return Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }
    */
    
    static private string FormatValue(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return string.Empty;

        object Value = Row.Values[ColumnIndex];
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        if (ColumnDef != null && ColumnDef.IsLookup)
            return GridViewLookupHelper.GetDisplayTextByValue(ColumnDef, Value);

        Type CoreType = GetCoreDataType(ColumnDef);

        if (CoreType != null && CoreType.IsEnum)
        {
            try
            {
                object EnumValue = Value.GetType().IsEnum
                    ? Value
                    : Enum.ToObject(CoreType, Value);

                return EnumValue.ToString();
            }
            catch
            {
            }
        }

        string DisplayFormat = ColumnDef != null ? ColumnDef.DisplayFormat : string.Empty;
        if (!string.IsNullOrWhiteSpace(DisplayFormat) && Value is IFormattable Formattable)
            return Formattable.ToString(DisplayFormat, System.Globalization.CultureInfo.InvariantCulture);

        return Convert.ToString(Value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }
    
    
    static private FontWeight GetFontWeight(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null)
            return FontWeight.Normal;

        if ((Row.IsGroup || Row.IsFooter || Row.IsGrandTotal) && IsLabelCell(Row, ColumnIndex))
            return FontWeight.Bold;

        if ((Row.IsFooter || Row.IsGrandTotal) && HasValue(Row, ColumnIndex))
            return FontWeight.Bold;
        
        if (Row.IsGroup && Row.Node != null && !Row.Node.IsExpanded && HasValue(Row, ColumnIndex))
            return FontWeight.Bold;

        return FontWeight.Normal;
    }
    static private HorizontalAlignment GetHorizontalAlignment(GridViewRenderRow Row, GridViewColumnDef ColumnDef, int ColumnIndex)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal) && IsLabelCell(Row, ColumnIndex))
            return HorizontalAlignment.Left;

        if (ColumnDef != null && (ColumnDef.IsNumeric || ColumnDef.IsDateTime))
            return HorizontalAlignment.Right;

        return HorizontalAlignment.Left;
    }
    static private IBrush GetRowBackground(GridViewRenderRow Row)
    {
        if (Row != null && (Row.IsGroup || Row.IsFooter || Row.IsGrandTotal))
            return Brushes.Gainsboro;

        return Brushes.Transparent;
    }
    static private GridState GetState(DataGrid Grid)
    {
        if (Grid == null)
            return null;

        if (!fStates.TryGetValue(Grid, out GridState State))
        {
            State = new GridState();
            fStates.Add(Grid, State);
        }

        return State;
    }
    static private string GetStructureKey(GridViewRenderData RenderData)
    {
        if (RenderData == null || RenderData.Columns == null || RenderData.Columns.Count == 0)
            return string.Empty;

        return string.Join("\u001F", RenderData.Columns.Select(x =>
        {
            string FieldName = x.FieldName ?? string.Empty;
            string Title = x.Title ?? string.Empty;
            string DataType = x.DataType != null ? x.DataType.FullName : string.Empty;
            return $"{FieldName}|{Title}|{x.IsReadOnly}|{x.VisibleIndex}|{x.GroupIndex}|{x.Aggregate}|{DataType}";
        }));
    }
    static private bool HasStructureChanged(GridState State, GridViewRenderData RenderData)
    {
        string NewKey = GetStructureKey(RenderData);

        if (State == null)
            return true;

        return !string.Equals(State.StructureKey, NewKey, StringComparison.Ordinal);
    }
    static private bool HasValue(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null || Row.Values == null || ColumnIndex < 0 || ColumnIndex >= Row.Values.Length)
            return false;

        return Row.Values[ColumnIndex] != null;
    }
    static private bool IsLabelCell(GridViewRenderRow Row, int ColumnIndex)
    {
        if (Row == null)
            return false;

        return Row.LabelColumnIndex == ColumnIndex;
    }
    static private void RemoveRows(ObservableCollection<GridViewRenderRow> TargetRows, int StartIndex)
    {
        while (TargetRows.Count > StartIndex)
            TargetRows.RemoveAt(TargetRows.Count - 1);
    }
    static private void UpdateRows(ObservableCollection<GridViewRenderRow> TargetRows, IList<GridViewRenderRow> SourceRows)
    {
        SourceRows ??= Array.Empty<GridViewRenderRow>();

        int CommonCount = Math.Min(TargetRows.Count, SourceRows.Count);

        for (int i = 0; i < CommonCount; i++)
        {
            if (!ReferenceEquals(TargetRows[i], SourceRows[i]))
                TargetRows[i] = SourceRows[i];
        }

        if (TargetRows.Count > SourceRows.Count)
            RemoveRows(TargetRows, SourceRows.Count);
        else if (SourceRows.Count > TargetRows.Count)
            AddRows(TargetRows, SourceRows, TargetRows.Count);
    }
    static private void UpdateState(GridState State, GridViewData Data, GridViewController Controller)
    {
        if (State == null)
            return;

        State.Data = Data;
        State.Controller = Controller;
    }
 
    static internal void Apply(DataGrid Grid, GridViewController Controller)
    {
        if (Controller == null)
            throw new ArgumentNullException(nameof(Controller));

        Apply(Grid, Controller.Data, Controller);
    }
    static private void Apply(DataGrid Grid, GridViewData Data)
    {
        Apply(Grid, Data, null);
    }
    static private void Apply(DataGrid Grid, GridViewData Data, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        GridState State = GetState(Grid);
        UpdateState(State, Data, Controller);

        if (Data == null)
        {
            Grid.AutoGenerateColumns = false;
            Grid.Columns.Clear();
            Grid.ItemsSource = null;
            ClearState(State);
            return;
        }

        GridViewRenderData RenderData = GridViewRenderer.Render(Data);
        Apply(Grid, RenderData, Data, Controller);
    }
    static private void Apply(DataGrid Grid, GridViewRenderData RenderData)
    {
        Apply(Grid, RenderData, null, null);
    }
    static private void Apply(DataGrid Grid, GridViewRenderData RenderData, GridViewData Data)
    {
        Apply(Grid, RenderData, Data, null);
    }
    static private void Apply(DataGrid Grid, GridViewRenderData RenderData, GridViewData Data, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (RenderData == null)
            throw new ArgumentNullException(nameof(RenderData));

        GridState State = GetState(Grid);
        UpdateState(State, Data, Controller);
        State.RenderData = RenderData;
        
        if (HasStructureChanged(State, RenderData))
            BuildStructure(Grid, RenderData, State);

        BindRows(Grid, RenderData, State);
    }
    
    // ● static public methods
    static public void Bind(DataGrid Grid, GridViewController Controller)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        GridViewGridLink Result = new(Grid, Controller);
        AddOrReplaceLink(Grid, Result);
        //return Result;
    }
    static public void Refresh(DataGrid Grid)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (fLinks.TryGetValue(Grid, out GridViewGridLink Link))
            Link.Refresh();
    }
    static public void Unbind(DataGrid Grid)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (fLinks.TryGetValue(Grid, out GridViewGridLink Link))
        {
            Link.Dispose();
            fLinks.Remove(Grid);
        }

        if (fStates.TryGetValue(Grid, out GridState State))
        {
            ClearState(State);
            fStates.Remove(Grid);
        }
    }

    // ● private classes
    private class GridState
    {
        // ● properties
        public GridViewController Controller { get; set; }
        public GridViewData Data { get; set; }
        public ObservableCollection<GridViewRenderRow> Rows { get; set; }
        public string StructureKey { get; set; }
        public bool EditHandlersAttached { get; set; }
        public GridViewRenderData RenderData { get; set; }
    }
}