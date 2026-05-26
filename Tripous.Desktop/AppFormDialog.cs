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
    bool IsWindowInitialized = false;

    // ● overridables
    protected virtual void WindowInitialize()
    {
    }
    protected override void OnOpened(EventArgs e)
    {
        if (IsWindowInitialized)
            return;
        WindowInitialize();
 
        IsWindowInitialized = true;
        
        base.OnOpened(e);
    }
    
    // ● public
    /// <summary>
    /// Shows a modal window with an embedded form (UserControl) 
    /// </summary>
    static public async Task<FormContext> ShowModal(FormContext Context)
    {
        if (Context == null)
            throw new TripousArgumentNullException(nameof(Context));
        
        AppFormDialog Dialog = new(); 
        
        Context.DisplayMode = FormDisplayMode.Dialog;
        Context.ParentControl = Dialog;
        
        Dialog.OwnerWindow = Context.Caller is Window? Context.Caller as Window: Context.Caller.GetParentWindow(); 
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
    static public async Task<DataFormContext> ShowModalDataForm(DataFormContext Context) => await ShowModal(Context) as DataFormContext;
    
    // ● properties
    public FormContext Context { get; private set; }
    public Window OwnerWindow { get; private set; }
}