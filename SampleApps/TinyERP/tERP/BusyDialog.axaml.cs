/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Displays a modal busy message while a UI-owned operation is running.
/// </summary>
public partial class BusyDialog : Window
{
    // ● private fields
    bool fCanClose;

    // ● protected
    /// <summary>
    /// Prevents user-initiated closing while the operation is running.
    /// </summary>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!fCanClose)
            e.Cancel = true;

        base.OnClosing(e);
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public BusyDialog()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public BusyDialog(string Message)
        : this()
    {
        edtMessage.Text = Message;
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
        get => edtMessage.Text;
        set => edtMessage.Text = value;
    }
}
