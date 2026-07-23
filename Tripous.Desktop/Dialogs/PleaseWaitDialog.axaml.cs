/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Displays a non-closable please-wait dialog while a UI-owned operation is running.
/// </summary>
public partial class PleaseWaitDialog : Window
{
    // ● private fields
    bool fCanClose;

    // ● protected
    /// <summary>
    /// Prevents user-initiated closing while the operation is running.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!fCanClose)
            e.Cancel = true;

        base.OnClosing(e);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="PleaseWaitDialog"/> class.
    /// </summary>
    public PleaseWaitDialog()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PleaseWaitDialog"/> class.
    /// </summary>
    /// <param name="Message">The displayed message.</param>
    public PleaseWaitDialog(string Message)
        : this()
    {
        this.Message = Message;
    }

    // ● public
    /// <summary>
    /// Closes the dialog from code.
    /// </summary>
    public void CloseDialog()
    {
        fCanClose = true;
        Close();
    }

    // ● properties
    /// <summary>
    /// Gets or sets the displayed message.
    /// </summary>
    public string Message
    {
        get => lblMessage.Text;
        set
        {
            lblMessage.Text = value;
            lblMessage.IsVisible = !string.IsNullOrWhiteSpace(value);
        }
    }
}
