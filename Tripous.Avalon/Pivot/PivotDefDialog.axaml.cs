using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Tripous.Data;

namespace Tripous.Avalon;

public partial class PivotDefDialog : DialogWindow
{
    private PivotViewDef PivotViewDef;
    
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (btnCancel == sender)
            this.ModalResult = ModalResult.Cancel;
        else if (btnOK == sender)
            await ControlsToItem();

        else if (btnUpRow == sender)
            MoveRow(true);
        else if (btnDownRow == sender)
            MoveRow(false);

        else if (btnUpColumn == sender)
            MoveColumn(true);
        else if (btnDownColumn == sender)
            MoveColumn(false);

        else if (btnUpValue == sender)
            MoveValue(true);
        else if (btnDownValue == sender)
            MoveValue(false);
    }
    void lboFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 1. save previous
        if (e.RemovedItems.Count > 0 && e.RemovedItems[0] is PivotFieldDef OldItem)
            ControlsToField(OldItem); 

        // 2. load new
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is PivotFieldDef NewItem)
            FieldToControls(NewItem);
    }
    void AnyColumnCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lboFields.SelectedItem is PivotFieldDef FieldDef)
            ComboBoxesToField(FieldDef); 
    }

    // ● private
    void Move(ListBox Box, List<PivotFieldDef> List, bool Up)
    {
        if (Box.SelectedItem is not PivotFieldDef FieldDef)
            return;

        int i = List.IndexOf(FieldDef);
        if (i < 0)
            return;

        int j = Up ? i - 1 : i + 1;
        if (j < 0 || j >= List.Count)
            return;

        int Temp = List[i].OrdinalIndex;
        List[i].OrdinalIndex = List[j].OrdinalIndex;
        List[j].OrdinalIndex = Temp;

        RefreshListBoxes();
    }
    void MoveRow(bool Up) =>  Move(lboRows, PivotViewDef.GetRowFields().ToList(), Up);
    void MoveColumn(bool Up) =>  Move(lboColumns, PivotViewDef.GetColumnFields().ToList(), Up);
    void MoveValue(bool Up) =>  Move(lboValues, PivotViewDef.GetValueFields().ToList(), Up);

    void RefreshListBoxes()
    {
        var RowFields = PivotViewDef.GetRowFields();
        lboRows.ItemsSource = RowFields;

        var ColumnFields = PivotViewDef.GetColumnFields();
        lboColumns.ItemsSource = ColumnFields;

        var ValueFields = PivotViewDef.GetValueFields();
        lboValues.ItemsSource = ValueFields;
        
        edtSummary.Text = PivotViewDef.GetDescription();
    }

    void FieldToControls(PivotFieldDef FieldDef)
    {
        if (FieldDef == null)
            return;
        
        edtFieldName.Text = FieldDef.FieldName;
        edtCaption.Text = FieldDef.Caption;
        edtDataType.Text = FieldDef.DataType != null ? FieldDef.DataType.Name : string.Empty;
        
        cboAxis.SelectedItem = FieldDef.FieldMode;
        cboSortDirection.SelectedItem = FieldDef.SortDirection;
        edtFormat.Text = FieldDef.Format;
        chSortByValue.IsChecked = FieldDef.SortByValue;
        cboValueAggregateType.SelectedItem = FieldDef.ValueAggregateType;
/*
 *
        edtFieldName ( Read Only)
   edtCaption
   edtDataType ( Read Only)
   cboAxis
   cboSortDirection
   edtFormat
   chSortByValue
   cboValueAggregateType
 */
    }
    void ControlsToField(PivotFieldDef FieldDef)
    {
        if (FieldDef == null)
            return;
        
        FieldDef.Caption = edtCaption.Text;
        FieldDef.Format = edtFormat.Text;
        FieldDef.SortByValue = chSortByValue.IsChecked == true;

        ComboBoxesToField(FieldDef);
        
        edtSummary.Text = PivotViewDef.GetDescription();
    }
    void ComboBoxesToField(PivotFieldDef FieldDef)
    {
        if (cboAxis.SelectedItem is PivotFieldMode Axis)
            FieldDef.FieldMode = Axis;
        
        if (cboSortDirection.SelectedItem is ListSortDirection SortDirection)
            FieldDef.SortDirection = SortDirection;
        
        if (cboValueAggregateType.SelectedItem is PivotValueAggregateType ValueAggregateType)
            FieldDef.ValueAggregateType = ValueAggregateType;

        RefreshListBoxes();
        edtSummary.Text = PivotViewDef.GetDescription();
    }
    
    // ● overrides
    protected override async Task WindowInitialize()
    {
        if (Design.IsDesignMode)
            return;

        PivotViewDef = InputData as PivotViewDef;
        ResultData = PivotViewDef;
        
        btnCancel.Focus();

        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        if (Design.IsDesignMode)
            return;

        if (PivotViewDef == null)
            return;
        
        // ● General
        edtName.Text = PivotViewDef.Name;
        edtName.IsReadOnly = PivotViewDef.IsNameReadOnly;
        chShowSubtotals.IsChecked = PivotViewDef.ShowSubtotals;
        chShowGrandTotals.IsChecked = PivotViewDef.ShowGrandTotals;
        chRepeatRowHeaders.IsChecked = PivotViewDef.RepeatRowHeaders;
        edtSummary.Text = PivotViewDef.GetDescription();
        
        // ● Columns
        lboFields.ItemsSource = PivotViewDef.Fields;
        cboAxis.ItemsSource = Enum.GetValues(typeof(PivotFieldMode));
        cboSortDirection.ItemsSource = Enum.GetValues(typeof(ListSortDirection));
        cboValueAggregateType.ItemsSource = Enum.GetValues(typeof(PivotValueAggregateType));

        RefreshListBoxes();
        /*
        var RowFields = PivotDef.GetRowFields();
        lboRows.ItemsSource = RowFields;

        var ColumnFields = PivotDef.GetColumnFields();
        lboColumns.ItemsSource = ColumnFields;

        var ValueFields = PivotDef.GetValueFields();
        lboValues.ItemsSource = ValueFields;
        */
        
        // miscs
        lboFields.SelectionChanged += lboFields_SelectionChanged;
        
        if (PivotViewDef.Fields.Count > 0)
            lboColumns.SelectedIndex = 0;
        
        cboAxis.SelectionChanged += AnyColumnCombo_SelectionChanged;
        cboSortDirection.SelectionChanged += AnyColumnCombo_SelectionChanged;
        cboValueAggregateType.SelectionChanged += AnyColumnCombo_SelectionChanged;

        if (PivotViewDef.Fields.Count > 0)
            lboFields.SelectedIndex = 0;

        await Task.CompletedTask;
    }
    protected override async Task ControlsToItem()
    {
        if (Design.IsDesignMode)
            return;

        if (PivotViewDef == null)
            return;
        
        // ● General
        PivotViewDef.Name = edtName.Text;

        PivotViewDef.ShowSubtotals = chShowSubtotals.IsChecked == true;
        PivotViewDef.ShowGrandTotals = chShowGrandTotals.IsChecked == true;
        PivotViewDef.RepeatRowHeaders = chRepeatRowHeaders.IsChecked == true; 
        
        if (lboFields.SelectedItem is PivotFieldDef FieldDef)
            ComboBoxesToField(FieldDef); 

        string Errors = PivotViewDef.GetErrors();
        if (!string.IsNullOrWhiteSpace(Errors))
        {
            await MessageBox.Error(Errors, this);
            return;
        }
        
        await Task.CompletedTask;
        this.ModalResult = ModalResult.Ok;
    }
    
    // ● construction
    public PivotDefDialog()
    {
        InitializeComponent();
    }
}