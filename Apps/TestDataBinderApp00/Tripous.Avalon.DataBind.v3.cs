using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Tripous.Data;
using Avalonia.VisualTree;
using InputElement = Avalonia.Input.InputElement;
using KeyEventArgs = Avalonia.Input.KeyEventArgs;
using KeyModifiers = Avalonia.Input.KeyModifiers;


namespace Tripous.Avalon;


static public class DataGridExtensions
{
    // ● private classes
    private class LookupDisplayConverter: IValueConverter
    {
        DataSource fSource;
        DataSourceColumn fColumn;

        // ● constructor
        public LookupDisplayConverter(DataSource Source, DataSourceColumn Column)
        {
            fSource = Source;
            fColumn = Column;
        }

        // ● public methods
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value is DataSourceRow Row)
                return GetLookupDisplayText(fSource, fColumn, Row);

            return null;
        }
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Value;
        }
    }
    private class TextValueConverter: IValueConverter
    {
        DataSourceColumn fColumn;

        // ● constructor
        public TextValueConverter(DataSourceColumn Column)
        {
            fColumn = Column;
        }

        // ● public methods
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return FormatDisplayValue(fColumn, Value);
        }
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Value;
        }
    }

    // ● private methods
    static private object GetPropertyValue(object Instance, string PropertyName)
    {
        if (Instance == null)
            return null;

        if (string.IsNullOrWhiteSpace(PropertyName))
            return Instance;

        try
        {
            if (Instance is DataSourceRow SourceRow)
                return SourceRow[PropertyName];

            if (Instance is DataRowView RowView)
                return RowView[PropertyName];

            if (Instance is DataRow Row)
                return Row[PropertyName];

            PropertyInfo Prop = Instance.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (Prop != null)
                return Prop.GetValue(Instance);
        }
        catch
        {
        }

        return null;
    }
    static private Binding CreateColumnBinding(string ColumnName)
    {
        return new Binding($"[{ColumnName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
    }
    static private ILookupSource GetLookupSource(DataSource Source, DataSourceColumn Column)
    {
        if (Column == null)
            return null;

        if (Column.LookupSource != null)
            return Column.LookupSource;

        if (Source != null && !string.IsNullOrWhiteSpace(Column.LookupSourceName))
            return Source.Lookups.Get(Column.LookupSourceName);

        return null;
    }
    static private Type GetCoreType(Type DataType)
    {
        return Nullable.GetUnderlyingType(DataType) ?? DataType;
    }
    static private bool IsNumericType(Type DataType)
    {
        Type CoreType = GetCoreType(DataType);

        return CoreType == typeof(byte)
            || CoreType == typeof(short)
            || CoreType == typeof(int)
            || CoreType == typeof(long)
            || CoreType == typeof(float)
            || CoreType == typeof(double)
            || CoreType == typeof(decimal);
    }
    static private bool IsDateType(Type DataType)
    {
        Type CoreType = GetCoreType(DataType);
        return CoreType == typeof(DateTime) || CoreType == typeof(DateTimeOffset);
    }
    static private HorizontalAlignment GetDisplayHorizontalAlignment(DataSourceColumn Column)
    {
        if (Column?.DataType == null)
            return HorizontalAlignment.Left;

        Type CoreType = GetCoreType(Column.DataType);

        if (CoreType == typeof(bool))
            return HorizontalAlignment.Center;

        if (IsNumericType(CoreType) || IsDateType(CoreType))
            return HorizontalAlignment.Right;

        return HorizontalAlignment.Left;
    }
    static private HorizontalAlignment GetEditorHorizontalAlignment(DataSourceColumn Column)
    {
        if (Column?.DataType == null)
            return HorizontalAlignment.Left;

        Type CoreType = GetCoreType(Column.DataType);

        if (IsNumericType(CoreType) || IsDateType(CoreType))
            return HorizontalAlignment.Right;

        return HorizontalAlignment.Left;
    }
    static private string FormatDisplayValue(DataSourceColumn Column, object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        if (Column?.DataType != null)
        {
            Type CoreType = GetCoreType(Column.DataType);

            if (CoreType == typeof(bool))
            {
                try
                {
                    return Convert.ToBoolean(Value, CultureInfo.InvariantCulture) ? "x" : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        if (Column != null && !string.IsNullOrWhiteSpace(Column.DisplayFormat) && Value is IFormattable Formattable)
        {
            try
            {
                return Formattable.ToString(Column.DisplayFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        return Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
    static private string FormatEditValue(DataSourceColumn Column, object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        if (Column != null && !string.IsNullOrWhiteSpace(Column.EditFormat) && Value is IFormattable Formattable)
        {
            try
            {
                return Formattable.ToString(Column.EditFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        return Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
    static private object ConvertTextValue(DataSourceColumn Column, string Text, object OriginalValue)
    {
        if (Column?.DataType == null)
            return Text;

        Type DataType = Column.DataType;
        Type CoreType = GetCoreType(DataType);
        bool IsNullable = Nullable.GetUnderlyingType(DataType) != null || Column.AllowsNull;

        if (string.IsNullOrWhiteSpace(Text))
        {
            if (CoreType == typeof(string))
                return string.Empty;

            if (IsNullable)
                return null;
        }

        try
        {
            if (CoreType == typeof(string))
                return Text;
            if (CoreType == typeof(byte))
                return byte.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(short))
                return short.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(int))
                return int.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(long))
                return long.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(float))
                return float.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(double))
                return double.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(decimal))
                return decimal.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(DateTime))
                return DateTime.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(DateTimeOffset))
                return DateTimeOffset.Parse(Text, CultureInfo.InvariantCulture);
            if (CoreType == typeof(Guid))
                return Guid.Parse(Text);
            if (CoreType.IsEnum)
                return Enum.Parse(CoreType, Text, true);

            return Convert.ChangeType(Text, CoreType, CultureInfo.InvariantCulture);
        }
        catch
        {
            return OriginalValue;
        }
    }
    static private object GetItemValue(object Item, string ValueMember)
    {
        if (Item == null)
            return null;

        if (string.IsNullOrWhiteSpace(ValueMember))
            return Item;

        return GetPropertyValue(Item, ValueMember);
    }
    static private string GetItemDisplayText(object Item, string DisplayMember)
    {
        if (Item == null)
            return null;

        object Value = string.IsNullOrWhiteSpace(DisplayMember) ? Item : GetPropertyValue(Item, DisplayMember);
        return Value != null ? Convert.ToString(Value, CultureInfo.InvariantCulture) : null;
    }
    static private object FindItemByValue(DataSource Source, DataSourceColumn Column, object Value)
    {
        ILookupSource LookupSource = GetLookupSource(Source, Column);
        if (LookupSource == null)
            return null;

        foreach (object Item in LookupSource.Items)
        {
            object ItemValue = GetItemValue(Item, Column.ValueMember);
            if (DataSource.AreEqual(ItemValue, Value))
                return Item;
        }

        return null;
    }
    static private string GetLookupDisplayText(DataSource Source, DataSourceColumn Column, DataSourceRow Row)
    {
        if (Source == null || Column == null || Row == null)
            return null;

        object Value = Row[Column.Name];
        object Item = FindItemByValue(Source, Column, Value);

        if (Item != null)
            return GetItemDisplayText(Item, Column.DisplayMember);

        return Value != null ? Convert.ToString(Value, CultureInfo.InvariantCulture) : null;
    }
    static private Border CreateCellBorder(Control Child)
    {
        return new Border
        {
            Padding = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Child = Child
        };
    }
    static private Grid CreateStretchHost(Control Child)
    {
        Grid Result = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        Result.Children.Add(Child);
        return Result;
    }
    static private DataGridTemplateColumn CreateTextColumn(DataSourceColumn Column)
    {
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        DataGridTemplateColumn Result = new();

        Result.CellTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBlock Text = new()
            {
                Margin = new Thickness(6, 2, 6, 2),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = GetDisplayHorizontalAlignment(Column) == HorizontalAlignment.Right ? TextAlignment.Right : GetDisplayHorizontalAlignment(Column) == HorizontalAlignment.Center ? TextAlignment.Center : TextAlignment.Left,
            };

            Text.Bind(TextBlock.TextProperty, new Binding($"[{Column.Name}]")
            {
                Mode = BindingMode.OneWay,
                Converter = new TextValueConverter(Column)
            });

            return CreateCellBorder(CreateStretchHost(Text));
        });

        Result.CellEditingTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBox Box = new()
            {
                Margin = new Thickness(4, 1, 4, 1),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = GetEditorHorizontalAlignment(Column),
                IsReadOnly = Column.ReadOnly,
            };

            if (Item is DataSourceRow Row)
                Box.Text = FormatEditValue(Column, Row[Column.Name]);
 
            Box.AttachedToVisualTree += (s, e) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (!Box.IsVisible || !Box.IsEffectivelyEnabled)
                        return;

                    Box.Focus();

                    DataGrid Grid = Box.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault();
                    DataGridHost Host = Grid != null ? Grid.Tag as DataGridHost : null;

                    if (Host != null && Host.TryConsumePendingEditText(Box))
                        return;

                    Box.SelectAll();
                }, DispatcherPriority.Input);
            };

            void CommitEdit()
            {
                if (Item is not DataSourceRow Row)
                    return;

                object OriginalValue = Row[Column.Name];
                object NewValue = ConvertTextValue(Column, Box.Text ?? string.Empty, OriginalValue);

                if (!DataSource.AreEqual(OriginalValue, NewValue))
                    Row[Column.Name] = NewValue;
            }

            Box.LostFocus += (s, e) =>
            {
                CommitEdit();
            };

            Box.KeyDown += (s, e) =>
            {
                if (e.Handled)
                    return;

                switch (e.Key)
                {
                    case Key.Enter:
                    case Key.Tab:
                        CommitEdit();
                        break;
                }
            };

            return CreateCellBorder(CreateStretchHost(Box));
        });

        return Result;
    }
    static private DataGridTemplateColumn CreateBoolColumn(DataSourceColumn Column)
    {
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        DataGridTemplateColumn Result = new();

        Result.CellTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            CheckBox Box = new()
            {
                Margin = new Thickness(6, 1, 6, 1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                Focusable = false,
                IsThreeState = GetCoreType(Column.DataType) != typeof(bool),
            };

            Box.Bind(ToggleButton.IsCheckedProperty, new Binding($"[{Column.Name}]")
            {
                Mode = BindingMode.OneWay
            });

            return CreateCellBorder(CreateStretchHost(Box));
        });

        Result.CellEditingTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            CheckBox Box = new()
            {
                Margin = new Thickness(6, 1, 6, 1),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsThreeState = GetCoreType(Column.DataType) != typeof(bool),
                IsEnabled = !Column.ReadOnly,
            };

            Box.Bind(ToggleButton.IsCheckedProperty, CreateColumnBinding(Column.Name));

            Box.AttachedToVisualTree += (s, e) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (Box.IsVisible && Box.IsEffectivelyEnabled)
                        Box.Focus();
                }, DispatcherPriority.Input);
            };

            return CreateCellBorder(CreateStretchHost(Box));
        });

        return Result;
    }
    static private bool GetAllowNullSelection(DataSourceColumn Column)
    {
        return Column != null && Column.AllowsNull;
    }
    static private string GetNullText(DataSourceColumn Column)
    {
        return !string.IsNullOrWhiteSpace(Column?.NullText) ? Column.NullText : "(None)";
    }
    static private List<object> BuildLookupItems(DataSource Source, DataSourceColumn Column)
    {
        List<object> Result = new();

        ILookupSource LookupSource = GetLookupSource(Source, Column);
        if (LookupSource == null)
            return Result;

        if (GetAllowNullSelection(Column))
            Result.Add(new LookupNullItem(GetNullText(Column)));

        if (LookupSource.Items != null)
        {
            foreach (object Item in LookupSource.Items)
                Result.Add(Item);
        }

        return Result;
    }
    static private object GetLookupEditorItemValue(object Item, DataSourceColumn Column)
    {
        if (Item == null)
            return null;

        if (Item is LookupNullItem)
            return null;

        return GetItemValue(Item, Column.ValueMember);
    }
    static private string GetLookupEditorItemText(object Item, DataSourceColumn Column)
    {
        if (Item == null)
            return string.Empty;

        if (Item is LookupNullItem NullItem)
            return NullItem.Text;

        return GetItemDisplayText(Item, Column.DisplayMember) ?? string.Empty;
    }
    static private object FindLookupEditorItem(List<object> Items, DataSourceColumn Column, object Value)
    {
        if (Items == null || Column == null)
            return null;

        if (Value == null || Value == DBNull.Value)
        {
            foreach (object Item in Items)
            {
                if (Item is LookupNullItem)
                    return Item;
            }

            return null;
        }

        foreach (object Item in Items)
        {
            if (Item is LookupNullItem)
                continue;

            object ItemValue = GetLookupEditorItemValue(Item, Column);
            if (DataSource.AreEqual(ItemValue, Value))
                return Item;
        }

        return null;
    }
    static private DataGridTemplateColumn CreateLookupColumn(DataSource Source, DataSourceColumn Column)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        DataGridTemplateColumn Result = new();

        Result.CellTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBlock Text = new()
            {
                Margin = new Thickness(6, 2, 6, 2),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TextAlignment = TextAlignment.Left,
            };

            Text.Bind(TextBlock.TextProperty, new Binding(".")
            {
                Mode = BindingMode.OneWay,
                Converter = new LookupDisplayConverter(Source, Column)
            });

            return CreateCellBorder(CreateStretchHost(Text));
        });

        Result.CellEditingTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            ComboBox Box = new()
            {
                Margin = new Thickness(0),
                Padding = new Thickness(4, 1, 4, 1),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsEnabled = !Column.ReadOnly,
            };

            List<object> Items = BuildLookupItems(Source, Column);

            Box.ItemsSource = Items;

            Box.ItemTemplate = new FuncDataTemplate<object>((LookupItem, _) =>
            {
                return new TextBlock
                {
                    Text = GetLookupEditorItemText(LookupItem, Column),
                    Margin = new Thickness(4, 2, 4, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Left,
                };
            });

            Box.SelectionBoxItemTemplate = Box.ItemTemplate;

            if (Item is DataSourceRow Row)
                Box.SelectedItem = FindLookupEditorItem(Items, Column, Row[Column.Name]);

            Box.AttachedToVisualTree += (s, e) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (Box.IsVisible && Box.IsEffectivelyEnabled)
                        Box.Focus();
                }, DispatcherPriority.Input);
            };

            void CommitSelection()
            {
                if (Item is not DataSourceRow Row)
                    return;

                object OriginalValue = Row[Column.Name];
                object NewValue = GetLookupEditorItemValue(Box.SelectedItem, Column);

                if (!DataSource.AreEqual(OriginalValue, NewValue))
                    Row[Column.Name] = NewValue;
            }

            Box.SelectionChanged += (s, e) =>
            {
                CommitSelection();
            };

            Box.DropDownClosed += (s, e) =>
            {
                CommitSelection();
            };

            Box.LostFocus += (s, e) =>
            {
                CommitSelection();
            };

            return CreateCellBorder(CreateStretchHost(Box));
        });

        Result.CellStyleClasses.Add("LookupColumn");
        return Result;
    }

    // ● static public methods
    static public DataGridColumn CreateColumn(DataSource Source, DataSourceColumn Column)
    {
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        DataGridTemplateColumn Result;

        if (Column.HasLookup)
            Result = CreateLookupColumn(Source, Column);
        else if (GetCoreType(Column.DataType) == typeof(bool))
            Result = CreateBoolColumn(Column);
        else
            Result = CreateTextColumn(Column);

        ApplyColumnState(Result, Column);
        return Result;
    }
    static public List<DataGridColumn> CreateColumns(DataSource Source, IEnumerable<DataSourceColumn> Columns)
    {
        List<DataGridColumn> Result = new();

        if (Columns != null)
        {
            foreach (DataSourceColumn Column in Columns)
                Result.Add(CreateColumn(Source, Column));
        }

        return Result;
    }
    static public void AddColumns(this DataGrid Grid, DataSource Source, IEnumerable<DataSourceColumn> Columns)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        if (Columns != null)
        {
            foreach (DataGridColumn Column in CreateColumns(Source, Columns))
                Grid.Columns.Add(Column);
        }
    }
    static public void ApplyColumnState(DataGridColumn GridColumn, DataSourceColumn Column)
    {
        if (GridColumn == null)
            throw new ArgumentNullException(nameof(GridColumn));
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        GridColumn.Header = Column.Caption;
        GridColumn.IsReadOnly = Column.ReadOnly;
        GridColumn.Tag = Column;
    }
    static public DataGridColumn FindColumn(this DataGrid Grid, DataSourceColumn Column)
    {
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));
        if (Column == null)
            return null;

        foreach (DataGridColumn GridColumn in Grid.Columns)
        {
            if (ReferenceEquals(GridColumn.Tag, Column))
                return GridColumn;
        }

        return null;
    }
}


