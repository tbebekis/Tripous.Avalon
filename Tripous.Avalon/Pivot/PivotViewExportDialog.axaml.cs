using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Tripous.Avalon;

public partial class PivotViewExportDialog : DialogWindow
{
    
    private PivotViewExportOptions Options;
    
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
        PivotViewExportFormat Format = (PivotViewExportFormat)cboFormat.SelectedItem;
        string FilePath = await Ui.SaveFileDialog(this, Format.GetExportFileExtension());
        if (!string.IsNullOrWhiteSpace(FilePath))
            edtExportFilePath.Text = FilePath;
    }
    
    protected override async Task WindowInitialize()
    {
        if (Design.IsDesignMode)
            return;

        Options = InputData as PivotViewExportOptions;
        ResultData = Options;
 
        btnCancel.Focus();
        
        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        if (Design.IsDesignMode)
            return;
        // tabGeneral
        cboFormat.ItemsSource = Enum.GetValues(typeof(PivotViewExportFormat));
        cboFormat.SelectedItem = Options.Format;
        edtExportFilePath.IsReadOnly = true;
        edtExportFilePath.Text = Options.ExportFilePath;
        // tabGeneral - General Options
        chIncludeHeaders.IsChecked = Options.IncludeHeaders;
        chUseFormattedValues.IsChecked = Options.UseFormattedValues;
        chBooleanAsCheckMark.IsChecked = Options.BooleanAsCheckMark;
        
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
                PivotViewExportFormat Format = (PivotViewExportFormat)cboFormat.SelectedItem;

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
        Options.Format = (PivotViewExportFormat)cboFormat.SelectedItem;
        Options.ExportFilePath = edtExportFilePath.Text.Trim();
        // tabGeneral - General Options
        Options.IncludeHeaders = chIncludeHeaders.GetValue();
        Options.UseFormattedValues = chUseFormattedValues.GetValue();
        Options.BooleanAsCheckMark = chBooleanAsCheckMark.GetValue();
        
        // tabData
        Options.Culture = cboCulture.SelectedItem.ToString();
        Options.DateTimeFormat = edtDateTimeFormat.GetText();
        Options.NumberFormat = edtNumberFormat.GetText();
        Options.BooleanTrueValue  = edtBooleanTrueValue.GetText();
        Options.BooleanFalseValue   = edtBooleanFalseValue.GetText();
 
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
    public PivotViewExportDialog()
    {
        InitializeComponent();
    }
}