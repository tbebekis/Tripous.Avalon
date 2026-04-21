using System;
using System.Collections;
using System.Data;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace DataGridTest00;

public partial class MainWindow : Window
{
    bool fIsWindowInitialized;
    bool fCurrentIsPoco = true;
    bool fCurrentUseTemplateColumns;
    bool fCurrentUseLookupColumn;
    
    // ● event handlers
    void AnyClick(object Sender, RoutedEventArgs e)
    {
        if (Sender == btnLoadPoco)
            LoadPoco();
        else if (Sender == btnLoadDataTable)
            LoadDataTable();
        else if (Sender == btnClearLog)
            edtLog.Text = string.Empty;
    }
    void AnySelectionChanged(object Sender, RoutedEventArgs e)
    {
        if (!fIsWindowInitialized)
            return;

        RebindGrid();
    }

    // ● private methods
    void WindowInitialize()
    {
        TestData.Initialize(100);

        btnLoadPoco.Click += AnyClick;
        btnLoadDataTable.Click += AnyClick;
        btnClearLog.Click += AnyClick;

        cboColumnType.SelectionChanged += AnySelectionChanged;
        chUseLookupColumn.IsCheckedChanged += AnySelectionChanged;
        chAttachTypist.IsCheckedChanged += AnySelectionChanged;

        gridTest.AutoGenerateColumns = false;
        gridTest.IsReadOnly = false;
        gridTest.CanUserResizeColumns = true;

        HookGridLogEvents();

        cboColumnType.SelectedIndex = 0;
        chUseLookupColumn.IsChecked = true;
        chAttachTypist.IsChecked = false;

        LoadPoco();
    }
    