/// <summary>
/// Holds binding metadata for a control
/// </summary>
public class ControlBinding
{
    // ● private
    Control fControl;
    string fColumnName;
    ILookupSource fLookupSource;
    string fLookupSourceName;
    string fDisplayMember;
    string fValueMember;
    EventHandler<SelectionChangedEventArgs> fSelectionChangedHandler;
    bool fIsRefreshing;
    bool? fAllowNullSelection;
    string fNullText;
    object fLookupNullItem;
    List<object> fLookupItems = new();
    DataGrid fGrid;
    EventHandler<SelectionChangedEventArgs> fGridSelectionChangedHandler;
    bool fIsGridSelectionSyncing;
    Dictionary<DataSourceColumn, EventHandler> fGridColumnChangedHandlers = new();
    DataGridHost fGridHost;

    // ● constructor
    public ControlBinding(Control Control, string ColumnName)
    {
        fControl = Control;
        fColumnName = ColumnName;
        fGrid = Control as DataGrid;
    }

    // ● public methods
    public override string ToString()
    {
        return $"{Control?.GetType().Name}: {ColumnName}";
    }
    public void ClearLookupItems()
    {
        fLookupItems.Clear();
    }
    public void AddLookupItem(object Item)
    {
        fLookupItems.Add(Item);
    }
    public IEnumerable GetLookupItems()
    {
        return fLookupItems != null && fLookupItems.Count > 0 ? fLookupItems : null;
    }
    
