using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tripous.Avalon;

public enum MessageBoxMode
{
    Info,
    Error,
    Question
}

public partial class MessageBox : Window
{
    private MessageBoxMode BoxMode;
    private Button btnYes;
    private Button btnNo;
    private Button btnClose;
    
    private void SetIcon(MessageBoxMode boxMode)
    {
        BoxMode = boxMode;
        
        string fileName = boxMode switch
        {
            MessageBoxMode.Info => "information.png",
            MessageBoxMode.Error => "error.png",
            MessageBoxMode.Question => "emotion_question.png",
            _ => "information.png"
        };

       Ui.SetImage(imgIcon, fileName);
        
        // Φόρτωση του Asset (υποθέτοντας ότι είναι στο φάκελο Assets)
        //var uri = new Uri($"avares://DbPark/Images/{fileName}");
       //imgIcon.Source = new Avalonia.Media.Imaging.Bitmap(Avalonia.Platform.AssetLoader.Open(uri));
    }
    
    // ● Private Helper για την εμφάνιση
    private static async Task<bool> ShowDialog(string title, string Message, bool isQuestion, MessageBoxMode boxMode, Control Caller)
    {
        var Dlg = new MessageBox();
        Dlg.Title = title;
        Dlg.edtMessage.Text = Message;
        Dlg.SetIcon(boxMode); // <--- Ορισμός εικονιδίου

        // Δυναμική δημιουργία Buttons
        if (isQuestion)
        {
            Dlg.btnYes = new Button
            {
                Content = "Yes", Width = 70, IsDefault = true,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            Dlg.btnYes.Click += (s, e) => { Dlg.DialogResultValue = true; Dlg.Close(); };
            
            Dlg.btnNo = new Button
            {
                Content = "No", Width = 70, IsCancel = true,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            Dlg.btnNo.Click += (s, e) => { Dlg.DialogResultValue = false; Dlg.Close(); };

            Dlg.pnlButtons.Children.Add(Dlg.btnYes);
            Dlg.pnlButtons.Children.Add(Dlg.btnNo);
 
        }
        else
        {
            Dlg.btnClose = new Button
            {
                Content = "Close", Width = 70, IsDefault = true, IsCancel = true,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            Dlg.btnClose.Click += (s, e) => Dlg.Close();
            Dlg.pnlButtons.Children.Add(Dlg.btnClose);
        }

        if (Caller == null)
            Caller = Ui.MainWindow;
        
        Window ParentWindow = Caller is Window? Caller as Window: Caller.GetParentWindow(); 
 
        await Dlg.ShowDialog(ParentWindow);
        return Dlg.DialogResultValue;
    }
    
    public MessageBox()
    {
        InitializeComponent();
        
        this.Loaded += (s, e) =>
        {
            //edtMessage.Focus();
            //this.Focus();
            if (btnClose != null)
                btnClose.Focus();
            else
                btnNo.Focus();
        };
    }
    
    // ● Static Methods
    public static async Task Info(string Message, Control Caller = null) 
        => await ShowDialog("Information", Message, false, MessageBoxMode.Info, Caller);

    public static async Task Error(string Message, Control Caller = null) 
        => await ShowDialog("Error", Message, false, MessageBoxMode.Error, Caller);

    public static async Task Error(Exception e, Control Caller = null) 
        => await ShowDialog("Error", e.Message, false, MessageBoxMode.Error, Caller);

    public static async Task<bool> YesNo(string Message, Control Caller = null) 
        => await ShowDialog("Question", Message, true, MessageBoxMode.Question, Caller);
    
 
    public bool DialogResultValue { get; private set; }
 
}