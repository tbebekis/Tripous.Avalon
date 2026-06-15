/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;
 

/// <summary>
/// Creates and manages SQL filter controls in a panel.
/// </summary>
public class SqlFilterPanelHandler
{
    // ● private fields
    /// <summary>
    /// The filter UI information list.
    /// </summary>
    List<SqlFilterInfo> fFilterInfos = new();
    /// <summary>
    /// The panel hosting the filter controls.
    /// </summary>
    StackPanel Panel;

    // ● private
    /// <summary>
    /// Returns the default filter margin.
    /// </summary>
    /// <returns>The default filter margin.</returns>
    static Thickness GetMargin() => new Thickness(0, 0, 0, 8);
    /// <summary>
    /// Returns the default small margin.
    /// </summary>
    /// <returns>The default small margin.</returns>
    static Thickness GetSmallMargin() => new Thickness(0, 2, 0, 0);
    /// <summary>
    /// Creates a filter label.
    /// </summary>
    /// <param name="FilterDef">The filter definition.</param>
    /// <returns>The created label.</returns>
    static TextBlock CreateLabel(SqlFilterDef FilterDef)
    {
        return new TextBlock
        {
            Text = FilterDef.Title,
            Margin = GetSmallMargin()
        };
    }
    /// <summary>
    /// Creates a boolean operator combo box.
    /// </summary>
    /// <param name="FilterDef">The filter definition.</param>
    /// <returns>The created combo box.</returns>
    static ComboBox CreateBoolOpCombo(SqlFilterDef FilterDef)
    {
        ComboBox Result = new();
        Result.ItemsSource = new[] { BoolOp.And, BoolOp.Or };
        Result.SelectedItem = FilterDef.BoolOp == BoolOp.Or ? BoolOp.Or : BoolOp.And;
        return Result;
    }
    /// <summary>
    /// Creates a condition operator combo box.
    /// </summary>
    /// <param name="FilterDef">The filter definition.</param>
    /// <returns>The created combo box.</returns>
    static ComboBox CreateConditionOpCombo(SqlFilterDef FilterDef)
    {
        ComboBox Result = new();
        Result.ItemsSource = GetConditionOps(FilterDef.FilterDataType);
        Result.SelectedItem = Result.Items.Cast<ConditionOp>().Contains(FilterDef.ConditionOp) ? FilterDef.ConditionOp : ConditionOp.Equal;
        return Result;
    }
    /// <summary>
    /// Returns the condition operators supported by a data type.
    /// </summary>
    /// <param name="DataType">The data type.</param>
    /// <returns>The condition operators.</returns>
    static ConditionOp[] GetConditionOps(DataFieldType DataType)
    {
        if (DataType == DataFieldType.String)
        {
            return new[]
            {
                ConditionOp.Equal,
                ConditionOp.Contains,
                ConditionOp.StartsWith,
                ConditionOp.EndsWith
            };
        }

        return new[]
        {
            ConditionOp.Equal,
            ConditionOp.GreaterOrEqual,
            ConditionOp.LessOrEqual,
            ConditionOp.Between
        };
    }
    /// <summary>
    /// Creates a value control for a filter definition.
    /// </summary>
    /// <param name="FilterDef">The filter definition.</param>
    /// <param name="IsSecond">True to create the second value control.</param>
    /// <returns>The created value control.</returns>
    static Control CreateValueControl(SqlFilterDef FilterDef, bool IsSecond)
    {
        object Value = IsSecond ? FilterDef.Value2 : FilterDef.Value;

        if (FilterDef.FilterDataType.IsDateTime())
        {
            DatePicker Result = new();
            if (Value is DateTime Date)
                Result.SelectedDate = Date;
            return Result;
        }
        else
        {
            TextBox Result = new();
            Result.Text = Value == null ? string.Empty : Convert.ToString(Value, CultureInfo.CurrentCulture);
            return Result;
        }
    }
    /// <summary>
    /// Returns a value from a filter control.
    /// </summary>
    /// <param name="Control">The filter control.</param>
    /// <param name="DataType">The filter data type.</param>
    /// <returns>The control value.</returns>
    static object GetControlValue(Control Control, DataFieldType DataType)
    {
        if (Control is DatePicker DatePicker)
            return DatePicker.SelectedDate.HasValue ? DatePicker.SelectedDate.Value.DateTime : null;

