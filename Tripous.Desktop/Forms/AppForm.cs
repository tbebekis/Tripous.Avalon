/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// A base class for a UI embeddable in <see cref="TabItem"/> controls.
/// </summary>
[TypeStore]
public class AppForm: UserControl, IBroadcasterListener
{
    // ● private fields
    /// <summary>
    /// True when this form is registered as a broadcaster listener.
    /// </summary>
    bool fBroadcasterRegistered;
    /// <summary>
    /// The modal result of the modal dialog displaying this form.
    /// </summary>
    private ModalResult fModalResult;

    // ● protected fields
    /// <summary>
    /// The title text of the form.
    /// </summary>
    protected string fTitleText;

    // ● protected
    /// <summary>
    /// Called just before form initialization
    /// </summary>
    protected virtual void FormInitializing()
    {
        ProcessFormOptions();
    }
    /// <summary>
    /// Called in order to initialize the form
    /// </summary>
    protected virtual void FormInitialize()
    {            
    }
    /// <summary>
    /// Called just after form initialization
    /// </summary>
    protected virtual void FormInitialized()
    {
    }
    
    /// <summary>
    /// Executes any first operation on the form.
    /// <para>NOTE: When this method is called the form has already a parent, the <see cref="Context"/> is assigned and the <see cref="FormInitialize"/> is finished. </para>
    /// </summary>
    protected virtual async Task Start()
    {
        await Task.CompletedTask;
    }
    /// <summary>
    /// This is called just after the <see cref="Context"/> is assigned.
    /// <para>NOTE: When this method is called the form has already a parent, the <see cref="Context"/> is assigned buth the <see cref="FormInitialize"/> has not been called. </para>
    /// </summary>
    protected virtual void Setup()
    {
    }
    
    /// <summary>
    /// Processes the form options
    /// </summary>
    protected virtual void ProcessFormOptions()
    {
    }
    /// <summary>
    /// Called just before the form is closed.
    /// </summary>
    protected virtual void Closing()
    {
    }
    /// <summary>
    /// Called just after the form is closed.
    /// </summary>
    protected virtual void Closed()
    {
        UnregisterBroadcaster();
    }
    /// <summary>
    /// Called when the <see cref="TitleText"/> property changes.
    /// </summary>
    protected virtual void TitleTextChanged()
    {
        if (this.ParentTabPage != null)
            this.ParentTabPage.Header = TitleText;
        if (this.ParentWindow != null)
            this.ParentWindow.Title = TitleText;
    }
    
    // ● miscs
    /// <summary>
    /// Passes any result to the caller of the form, if any. Useful with modal forms.
    /// </summary>
    protected virtual void PassResultBack()
    {
    }
    /// <summary>
    /// Returns the control that is last added to the container
    /// </summary>
    /// <param name="Container">The container to search.</param>
    /// <returns>The first focusable control, if any; otherwise, null.</returns>
    protected virtual Control FindFirstFocusableControl(Control Container)
    {
        return null;
    }
    /// <summary>
    /// Handles a broadcaster event.
    /// </summary>
    /// <param name="EventName">The broadcaster event name.</param>
    /// <param name="Args">The broadcaster event arguments.</param>
    protected virtual void HandleBroadcasterEvent(string EventName, IDictionary<string, object> Args)
    {
        switch (EventName)
        {
            case "NOT_EXISTED_EVENT_NAME": 
                break;
        }
    }
    /// <summary>
    /// It is called by the OnKeyDown() method. 
    /// <para>Returns true if processes the key</para>
    /// </summary>
    /// <param name="e">The key event arguments.</param>
    /// <returns>True if the key was processed; otherwise, false.</returns>
    protected virtual bool ProcessKeyDown(KeyEventArgs e)
    {
        if (!Design.IsDesignMode)
        {
            if (e.Key == Key.Escape)
            {
                return ProcessEscapeKey();
            }
        }
        return false;
    }
    /// <summary>
    /// It is called when the escape key is pressed. 
    /// <para>Returning true indicates that the key press is handled.</para>
    /// <para>NOTE: By default, when is a modal dialog, it sets <see cref="ModalResult"/> to Cancel, and closes the form.</para>
    /// </summary>
    /// <returns>True if the escape key was processed; otherwise, false.</returns>
    protected virtual bool ProcessEscapeKey()
    {
        if (this.IsModal)
        {
            this.ModalResult = ModalResult.Cancel;
            return true;
        }

        return false;
    }  
    /// <summary>
    /// Registers this form as a broadcaster listener.
    /// </summary>
    protected virtual void RegisterBroadcaster()
    {
        if (!fBroadcasterRegistered)
        {
            Broadcaster.Add(this);
            fBroadcasterRegistered = true;
        }
    }
    /// <summary>
    /// Unregisters this form from the broadcaster.
    /// </summary>
    protected virtual void UnregisterBroadcaster()
    {
        if (fBroadcasterRegistered)
        {
            Broadcaster.Remove(this);
            fBroadcasterRegistered = false;
        }
    }
    
    // ● overrides
    /// <summary>
    /// Called when the control is added to a rooted visual tree. 
    /// </summary>
    /// <param name="e">The visual tree attachment event arguments.</param>
    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (!Design.IsDesignMode && IsSetupDone)
            RegisterBroadcaster();

