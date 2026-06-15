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

/// <summary>
/// Defines the display mode of a message box.
/// </summary>
public enum MessageBoxMode
{
    /// <summary>
    /// Information message.
    /// </summary>
    Info,
    /// <summary>
    /// Error message.
    /// </summary>
    Error,
    /// <summary>
    /// Question message.
    /// </summary>
    Question
}

/// <summary>
/// Displays a modal message box.
/// </summary>
public partial class MessageBox : Window
{
    // ● private fields
    /// <summary>
    /// The current message box mode.
    /// </summary>
    private MessageBoxMode fBoxMode;
    /// <summary>
    /// The Yes button.
    /// </summary>
    private Button fBtnYes;
    /// <summary>
    /// The No button.
    /// </summary>
    private Button fBtnNo;
    /// <summary>
    /// The Close button.
    /// </summary>
    private Button fBtnClose;
    
    // ● private
    /// <summary>
    /// Sets the message box icon.
    /// </summary>
    /// <param name="boxMode">The message box mode.</param>
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
    /// <summary>
    /// Focuses the default button.
    /// </summary>
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
    /// <summary>
    /// Handles arrow key navigation between question buttons.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The key event arguments.</param>
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
    /// <summary>
    /// Shows a modal message box.
    /// </summary>
    /// <param name="title">The window title.</param>
    /// <param name="Message">The message text.</param>
    /// <param name="isQuestion">True to show Yes and No buttons.</param>
    /// <param name="boxMode">The message box mode.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>True when the user selects Yes; otherwise, false.</returns>
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
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageBox"/> class.
    /// </summary>
    public MessageBox()
    {
        InitializeComponent();
        Loaded += (s, e) => FocusDefaultButton();
    }
    
    // ● static public
    /// <summary>
    /// Shows an information message.
    /// </summary>
    /// <param name="Message">The message text.</param>
    /// <param name="Caller">The caller control.</param>
    public static async Task Info(string Message, Control Caller = null) 
        => await ShowDialog("Information", Message, false, MessageBoxMode.Info, Caller);

    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="Message">The message text.</param>
    /// <param name="Caller">The caller control.</param>
    public static async Task Error(string Message, Control Caller = null) 
        => await ShowDialog("Error", Message, false, MessageBoxMode.Error, Caller);

    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="e">The exception.</param>
    /// <param name="Caller">The caller control.</param>
    public static async Task Error(Exception e, Control Caller = null) 
        => await ShowDialog("Error", e.Message, false, MessageBoxMode.Error, Caller);

    /// <summary>
    /// Shows a Yes/No question message.
    /// </summary>
    /// <param name="Message">The message text.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>True when the user selects Yes; otherwise, false.</returns>
    public static async Task<bool> YesNo(string Message, Control Caller = null) 
        => await ShowDialog("Question", Message, true, MessageBoxMode.Question, Caller);
    
    // ● properties
    /// <summary>
    /// Gets the dialog result value.
    /// </summary>
    public bool DialogResultValue { get; private set; }
 
}