    void LoadPoco()
    {
        fCurrentIsPoco = true;
        RebindGrid();
    }
    void LoadDataTable()
    {
        fCurrentIsPoco = false;
        RebindGrid();
    }
    void RebindGrid()
    {
        fCurrentUseTemplateColumns = cboColumnType.SelectedIndex == 1;
        fCurrentUseLookupColumn = chUseLookupColumn.IsChecked == true;

        gridTest.Columns.Clear();

        if (fCurrentUseTemplateColumns)
            BuildTemplateColumns();
        else
            BuildBuiltInColumns();

        if (fCurrentIsPoco)
            gridTest.ItemsSource = TestData.Products;
        else
            gridTest.ItemsSource = TestData.tblProduct.DefaultView;

        Log($"Bind: Source={(fCurrentIsPoco ? "POCO" : "DataTable")}, Columns={(fCurrentUseTemplateColumns ? "Template" : "BuiltIn")}, Lookup={fCurrentUseLookupColumn}, Typist={(chAttachTypist.IsChecked == true)}");
    }
    void BuildBuiltInColumns()
    {
        gridTest.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = CreateBinding("Name")
        });
        gridTest.Columns.Add(new DataGridTextColumn
        {
            Header = "Amount",
            Binding = CreateBinding("Amount")
        });
        gridTest.Columns.Add(new DataGridCheckBoxColumn
        {
            Header = "Flag",
            Binding = CreateBinding("Flag")
        });

        if (fCurrentUseLookupColumn)
        {
            gridTest.Columns.Add(CreateLookupColumn("CategoryId", "Category", "Id", "Name"));
        }
        else
        {
            gridTest.Columns.Add(new DataGridTextColumn
            {
                Header = "CategoryId",
                Binding = CreateBinding("CategoryId")
            });
        }
    }
    void BuildTemplateColumns()
    {
        gridTest.Columns.Add(CreateTextTemplateColumn("Name"));
        gridTest.Columns.Add(CreateTextTemplateColumn("Amount"));
        gridTest.Columns.Add(CreateBoolTemplateColumn("Flag"));

        if (fCurrentUseLookupColumn)
            gridTest.Columns.Add(CreateLookupColumn("CategoryId", "Category", "Id", "Name"));
        else
            gridTest.Columns.Add(CreateTextTemplateColumn("CategoryId"));
    }
    void HookGridLogEvents()
    {
        gridTest.AddHandler(InputElement.KeyDownEvent, Grid_KeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);

        gridTest.BeginningEdit += (s, e) =>
        {
            Log($"Grid.BeginningEdit: Column={e.Column?.Header}");
        };
        gridTest.CellEditEnded += (s, e) =>
        {
            Log($"Grid.CellEditEnded: Column={e.Column?.Header}, Action={e.EditAction}");
        };
        gridTest.GotFocus += (s, e) =>
        {
            if (e.Source is Control Control)
                Log($"Focus.Got: {Control.GetType().Name}");
        };
        gridTest.LostFocus += (s, e) =>
        {
            if (e.Source is Control Control)
                Log($"Focus.Lost: {Control.GetType().Name}");
        };
    }
    void Grid_KeyDown(object Sender, KeyEventArgs e)
    {
        string KeyText = $"{e.Key}";
        if (e.KeyModifiers != KeyModifiers.None)
            KeyText = $"{e.KeyModifiers}+{e.Key}";

        string SourceText = e.Source != null ? e.Source.GetType().Name : "null";
        Log($"Grid.KeyDown: Key={KeyText}, Source={SourceText}, Handled={e.Handled}");
    }
    void Log(string Text)
    {
        string S = $"{DateTime.Now:HH:mm:ss.fff}  {Text}";
        edtLog.Text = string.IsNullOrWhiteSpace(edtLog.Text) ? S : edtLog.Text + Environment.NewLine + S;
        edtLog.CaretIndex = edtLog.Text?.Length ?? 0;
    }
    Binding CreateBinding(string PropertyName)
    {
        string Path = fCurrentIsPoco ? PropertyName : $"[{PropertyName}]";

        return new Binding(Path)
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
    }
    static object GetMemberValue(object Instance, string MemberName)
    {
        if (Instance == null)
            return null;

        if (string.IsNullOrWhiteSpace(MemberName))
            return Instance;

        try
        {
            if (Instance is DataRowView RowView)
                return RowView[MemberName];

            if (Instance is DataRow Row)
                return Row[MemberName];

            var Prop = Instance.GetType().GetProperty(MemberName);
            if (Prop != null)
                return Prop.GetValue(Instance);
        }
        catch
        {
        }

        return null;
    }
    static string GetDisplayText(object Instance, string MemberName)
    {
        object Value = GetMemberValue(Instance, MemberName);
        return Value != null ? Convert.ToString(Value, CultureInfo.InvariantCulture) : string.Empty;
    }
    IEnumerable GetLookupItems(string LookupName)
    {
        if (fCurrentIsPoco)
            return TestData.Categories;

        return TestData.tblCategory.DefaultView;
    }
    object FindLookupItem(object Value, IEnumerable Items, string ValueMember)
    {
        if (Items == null)
            return null;

        foreach (object Item in Items)
        {
            object ItemValue = GetMemberValue(Item, ValueMember);
            if (Equals(ItemValue, Value))
                return Item;
        }

        return null;
    }
    string GetLookupDisplayText(object RowItem, string ValueMember, string DisplayMember, IEnumerable Items, string ColumnName)
    {
        object Value = GetMemberValue(RowItem, ColumnName);
        object Item = FindLookupItem(Value, Items, ValueMember);

        if (Item == null)
            return string.Empty;

        return GetDisplayText(Item, DisplayMember);
    }
    DataGridTemplateColumn CreateTextTemplateColumn(string ColumnName)
    {
        DataGridTemplateColumn Result = new()
        {
            Header = ColumnName
        };

        Result.CellTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBlock Text = new()
            {
                Margin = new Avalonia.Thickness(6, 2, 6, 2),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            Text.Bind(TextBlock.TextProperty, CreateBinding(ColumnName));
            return Text;
        });

        Result.CellEditingTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBox Editor = new()
            {
                Margin = new Avalonia.Thickness(2),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            Editor.Bind(TextBox.TextProperty, CreateBinding(ColumnName));

            Editor.AttachedToVisualTree += (s, e) =>
            {
                Log($"Editor.Attached: TextBox({ColumnName})");
            };
            Editor.GotFocus += (s, e) =>
            {
                Log($"Editor.GotFocus: TextBox({ColumnName})");
            };
            Editor.LostFocus += (s, e) =>
            {
                Log($"Editor.LostFocus: TextBox({ColumnName})");
            };
            Editor.KeyDown += (s, e) =>
            {
                Log($"Editor.KeyDown: TextBox({ColumnName}), Key={e.Key}, Handled={e.Handled}");
            };

            return Editor;
        });

        return Result;
    }
    DataGridTemplateColumn CreateBoolTemplateColumn(string ColumnName)
    {
        DataGridTemplateColumn Result = new()
        {
            Header = ColumnName
        };

        Result.CellTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            CheckBox Box = new()
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                IsHitTestVisible = false,
                Focusable = false
            };

            Box.Bind(ToggleButton.IsCheckedProperty, CreateBinding(ColumnName));
            return Box;
        });

        Result.CellEditingTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            CheckBox Box = new()
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            Box.Bind(ToggleButton.IsCheckedProperty, CreateBinding(ColumnName));

            Box.AttachedToVisualTree += (s, e) =>
            {
                Log($"Editor.Attached: CheckBox({ColumnName})");
            };
            Box.GotFocus += (s, e) =>
            {
                Log($"Editor.GotFocus: CheckBox({ColumnName})");
            };
            Box.LostFocus += (s, e) =>
            {
                Log($"Editor.LostFocus: CheckBox({ColumnName})");
            };
            Box.KeyDown += (s, e) =>
            {
                Log($"Editor.KeyDown: CheckBox({ColumnName}), Key={e.Key}, Handled={e.Handled}");
            };

            return Box;
        });

        return Result;
    }
    DataGridTemplateColumn CreateLookupColumn(string ColumnName, string LookupName, string ValueMember, string DisplayMember)
    {
        DataGridTemplateColumn Result = new()
        {
            Header = ColumnName
        };

        Result.CellTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            TextBlock Text = new()
            {
                Margin = new Avalonia.Thickness(6, 2, 6, 2),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Text = GetLookupDisplayText(Item, ValueMember, DisplayMember, GetLookupItems(LookupName), ColumnName)
            };

            return Text;
        });

        Result.CellEditingTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            ComboBox Box = new()
            {
                Margin = new Avalonia.Thickness(0),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                ItemsSource = GetLookupItems(LookupName)
            };

            Box.ItemTemplate = new FuncDataTemplate<object>((LookupItem, _) =>
            {
                return new TextBlock
                {
                    Text = GetDisplayText(LookupItem, DisplayMember),
                    Margin = new Avalonia.Thickness(4, 2, 4, 2)
                };
            });

            Box.SelectionBoxItemTemplate = Box.ItemTemplate;

            object CurrentValue = GetMemberValue(Item, ColumnName);
            Box.SelectedItem = FindLookupItem(CurrentValue, GetLookupItems(LookupName), ValueMember);

            Box.SelectionChanged += (s, e) =>
            {
                object Value = GetMemberValue(Box.SelectedItem, ValueMember);

                if (Item is Product Product)
                    Product.CategoryId = Value != null ? Convert.ToInt32(Value, CultureInfo.InvariantCulture) : 0;
                else if (Item is DataRowView RowView)
                    RowView[ColumnName] = Value ?? DBNull.Value;
            };

            Box.AttachedToVisualTree += (s, e) =>
            {
                Log($"Editor.Attached: ComboBox({ColumnName})");
            };
            Box.GotFocus += (s, e) =>
            {
                Log($"Editor.GotFocus: ComboBox({ColumnName})");
            };
            Box.LostFocus += (s, e) =>
            {
                Log($"Editor.LostFocus: ComboBox({ColumnName})");
            };
            Box.KeyDown += (s, e) =>
            {
                Log($"Editor.KeyDown: ComboBox({ColumnName}), Key={e.Key}, Handled={e.Handled}");
            };

            return Box;
        });

        return Result;
    }



    // ● construction
    public MainWindow()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            if (fIsWindowInitialized)
                return;

            WindowInitialize();
            fIsWindowInitialized = true;
        };
    }
}