    // ● properties
    public Control Control => fControl;
    public string ColumnName => fColumnName;
    public ILookupSource LookupSource
    {
        get => fLookupSource;
        set => fLookupSource = value;
    }
    public string LookupSourceName
    {
        get => fLookupSourceName;
        set => fLookupSourceName = value;
    }
    public string DisplayMember
    {
        get => fDisplayMember;
        set => fDisplayMember = value;
    }
    public string ValueMember
    {
        get => fValueMember;
        set => fValueMember = value;
    }
    public EventHandler<SelectionChangedEventArgs> SelectionChangedHandler
    {
        get => fSelectionChangedHandler;
        set => fSelectionChangedHandler = value;
    }
    public bool IsRefreshing
    {
        get => fIsRefreshing;
        set => fIsRefreshing = value;
    }
    public bool IsLookup => LookupSource != null;
    public bool? AllowNullSelection
    {
        get => fAllowNullSelection;
        set => fAllowNullSelection = value;
    }
    public string NullText
    {
        get => fNullText;
        set => fNullText = value;
    }
    public object LookupNullItem
    {
        get => fLookupNullItem;
        set => fLookupNullItem = value;
    }
    public DataGrid Grid => fGrid ?? Control as DataGrid;
    public EventHandler<SelectionChangedEventArgs> GridSelectionChangedHandler
    {
        get => fGridSelectionChangedHandler;
        set => fGridSelectionChangedHandler = value;
    }
    public bool IsGridSelectionSyncing
    {
        get => fIsGridSelectionSyncing;
        set => fIsGridSelectionSyncing = value;
    }
    public Dictionary<DataSourceColumn, EventHandler> GridColumnChangedHandlers => fGridColumnChangedHandlers;
    public DataGridHost GridHost
    {
        get => fGridHost;
        set => fGridHost = value;
    }
}

/// <summary>
/// Synthetic lookup item representing a null value
/// </summary>
sealed public class LookupNullItem
{
    // ● constructor
    public LookupNullItem(string Text)
    {
        fText = Text;
    }

    // ● private
    string fText;

    // ● public methods
    public override string ToString()
    {
        return fText;
    }

    // ● properties
    public string Text => fText;
}


