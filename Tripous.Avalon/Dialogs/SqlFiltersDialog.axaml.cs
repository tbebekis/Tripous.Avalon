using System.Threading.Tasks;
using Avalonia;
using Avalonia.Interactivity;
using Tripous;
using Tripous.Data;

namespace Tripous.Avalon;

public partial class SqlFiltersDialog : DialogWindow
{
    SelectDef SelectDef;
    private SelectFilterPanelHandler PanelHandler;
 
    // ● event handlers
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (sender == btnCancel)
            this.ModalResult = ModalResult.Cancel;
        else if (sender == btnOK)
            await ControlsToItem();
    }
 
    // ● overrides
    protected override async Task WindowInitialize()
    {
        SelectDef = InputData as SelectDef;
        ResultData = SelectDef;
        this.Title = $"Filters: {SelectDef.Name}";
        btnCancel.Focus();

        PanelHandler = new SelectFilterPanelHandler(SelectDef, pnlRuntimeFilters, Ui.IdColumnsVisible);
        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        int MinusY = await PanelHandler.ItemToControls();
        this.Position = new PixelPoint(Position.X, Position.Y - MinusY);
    }
    protected override async Task ControlsToItem()
    {
        if (await PanelHandler.CheckIsOk(true))
        {
            await PanelHandler.ControlsToItem();

            Ui.IdColumnsVisible = PanelHandler.IdColumnsVisible;
            this.ModalResult = ModalResult.Ok;  
        }
    }

    // ● construction
    public SqlFiltersDialog()
    {
        InitializeComponent();
    }
}