        if (Control is TextBox TextBox)
        {
            string Text = TextBox.Text;
            if (string.IsNullOrWhiteSpace(Text))
                return null;

            if (DataType == DataFieldType.String)
                return Text;

            if (DataType == DataFieldType.Integer)
                return Convert.ToInt32(Text, CultureInfo.CurrentCulture);

            if (DataType == DataFieldType.Decimal || DataType == DataFieldType.Decimal_)
                return Convert.ToDecimal(Text, CultureInfo.CurrentCulture);

            if (DataType == DataFieldType.Double)
                return Convert.ToDouble(Text, CultureInfo.CurrentCulture);

            return Text;
        }

        return null;
    }
    /// <summary>
    /// Sets the visibility of a control.
    /// </summary>
    /// <param name="Control">The control.</param>
    /// <param name="Visible">True to show the control.</param>
    static void SetControlVisible(Control Control, bool Visible)
    {
        if (Control != null)
            Control.IsVisible = Visible;
    }

    // ● protected
    /// <summary>
    /// Creates filter UI information for a filter definition.
    /// </summary>
    /// <param name="FilterDef">The filter definition.</param>
    /// <returns>The created filter information.</returns>
    protected virtual SqlFilterInfo CreateFilterInfo(SqlFilterDef FilterDef)
    {
        SqlFilterInfo Result = new();
        Result.FilterDef = FilterDef;
        Result.Control = CreateValueControl(FilterDef, false);
        Result.Control2 = CreateValueControl(FilterDef, true);
        return Result;
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFilterPanelHandler"/> class.
    /// </summary>
    /// <param name="Panel">The panel hosting the filter controls.</param>
    public SqlFilterPanelHandler(StackPanel Panel)
    {
        this.Panel = Panel;
    }

    // ● public
    /// <summary>
    /// Creates the controls for a specified <see cref="SqlFilterDefs"/> in the filters panel.
    /// </summary>
    /// <param name="FilterDefs">The filter definitions.</param>
    public void CreateFilterControls(SqlFilterDefs FilterDefs)
    {
        Clear();
        Panel.Children.Clear();

        if (FilterDefs == null)
            return;

        foreach (SqlFilterDef FilterDef in FilterDefs)
        {
            SqlFilterInfo Info = CreateFilterInfo(FilterDef);
            fFilterInfos.Add(Info);

            Border Border = new();
            Border.Margin = GetMargin();

            StackPanel FilterPanel = new();
            FilterPanel.Spacing = 4;

            Grid HeaderGrid = new();
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            HeaderGrid.ColumnSpacing = 6;

            ComboBox BoolOpCombo = CreateBoolOpCombo(FilterDef);
            TextBlock Label = CreateLabel(FilterDef);
            ComboBox ConditionOpCombo = CreateConditionOpCombo(FilterDef);

            BoolOpCombo.Tag = Info;
            ConditionOpCombo.Tag = Info;
            Info.FilterDef.Tag = Info;

            Grid.SetColumn(BoolOpCombo, 0);
            Grid.SetColumn(Label, 1);
            Grid.SetColumn(ConditionOpCombo, 2);

            HeaderGrid.Children.Add(BoolOpCombo);
            HeaderGrid.Children.Add(Label);
            HeaderGrid.Children.Add(ConditionOpCombo);

            Info.Control.Margin = GetSmallMargin();
            Info.Control2.Margin = GetSmallMargin();

            FilterPanel.Children.Add(HeaderGrid);
            FilterPanel.Children.Add(Info.Control);
            FilterPanel.Children.Add(Info.Control2);
            
            Info.BoolOpCombo = BoolOpCombo;
            Info.ConditionOpCombo = ConditionOpCombo;

            SetControlVisible(Info.Control2, FilterDef.ConditionOp == ConditionOp.Between);

            ConditionOpCombo.SelectionChanged += (Sender, Args) =>
            {
                if (ConditionOpCombo.SelectedItem is ConditionOp ConditionOp)
                    SetControlVisible(Info.Control2, ConditionOp == ConditionOp.Between);
            };

            Border.Child = FilterPanel;
            Panel.Children.Add(Border);
        }
    }
        
    
    /// <summary>
    /// Collects values from the filter controls and returns active filters only.
    /// </summary>
    /// <returns>The active filter definitions.</returns>
    public SqlFilterDefs CollectValues_OLD()
    {
        SqlFilterDefs Result = new();
        Result.AllowDuplicateNames = true;

        foreach (SqlFilterInfo Info in fFilterInfos)
        {
            SqlFilterDef Source = Info.FilterDef;
            SqlFilterDef FilterDef = Source.Clone() as SqlFilterDef;

            ComboBox BoolOpCombo = null;
            ComboBox ConditionOpCombo = null;

            if (Source.Tag is SqlFilterInfo)
            {
            }

            Control Parent = Info.Control.Parent as Control;
            if (Parent is StackPanel Panel)
            {
                BoolOpCombo = Panel.Children.OfType<ComboBox>().FirstOrDefault();
                ConditionOpCombo = Panel.Children.OfType<ComboBox>().Skip(1).FirstOrDefault();
            }

            if (BoolOpCombo != null && BoolOpCombo.SelectedItem is BoolOp BoolOp)
                FilterDef.BoolOp = BoolOp;

            if (ConditionOpCombo != null && ConditionOpCombo.SelectedItem is ConditionOp ConditionOp)
                FilterDef.ConditionOp = ConditionOp;

            FilterDef.Value = GetControlValue(Info.Control, FilterDef.FilterDataType);
            FilterDef.Value2 = GetControlValue(Info.Control2, FilterDef.FilterDataType);

            if (FilterDef.ConditionOp == ConditionOp.Between)
            {
                if (FilterDef.Value != null && FilterDef.Value2 != null)
                    Result.Add(FilterDef);
            }
            else
            {
                FilterDef.Value2 = null;

                if (FilterDef.Value != null)
                    Result.Add(FilterDef);
            }
        }

        return Result;
    }
    /// <summary>
    /// Collects values from the filter controls and returns active filters only.
    /// </summary>
    /// <returns>The active filter definitions.</returns>
    public SqlFilterDefs CollectValues()
    {
        SqlFilterDefs Result = new();
        Result.AllowDuplicateNames = true;

        foreach (SqlFilterInfo Info in fFilterInfos)
        {
            SqlFilterDef Source = Info.FilterDef;
            SqlFilterDef FilterDef = Source.Clone() as SqlFilterDef;

            ComboBox BoolOpCombo = Info.BoolOpCombo;
            ComboBox ConditionOpCombo = Info.ConditionOpCombo;

            if (BoolOpCombo != null && BoolOpCombo.SelectedItem is BoolOp BoolOp)
                FilterDef.BoolOp = BoolOp;

            if (ConditionOpCombo != null && ConditionOpCombo.SelectedItem is ConditionOp ConditionOp)
                FilterDef.ConditionOp = ConditionOp;

            FilterDef.Value = GetControlValue(Info.Control, FilterDef.FilterDataType);
            FilterDef.Value2 = GetControlValue(Info.Control2, FilterDef.FilterDataType);

            if (FilterDef.ConditionOp == ConditionOp.Between)
            {
                if (FilterDef.Value != null && FilterDef.Value2 != null)
                    Result.Add(FilterDef);
            }
            else
            {
                FilterDef.Value2 = null;

                if (FilterDef.Value != null)
                    Result.Add(FilterDef);
            }
        }

        return Result;
    }
    
    /// <summary>
    /// Returns the SQL WHERE text produced by the active filters.
    /// </summary>
    /// <returns>The SQL WHERE text.</returns>
    public string GetWhere()
    {
        SqlFilterDefs Defs = CollectValues();
        string Result = Defs.GetSqlWhereFilterTextInline();
        return Result;
    }
    /// <summary>
    /// Clears all filter control values.
    /// </summary>
    public void Clear()
    {
        foreach (var Info in fFilterInfos)
        {
            // values
            if (Info.Control is TextBox tb) tb.Text = "";
            if (Info.Control is DatePicker dp) dp.SelectedDate = null;

            if (Info.Control2 is TextBox tb2) tb2.Text = "";
            if (Info.Control2 is DatePicker dp2) dp2.SelectedDate = null;

            // combos
            Info.BoolOpCombo.SelectedItem = BoolOp.And;
            Info.ConditionOpCombo.SelectedItem = ConditionOp.Equal;

            // visibility
            SetControlVisible(Info.Control2, false);
        }
    }

    // ● properties
    /// <summary>
    /// Gets the filter UI information list.
    /// </summary>
    public IReadOnlyList<SqlFilterInfo> FilterInfos => fFilterInfos;
}
