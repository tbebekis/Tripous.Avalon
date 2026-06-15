/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tripous.Desktop;

public enum MessageBoxMode
{
    Info,
    Error,
    Question
}

public partial class MessageBox : Window
{
    // ● private fields
    private MessageBoxMode fBoxMode;
    private Button fBtnYes;
    private Button fBtnNo;
    private Button fBtnClose;
    
    // ● private
    private void SetIcon(MessageBoxMode boxMode)
    {
        fBoxMode = boxMode;
        
        string fileName = boxMode switch
        {
            MessageBoxMode.Info => "information.png",
            MessageBoxMode.Error => "error.png",
            MessageBoxMode.Question => "emotion_question.png",
            _ => "information.png"
        };

       
        AvaloniaAssets.SetImage(imgIcon, fileName);
 
    }
    private void FocusDefaultButton()
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (fBtnClose != null)
                fBtnClose.Focus(NavigationMethod.Tab, KeyModifiers.None);
            else
                fBtnNo.Focus(NavigationMethod.Tab, KeyModifiers.None);
        }, DispatcherPriority.Input);
    }
    private void QuestionButton_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key != Key.Left && Args.Key != Key.Right && Args.Key != Key.Up && Args.Key != Key.Down)
            return;

        if (Sender == fBtnYes)
            fBtnNo.Focus(NavigationMethod.Directional, KeyModifiers.None);
        else
            fBtnYes.Focus(NavigationMethod.Directional, KeyModifiers.None);

        Args.Handled = true;
    }
    private static async Task<bool> ShowDialog(string title, string Message, bool isQuestion, MessageBoxMode boxMode, Control Caller)
    {
        var Dlg = new MessageBox();
        Dlg.Title = title;
        Dlg.edtMessage.Text = Message;
        Dlg.SetIcon(boxMode);  

 
        if (isQuestion)
        {
            Dlg.fBtnYes = new Button
            {
                Content = "Yes", Width = 70,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            Dlg.fBtnYes.Click += (s, e) => { Dlg.DialogResultValue = true; Dlg.Close(); };
            Dlg.fBtnYes.KeyDown += Dlg.QuestionButton_KeyDown;
            
            Dlg.fBtnNo = new Button
            {
                Content = "No", Width = 70, IsDefault = true, IsCancel = true,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            Dlg.fBtnNo.Click += (s, e) => { Dlg.DialogResultValue = false; Dlg.Close(); };
            Dlg.fBtnNo.KeyDown += Dlg.QuestionButton_KeyDown;

            Dlg.pnlButtons.Children.Add(Dlg.fBtnYes);
            Dlg.pnlButtons.Children.Add(Dlg.fBtnNo);
 
        }
        else
        {
            Dlg.fBtnClose = new Button
            {
                Content = "Close", Width = 70, IsDefault = true, IsCancel = true,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };
            Dlg.fBtnClose.Click += (s, e) => Dlg.Close();
            Dlg.pnlButtons.Children.Add(Dlg.fBtnClose);
        }

        Window ParentWindow = Caller.GetOwnerWindow();
 
        await Dlg.ShowDialog(ParentWindow);
        return Dlg.DialogResultValue;
    }
    
    public MessageBox()
    {
        InitializeComponent();
        Loaded += (s, e) => FocusDefaultButton();
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
