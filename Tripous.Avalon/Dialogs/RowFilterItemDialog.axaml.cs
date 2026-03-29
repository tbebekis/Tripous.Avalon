
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
    private RowFilterItem RowFilterItem;
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
        RowFilterItem = InputData as RowFilterItem;
        ColumnInfo = RowFilterItem.Tag as GridColumnInfo;
        ResultData = RowFilterItem;
        this.Title = $"Filter: {RowFilterItem.FieldName}";
        btnCancel.Focus();
        
        await Task.CompletedTask;
    }
 
    protected override async Task ItemToControls()
    {
        pnlFilterItem.Children.Clear();

        lblInfo = new TextBlock
        {
            Text = $"{RowFilterItem.FieldName} ({GetTypeText(ColumnInfo.UnderlyingType)})"
        };
        pnlFilterItem.Children.Add(lblInfo);

        List<OpItem> Ops = new();

        if (ColumnInfo.IsStringColumn)
        {
            Ops.Add(new OpItem("Equal", ConditionOp.Equal));
            Ops.Add(new OpItem("Not Equal", ConditionOp.NotEqual));
            Ops.Add(new OpItem("Contains", ConditionOp.Contains));
            Ops.Add(new OpItem("Starts With", ConditionOp.StartsWith));
            Ops.Add(new OpItem("Ends With", ConditionOp.EndsWith));
        }
        else if (ColumnInfo.IsNumericColumn || ColumnInfo.IsDateColumn)
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

        if (ColumnInfo.IsDateColumn)
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

        if (RowFilterItem.ConditionOp == ConditionOp.Null)
        {
            chkIsNull.IsChecked = true;
        }
        else
        {
            OpItem Op = FindOpItem(RowFilterItem.ConditionOp);
            if (Op != null)
                cboOperator.SelectedItem = Op;

            if (ColumnInfo.IsDateColumn)
            {
                if (RowFilterItem.Value is DateTime DT)
                    dtpValue.SelectedDate = DT.Date;
            }
            else
            {
                edtValue.Text = RowFilterItem.Value?.ToString() ?? string.Empty;
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
        RowFilterItem.FieldName = ColumnInfo.FieldName;
        RowFilterItem.Value2 = null;

        if (chkIsNull?.IsChecked == true)
        {
            RowFilterItem.ConditionOp = ConditionOp.Null;
            RowFilterItem.Value = null;
            this.ModalResult = ModalResult.Ok;
            return;
        }

        if (cboOperator.SelectedItem is not OpItem Op)
            throw new ApplicationException("No operator selected.");

        RowFilterItem.ConditionOp = Op.Op;

        if (ColumnInfo.IsStringColumn)
        {
            string S = edtValue?.Text ?? string.Empty;

            switch (Op.Op)
            {
                case ConditionOp.Equal:
                case ConditionOp.NotEqual:
                    RowFilterItem.Value = S;
                    break;

                case ConditionOp.Contains:
                    RowFilterItem.Value = S;
                    break;

                case ConditionOp.StartsWith:
                    RowFilterItem.Value = S;
                    break;

                case ConditionOp.EndsWith:
                    RowFilterItem.Value = S;
                    break;

                default:
                    throw new ApplicationException($"Unsupported operator for string column: {Op.Op}");
            }
        }
        else if (ColumnInfo.IsNumericColumn)
        {
            string S = edtValue?.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(S))
                throw new ApplicationException("A numeric value is required.");

            Type T = ColumnInfo.UnderlyingType;

            if (T == typeof(byte))
                RowFilterItem.Value = byte.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(short))
                RowFilterItem.Value = short.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(int))
                RowFilterItem.Value = int.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(long))
                RowFilterItem.Value = long.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(float))
                RowFilterItem.Value = float.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(double))
                RowFilterItem.Value = double.Parse(S, CultureInfo.InvariantCulture);
            else if (T == typeof(decimal))
                RowFilterItem.Value = decimal.Parse(S, CultureInfo.InvariantCulture);
            else
                throw new ApplicationException($"Unsupported numeric type: {T}");
        }
        else if (ColumnInfo.IsDateColumn)
        {
            if (dtpValue?.SelectedDate == null)
                throw new ApplicationException("A date value is required.");

            RowFilterItem.Value = dtpValue.SelectedDate.Value.DateTime;
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