/// <summary>
/// Lookup helper methods
/// </summary>
static internal class DataBinderLookupHelper
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
        if (Item is DataSourceRow Row)
            return Row[MemberName];
        if (Item is DataRowView RowView)
            return RowView[MemberName];
        if (Item is DataRow DataRow)
            return DataRow[MemberName];

        PropertyInfo Prop = Item.GetType().GetProperty(MemberName, BindingFlags.Instance | BindingFlags.Public);
        if (Prop != null)
            return Prop.GetValue(Item);

        return null;
    }

    // ● static public methods
    static public IEnumerable GetItems(ControlBinding Binding)
    {
        if (Binding == null)
            return null;

        IEnumerable Items = Binding.GetLookupItems();
        if (Items != null)
            return Items;

        if (Binding.LookupSource == null)
            return null;

        return Binding.LookupSource.Items;
    }
    static public object GetItemValue(object Item, ControlBinding Binding)
    {
        if (Binding == null)
            return null;
        if (Item == null)
            return null;
        if (ReferenceEquals(Item, Binding.LookupNullItem))
            return null;

        return GetMemberValue(Item, Binding.ValueMember);
    }
    static public string GetItemDisplayText(object Item, ControlBinding Binding)
    {
        if (Binding == null)
            return string.Empty;
        if (Item == null)
            return string.Empty;
        if (ReferenceEquals(Item, Binding.LookupNullItem) && Item is LookupNullItem NullItem)
            return NullItem.Text;

        object Value = GetMemberValue(Item, Binding.DisplayMember);
        return Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
    static public object FindItemByValue(ControlBinding Binding, object Value)
    {
        IEnumerable Items = GetItems(Binding);
        if (Items == null)
            return null;

        if (Value == null || Value == DBNull.Value)
        {
            if (Binding.LookupNullItem != null)
                return Binding.LookupNullItem;
        }

        foreach (object Item in Items)
        {
            if (ReferenceEquals(Item, Binding.LookupNullItem))
                continue;

            object ItemValue = GetItemValue(Item, Binding);
            if (AreEqual(ItemValue, Value))
                return Item;
        }

        return null;
    }
}
/// <summary>
/// Avalonia helper methods for control binding
/// </summary>
static internal class DataBinderControlBindingHelper
{
    // ● private methods
    static private bool GetAllowNullSelection(DataBinder Binder, string ColumnName, bool? AllowNullSelection)
    {
        if (AllowNullSelection != null)
            return AllowNullSelection.Value;

        if (Binder?.Source != null)
        {
            DataSourceColumn Column = Binder.Source.Columns.FirstOrDefault(Item => string.Equals(Item.Name, ColumnName, StringComparison.OrdinalIgnoreCase));
            if (Column != null)
                return Column.AllowsNull;
        }

        return false;
    }
    static private string GetNullText(string NullText)
    {
        return !string.IsNullOrWhiteSpace(NullText) ? NullText : "(None)";
    }
    static private void BuildLookupItems(ControlBinding Binding)
    {
        if (Binding == null || Binding.LookupSource == null)
            return;

        Binding.ClearLookupItems();

        if (Binding.LookupNullItem != null)
            Binding.AddLookupItem(Binding.LookupNullItem);

        if (Binding.LookupSource.Items != null)
        {
            foreach (object Item in Binding.LookupSource.Items)
                Binding.AddLookupItem(Item);
        }
    }
    static private Binding CreateTwoWayBinding(string ColumnName)
    {
        return new Binding($"[{ColumnName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
    }
    static private void RefreshLookupSelection(DataBinder Binder, ControlBinding Binding)
    {
        if (Binder == null || Binding == null || !Binding.IsLookup)
            return;

        Binding.IsRefreshing = true;
        try
        {
            object Value = Binder[Binding.ColumnName];
            object Item = DataBinderLookupHelper.FindItemByValue(Binding, Value);

            if (Binding.Control is ComboBox ComboBox)
                ComboBox.SelectedItem = Item;
            else if (Binding.Control is ListBox ListBox)
                ListBox.SelectedItem = Item;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    static private void RefreshGridSelection(DataBinder Binder, ControlBinding Binding)
    {
        if (Binder == null || Binding == null)
            return;
        if (Binding.Control is not DataGrid Grid)
            return;

        Binding.IsGridSelectionSyncing = true;
        try
        {
            var Row = Binder.Source.CurrentRow;
            Grid.SelectedItem = Row;

            if (Row != null)
                Grid.ScrollIntoView(Row, null);
        }
        finally
        {
            Binding.IsGridSelectionSyncing = false;
        }
    }
    static private void SubscribeGridColumns(DataGrid Grid, ControlBinding Binding)
    {
        if (Grid == null || Binding == null)
            return;

        Binding.GridColumnChangedHandlers.Clear();

        foreach (DataGridColumn GridColumn in Grid.Columns)
        {
            if (GridColumn.Tag is not DataSourceColumn Column)
                continue;

            EventHandler Handler = (Sender, Args) =>
            {
                DataGridColumn TargetColumn = Grid.FindColumn(Column);
                if (TargetColumn != null)
                    DataGridExtensions.ApplyColumnState(TargetColumn, Column);
            };

            Binding.GridColumnChangedHandlers[Column] = Handler;
            Column.Changed += Handler;

            DataGridExtensions.ApplyColumnState(GridColumn, Column);
        }
    }
    
    // ● static public methods
    static public ControlBinding Bind(DataBinder Binder, TextBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(TextBox.TextProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, CheckBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(ToggleButton.IsCheckedProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, ComboBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(SelectingItemsControl.SelectedItemProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, ListBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(SelectingItemsControl.SelectedItemProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    
    static private ControlBinding BindLookupSelector(DataBinder Binder, SelectingItemsControl Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (LookupSource == null)
            throw new ArgumentNullException(nameof(LookupSource));
        if (string.IsNullOrWhiteSpace(TargetColumnName))
            throw new ArgumentNullException(nameof(TargetColumnName));

        ControlBinding Result = new(Box, TargetColumnName);
        Result.LookupSource = LookupSource;
        Result.LookupSourceName = LookupSource.Name;
        Result.DisplayMember = DisplayMember;
        Result.ValueMember = ValueMember;
        Result.AllowNullSelection = AllowNullSelection;
        Result.NullText = GetNullText(NullText);

        bool AllowsNull = GetAllowNullSelection(Binder, TargetColumnName, AllowNullSelection);

        DataSourceColumn Column = Binder.Source.Columns.FirstOrDefault(x => x.Name.IsSameText(TargetColumnName));
        if (Column != null)
        {
            Column.LookupSource = LookupSource;
            Column.LookupSourceName = LookupSource.Name;
            Column.DisplayMember = DisplayMember;
            Column.ValueMember = ValueMember;
            Column.AllowsNull = AllowsNull;
            Column.NullText = Result.NullText;
        }

        if (AllowsNull)
            Result.LookupNullItem = new LookupNullItem(Result.NullText);

        BuildLookupItems(Result);

        Box.DataContext = null;
        Box.ItemsSource = Result.GetLookupItems();
        Box.ItemTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            return new TextBlock
            {
                Text = DataBinderLookupHelper.GetItemDisplayText(Item, Result)
            };
        });

        EventHandler<SelectionChangedEventArgs> Handler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;

            object Item = Box.SelectedItem;
            object Value = DataBinderLookupHelper.GetItemValue(Item, Result);
            Binder[TargetColumnName] = Value;
        };

        Result.SelectionChangedHandler = Handler;
        Box.SelectionChanged += Handler;

        RefreshLookupSelection(Binder, Result);
        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, ComboBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        return BindLookupSelector(Binder, Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText);
    }
    static public ControlBinding Bind(DataBinder Binder, ListBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        return BindLookupSelector(Binder, Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText);
    }
    
    static public ControlBinding Bind(DataBinder Binder, DatePicker Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(DatePicker.SelectedDateProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, NumericUpDown Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(NumericUpDown.ValueProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, ToggleSwitch Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(ToggleSwitch.IsCheckedProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    
    static public ControlBinding Bind(DataBinder Binder, DataGrid Grid, bool AutoGenerateColumns = true)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Grid == null)
            throw new ArgumentNullException(nameof(Grid));

        ControlBinding Result = new(Grid, string.Empty);

        Grid.ItemsSource = Binder.Source.Rows;

        if (AutoGenerateColumns)
        {
            Grid.AutoGenerateColumns = false;
            Grid.Columns.Clear();
            Grid.AddColumns(Binder.Source, Binder.Source.Columns);
        }

        SubscribeGridColumns(Grid, Result);

        EventHandler<SelectionChangedEventArgs> Handler = (Sender, Args) =>
        {
            if (Result.IsGridSelectionSyncing)
                return;

            Result.IsGridSelectionSyncing = true;
            try
            {
                Binder.Source.CurrentRow = Grid.SelectedItem as DataSourceRow;
            }
            finally
            {
                Result.IsGridSelectionSyncing = false;
            }
        };

        Result.GridSelectionChangedHandler = Handler;
        Grid.SelectionChanged += Handler;

        Result.GridHost = new DataGridHost(Grid);
        Result.GridHost.Initialize();

        RefreshGridSelection(Binder, Result);
        return Result;
    }
 
    static public void Refresh(DataBinder Binder)
    {
        if (Binder == null)
            return;

        foreach (ControlBinding Binding in Binder.Bindings)
            Refresh(Binder, Binding);
    }
    static public void Refresh(DataBinder Binder, ControlBinding Binding)
    {
        if (Binder == null || Binding == null)
            return;

        if (Binding.Control is DataGrid)
            RefreshGridSelection(Binder, Binding);
        else if (Binding.IsLookup)
            RefreshLookupSelection(Binder, Binding);
        else
            Binder.Notify(Binding.ColumnName);
    }
    
}

public interface IInplaceEditorRowProvider
{
    // ● public methods
    int GetRowCount();
    int GetRowIndex(object Row);
    object GetRow(int RowIndex);
    object GetCurrentRow();
}
public interface IInplaceEditorColumnProvider
{
    // ● public methods
    int GetColumnCount();
    int GetColumnIndex(object Column);
    object GetColumn(int ColumnIndex);
    object GetCurrentColumn();
    bool IsColumnVisible(object Column);
    bool IsColumnEditable(object Column);
}
public interface IInplaceEditorCellActivator
{
    // ● public methods
    bool ActivateCell(InplaceEditorCellInfo CellInfo);
}
public class InplaceEditorContext
{
    // ● private fields
    private Control fOwner;
    private IInplaceEditorRowProvider fRowProvider;
    private IInplaceEditorColumnProvider fColumnProvider;
    private IInplaceEditorCellActivator fCellActivator;

    // ● constructors
    public InplaceEditorContext(Control Owner, IInplaceEditorRowProvider RowProvider, IInplaceEditorColumnProvider ColumnProvider, IInplaceEditorCellActivator CellActivator)
    {
        fOwner = Owner;
        fRowProvider = RowProvider;
        fColumnProvider = ColumnProvider;
        fCellActivator = CellActivator;
    }

    // ● public methods
    public bool IsValid()
    {
        return Owner != null && RowProvider != null && ColumnProvider != null && CellActivator != null;
    }

    // ● properties
    public Control Owner => fOwner;
    public IInplaceEditorRowProvider RowProvider => fRowProvider;
    public IInplaceEditorColumnProvider ColumnProvider => fColumnProvider;
    public IInplaceEditorCellActivator CellActivator => fCellActivator;
}
public class InplaceEditorCellInfo
{
    // ● constructors
    public InplaceEditorCellInfo(Control Owner, object Row, object Column)
    {
        this.Owner = Owner;
        this.Row = Row;
        this.Column = Column;
        this.RowIndex = -1;
        this.ColumnIndex = -1;
    }

    // ● public methods
    public override string ToString()
    {
        return $"RowIndex: {RowIndex}, ColumnIndex: {ColumnIndex}";
    }

    // ● properties
    public Control Owner { get; }
    public object Row { get; }
    public object Column { get; }
    public object Value { get; set; }
    public string DisplayText { get; set; }
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public bool IsEditable { get; set; }
}
public abstract class InplaceEditorNavigator
{
    // ● public methods
    public abstract InplaceEditorCellInfo GetCurrentCell(InplaceEditorContext Context);
    public abstract InplaceEditorCellInfo GetNextCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo);
    public abstract InplaceEditorCellInfo GetPreviousCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo);
    public abstract InplaceEditorCellInfo GetUpCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo);
    public abstract InplaceEditorCellInfo GetDownCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo);
    public virtual bool CanEdit(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo)
    {
        return Context != null && CellInfo != null && CellInfo.IsEditable;
    }
    public virtual bool ActivateCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo)
    {
        return Context != null && Context.CellActivator != null && Context.CellActivator.ActivateCell(CellInfo);
    }
}
public class InplaceEditorController
{
    // ● private fields
    private InplaceEditorContext fContext;
    private InplaceEditorNavigator fNavigator;
    private InplaceEditorCellInfo fActiveCell;
    private bool fIsEditing;

    // ● private methods
    private DataGridHost GetHost()
    {
        return Context?.RowProvider as DataGridHost;
    }
    private void SetEditing(InplaceEditorCellInfo CellInfo, bool Value)
    {
        fActiveCell = Value ? CellInfo : null;
        fIsEditing = Value;
    }
    private bool TryActivateCell(InplaceEditorCellInfo CellInfo)
    {
        return Navigator != null && CellInfo != null && Navigator.ActivateCell(Context, CellInfo);
    }
    private bool MoveToCell(InplaceEditorCellInfo CellInfo)
    {
        if (CellInfo == null)
            return false;

        return TryActivateCell(CellInfo);
    }

    // ● constructors
    public InplaceEditorController(InplaceEditorContext Context, InplaceEditorNavigator Navigator)
    {
        fContext = Context;
        fNavigator = Navigator;
    }

    // ● public methods
    public bool CanBeginEdit(InplaceEditorCellInfo CellInfo)
    {
        if (IsEditing)
            return false;
        if (Navigator == null || Context == null || CellInfo == null)
            return false;

        return Navigator.CanEdit(Context, CellInfo);
    }
    public bool BeginEdit(InplaceEditorCellInfo CellInfo)
    {
        if (!CanBeginEdit(CellInfo))
            return false;

        DataGridHost Host = GetHost();
        if (Host == null)
            return false;
        if (!Host.TryPrepareCellForEditing(CellInfo))
            return false;
        if (!Host.BeginGridEdit())
            return false;

        SetEditing(CellInfo, true);
        return true;
    }
    public bool CommitEdit()
    {
        if (!IsEditing)
            return false;

        DataGridHost Host = GetHost();
        if (Host == null)
            return false;
        if (!Host.CommitGridEdit())
            return false;

        SetEditing(null, false);
        return true;
    }
    public void CancelEdit()
    {
        if (!IsEditing)
            return;

        DataGridHost Host = GetHost();
        if (Host != null)
            Host.CancelGridEdit();

        SetEditing(null, false);
    }
    public void HandleExternalEditEnd()
    {
        SetEditing(null, false);
    }
    public bool MoveNext()
    {
        InplaceEditorCellInfo CellInfo = IsEditing ? ActiveCell : GetCurrentCell();
        return MoveToCell(GetNextCell(CellInfo));
    }
    public bool MovePrevious()
    {
        InplaceEditorCellInfo CellInfo = IsEditing ? ActiveCell : GetCurrentCell();
        return MoveToCell(GetPreviousCell(CellInfo));
    }
    public bool MoveUp()
    {
        InplaceEditorCellInfo CellInfo = IsEditing ? ActiveCell : GetCurrentCell();
        return MoveToCell(GetUpCell(CellInfo));
    }
    public bool MoveDown()
    {
        InplaceEditorCellInfo CellInfo = IsEditing ? ActiveCell : GetCurrentCell();
        return MoveToCell(GetDownCell(CellInfo));
    }
    public InplaceEditorCellInfo GetCurrentCell()
    {
        return Navigator != null ? Navigator.GetCurrentCell(Context) : null;
    }
    public InplaceEditorCellInfo GetNextCell(InplaceEditorCellInfo CellInfo)
    {
        return Navigator != null ? Navigator.GetNextCell(Context, CellInfo) : null;
    }
    public InplaceEditorCellInfo GetPreviousCell(InplaceEditorCellInfo CellInfo)
    {
        return Navigator != null ? Navigator.GetPreviousCell(Context, CellInfo) : null;
    }
    public InplaceEditorCellInfo GetUpCell(InplaceEditorCellInfo CellInfo)
    {
        return Navigator != null ? Navigator.GetUpCell(Context, CellInfo) : null;
    }
    public InplaceEditorCellInfo GetDownCell(InplaceEditorCellInfo CellInfo)
    {
        return Navigator != null ? Navigator.GetDownCell(Context, CellInfo) : null;
    }

    // ● properties
    public InplaceEditorContext Context => fContext;
    public InplaceEditorNavigator Navigator => fNavigator;
    public InplaceEditorCellInfo ActiveCell => fActiveCell;
    public bool IsEditing => fIsEditing;
}
public class DataGridHost: IInplaceEditorRowProvider, IInplaceEditorColumnProvider, IInplaceEditorCellActivator
{
    // ● private fields
    private string fPendingEditText;
    private bool fReplacePendingEditText;
    private DataGrid fGrid;
    private InplaceEditorContext fContext;
    private InplaceEditorNavigator fNavigator;
    private InplaceEditorController fController;
    private EventHandler<DataGridCellEditEndedEventArgs> fCellEditEndedHandler;

    // ● private methods
    private static bool IsEditorControl(Control Control)
    {
        return Control is TextBox
               || Control is ComboBox
               || Control is CheckBox;
    }
    private static bool ComesFromEditor(KeyEventArgs e)
    {
        Control Control = e?.Source as Control;

        while (Control != null)
        {
            if (IsEditorControl(Control))
                return true;

            Control = Control.Parent as Control;
        }

        return false;
    }
    private static string GetEditStartText(KeyEventArgs e)
    {
        if (e == null)
            return null;

        return e.Key switch
        {
            >= Key.A and <= Key.Z => e.Key.ToString().ToLowerInvariant(),
            >= Key.D0 and <= Key.D9 => ((char)('0' + (e.Key - Key.D0))).ToString(),
            >= Key.NumPad0 and <= Key.NumPad9 => ((char)('0' + (e.Key - Key.NumPad0))).ToString(),
            Key.Space => " ",
            Key.OemPlus => "+",
            Key.OemMinus => "-",
            Key.OemComma => ",",
            Key.OemPeriod => ".",
            _ => null,
        };
    }
    private bool TryBeginEditCurrentCell(string InitialText = null)
    {
        InplaceEditorCellInfo CellInfo = fController?.GetCurrentCell();
        if (CellInfo == null)
            return false;

        fPendingEditText = InitialText;
        fReplacePendingEditText = !string.IsNullOrEmpty(InitialText);

        bool Result = fController.BeginEdit(CellInfo);

        if (!Result)
        {
            fPendingEditText = null;
            fReplacePendingEditText = false;
        }

        return Result;
    }
    public bool TryConsumePendingEditText(TextBox Editor)
    {
        if (Editor == null)
            return false;

        if (!fReplacePendingEditText)
            return false;

        Editor.Text = fPendingEditText ?? string.Empty;
        Editor.CaretIndex = Editor.Text?.Length ?? 0;
        Editor.SelectionStart = Editor.CaretIndex;
        Editor.SelectionEnd = Editor.CaretIndex;

        fPendingEditText = null;
        fReplacePendingEditText = false;
        return true;
    }
    public void ClearPendingEditText()
    {
        fPendingEditText = null;
        fReplacePendingEditText = false;
    }
 
    private static bool IsEditStartKey(KeyEventArgs e)
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
    private void HookEvents()
    {
        if (fGrid == null)
            return;

        fGrid.AddHandler(InputElement.KeyDownEvent, Grid_KeyDown, RoutingStrategies.Bubble, true);

        fCellEditEndedHandler = (Sender, Args) =>
        {
            ClearPendingEditText();

            if (fController != null)
                fController.HandleExternalEditEnd();
        };
        fGrid.CellEditEnded += fCellEditEndedHandler;
    }
    private void UnhookEvents()
    {
        if (fGrid == null)
            return;

        fGrid.RemoveHandler(InputElement.KeyDownEvent, Grid_KeyDown);

        if (fCellEditEndedHandler != null)
        {
            fGrid.CellEditEnded -= fCellEditEndedHandler;
            fCellEditEndedHandler = null;
        }
    }
    private void Grid_KeyDown(object Sender, KeyEventArgs e)
    {
        if (e == null || e.Handled || fController == null)
            return;

        if (fController.IsEditing)
        {
            if (ComesFromEditor(e))
                return;

            HandleEditModeKeys(e);
            return;
        }

        HandleDisplayModeKeys(e);
    }
    private void HandleDisplayModeKeys(KeyEventArgs e)
    {
        bool Handled = false;

        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && e.Key == Key.Tab)
            Handled = fController.MovePrevious();
        else
        {
            switch (e.Key)
            {
                case Key.Tab:
                    Handled = fController.MoveNext();
                    break;
                case Key.Up:
                    Handled = fController.MoveUp();
                    break;
                case Key.Down:
                    Handled = fController.MoveDown();
                    break;
                case Key.Enter:
                    Handled = fController.MoveDown();
                    break;
                case Key.F2:
                    Handled = TryBeginEditCurrentCell();
                    break;
            }
        }

        if (!Handled && IsEditStartKey(e))
            Handled = TryBeginEditCurrentCell(GetEditStartText(e));

        if (Handled)
            e.Handled = true;
    }
    private void HandleEditModeKeys(KeyEventArgs e)
    {
        bool Handled = false;

        if (e.Key == Key.Escape)
        {
            fController.CancelEdit();
            Handled = true;
        }
        else if (e.KeyModifiers.HasFlag(KeyModifiers.Shift) && e.Key == Key.Tab)
        {
            Handled = fController.CommitEdit();
            if (Handled)
                Handled = fController.MovePrevious();
        }
        else
        {
            switch (e.Key)
            {
                case Key.Tab:
                    Handled = fController.CommitEdit();
                    if (Handled)
                        Handled = fController.MoveNext();
                    break;
                case Key.Enter:
                    Handled = fController.CommitEdit();
                    if (Handled)
                        Handled = fController.MoveDown();
                    break;
            }
        }

        if (Handled)
            e.Handled = true;
    }
    private DataSourceColumn GetDataSourceColumn(DataGridColumn GridColumn)
    {
        return GridColumn != null ? GridColumn.Tag as DataSourceColumn : null;
    }
    private InplaceEditorCellInfo CreateCellInfo(object Row, object Column)
    {
        InplaceEditorCellInfo Result = new(fGrid, Row, Column);

        Result.RowIndex = GetRowIndex(Row);
        Result.ColumnIndex = GetColumnIndex(Column);
        Result.IsEditable = IsColumnEditable(Column);

        if (Row is DataSourceRow DataRow && GetDataSourceColumn(Column as DataGridColumn) is DataSourceColumn DataColumn)
        {
            Result.Value = DataRow[DataColumn.Name];
            Result.DisplayText = Convert.ToString(Result.Value, CultureInfo.InvariantCulture);
        }

        return Result;
    }
    private void FocusCurrentEditor()
    {
        if (fGrid == null)
            return;

        Control Editor = fGrid.GetVisualDescendants()
            .OfType<Control>()
            .FirstOrDefault(x =>
                x.IsVisible &&
                x.IsEffectivelyEnabled &&
                (x is TextBox || x is ComboBox || x is CheckBox));

        if (Editor == null)
            return;

        Editor.Focus();

        if (Editor is TextBox TextEditor)
            TextEditor.SelectAll();
    }

    
    // ● constructors
    public DataGridHost(DataGrid Grid)
    {
        fGrid = Grid ?? throw new ArgumentNullException(nameof(Grid));
    }

    // ● public methods
    public void Initialize()
    {
        if (fContext != null)
            return;

        fContext = new InplaceEditorContext(fGrid, this, this, this);
        fNavigator = new DataGridInplaceEditorNavigator();
        fController = new InplaceEditorController(fContext, fNavigator);
        HookEvents();
    }
    public void FinalizeHost()
    {
        UnhookEvents();
        fController = null;
        fNavigator = null;
        fContext = null;
    }
    public bool TryBeginEditCurrentCell()
    {
        if (fController == null)
            return false;
        if (!TryGetCurrentCell(out InplaceEditorCellInfo CellInfo))
            return false;

        return fController.BeginEdit(CellInfo);
    }
    public bool TryGetCurrentCell(out InplaceEditorCellInfo CellInfo)
    {
        CellInfo = null;

        object Row = GetCurrentRow();
        object Column = GetCurrentColumn();

        if (Row == null || Column == null)
            return false;

        int RowIndex = GetRowIndex(Row);
        int ColumnIndex = GetColumnIndex(Column);

        if (RowIndex < 0 || ColumnIndex < 0)
            return false;

        CellInfo = CreateCellInfo(Row, Column);
        return CellInfo != null;
    }
    public bool TryPrepareCellForEditing(InplaceEditorCellInfo CellInfo)
    {
        if (fGrid == null || CellInfo == null)
            return false;
        if (CellInfo.Row == null || CellInfo.Column is not DataGridColumn GridColumn)
            return false;

        fGrid.SelectedItem = CellInfo.Row;
        fGrid.CurrentColumn = GridColumn;
        fGrid.ScrollIntoView(CellInfo.Row, GridColumn);
        fGrid.Focus();
        fGrid.UpdateLayout();

        return true;
    }
    public bool BeginGridEdit()
    {
        if (fGrid == null)
            return false;

        fGrid.BeginEdit();

        Dispatcher.UIThread.Post(() =>
        {
            FocusCurrentEditor();
        }, DispatcherPriority.Background);

        return true;
    }
    public bool CommitGridEdit()
    {
        return fGrid != null && fGrid.CommitEdit();
    }
    public void CancelGridEdit()
    {
        if (fGrid != null)
            fGrid.CancelEdit();
    }
    public InplaceEditorCellInfo CreateNavigatorCellInfo(object Row, object Column)
    {
        return CreateCellInfo(Row, Column);
    }
    public int GetRowCount()
    {
        if (fGrid?.ItemsSource is not IEnumerable List)
            return 0;

        int Result = 0;
        foreach (object Item in List)
            Result++;

        return Result;
    }
    public int GetRowIndex(object Row)
    {
        if (Row == null || fGrid?.ItemsSource is not IEnumerable List)
            return -1;

        int Index = 0;
        foreach (object Item in List)
        {
            if (ReferenceEquals(Item, Row))
                return Index;

            Index++;
        }

        return -1;
    }
    public object GetRow(int RowIndex)
    {
        if (RowIndex < 0 || fGrid?.ItemsSource is not IEnumerable List)
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
    public object GetCurrentRow()
    {
        return fGrid != null ? fGrid.SelectedItem : null;
    }
    public int GetColumnCount()
    {
        return fGrid != null ? fGrid.Columns.Count : 0;
    }
    public int GetColumnIndex(object Column)
    {
        if (fGrid == null || Column is not DataGridColumn GridColumn)
            return -1;

        return fGrid.Columns.IndexOf(GridColumn);
    }
    public object GetColumn(int ColumnIndex)
    {
        if (fGrid == null || ColumnIndex < 0 || ColumnIndex >= fGrid.Columns.Count)
            return null;

        return fGrid.Columns[ColumnIndex];
    }
    public object GetCurrentColumn()
    {
        return fGrid != null ? fGrid.CurrentColumn : null;
    }
    public bool IsColumnVisible(object Column)
    {
        return Column is DataGridColumn GridColumn && GridColumn.IsVisible;
    }
    public bool IsColumnEditable(object Column)
    {
        if (Column is not DataGridColumn GridColumn)
            return false;
        if (!IsColumnVisible(GridColumn))
            return false;
        if (GridColumn.IsReadOnly)
            return false;

        DataSourceColumn DataColumn = GetDataSourceColumn(GridColumn);
        if (DataColumn != null && DataColumn.ReadOnly)
            return false;

        return true;
    }
    public bool ActivateCell(InplaceEditorCellInfo CellInfo)
    {
        if (fGrid == null || CellInfo == null)
            return false;
        if (CellInfo.RowIndex < 0 || CellInfo.ColumnIndex < 0)
            return false;
        if (CellInfo.RowIndex >= GetRowCount() || CellInfo.ColumnIndex >= GetColumnCount())
            return false;

        object Row = GetRow(CellInfo.RowIndex);
        object Column = GetColumn(CellInfo.ColumnIndex);

        if (Row == null || Column is not DataGridColumn GridColumn)
            return false;

        fGrid.SelectedItem = Row;
        fGrid.CurrentColumn = GridColumn;
        fGrid.ScrollIntoView(Row, GridColumn);
        fGrid.Focus();
        return true;
    }

    // ● properties
    public DataGrid Grid => fGrid;
    public InplaceEditorContext Context => fContext;
    public InplaceEditorNavigator Navigator => fNavigator;
    public InplaceEditorController Controller => fController;
}
public class DataGridInplaceEditorNavigator: InplaceEditorNavigator
{
    // ● private methods
    private static DataGridHost GetHost(InplaceEditorContext Context)
    {
        return Context?.RowProvider as DataGridHost;
    }
    private static bool IsValid(InplaceEditorContext Context)
    {
        DataGridHost Host = GetHost(Context);
        return Host != null && Context != null && Context.IsValid();
    }
    private static bool CanUseCell(InplaceEditorContext Context, int RowIndex, int ColumnIndex)
    {
        if (Context == null)
            return false;
        if (RowIndex < 0 || ColumnIndex < 0)
            return false;
        if (RowIndex >= Context.RowProvider.GetRowCount())
            return false;
        if (ColumnIndex >= Context.ColumnProvider.GetColumnCount())
            return false;

        object Column = Context.ColumnProvider.GetColumn(ColumnIndex);
        if (Column == null)
            return false;
        if (!Context.ColumnProvider.IsColumnVisible(Column))
            return false;
        if (!Context.ColumnProvider.IsColumnEditable(Column))
            return false;

        object Row = Context.RowProvider.GetRow(RowIndex);
        return Row != null;
    }
    private static InplaceEditorCellInfo CreateCellInfo(InplaceEditorContext Context, int RowIndex, int ColumnIndex)
    {
        if (!CanUseCell(Context, RowIndex, ColumnIndex))
            return null;

        DataGridHost Host = GetHost(Context);
        object Row = Context.RowProvider.GetRow(RowIndex);
        object Column = Context.ColumnProvider.GetColumn(ColumnIndex);

        if (Host == null || Row == null || Column == null)
            return null;

        return Host.CreateNavigatorCellInfo(Row, Column);
    }
    private static int FindFirstEditableColumn(InplaceEditorContext Context)
    {
        int ColumnCount = Context.ColumnProvider.GetColumnCount();

        for (int i = 0; i < ColumnCount; i++)
        {
            object Column = Context.ColumnProvider.GetColumn(i);
            if (Column != null &&
                Context.ColumnProvider.IsColumnVisible(Column) &&
                Context.ColumnProvider.IsColumnEditable(Column))
                return i;
        }

        return -1;
    }
    private static int FindLastEditableColumn(InplaceEditorContext Context)
    {
        int ColumnCount = Context.ColumnProvider.GetColumnCount();

        for (int i = ColumnCount - 1; i >= 0; i--)
        {
            object Column = Context.ColumnProvider.GetColumn(i);
            if (Column != null &&
                Context.ColumnProvider.IsColumnVisible(Column) &&
                Context.ColumnProvider.IsColumnEditable(Column))
                return i;
        }

        return -1;
    }
    private static int FindNextEditableColumn(InplaceEditorContext Context, int ColumnIndex)
    {
        int ColumnCount = Context.ColumnProvider.GetColumnCount();

        for (int i = ColumnIndex + 1; i < ColumnCount; i++)
        {
            object Column = Context.ColumnProvider.GetColumn(i);
            if (Column != null &&
                Context.ColumnProvider.IsColumnVisible(Column) &&
                Context.ColumnProvider.IsColumnEditable(Column))
                return i;
        }

        return -1;
    }
    private static int FindPreviousEditableColumn(InplaceEditorContext Context, int ColumnIndex)
    {
        for (int i = ColumnIndex - 1; i >= 0; i--)
        {
            object Column = Context.ColumnProvider.GetColumn(i);
            if (Column != null &&
                Context.ColumnProvider.IsColumnVisible(Column) &&
                Context.ColumnProvider.IsColumnEditable(Column))
                return i;
        }

        return -1;
    }
    private static int FindNextRow(InplaceEditorContext Context, int RowIndex)
    {
        int RowCount = Context.RowProvider.GetRowCount();

        for (int i = RowIndex + 1; i < RowCount; i++)
        {
            object Row = Context.RowProvider.GetRow(i);
            if (Row != null)
                return i;
        }

        return -1;
    }
    private static int FindPreviousRow(InplaceEditorContext Context, int RowIndex)
    {
        for (int i = RowIndex - 1; i >= 0; i--)
        {
            object Row = Context.RowProvider.GetRow(i);
            if (Row != null)
                return i;
        }

        return -1;
    }

    // ● public methods
    public override InplaceEditorCellInfo GetCurrentCell(InplaceEditorContext Context)
    {
        if (!IsValid(Context))
            return null;

        DataGridHost Host = GetHost(Context);
        return Host != null && Host.TryGetCurrentCell(out InplaceEditorCellInfo CellInfo) ? CellInfo : null;
    }
    public override InplaceEditorCellInfo GetNextCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo)
    {
        if (!IsValid(Context) || CellInfo == null)
            return null;

        int TargetColumnIndex = FindNextEditableColumn(Context, CellInfo.ColumnIndex);
        if (TargetColumnIndex >= 0)
            return CreateCellInfo(Context, CellInfo.RowIndex, TargetColumnIndex);

        int TargetRowIndex = FindNextRow(Context, CellInfo.RowIndex);
        if (TargetRowIndex < 0)
            return null;

        TargetColumnIndex = FindFirstEditableColumn(Context);
        return TargetColumnIndex >= 0 ? CreateCellInfo(Context, TargetRowIndex, TargetColumnIndex) : null;
    }
    public override InplaceEditorCellInfo GetPreviousCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo)
    {
        if (!IsValid(Context) || CellInfo == null)
            return null;

        int TargetColumnIndex = FindPreviousEditableColumn(Context, CellInfo.ColumnIndex);
        if (TargetColumnIndex >= 0)
            return CreateCellInfo(Context, CellInfo.RowIndex, TargetColumnIndex);

        int TargetRowIndex = FindPreviousRow(Context, CellInfo.RowIndex);
        if (TargetRowIndex < 0)
            return null;

        TargetColumnIndex = FindLastEditableColumn(Context);
        return TargetColumnIndex >= 0 ? CreateCellInfo(Context, TargetRowIndex, TargetColumnIndex) : null;
    }
    public override InplaceEditorCellInfo GetUpCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo)
    {
        if (!IsValid(Context) || CellInfo == null)
            return null;

        int TargetRowIndex = FindPreviousRow(Context, CellInfo.RowIndex);
        return TargetRowIndex >= 0 && CanUseCell(Context, TargetRowIndex, CellInfo.ColumnIndex)
            ? CreateCellInfo(Context, TargetRowIndex, CellInfo.ColumnIndex)
            : null;
    }
    public override InplaceEditorCellInfo GetDownCell(InplaceEditorContext Context, InplaceEditorCellInfo CellInfo)
    {
        if (!IsValid(Context) || CellInfo == null)
            return null;

        int TargetRowIndex = FindNextRow(Context, CellInfo.RowIndex);
        return TargetRowIndex >= 0 && CanUseCell(Context, TargetRowIndex, CellInfo.ColumnIndex)
            ? CreateCellInfo(Context, TargetRowIndex, CellInfo.ColumnIndex)
            : null;
    }
}