        if (!Design.IsDesignMode && !IsFormInitialized)
        {
            FormInitializing();
            FormInitialize();
            this.IsFormInitialized = true;
            FormInitialized();
            await Start();
            this.IsFormInitialized = true;
        }
    }
    /// <summary>
    /// Called when the control is removed from a rooted visual tree.
    /// </summary>
    /// <param name="e">The visual tree attachment event arguments.</param>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UnregisterBroadcaster();
        base.OnDetachedFromVisualTree(e);
    }
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public AppForm()
    {
        Classes.Add("AppForm");
        AddHandler(InputElement.KeyDownEvent, (Sender, Args) =>
        {
            if (Args.Handled)
                return;

            if (!Design.IsDesignMode)
            {
                if (ProcessKeyDown(Args))
                    Args.Handled = true;
            }
        }, RoutingStrategies.Bubble, handledEventsToo: true);
    }
    
    // ● public
    /// <summary>
    /// This is called from the <see cref="AppFormPagerHandler"/> or the <see cref="AppFormDialog"/> right after the form creation.
    /// <para>NOTE: When this is called the form has just added to its parent.</para>
    /// </summary>
    /// <param name="Context">The form context.</param>
    public void Setup(FormContext Context)
    {
        if (!IsSetupDone)
        {
            this.Context = Context;
            this.TitleText = Context.Title;
            Setup();
            IsSetupDone = true;
            RegisterBroadcaster();
            
            Dispatcher.UIThread.Post(() => 
            {  
                Context.ParentControl.Content = this; // this triggers the OnAttachedToVisualTree
            }, DispatcherPriority.Background);   
        }
    }
    /// <summary>
    /// Processes a broadcaster message.
    /// </summary>
    /// <param name="Args">The broadcaster event arguments.</param>
    public virtual void ProcessBroadcasterMessage(BroadcasterArgs Args)
    {
        if (Args != null)
            HandleBroadcasterEvent(Args.EventName, Args.Params);
    }
    /// <summary>
    /// Set the <see cref="ParentTabPage"/> as the selected tab page of its parent <see cref="TabControl"/>
    /// </summary>
    public virtual void SetAsSelectedForm()
    {
        if (!IsSelectedForm && (ParentTabControl != null) && (ParentTabPage != null))
            ParentTabControl.SelectedItem = ParentTabPage;
    }
    /// <summary>
    /// Returns true if this page can close.
    /// </summary>
    public virtual bool CanCloseForm() => true;
    /// <summary>
    /// Closes this page.
    /// <para>This default implementation it just removes the parent <see cref="ParentTabPage"/> from its parent <see cref="TabControl"/>.</para>
    /// </summary>
    public virtual void CloseForm()
    {
        if (CanCloseForm())
        {
            IsClosing = true;
            try
            {
                Closing();
                
                if ((ParentTabControl != null) && (ParentTabPage != null))
                    ParentTabControl.Items.Remove(ParentTabPage);
                else if (ParentWindow != null)
                    ParentWindow.Close();
            }
            finally
            {
                IsClosing = false;
            }

            Closed();
        }
        

    }
 
    // ● properties
    /// <summary>
    /// Gets the form context.
    /// </summary>
    public FormContext Context { get; private set; }
    /// <summary>
    /// A unique id among all pages hosted in the same <see cref="TabControl"/>
    /// </summary>
    public string FormId => Context.FormId;
    
    /// <summary>
    /// The parent TabItem or Window.
    /// </summary>
    public ContentControl ParentControl => Context.ParentControl;
    /// <summary>
    /// The parent <see cref="TabItem"/> hosting this page.
    /// </summary>
    public TabItem ParentTabPage => ParentControl as TabItem;
    /// <summary>
    /// The parent <see cref="TabControl"/>
    /// </summary>
    public TabControl ParentTabControl => (ParentTabPage != null) ? ParentTabPage.FindAncestorOfType<TabControl>() : null;
    /// <summary>
    /// The window dialog showing the form.
    /// </summary>
    public Window ParentWindow => ParentControl as Window;
    
    /// <summary>
    /// True when the setup of this page is done.
    /// </summary>
    public bool IsSetupDone { get; protected set; }
    /// <summary>
    /// True when initialization is done.
    /// </summary>
    public bool IsFormInitialized { get; protected set; }
    /// <summary>
    /// When true then the user can close this page by middle clicking the parent <see cref="ParentTabPage"/>
    /// </summary>
    public bool ClosableByUser { get; protected set; } = true;
    /// <summary>
    /// True when the <see cref="ParentTabPage"/> is the selected page in its parent <see cref="TabControl"/>
    /// </summary>
    public bool IsSelectedForm => (ParentTabControl != null) && (ParentTabPage != null)? ParentTabControl.SelectedItem ==  ParentTabPage : false;
 
    /// <summary>
    /// The text to display in the tab
    /// </summary>
    public string TitleText
    {
        get => fTitleText;
        set
        {
            if (fTitleText != value)
            {
                fTitleText = value;
                TitleTextChanged();
            }
        }
    }
    /// <summary>
    /// True while closing.
    /// </summary>
    public bool IsClosing { get; private set; }
    /// <summary>
    /// True when this is displayed in a modal dialog
    /// </summary>
    public bool IsModal => Context.DisplayMode == FormDisplayMode.Dialog;
    /// <summary>
    /// The modal result of the modal dialog displaying this instance.
    /// <para>Setting this to anything other than <see cref="ModalResult.None"/> closes the container form.</para>
    /// </summary>
    public virtual ModalResult ModalResult
    {
        get => IsModal ? fModalResult : ModalResult.None;
        set
        {
            if (IsModal && fModalResult != value)
            {
                fModalResult = value;
                if (fModalResult != ModalResult.None)
                {
                    Context.ModalResult = fModalResult;
                    PassResultBack();
                    CloseForm();     
                }
            }
        }
    }
}
