
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

using Tripous;
using Tripous.Data;

namespace Tripous.Avalon;

public partial class RowFilterItemDialog : DialogWindow
{
    private sealed class OpItem
    {
        public OpItem(string Text, ConditionOp Op)
        {
            this.Text = Text;
            this.Op = Op;
        }

        public string Text { get; }
        public ConditionOp Op { get; }

        public override string ToString() => Text;
    }
    
    private RowFilterDef RowFilterDef;
    private GridColumnInfo ColumnInfo; 
    private TextBlock lblInfo;
    private ComboBox cboOperator;
    private TextBox edtValue;
    private DatePicker dtpValue;
    private CheckBox chkIsNull;
    
    // ● event handlers
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (sender == btnCancel)
            this.ModalResult = ModalResult.Cancel;
        else if (sender == btnOK)
            await ControlsToItem();
    }
 
    // ● private
    private OpItem FindOpItem(ConditionOp Op)
    {
        if (cboOperator?.ItemsSource is IEnumerable Items)
        {
            foreach (object Item in Items)
            {
                if (Item is OpItem X && X.Op == Op)
                    return X;
            }
        }

        return null;
    }
    private void UpdateNullState()
    {
        bool IsNull = chkIsNull?.IsChecked == true;

        if (cboOperator != null)
            cboOperator.IsEnabled = !IsNull;

        if (edtValue != null)
            edtValue.IsEnabled = !IsNull;

        if (dtpValue != null)
            dtpValue.IsEnabled = !IsNull;
    }
    private static string GetTypeText(Type T)
    {
        if (T == typeof(string))
            return "String";

        if (T == typeof(DateTime))
            return "Date";

        if (T == typeof(byte) || T == typeof(short) || T == typeof(int) || T == typeof(long))
            return "Integer";

        if (T == typeof(float) || T == typeof(double) || T == typeof(decimal))
            return "Number";

        return T.Name;
    }
    
    // ● overrides
    protected override async Task WindowInitialize()
    {
        RowFilterDef = InputData as RowFilterDef;
        ColumnInfo = RowFilterDef.Tag as GridColumnInfo;
        ResultData = RowFilterDef;
        this.Title = $"Filter: {RowFilterDef.FieldName}";
        btnCancel.Focus();
        
        await Task.CompletedTask;
    }
 
    protected override async Task ItemToControls()
    {
        pnlFilterItem.Children.Clear();

        lblInfo = new TextBlock
        {
            Text = $"{RowFilterDef.FieldName} ({GetTypeText(ColumnInfo.UnderlyingType)})"
        };
        pnlFilterItem.Children.Add(lblInfo);

        List<OpItem> Ops = new();

        if (ColumnInfo.IsString)
        {
            Ops.Add(new OpItem("Equal", ConditionOp.Equal));
            Ops.Add(new OpItem("Not Equal", ConditionOp.NotEqual));
            Ops.Add(new OpItem("Contains", ConditionOp.Contains));
            Ops.Add(new OpItem("Starts With", ConditionOp.StartsWith));
            Ops.Add(new OpItem("Ends With", ConditionOp.EndsWith));
        }
        else if (ColumnInfo.IsNumeric || ColumnInfo.IsDate)
        {
            Ops.Add(new OpItem("Equal", ConditionOp.Equal));
            Ops.Add(new OpItem("Not Equal", ConditionOp.NotEqual));
            Ops.Add(new OpItem("Greater", ConditionOp.Greater));
            Ops.Add(new OpItem("Greater Or Equal", ConditionOp.GreaterOrEqual));
            Ops.Add(new OpItem("Less", ConditionOp.Less));
            Ops.Add(new OpItem("Less Or Equal", ConditionOp.LessOrEqual));
        }
        else
        {
            throw new ApplicationException($"Unsupported column type: {ColumnInfo.UnderlyingType}");
        }

        cboOperator = new ComboBox
        {
            ItemsSource = Ops,
            SelectedIndex = 0
        };
        pnlFilterItem.Children.Add(cboOperator);

        if (ColumnInfo.IsDate)
        {
            dtpValue = new DatePicker();
            pnlFilterItem.Children.Add(dtpValue);
        }
        else
        {
            edtValue = new TextBox();
            pnlFilterItem.Children.Add(edtValue);
        }

        chkIsNull = new CheckBox
        {
            Content = "Is Null"
        };
        chkIsNull.IsCheckedChanged += (_, _) => UpdateNullState();
        pnlFilterItem.Children.Add(chkIsNull);

        if (RowFilterDef.ConditionOp == ConditionOp.Null)
        {
            chkIsNull.IsChecked = true;
        }
        else
        {
            OpItem Op = FindOpItem(RowFilterDef.ConditionOp);
            if (Op != null)
                cboOperator.SelectedItem = Op;

            if (ColumnInfo.IsDate)
            {
                if (RowFilterDef.Value is DateTime DT)
                    dtpValue.SelectedDate = DT.Date;
            }
            else
            {
                edtValue.Text = RowFilterDef.Value?.ToString() ?? string.Empty;
            }
        }

        UpdateNullState();

        if (edtValue != null)
            edtValue.Focus();
        else if (dtpValue != null)
            dtpValue.Focus();

        await Task.CompletedTask;
    }
    protected override async Task ControlsToItem()
    {
        RowFilterDef.FieldName = ColumnInfo.FieldName;
        RowFilterDef.Value2 = null;

        if (chkIsNull?.IsChecked == true)
        {
            RowFilterDef.ConditionOp = ConditionOp.Null;
            RowFilterDef.Value = null;
            this.ModalResult = ModalResult.Ok;
            return;
        }

        if (cboOperator.SelectedItem is not OpItem Op)
            throw new ApplicationException("No operator selected.");

        RowFilterDef.ConditionOp = Op.Op;

        if (ColumnInfo.IsString)
        {
            string S = edtValue?.Text ?? string.Empty;

            switch (Op.Op)
            {
                case ConditionOp.Equal:
                case ConditionOp.NotEqual:
                    RowFilterDef.Value = S;
                    break;

                case ConditionOp.Contains:
                    RowFilterDef.Value = S;
                    break;

                case ConditionOp.StartsWith:
                    RowFilterDef.Value = S;
                    break;

                case ConditionOp.EndsWith:
                    RowFilterDef.Value = S;
                    break;

                default:
                    throw new ApplicationException($"Unsupported operator for string column: {Op.Op}");
            }
        }
        else if (ColumnInfo.IsNumeric)
        {
            string S = edtValue?.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(S))
                throw new ApplicationException("A numeric value is required.");

            Type T = ColumnInfo.UnderlyingType;

            if (T == typeof(byte))
                RowFilterDef.Value = byte.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(short))
                RowFilterDef.Value = short.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(int))
                RowFilterDef.Value = int.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(long))
                RowFilterDef.Value = long.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(float))
                RowFilterDef.Value = float.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(double))
                RowFilterDef.Value = double.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(decimal))
                RowFilterDef.Value = decimal.Parse(S, CultureInfo.InvariantCulture);
            else
                throw new ApplicationException($"Unsupported numeric type: {T}");
        }
        else if (ColumnInfo.IsDate)
        {
            if (dtpValue?.SelectedDate == null)
                throw new ApplicationException("A date value is required.");

            RowFilterDef.Value = dtpValue.SelectedDate.Value.DateTime;
        }
        else
        {
            throw new ApplicationException($"Unsupported column type: {ColumnInfo.UnderlyingType}");
        }

        this.ModalResult = ModalResult.Ok;
        await Task.CompletedTask;
    }    

    // ● construction
    public RowFilterItemDialog()
    {
        InitializeComponent();
    }
}