/// <summary>
/// Thin UI facade over a DataSource
/// </summary>
public class DataBinder: INotifyPropertyChanged
{
    // ● private
    DataSource fSource;
    List<ControlBinding> fBindings = new();

    // ● private methods
    void Subscribe()
    {
        if (fSource == null)
            return;

        fSource.CurrentRowChanged += OnCurrentRowChanged;
        fSource.DataChanged += OnDataChanged;
    }
    void Unsubscribe()
    {
        if (fSource == null)
            return;

        fSource.CurrentRowChanged -= OnCurrentRowChanged;
        fSource.DataChanged -= OnDataChanged;
    }
    void OnCurrentRowChanged(object Sender, EventArgs Args)
    {
        RefreshBindings();
    }
    void OnDataChanged(object Sender, EventArgs Args)
    {
        NotifyAll();

        foreach (ControlBinding Binding in fBindings)
        {
            if (Binding.IsLookup)
                DataBinderControlBindingHelper.Refresh(this, Binding);
        }
    }
    
    ControlBinding FindBinding(Control Control)
    {
        return fBindings.Find(Item => ReferenceEquals(Item.Control, Control));
    }

    // ● constructor
    public DataBinder(DataSource Source)
    {
        fSource = Source ?? throw new ArgumentNullException(nameof(Source));
        Subscribe();
    }

