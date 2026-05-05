namespace Tripous.Desktop;
 

public class SqlFilterPanelHandler
{
    // ● private fields
    List<SqlFilterInfo> fFilterInfos = new();
    StackPanel Panel;

    // ● private
    static Thickness GetMargin() => new Thickness(0, 0, 0, 8);
    static Thickness GetSmallMargin() => new Thickness(0, 2, 0, 0);
    static TextBlock CreateLabel(SqlFilterDef FilterDef)
    {
        return new TextBlock
        {
            Text = FilterDef.Title,
            Margin = GetSmallMargin()
        };
    }
    static ComboBox CreateBoolOpCombo(SqlFilterDef FilterDef)
    {
        ComboBox Result = new();
        Result.ItemsSource = new[] { BoolOp.And, BoolOp.Or };
        Result.SelectedItem = FilterDef.BoolOp == BoolOp.Or ? BoolOp.Or : BoolOp.And;
        return Result;
    }
    static ComboBox CreateConditionOpCombo(SqlFilterDef FilterDef)
    {
        ComboBox Result = new();
        Result.ItemsSource = GetConditionOps(FilterDef.FilterDataType);
        Result.SelectedItem = Result.Items.Cast<ConditionOp>().Contains(FilterDef.ConditionOp) ? FilterDef.ConditionOp : ConditionOp.Equal;
        return Result;
    }
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
    static void SetControlVisible(Control Control, bool Visible)
    {
        if (Control != null)
            Control.IsVisible = Visible;
    }

    // ● protected
    protected virtual SqlFilterInfo CreateFilterInfo(SqlFilterDef FilterDef)
    {
        SqlFilterInfo Result = new();
        Result.FilterDef = FilterDef;
        Result.Control = CreateValueControl(FilterDef, false);
        Result.Control2 = CreateValueControl(FilterDef, true);
        return Result;
    }

    // ● constructors
    public SqlFilterPanelHandler(StackPanel Panel)
    {
        this.Panel = Panel;
    }

    // ● public
    /// <summary>
    /// Creates the controls for a specified <see cref="SqlFilterDefs"/> in the filters panel.
    /// </summary>
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
    
    public string GetWhere()
    {
        SqlFilterDefs Defs = CollectValues();
        string Result = Defs.GetSqlWhereFilterTextInline();
        return Result;
    }
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
    public IReadOnlyList<SqlFilterInfo> FilterInfos => fFilterInfos;
}