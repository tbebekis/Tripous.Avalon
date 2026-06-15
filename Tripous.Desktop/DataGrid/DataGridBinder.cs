/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;
 
public static class DataGridBinder
{
    class BoolDisplayConverter: IValueConverter
    {
        // ● public methods
        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            bool Flag = false;
            if (!Sys.IsNull(Value))
            {
                try
                {
                    if (Value is bool B)
                        Flag = B;
                    else
                        Flag = System.Convert.ToInt32(Value, CultureInfo.InvariantCulture) != 0;
                }
                catch
                {
                    Flag = System.Convert.ToString(Value, CultureInfo.InvariantCulture).IsSameText("true");
                }
            }
            return Flag ? "x" : string.Empty;
        }
        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            return Avalonia.Data.BindingOperations.DoNothing;
        }
    }

    static Thickness GetCellPadding()
    {
        return new Thickness(6, 2, 6, 2);
    }

    static object GetValue(DataRowView RowView, string ColumnName)
    {
        if (RowView == null || RowView.Row == null || RowView.Row.RowState.In(DataRowState.Deleted | DataRowState.Detached))
            return null;

        try
        {
            object Result = RowView[ColumnName];
            return Result == DBNull.Value ? null : Result;
        }
        catch (RowNotInTableException)
        {
            return null;
        }
    }
    static void SetValue(DataRowView RowView, string ColumnName, object Value)
    {
        if (RowView == null)
            return;

        DataColumn Column = RowView.Row.Table.Columns[ColumnName];
        Type DataType = Column.DataType;

        if (Value == null || Value == DBNull.Value)
        {
            RowView[ColumnName] = DBNull.Value;
            return;
        }

        try
        {
            object Result;

            if (DataType == typeof(string))
                Result = Convert.ToString(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(int))
                Result = Convert.ToInt32(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(long))
                Result = Convert.ToInt64(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(decimal))
                Result = Convert.ToDecimal(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(double))
                Result = Convert.ToDouble(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(float))
                Result = Convert.ToSingle(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(bool))
                Result = Convert.ToBoolean(Value, CultureInfo.CurrentCulture);
            else if (DataType == typeof(DateTime))
                Result = Convert.ToDateTime(Value, CultureInfo.CurrentCulture);
            else
                Result = Value;

            //RowView[ColumnName] = Result;
            object Current = RowView[ColumnName];

            if (!object.Equals(Current, Result))
                RowView[ColumnName] = Result;
        }
        catch
        {
            // ● ignore invalid input for now
        }
    }
    static bool AsBoolean(object Value)
    {
        if (Sys.IsNull(Value))
            return false;

        try
        {
            if (Value is bool B)
                return B;
            return System.Convert.ToInt32(Value, CultureInfo.InvariantCulture) != 0;
        }
        catch
        {
            return System.Convert.ToString(Value, CultureInfo.InvariantCulture).IsSameText("true");
        }
    }
    static void SetBooleanValue(DataRowView RowView, string ColumnName, bool Value)
    {
        if (RowView == null || RowView.Row == null || RowView.Row.RowState.In(DataRowState.Deleted | DataRowState.Detached))
            return;

        DataColumn Column = RowView.Row.Table.Columns[ColumnName];
        object NewValue = Column.DataType == typeof(bool) ? Value : System.Convert.ChangeType(Value ? 1 : 0, Column.DataType, CultureInfo.InvariantCulture);
        object Current = RowView[ColumnName];

        if (!object.Equals(Current, NewValue))
        {
            RowView.BeginEdit();
            RowView[ColumnName] = NewValue;
        }
    }
    static void RestoreGridCellFocus(DataGrid Grid, object SelectedItem, DataGridColumn CurrentColumn)
    {
        if (Grid == null || SelectedItem == null || CurrentColumn == null)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            Grid.SelectedItem = SelectedItem;
            Grid.CurrentColumn = CurrentColumn;
            Grid.ScrollIntoView(SelectedItem, CurrentColumn);

            Dispatcher.UIThread.Post(() =>
            {
                foreach (DataGridCell Cell in Grid.GetVisualDescendants().OfType<DataGridCell>())
                {
                    if (!object.Equals(Cell.DataContext, SelectedItem))
                        continue;
                    if (DataGridColumn.GetColumnContainingElement(Cell) != CurrentColumn)
                        continue;

                    Cell.Focus(NavigationMethod.Tab, KeyModifiers.None);
                    break;
                }
            }, DispatcherPriority.Input);
        }, DispatcherPriority.Background);
    }
    static void SetLookupSelectedItem(ComboBox ComboBox, LookupSource LookupSource, object Value)
    {
        ComboBox.SelectedItem = null;
        ComboBox.SelectedIndex = -1;
        ComboBox.SelectedItem = LookupSource.FindItem(Value);
    }
    // Display templates listen to ColumnChanged because recycled cells may not refresh after programmatic row updates.
    // Edit templates do not listen because refreshing while editing can overwrite typing or selection state.
 
    static IDataTemplate CreateTextDisplayTemplate(string ColumnName, TextAlignment Alignment, string Format, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();
            DataRowView CurrentItem = null;
            DataColumnChangeEventHandler Handler = null;

            void Refresh()
            {
                object Value = GetValue(CurrentItem, ColumnName);
                Result.Text = FormatValue(Value, Format);
            }
            void SetCurrentItem(DataRowView RowView)
            {
                if (CurrentItem?.Row?.Table != null)
                    CurrentItem.Row.Table.ColumnChanged -= Handler;

                CurrentItem = RowView;
                if (CurrentItem?.Row?.Table != null)
                    CurrentItem.Row.Table.ColumnChanged += Handler;

                Refresh();
            }

            Handler = (Sender, Args) =>
            {
                if (CurrentItem == null || Args.Row != CurrentItem.Row || !Args.Column.ColumnName.IsSameText(ColumnName))
                    return;

                Refresh();
            };

            SetCurrentItem(Item);
            Result.DataContextChanged += (Sender, Args) => SetCurrentItem(Result.DataContext as DataRowView);
            Result.DetachedFromVisualTree += (Sender, Args) =>
            {
                if (CurrentItem?.Row?.Table != null)
                    CurrentItem.Row.Table.ColumnChanged -= Handler;
                CurrentItem = null;
            };

            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = Alignment;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateTextEditTemplate(string ColumnName, TextAlignment Alignment, string Format, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBox Result = new();
            bool IsLoading = true;

            object Value = GetValue(Item, ColumnName);
            Result.Text = FormatValue(Value, Format);

            Result.Padding = GetCellPadding();
            Result.VerticalContentAlignment = VerticalAlignment.Center;
            Result.TextAlignment = Alignment;

            Result.TextChanged += (Sender, Args) =>
            {
                if (IsLoading)
                    return;

                SetValue(Item, ColumnName, Result.Text);
            };

            EventHandler<VisualTreeAttachmentEventArgs> AttachedHandler = null;
            AttachedHandler = (Sender, Args) =>
            {
                Result.AttachedToVisualTree -= AttachedHandler;

                Dispatcher.UIThread.Post(() =>
                {
                    Item?.BeginEdit();
                    IsLoading = false;
                    Result.Focus(NavigationMethod.Tab, KeyModifiers.None);
                    Dispatcher.UIThread.Post(() => Result.SelectAll(), DispatcherPriority.Input);
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;

            return Result;
        }, SupportsRecycling);
    }
    
    static IDataTemplate CreateLookupDisplayTemplate(string ColumnName, LookupSource LookupSource, GridColumnBinding Binding, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();
            DataRowView CurrentItem = null;
            DataColumnChangeEventHandler Handler = null;

            LookupSource GetLookupSource() => Binding?.LookupSource ?? LookupSource;
            void Refresh()
            {
                object Value = GetValue(CurrentItem, ColumnName);
                LookupItem LookupItem = GetLookupSource()?.FindItem(Value);
                Result.Text = LookupItem?.DisplayText ?? string.Empty;
            }
            void SetCurrentItem(DataRowView RowView)
            {
                if (CurrentItem?.Row?.Table != null)
                    CurrentItem.Row.Table.ColumnChanged -= Handler;

                CurrentItem = RowView;
                if (CurrentItem?.Row?.Table != null)
                    CurrentItem.Row.Table.ColumnChanged += Handler;

                Refresh();
            }

            Handler = (Sender, Args) =>
            {
                if (CurrentItem == null || Args.Row != CurrentItem.Row || !Args.Column.ColumnName.IsSameText(ColumnName))
                    return;

                Refresh();
            };

            SetCurrentItem(Item);
            Result.DataContextChanged += (Sender, Args) => SetCurrentItem(Result.DataContext as DataRowView);
            Result.DetachedFromVisualTree += (Sender, Args) =>
            {
                if (CurrentItem?.Row?.Table != null)
                    CurrentItem.Row.Table.ColumnChanged -= Handler;
                CurrentItem = null;
            };
            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = TextAlignment.Left;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateLookupEditTemplate(string ColumnName, LookupSource LookupSource, GridColumnBinding Binding, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            ComboBox Result = new();
            bool IsLoading = true;
            DataRowView CurrentItem = Item;
            if (Binding != null)
                Binding.ActiveLookupComboBox = Result;

            LookupSource GetLookupSource() => Binding?.LookupSource ?? LookupSource;
            Result.ItemsSource = GetLookupSource().GetList();
            SetLookupSelectedItem(Result, GetLookupSource(), GetValue(CurrentItem, ColumnName));
            Result.Padding = new Thickness(0);
            Result.Margin = new Thickness(0);
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.VerticalAlignment = VerticalAlignment.Stretch;
            Result.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Result.VerticalContentAlignment = VerticalAlignment.Center;
            Result.MinHeight = 0;

            Result.ItemTemplate = new FuncDataTemplate<LookupItem>((LookupItem, _) =>
            {
                TextBlock Text = new();

                Text.Text = LookupItem?.DisplayText ?? string.Empty;
                Text.Padding = GetCellPadding();
                Text.TextAlignment = TextAlignment.Left;
                Text.VerticalAlignment = VerticalAlignment.Center;
                Text.HorizontalAlignment = HorizontalAlignment.Stretch;

                return Text;
            }, SupportsRecycling);

            EventHandler<VisualTreeAttachmentEventArgs> AttachedHandler = null;
            AttachedHandler = (Sender, Args) =>
            {
                Result.AttachedToVisualTree -= AttachedHandler;

                Dispatcher.UIThread.Post(() =>
                {
                    CurrentItem = Result.DataContext as DataRowView ?? CurrentItem;
                    CurrentItem?.BeginEdit();
                    Result.ItemsSource = GetLookupSource().GetList();
                    SetLookupSelectedItem(Result, GetLookupSource(), GetValue(CurrentItem, ColumnName));
                    IsLoading = false;

                    Result.Focus();
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;
            Result.DetachedFromVisualTree += (Sender, Args) =>
            {
                if (Binding != null && ReferenceEquals(Binding.ActiveLookupComboBox, Result))
                    Binding.ActiveLookupComboBox = null;
            };
            Result.DataContextChanged += (Sender, Args) =>
            {
                CurrentItem = Result.DataContext as DataRowView ?? CurrentItem;
                IsLoading = true;
                Result.ItemsSource = GetLookupSource().GetList();
                SetLookupSelectedItem(Result, GetLookupSource(), GetValue(CurrentItem, ColumnName));
                IsLoading = false;
            };
 
            void CommitSelection()
            {
                if (IsLoading)
                    return;

                if (Result.SelectedItem is LookupItem SelectedItem)
                {
                    object CurrentValue = GetValue(CurrentItem, ColumnName);
                    object NewValue = SelectedItem.Value;
                    if (Equals(CurrentValue, NewValue) || Convert.ToString(CurrentValue, CultureInfo.InvariantCulture) == Convert.ToString(NewValue, CultureInfo.InvariantCulture))
                        return;

                    CurrentItem?.BeginEdit();
                    SetValue(CurrentItem, ColumnName, NewValue);
                    Binding?.FieldDef?.TableDef?.AssignLookupSnapshots(CurrentItem?.Row, Binding.FieldDef, GetLookupSource(), SelectedItem);

                    DataGrid Grid = Result.FindAncestorOfType<DataGrid>();
                    object SelectedRow = Grid?.SelectedItem;
                    DataGridColumn CurrentColumn = Grid?.CurrentColumn;
                    Grid?.CommitEdit(DataGridEditingUnit.Cell, true);
                    RestoreGridCellFocus(Grid, SelectedRow, CurrentColumn);
                }
            }
            Result.SelectionChanged += (Sender, Args) =>
            {
                CommitSelection();
            };

            Result.DropDownOpened += (Sender, Args) =>
            {
                IsLoading = true;
                CurrentItem = Result.DataContext as DataRowView ?? CurrentItem;
                Result.ItemsSource = GetLookupSource().GetList();
                SetLookupSelectedItem(Result, GetLookupSource(), GetValue(CurrentItem, ColumnName));
                IsLoading = false;
            };

            Result.AddHandler(InputElement.KeyDownEvent, (Sender, Args) =>
            {
                if (!Result.IsDropDownOpen && Args.Key == Key.Down && Args.KeyModifiers.HasFlag(KeyModifiers.Alt))
                {
                    Result.IsDropDownOpen = true;
                    Args.Handled = true;
                    return;
                }

                if (Args.Key != Key.Escape)
                    return;

                if (Result.IsDropDownOpen)
                {
                    Result.IsDropDownOpen = false;
                    Args.Handled = true;
                    return;
                }

                DataGrid Grid = Result.FindAncestorOfType<DataGrid>();

                Item?.CancelEdit();
                Grid?.CancelEdit();

                Args.Handled = true;
            }, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateLocatorEditTemplate(string ColumnName, FieldDef FieldDef, LocatorDef LocatorDef, LocatorFieldDef LocatorFieldDef, Dictionary<string, string> TargetFieldMap, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            GridLocatorBox Result = new();
            object Value = GetValue(Item, ColumnName);
            Result.Initialize(LocatorDef, LocatorFieldDef, Item, FieldDef.Name, TargetFieldMap);
            Result.SetText(FormatValue(Value, null));

            EventHandler<VisualTreeAttachmentEventArgs> AttachedHandler = null;
            AttachedHandler = (Sender, Args) =>
            {
                Result.AttachedToVisualTree -= AttachedHandler;

                Dispatcher.UIThread.Post(() =>
                {
                    Item?.BeginEdit();
                    Result.FocusEditor();
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;

            return Result;
        }, SupportsRecycling);
    }
    
    static IDataTemplate CreateBoolDisplayTemplate(string ColumnName, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            TextBlock Result = new();
            object Value = GetValue(Item, ColumnName);
            Result.Text = new BoolDisplayConverter().Convert(Value, typeof(string), null, CultureInfo.CurrentCulture)?.ToString();

            Result.Padding = GetCellPadding();
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.HorizontalAlignment = HorizontalAlignment.Stretch;
            Result.TextAlignment = TextAlignment.Center;

            return Result;
        }, SupportsRecycling);
    }
    static IDataTemplate CreateBoolEditTemplate(string ColumnName, bool SupportsRecycling)
    {
        return new FuncDataTemplate<DataRowView>((Item, _) =>
        {
            CheckBox Result = new();
            bool IsLoading = true;

            Result.IsChecked = AsBoolean(GetValue(Item, ColumnName));
            Result.HorizontalAlignment = HorizontalAlignment.Center;
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.IsCheckedChanged += (Sender, Args) =>
            {
                if (IsLoading)
                    return;

                SetBooleanValue(Item, ColumnName, Result.IsChecked == true);

                DataGrid Grid = Result.FindAncestorOfType<DataGrid>();
                object SelectedItem = Grid?.SelectedItem;
                DataGridColumn CurrentColumn = Grid?.CurrentColumn;
                Grid?.CommitEdit(DataGridEditingUnit.Cell, true);
                RestoreGridCellFocus(Grid, SelectedItem, CurrentColumn);
            };

            EventHandler<VisualTreeAttachmentEventArgs> AttachedHandler = null;
            AttachedHandler = (Sender, Args) =>
            {
                Result.AttachedToVisualTree -= AttachedHandler;

                Dispatcher.UIThread.Post(() =>
                {
                    Item?.BeginEdit();
                    IsLoading = false;
                    Result.Focus();
                }, DispatcherPriority.Input);
            };

            Result.AttachedToVisualTree += AttachedHandler;

            Result.KeyDown += (Sender, Args) =>
            {
                if (Args.Key != Key.Escape)
                    return;

                DataGrid Grid = Result.FindAncestorOfType<DataGrid>();

                Item?.CancelEdit();
                Grid?.CancelEdit();

                Args.Handled = true;
            };

            return Result;
        }, SupportsRecycling);
    }

    static public string GetHeader(string Header)
    {
        if (!string.IsNullOrWhiteSpace(Header))
        {
            List<string> WordList = Header.SplitToWordList();
            
            if (WordList.Count == 1)
            {
                Header = WordList[0];
            }
            else
            {
                if ("Id".IsSameText(WordList[WordList.Count -1]))
                    WordList.RemoveAt(WordList.Count - 1);
                Header = string.Join(" ", WordList);
            }
        }
        return Header;
    }
    static public string GetHeader(string ColumnName, string Header) => string.IsNullOrWhiteSpace(Header)? GetHeader(ColumnName) : GetHeader(Header);
 
    static public string FormatValue(object Value, string Format)
    {
        if (Value == null)
            return string.Empty;
        if (!string.IsNullOrWhiteSpace(Format))
            return string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", Value);
        return string.Format(CultureInfo.CurrentCulture, "{0}", Value);
    }
    static public string GetDateAwareFormat(string FieldName, Type DataType, string Format)
    {
        if (DataType != typeof(DateTime))
            return Format;
        if (FieldName.EndsWithText("Date"))
            return Sys.Settings.DateFormat;
        if (FieldName.EndsWithText("DateTime") || FieldName.EndsWithText("DT"))
            return Sys.Settings.DateTimeFormat;
        return Format;
    }
    static public string GetDateAwareFormat(FieldDef FieldDef)
    {
        string Format = FieldDef.DisplayFormat;
        if (!FieldDef.DataType.IsDateTime())
            return Format;
        if (FieldDef.Name.EndsWithText("Date"))
            return Sys.Settings.DateFormat;
        if (FieldDef.Name.EndsWithText("DateTime") || FieldDef.Name.EndsWithText("DT"))
            return Sys.Settings.DateTimeFormat;
        return Format;
    }
 
    // ● private - create columns
    static DataGridColumn CreateTextColumn(string ColumnName, string Header = "", string Format = null, TextAlignment? Alignment = null, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        TextAlignment Align = Alignment ?? TextAlignment.Left;

        Result.Header = GetHeader(ColumnName, Header);  
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateTextDisplayTemplate(ColumnName, Align, Format, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateTextEditTemplate(ColumnName, Align, Format, SupportsRecycling);

        return Result;
    }
    static DataGridColumn CreateBoolColumn(string ColumnName, string Header = "", bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        DataGridTemplateColumn Result = new();

        Result.Header = GetHeader(ColumnName, Header);
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateBoolDisplayTemplate(ColumnName, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateBoolEditTemplate(ColumnName, SupportsRecycling);

        return Result;
    }
    static void ConfigureLookupColumn(DataGridTemplateColumn Column, string ColumnName, LookupSource LookupSource, GridColumnBinding Binding, string Header = "", bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        Column.Header = GetHeader(ColumnName, Header); 
        Column.IsReadOnly = IsReadOnly;
        Column.CellTemplate = CreateLookupDisplayTemplate(ColumnName, LookupSource, Binding, SupportsRecycling);
        Column.CellEditingTemplate = IsReadOnly ? null : CreateLookupEditTemplate(ColumnName, LookupSource, Binding, SupportsRecycling);
    }
    
    // ● static public
    static public List<DataGridColumn> BindGrid(SelectDef SelectDef, DataGrid Grid, DataView DataView, bool SupportsRecycling = false, bool GoToFirst = true)
    {
        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = null;
        Grid.Columns.Clear();

        DataColumn[] DataColumns = DataView.Table.Columns.Cast<DataColumn>().ToArray();

        if (SelectDef != null)
        {
            foreach (DataColumn Column in DataColumns)
            {
                if (SelectDef.DisplayLabels.TryGetValue(Column.ColumnName, out string Label))
                    Column.Caption = Label;

                if (SelectDef.ColumnTypes.TryGetValue(Column.ColumnName, out DataColumnType ColumnType))
                    Column.ExtendedProperties["ColumnType"] = ColumnType;
            }
        }

        List<DataGridColumn> Result = CreateColumns(Grid, DataColumns, SupportsRecycling);

        Grid.ItemsSource = new DataViewItemsSource(DataView);

        if (GoToFirst && DataView.Count > 0)
            Grid.SelectedItem = DataView[0];

        return Result;
    }
    static public List<DataGridColumn> BindGrid(DataGrid Grid, DataView DataView, bool SupportsRecycling = false, bool GoToFirst = true)
    {
        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = null;
        Grid.Columns.Clear();

        DataColumn[] DataColumns = DataView.Table.Columns.Cast<DataColumn>().ToArray();

        List<DataGridColumn> Result = CreateColumns(Grid, DataColumns, SupportsRecycling);
 
        Grid.ItemsSource = new DataViewItemsSource(DataView);

        if (GoToFirst && DataView.Count > 0)
            Grid.SelectedItem = DataView[0];

        return Result;
    }
    static public void UnBindGrid(DataGrid Grid)
    {
        Grid.ItemsSource = null;
        Grid.Columns.Clear();
    }
    
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, DataTable Table, bool SupportsRecycling = false) => CreateColumns(Grid, Table.Columns.Cast<DataColumn>().ToArray(), SupportsRecycling);
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, DataColumn[] DataColumns, bool SupportsRecycling = false)
    {
        List<DataGridColumn> Result = new();
        DataGridColumn GridColumn;
        
        foreach (DataColumn Column in DataColumns)
        {
            GridColumn = CreateGridColumn(Column,
                Format: Column.DataType.GetDefaultFormat(),
                Alignment: Column.DataType.GetTextAlignment(),
                IsReadOnly: Column.ReadOnly,
                SupportsRecycling: SupportsRecycling);
            
            Result.Add(GridColumn);
        }
        
        Grid.Columns.AddRange(Result);
        return Result;
    }  
    
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, TableDef TableDef, bool SupportsRecycling = false) => CreateColumns(Grid, TableDef.Fields.ToArray(), SupportsRecycling);
    static public List<DataGridColumn> CreateColumns(DataGrid Grid, FieldDef[] FieldDefs, bool SupportsRecycling = false)
    {
        List<DataGridColumn> Result = new();
        DataGridColumn GridColumn;
        
        foreach (FieldDef FieldDef in FieldDefs)
        {
            GridColumn = CreateGridColumn(FieldDef, SupportsRecycling: SupportsRecycling);
            Result.Add(GridColumn);
        }

        Grid.Columns.AddRange(Result);
        return Result;
    }

    static public DataGridColumn CreateGridColumn(DataColumn Column, string Format = null, TextAlignment? Alignment = null, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        Format = GetDateAwareFormat(Column.ColumnName, Column.DataType, Format);

        DataColumnType ColumnType = Column.ExtendedProperties.ContainsKey("ColumnType")
            ? (DataColumnType)Column.ExtendedProperties["ColumnType"]
            : DataColumnType.None;

        if (ColumnType.HasFlag(DataColumnType.Integer))
            Format = "0";

        bool IsBoolean = ColumnType.HasFlag(DataColumnType.Boolean)
                         || Column.DataType == typeof(bool)
                         || Column.IsCheckBox();
        
        TextAlignment Align = TextAlignment.Left;
        if (Alignment.HasValue)
            Align = Alignment.Value;
        else
            Align = IsBoolean ? TextAlignment.Center : Column.DataType.GetTextAlignment();

        string Caption = Texts.L(Column.Caption);
        Caption = GetHeader(Column.ColumnName, Caption);
        
        DataGridColumn Result = null;
        if (IsBoolean)
            Result = CreateBoolColumn(Column.ColumnName, Header: Caption, IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);
        else
            Result = CreateTextColumn(Column.ColumnName, Header: Caption, Format: Format, Alignment: Align, IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);
 
        GridColumnBinding CI = new GridColumnBinding(Result, Column);
        Result.Tag = CI;

        return Result;
    }
    static public DataGridColumn CreateGridColumn(string ColumnName, string Header, DataFieldType DataType, string Format = null, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
  
        bool IsBoolean = DataType == DataFieldType.Boolean;
        TextAlignment Align = IsBoolean ? TextAlignment.Center : DataType.GetTextAlignment();
        Type NetType = DataType.GetNetType();
        
        string Caption =  GetHeader(ColumnName, Header);

        DataGridColumn Result = null;
        if (IsBoolean)
            Result = CreateBoolColumn(ColumnName, Header: Caption, IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);
        else
            Result = CreateTextColumn(ColumnName, Header: Caption, Format: GetDateAwareFormat(ColumnName, NetType, Format), Alignment: Align, IsReadOnly: IsReadOnly, SupportsRecycling: SupportsRecycling);
 
        GridColumnBinding CI = new GridColumnBinding(Result, ColumnName, NetType);
        Result.Tag = CI;

        return Result;
    }
    static public DataGridColumn CreateLocatorColumn(string ColumnName, string Header, FieldDef FieldDef, LocatorDef LocatorDef, LocatorFieldDef LocatorFieldDef, Dictionary<string, string> TargetFieldMap, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        TextAlignment Align = LocatorFieldDef.DataType.GetTextAlignment();

        DataGridTemplateColumn Result = new();
        Result.Header = GetHeader(ColumnName, Header);
        Result.IsReadOnly = IsReadOnly;
        Result.CellTemplate = CreateTextDisplayTemplate(ColumnName, Align, null, SupportsRecycling);
        Result.CellEditingTemplate = IsReadOnly ? null : CreateLocatorEditTemplate(ColumnName, FieldDef, LocatorDef, LocatorFieldDef, TargetFieldMap, SupportsRecycling);

        GridColumnBinding CI = new GridColumnBinding(Result, FieldDef.Name, FieldDef.DataType.GetNetType());
        CI.FieldDef = FieldDef;
        CI.DisplayFieldName = ColumnName;
        CI.LocatorDef = LocatorDef;
        CI.Locator = LocatorDef.Create();
        CI.LocatorTargetFieldMap = TargetFieldMap;
        Result.Tag = CI;

        return Result;
    }
    static public DataGridColumn CreateGridColumn(FieldDef FieldDef, bool SupportsRecycling = false)
    {
        bool IsBoolean = FieldDef.IsBoolean;
        TextAlignment Align = IsBoolean ? TextAlignment.Center : FieldDef.DataType.GetTextAlignment();
 
        string Caption = GetHeader(FieldDef.Name, FieldDef.Title); 
        DataGridColumn Result = null;
        
        if (IsBoolean)
            Result = CreateBoolColumn(FieldDef.Name, Header: FieldDef.Title, IsReadOnly: FieldDef.IsReadOnly || FieldDef.IsReadOnlyUI, SupportsRecycling: SupportsRecycling);
        else
            Result = CreateTextColumn(FieldDef.Name, Header: FieldDef.Title, Format: GetDateAwareFormat(FieldDef), Alignment: Align, IsReadOnly: FieldDef.IsReadOnly || FieldDef.IsReadOnlyUI, SupportsRecycling: SupportsRecycling);
 
        GridColumnBinding CI = new GridColumnBinding(Result, FieldDef);
        Result.Tag = CI;

        return Result;                  
    }
    
    static public DataGridColumn CreateLookupColumn(DataColumn Column, LookupDef LookupDef, bool IsReadOnly = false, bool SupportsRecycling = false)
    {
        LookupSource LookupSource = LookupDef.Create();
        
        DataGridTemplateColumn Result = new();
        GridColumnBinding CI = new GridColumnBinding(Result, Column);
        ConfigureLookupColumn(Result, Column.ColumnName, LookupSource, CI, Column.Caption, IsReadOnly, SupportsRecycling: SupportsRecycling);
        CI.LookupSource = LookupSource;
        Result.Tag = CI; 
        return Result;
    }
    static public DataGridColumn CreateLookupColumn(FieldDef FieldDef, LookupDef LookupDef = null, bool SupportsRecycling = false)
    {
        LookupDef = LookupDef ?? DataRegistry.Lookups.Get(FieldDef.LookupSource);
        LookupSource LookupSource = LookupDef.Create();
        
        DataGridTemplateColumn Result = new();
        GridColumnBinding CI = new GridColumnBinding(Result, FieldDef);
        ConfigureLookupColumn(Result, FieldDef.Name, LookupSource, CI, FieldDef.Title, IsReadOnly: FieldDef.IsReadOnly || FieldDef.IsReadOnlyUI, SupportsRecycling: SupportsRecycling);
        CI.LookupSource = LookupSource;
        Result.Tag = CI;    
        return Result;
    }
    static public void ResetLookupColumnTemplates(DataGridColumn Column, LookupSource LookupSource, bool SupportsRecycling = false)
    {
        if (Column is not DataGridTemplateColumn TemplateColumn || LookupSource == null)
            return;

        GridColumnBinding Binding = Column.GetInfo();
        if (Binding == null)
            return;

        TemplateColumn.CellTemplate = CreateLookupDisplayTemplate(Binding.FieldName, LookupSource, Binding, SupportsRecycling);
        TemplateColumn.CellEditingTemplate = TemplateColumn.IsReadOnly ? null : CreateLookupEditTemplate(Binding.FieldName, LookupSource, Binding, SupportsRecycling);
    }

    static public GridColumnBinding GetInfo(this DataGridColumn Column) => Column != null ? Column.Tag as GridColumnBinding : null;

    static public List<GridColumnBinding> GetInfoList(this DataGrid Grid)
    {
        List<GridColumnBinding> Result = new();

        GridColumnBinding CI;
        if (Grid != null && Grid.Columns.Count > 0)
        {
            foreach (var GridColumn in Grid.Columns)
            {
                CI = GridColumn.GetInfo();
                if (CI != null)
                    Result.Add(CI);
            }
        }

        return Result;
    }
}