    // ● public methods
    public void Bind(TextBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(CheckBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(ComboBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    
    /// <summary>
    /// Default from source metadata: Binder.Bind(cboCustomer, "Customers", "CustomerId", "Name", "Id");
    /// Forced nullable: Binder.Bind(cboCustomer, "Customers", "CustomerId", "Name", "Id", true, "(None)");
    /// Forced non-nullable: Binder.Bind(cboCustomer, "Customers", "CustomerId", "Name", "Id", false);
    /// </summary>
    public void Bind(ComboBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText));
    }
    public void Bind(ComboBox Box, string LookupSourceName, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        if (string.IsNullOrWhiteSpace(LookupSourceName))
            throw new ArgumentNullException(nameof(LookupSourceName));

        ILookupSource LookupSource = Source.Lookups.Get(LookupSourceName);
        Bind(Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText);
    }
    
    public void Bind(ListBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(ListBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText));
    }
    public void Bind(ListBox Box, string LookupSourceName, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        if (string.IsNullOrWhiteSpace(LookupSourceName))
            throw new ArgumentNullException(nameof(LookupSourceName));

        ILookupSource LookupSource = Source.Lookups.Get(LookupSourceName);
        Bind(Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText);
    }
    
    public void Bind(DatePicker Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(NumericUpDown Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(ToggleSwitch Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    
    public void Bind(DataGrid Grid, bool AutoGenerateColumns = true)
    {
        Unbind(Grid);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Grid, AutoGenerateColumns));
    }
 
    public void Unbind(Control Control)
    {
        ControlBinding Binding = FindBinding(Control);
        if (Binding == null)
            return;

        if (Binding.SelectionChangedHandler != null)
        {
            if (Binding.Control is ComboBox ComboBox)
                ComboBox.SelectionChanged -= Binding.SelectionChangedHandler;
            else if (Binding.Control is ListBox ListBox)
                ListBox.SelectionChanged -= Binding.SelectionChangedHandler;
        }

        if (Binding.Control is DataGrid Grid && Binding.GridSelectionChangedHandler != null)
            Grid.SelectionChanged -= Binding.GridSelectionChangedHandler;

        if (Binding.GridHost != null)
        {
            Binding.GridHost.FinalizeHost();
            Binding.GridHost = null;
        }

        if (Binding.GridColumnChangedHandlers.Count > 0)
        {
            foreach (KeyValuePair<DataSourceColumn, EventHandler> Pair in Binding.GridColumnChangedHandlers)
                Pair.Key.Changed -= Pair.Value;

            Binding.GridColumnChangedHandlers.Clear();
        }

        Binding.Control.DataContext = null;
        fBindings.Remove(Binding);
    }
    public void UnbindAll()
    {
        List<ControlBinding> List = new(fBindings);

        foreach (ControlBinding Binding in List)
            Unbind(Binding.Control);
    }
    public void RefreshBindings()
    {
        DataBinderControlBindingHelper.Refresh(this);
    }
    public void RefreshControl(Control Control)
    {
        ControlBinding Binding = FindBinding(Control);
        if (Binding != null)
            DataBinderControlBindingHelper.Refresh(this, Binding);
    }
    public DataSourceRow CreateNew()
    {
        return Source.CreateNew();
    }
    public void Add(DataSourceRow Row)
    {
        Source.Add(Row);
    }
    public void DeleteCurrent()
    {
        Source.DeleteCurrent();
    }
    public void Refresh()
    {
        Source.Refresh();
    }
    public void Notify(string ColumnName)
    {
        OnPropertyChanged("Item");
        OnPropertyChanged(nameof(CurrentRow));
        OnPropertyChanged(nameof(CurrentItem));
    }
    public void NotifyAll()
    {
        OnPropertyChanged("Item");
        OnPropertyChanged(nameof(CurrentRow));
        OnPropertyChanged(nameof(CurrentItem));
    }
    public override string ToString()
    {
        return Source?.ToString() ?? base.ToString();
    }

    // ● properties
    public DataSource Source => fSource;
    public DataSourceRow CurrentRow => Source.CurrentRow;
    public object CurrentItem => Source.CurrentItem;
    public IReadOnlyList<DataSourceColumn> Columns => Source.Columns;
    public IReadOnlyList<ControlBinding> Bindings => fBindings;
    public object this[string ColumnName]
    {
        get => Source[ColumnName];
        set
        {
            Source[ColumnName] = value;
            Notify(ColumnName);
        }
    }

    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
}
