/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Shows a modal window with an embedded form (UserControl) 
/// </summary>
public class AppFormDialog : Window
{
    // ● private fields
    /// <summary>
    /// Indicates whether the window has been initialized.
    /// </summary>
    bool IsWindowInitialized = false;

    // ● overridables
    /// <summary>
    /// Initializes the window.
    /// </summary>
    protected virtual void WindowInitialize()
    {
    }
    /// <summary>
    /// Called when the window is opened.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnOpened(EventArgs e)
    {
        if (IsWindowInitialized)
            return;
        WindowInitialize();
 
        IsWindowInitialized = true;
        
        base.OnOpened(e);
    }

    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFormDialog"/> class.
    /// </summary>
    public AppFormDialog()
    {
        ShowInTaskbar = false;
    }

    // ● public
    /// <summary>
    /// Shows a modal window with an embedded form (UserControl) 
    /// </summary>
    /// <param name="Context">The form context.</param>
    /// <returns>The form context after the modal window is closed.</returns>
    static public async Task<FormContext> ShowModal(FormContext Context)
    {
        if (Context == null)
            throw new TripousArgumentNullException(nameof(Context));
        
        AppFormDialog Dialog = new(); 
        
        Context.DisplayMode = FormDisplayMode.Dialog;
        Context.ParentControl = Dialog;
        
        Dialog.OwnerWindow = Context.Caller.GetOwnerWindow();
        Dialog.Context = Context;
        Dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Dialog.Title = Context.Title;
        
        Dialog.Width = 960;
        Dialog.Height = 640;
        Dialog.MinWidth = 800;
        Dialog.MinHeight = 500;
        
        AppForm Form = Dialog.Context.CreateForm();
        Form.Setup(Context);
        
        await Dialog.ShowDialog(Dialog.OwnerWindow);
        return Dialog.Context;
    }
    /// <summary>
    /// Shows a modal window with an embedded form (UserControl) 
    /// </summary>
    /// <param name="Context">The data form context.</param>
    /// <returns>The data form context after the modal window is closed.</returns>
    static public async Task<DataFormContext> ShowModalDataForm(DataFormContext Context) => await ShowModal(Context) as DataFormContext;
    
    // ● properties
    /// <summary>
    /// Gets the form context.
    /// </summary>
    public FormContext Context { get; private set; }
    /// <summary>
    /// Gets the owner window.
    /// </summary>
    public Window OwnerWindow { get; private set; }
}
