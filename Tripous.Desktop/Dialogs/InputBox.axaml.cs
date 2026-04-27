namespace Tripous.Desktop;

public partial class InputBox : DialogWindow
{
    private InputBoxData BoxData;
    
    // ● event handlers
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (sender == btnCancel)
            this.ModalResult = ModalResult.Cancel;
        else if (sender == btnOK)
            await ControlsToItem();
    }
    
    protected override async Task WindowInitialize()
    {
        BoxData = InputData as InputBoxData;
        ResultData = BoxData;
        lblMessage.Content = BoxData.Message;
        edtValue.Text = BoxData.Value;
 
        btnCancel.Focus();
        
        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        await Task.CompletedTask;
    }
    protected override async Task ControlsToItem()
    {
        string Value = edtValue.Text.Trim();
        if (!string.IsNullOrWhiteSpace(Value))
        {
            BoxData.Value = Value;
            this.ModalResult = ModalResult.Ok;
        }
        await Task.CompletedTask;
    }
    
    // ● construction
    public InputBox()
    {
        InitializeComponent();
    }

    static public async Task<DialogData> ShowModal(string Message, string Value = "", Control Caller = null)
    {
        InputBoxData BoxData = new() { Message = Message, Value = Value };
        return await  ShowModal<InputBox>(BoxData, Caller);
    }
}

public class InputBoxData
{
    public string Message { get; set; } = "Please, enter a value";
    public string Value { get; set; } = string.Empty;
    public bool IsNumeric { get; set; }
}