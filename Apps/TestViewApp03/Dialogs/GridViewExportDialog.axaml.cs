using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Tripous.Avalon;

public partial class GridViewExportDialog : DialogWindow
{
    private GridViewExportOptions Options;
    
    // ● event handlers
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (btnCancel == sender)
            this.ModalResult = ModalResult.Cancel;
        else if (btnOK == sender)
            await ControlsToItem();
        else if (btnExportFilePath == sender)
            await ShowExportFilePathDialog();
    }

    async Task ShowExportFilePathDialog()
    {
        GridViewExportFormat Format = (GridViewExportFormat)cboFormat.SelectedItem;
        string FilePath = await Ui.SaveFileDialog(this,Format.GetExportFileExtension());
        if (!string.IsNullOrWhiteSpace(FilePath))
            edtExportFilePath.Text = FilePath;
    }
    
    protected override async Task WindowInitialize()
    {
        if (Design.IsDesignMode)
            return;
        
        Options = InputData as GridViewExportOptions;
        ResultData = Options;
 
        btnCancel.Focus();
        
        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        if (Design.IsDesignMode)
            return;
        
        // tabGeneral
        cboFormat.ItemsSource = Enum.GetValues(typeof(GridViewExportFormat));
        cboFormat.SelectedItem = Options.Format;
        edtExportFilePath.IsReadOnly = true;
        edtExportFilePath.Text = Options.ExportFilePath;
        // tabGeneral - General Options
        chIncludeHeaders.IsChecked = Options.IncludeHeaders;
        chOnlyVisibleColumns.IsChecked = Options.OnlyVisibleColumns;
        chUseFormattedValues.IsChecked = Options.UseFormattedValues;
        // tabGeneral - Grouping and Totals
        chIncludeGroups.IsChecked = Options.IncludeGroups;
        chIncludeFooters.IsChecked = Options.IncludeFooters;
        chIncludeGrandTotal.IsChecked = Options.IncludeGrandTotal;
        
        // tabData
        var Cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .OrderBy(c => c.Name)
            .Select(x => x.Name)
            .ToList();
        cboCulture.ItemsSource = Cultures;
        cboCulture.SelectedItem = Options.Culture;
        edtDateTimeFormat.Text = Options.DateTimeFormat;
        edtNumberFormat.Text = Options.NumberFormat;
        edtBooleanTrueValue.Text = Options.BooleanTrueValue;
        edtBooleanFalseValue.Text = Options.BooleanFalseValue;
        cboBlobMode.ItemsSource = Enum.GetValues(typeof(GridViewBlobExportMode));
        cboBlobMode.SelectedItem = Options.BlobMode;
        edtBlobPlaceholderText.Text = Options.BlobPlaceholderText;
        edtMemoPlaceholderText.Text = Options.MemoPlaceholderText;
        
        // tabSpecific
        var Encodings = CodePage.GetSupportedCodePages().OrderBy(c => c.Name).Select(x => x.Name).ToList();
        cboEncoding.ItemsSource = Encodings;
        int Index = Encodings.IndexOf(Options.Encoding);
        cboEncoding.SelectedIndex = Index != -1 ? Index : 0;
        edtCsvDelimiter.Text = Options.CsvDelimiter;
        edtHtmlTitle.Text = Options.HtmlTitle;
        edtExcelSheetName.Text = Options.ExcelSheetName;
        chIndentJson.IsChecked = Options.IndentJson;
        chCsvQuoteAll.IsChecked = Options.CsvQuoteAll;
        
        await Task.CompletedTask;

        cboFormat.SelectionChanged += (sender, args) =>
        {
            string FilePath = edtExportFilePath.GetText();
            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                GridViewExportFormat Format = (GridViewExportFormat)cboFormat.SelectedItem;

                FilePath = Path.ChangeExtension(FilePath, Format.GetExportFileExtension());
                edtExportFilePath.Text = FilePath;
            }
        };
    }
    protected override async Task ControlsToItem()
    {
        if (Design.IsDesignMode)
            return;

        if (string.IsNullOrWhiteSpace(edtExportFilePath.Text))
            return;
        
        // tabGeneral
        Options.Format = (GridViewExportFormat)cboFormat.SelectedItem;
        Options.ExportFilePath = edtExportFilePath.Text.Trim();
        // tabGeneral - General Options
        Options.IncludeHeaders = chIncludeHeaders.GetValue();
        Options.OnlyVisibleColumns = chOnlyVisibleColumns.GetValue();
        Options.UseFormattedValues = chUseFormattedValues.GetValue();
        // tabGeneral - Grouping and Totals
        Options.IncludeGroups = chIncludeGroups.GetValue();
        Options.IncludeFooters = chIncludeFooters.GetValue();
        Options.IncludeGrandTotal = chIncludeGrandTotal.GetValue();
        
        // tabData
        Options.Culture = cboCulture.SelectedItem.ToString();
        Options.DateTimeFormat = edtDateTimeFormat.GetText();
        Options.NumberFormat = edtNumberFormat.GetText();
        Options.BooleanTrueValue  = edtBooleanTrueValue.GetText();
        Options.BooleanFalseValue   = edtBooleanFalseValue.GetText();
        Options.BlobMode = (GridViewBlobExportMode)cboBlobMode.SelectedItem;
        Options.BlobPlaceholderText = edtBlobPlaceholderText.GetText();
        Options.MemoPlaceholderText = edtMemoPlaceholderText.GetText();
 
        // tabSpecific
        Options.Encoding = cboEncoding.SelectedItem.ToString();
        Options.CsvDelimiter = edtCsvDelimiter.GetText();
        Options.HtmlTitle = edtHtmlTitle.GetText();
        Options.ExcelSheetName = edtExcelSheetName.GetText();
 
        Options.IndentJson = chIndentJson.GetValue();
        Options.CsvQuoteAll = chCsvQuoteAll.GetValue();
            
        this.ModalResult = ModalResult.Ok;
        await Task.CompletedTask;
    }
    
    // ● construction
    public GridViewExportDialog()
    {
        InitializeComponent();
    }
}