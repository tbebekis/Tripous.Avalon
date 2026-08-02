/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Helper class for creating and managing toolbar controls.
/// </summary>
public class ToolBar
{
    // ● protected fields
    /// <summary>
    /// The panel hosting toolbar controls.
    /// </summary>
    protected StackPanel fPanel;

    // ● protected methods
    /// <summary>
    /// Configures a toolbar button.
    /// </summary>
    /// <param name="Button">The button to configure.</param>
    /// <param name="ImageFileName">The image file name.</param>
    /// <param name="ToolTipText">The tooltip text.</param>
    protected virtual void SetupButton(Button Button, string ImageFileName = null, string ToolTipText = null)
    {
        Image Image = AvaloniaAssets.FindImage(ImageFileName);
        if (Image != null)
            Button.Content = Image;
        else if (!string.IsNullOrWhiteSpace(ToolTipText))
            Button.Content = ToolTipText;
        
        if (!string.IsNullOrWhiteSpace(ToolTipText))
        {
            ToolTip.SetTip(Button, ToolTipText);
            ToolTip.SetShowOnDisabled(Button, true);
        }
    }

    /// <summary>
    /// Called before the panel changes.
    /// </summary>
    protected virtual void PanelChanging()
    {
        RemoveAll();
    }
    /// <summary>
    /// Called after the panel changes.
    /// </summary>
    protected virtual void PanelChanged()
    {
    }
    /// <summary>
    /// Called before all toolbar controls are removed.
    /// </summary>
    protected virtual void RemovingAll()
    {
    }
    /// <summary>
    /// Called after all toolbar controls are removed.
    /// </summary>
    protected virtual void RemovedAll()
    {
    }
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBar"/> class.
    /// </summary>
    public ToolBar()
    {
    }
    
    // ● public
    /// <summary>
    /// Removes all toolbar controls.
    /// </summary>
    public virtual void RemoveAll()
    {
        if (fPanel != null)
        {
            RemovingAll();
            fPanel.Children.Clear();
            RemovedAll();
        }
    }
    
    /// <summary>
    /// Adds a separator to the toolbar.
    /// </summary>
    /// <param name="Name">The separator name.</param>
    /// <returns>The created separator.</returns>
    public Border AddSeparator(string Name = null)
    {
        Border Result = new Border();

        if (!string.IsNullOrWhiteSpace(Name))
            Result.Name = Name;

        Result.Width = 1;
        Result.Height = 20;
        Result.Margin = new Thickness(4, 0, 4, 0);
        Result.Classes.Add("ToolBarSeparator");

        Panel.Children.Add(Result);

        return Result;
    }

    /// <summary>
    /// Adds a button to the toolbar.
    /// </summary>
    /// <returns>The created button.</returns>
    public Button AddButton()
    {
        string ImageFileName = "";
        string ToolTipText = "";
        EventHandler<RoutedEventArgs> OnClick = null;
        return AddButton(ImageFileName, ToolTipText, OnClick);
    }
    /// <summary>
    /// Adds a button to the toolbar.
    /// </summary>
    /// <param name="ImageFileName">The image file name.</param>
    /// <returns>The created button.</returns>
    public Button AddButton(string ImageFileName)
    {
        string ToolTipText = "";
        EventHandler<RoutedEventArgs> OnClick = null;
        return AddButton(ImageFileName, ToolTipText, OnClick);
    }
    /// <summary>
    /// Adds a button to the toolbar.
    /// </summary>
    /// <param name="ImageFileName">The image file name.</param>
    /// <param name="ToolTipText">The tooltip text.</param>
    /// <param name="OnClick">The click event handler.</param>
    /// <returns>The created button.</returns>
    public Button AddButton(string ImageFileName, string ToolTipText, EventHandler<RoutedEventArgs> OnClick)
    {
        Button Result = new Button();

        SetupButton(Result, ImageFileName, ToolTipText);

        if (OnClick != null)
            Result.Click += (Sender, Args) => OnClick(Sender, Args);
 
        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a button to the toolbar.
    /// </summary>
    /// <param name="ImageFileName">The image file name.</param>
    /// <param name="ToolTipText">The tooltip text.</param>
    /// <param name="Action">The asynchronous click action.</param>
    /// <returns>The created button.</returns>
    public Button AddButton(string ImageFileName, string ToolTipText, Func<Task> Action)
    {
        Button Result = new Button();

        SetupButton(Result, ImageFileName, ToolTipText);

        if (Action != null)
            Result.Click += async (Sender, Args) => await Action();

        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a button to the toolbar.
    /// </summary>
    /// <param name="ImageFileName">The image file name.</param>
    /// <param name="ToolTipText">The tooltip text.</param>
    /// <param name="Action">The click action.</param>
    /// <returns>The created button.</returns>
    public Button AddButton(string ImageFileName, string ToolTipText, Action Action)
    {
        Button Result = new Button();

        SetupButton(Result, ImageFileName, ToolTipText);

        if (Action != null)
            Result.Click += (Sender, Args) => Action();
 
        Panel.Children.Add(Result);

        return Result;
    }
    
    /// <summary>
    /// Adds a drop-down button to the toolbar.
    /// </summary>
    /// <param name="ImageFileName">The image file name.</param>
    /// <param name="ToolTipText">The tooltip text.</param>
    /// <param name="Menu">The context menu.</param>
    /// <param name="OnOpening">The menu opening handler.</param>
    /// <returns>The created button.</returns>
    public Button AddDropDownButton(string ImageFileName = null, string ToolTipText = null, ContextMenu Menu = null, CancelEventHandler OnOpening = null)
    {
        Button Result = new Button();

        SetupButton(Result, ImageFileName, ToolTipText);

        if (Menu != null)
        {
            Result.ContextMenu = Menu;

            // it is called with right click only
            if (OnOpening != null)
                Menu.Opening += OnOpening;
 
            // it is called on button click
           Result.Click += (Sender, Args) =>
           {
               CancelEventArgs ea = new CancelEventArgs();
               OnOpening?.Invoke(Menu, ea);
               if (!ea.Cancel)
                Menu.Open(Result);
           };
        }

        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a toggle button to the toolbar.
    /// </summary>
    /// <param name="ImageFileName">The image file name.</param>
    /// <param name="ToolTipText">The tooltip text.</param>
    /// <param name="Action">The checked changed action.</param>
    /// <returns>The created toggle button.</returns>
    public ToggleButton AddToggleButton(string ImageFileName, string ToolTipText, Action Action)
    {
        ToggleButton Result = new ToggleButton();

        SetupButton(Result, ImageFileName, ToolTipText);

        if (Action != null)
            Result.IsCheckedChanged += (Sender, Args) => Action();
 
        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a toggle button to the toolbar.
    /// </summary>
    /// <param name="ImageFileName">The image file name.</param>
    /// <param name="ToolTipText">The tooltip text.</param>
    /// <param name="OnCheckedChanged">The checked changed event handler.</param>
    /// <returns>The created toggle button.</returns>
    public ToggleButton AddToggleButton(string ImageFileName = null, string ToolTipText = null, EventHandler<RoutedEventArgs> OnCheckedChanged = null)
    {
        ToggleButton Result = new ToggleButton();

        SetupButton(Result, ImageFileName, ToolTipText);

        if (OnCheckedChanged != null)
            Result.IsCheckedChanged += (Sender, Args) => OnCheckedChanged(Sender, Args);
 
        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a text box to the toolbar.
    /// </summary>
    /// <param name="Text">The initial text.</param>
    /// <param name="Width">The text box width.</param>
    /// <returns>The created text box.</returns>
    public TextBox AddTextBox(string Text = null, double Width = double.NaN)
    {
        TextBox Result = new TextBox();

        if (!string.IsNullOrWhiteSpace(Text))
            Result.Text = Text;

        if (!double.IsNaN(Width))
            Result.Width = Width;

        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a text block to the toolbar.
    /// </summary>
    /// <param name="Text">The text.</param>
    /// <returns>The created text block.</returns>
    public TextBlock AddTextBlock(string Text = null)
    {
        TextBlock Result = new TextBlock();

        if (!string.IsNullOrWhiteSpace(Text))
            Result.Text = Text;

        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a label to the toolbar.
    /// </summary>
    /// <param name="Text">The label text.</param>
    /// <returns>The created label.</returns>
    public Label AddLabel(string Text = null)
    {
        Label Result = new Label();

        if (!string.IsNullOrWhiteSpace(Text))
            Result.Content = Text;

        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds a combo box to the toolbar.
    /// </summary>
    /// <param name="ItemsSource">The items source.</param>
    /// <param name="ItemIndex">The selected item index.</param>
    /// <param name="Width">The combo box width.</param>
    /// <returns>The created combo box.</returns>
    public ComboBox AddComboBox(IEnumerable ItemsSource = null, int ItemIndex = 0, double Width = double.NaN)
    {
        ComboBox Result = new ComboBox();

        if (ItemsSource != null)
            Result.ItemsSource = ItemsSource;

        if (!double.IsNaN(Width))
            Result.Width = Width;

        Panel.Children.Add(Result);
        
        if (ItemIndex >= 0 && Result.Items.Count > 0)
            Result.SelectedIndex = ItemIndex;

        return Result;
    }
    /// <summary>
    /// Adds a check box to the toolbar.
    /// </summary>
    /// <param name="Text">The check box text.</param>
    /// <param name="IsChecked">The checked value.</param>
    /// <returns>The created check box.</returns>
    public CheckBox AddCheckBox(string Text = null, bool? IsChecked = null)
    {
        CheckBox Result = new CheckBox();

        if (!string.IsNullOrWhiteSpace(Text))
            Result.Content = Text;

        if (IsChecked.HasValue)
            Result.IsChecked = IsChecked;

        Panel.Children.Add(Result);

        return Result;
    }

    /// <summary>
    /// Adds a command button to the toolbar.
    /// </summary>
    /// <param name="Cmd">The command.</param>
    /// <returns>The created button.</returns>
    public Button Add(Command Cmd)
    {
        Button Result = Cmd.IsToggle? new ToggleButton(): new Button();
        Result.Tag = Cmd;
        Cmd.Tag = Result;

        SetupButton(Result, Cmd.ImageFileName, Cmd.Title);

        if (Result is ToggleButton TB)
        {
            if (Cmd.IsAsync)
                TB.IsCheckedChanged += async (Sender, Args) => await Cmd.ExecuteAsync();
            else
                TB.IsCheckedChanged += (Sender, Args) => Cmd.Execute();
        }
        else
        {
            if (Cmd.IsAsync)
                Result.Click += async (Sender, Args) => await Cmd.ExecuteAsync();
            else
                Result.Click +=  (Sender, Args) => Cmd.Execute();
        }

 
        Panel.Children.Add(Result);

        return Result;
    }
    /// <summary>
    /// Adds command buttons to the toolbar.
    /// </summary>
    /// <param name="Commands">The commands.</param>
    public void AddRange(IEnumerable<Command> Commands)
    {
        foreach (Command Cmd in Commands)
            Add(Cmd);
    }
    
    // ● repositioning
    // Pivot and item controls must already be added to this toolbar.
    /// <summary>
    /// Places a toolbar control after another toolbar control.
    /// </summary>
    /// <param name="PivotItem">The pivot control.</param>
    /// <param name="Item">The control to move.</param>
    public void PlaceControlAfter(Control PivotItem, Control Item)
    {
        if (PivotItem == null)
            throw new ArgumentNullException(nameof(PivotItem));
        if (Item == null)
            throw new ArgumentNullException(nameof(Item));
        if (PivotItem == Item)
            throw new ArgumentException($"{nameof(PivotItem)} and {nameof(Item)} cannot be the same control.");
        if (!Panel.Children.Contains(PivotItem))
            throw new ArgumentException($"{nameof(PivotItem)} does not belong to this toolbar.", nameof(PivotItem));
        if (!Panel.Children.Contains(Item))
            throw new ArgumentException($"{nameof(Item)} does not belong to this toolbar.", nameof(Item));

        Panel.Children.Remove(Item);
        int PivotIndex = Panel.Children.IndexOf(PivotItem);
        Panel.Children.Insert(PivotIndex + 1, Item);
    }
    /// <summary>
    /// Places a toolbar control before another toolbar control.
    /// </summary>
    /// <param name="PivotItem">The pivot control.</param>
    /// <param name="Item">The control to move.</param>
    public void PlaceControlBefore(Control PivotItem, Control Item)
    {
        if (PivotItem == null)
            throw new ArgumentNullException(nameof(PivotItem));
        if (Item == null)
            throw new ArgumentNullException(nameof(Item));
        if (PivotItem == Item)
            throw new ArgumentException($"{nameof(PivotItem)} and {nameof(Item)} cannot be the same control.");
        if (!Panel.Children.Contains(PivotItem))
            throw new ArgumentException($"{nameof(PivotItem)} does not belong to this toolbar.", nameof(PivotItem));
        if (!Panel.Children.Contains(Item))
            throw new ArgumentException($"{nameof(Item)} does not belong to this toolbar.", nameof(Item));

        Panel.Children.Remove(Item);
        int PivotIndex = Panel.Children.IndexOf(PivotItem);
        Panel.Children.Insert(PivotIndex, Item);
    }
    /// <summary>
    /// Places a separator after a toolbar control.
    /// </summary>
    /// <param name="PivotItem">The pivot control.</param>
    /// <param name="Separator">The separator to move.</param>
    public void PlaceSeparatorAfter(Control PivotItem, Border Separator)
    {
        PlaceControlAfter(PivotItem, Separator);
    }
    /// <summary>
    /// Places a separator before a toolbar control.
    /// </summary>
    /// <param name="PivotItem">The pivot control.</param>
    /// <param name="Separator">The separator to move.</param>
    public void PlaceSeparatorBefore(Control PivotItem, Border Separator)
    {
        PlaceControlBefore(PivotItem, Separator);
    }

    /// <summary>
    /// Returns all toolbar buttons.
    /// </summary>
    /// <returns>The toolbar buttons.</returns>
    public Button[] GetButtons() => Panel.Children.OfType<Button>().ToArray();
    /// <summary>
    /// Returns all toolbar controls.
    /// </summary>
    /// <returns>The toolbar controls.</returns>
    public Control[] GetControls()=> Panel.Children.ToArray();
    
    // ● properties
    /// <summary>
    /// Gets or sets the panel hosting toolbar controls.
    /// </summary>
    public virtual StackPanel Panel
    {
        get => fPanel;
        set
        {
            if (fPanel != value)
            {
                PanelChanging();
                fPanel = value;
                Container = value != null? value.FindAncestorOfType<Border>(): null;
                PanelChanged();
            }
        }
    }
    /// <summary>
    /// Gets the toolbar container.
    /// </summary>
    public virtual Border Container { get; private set; }
    /// <summary>
    /// Gets or sets the toolbar visibility.
    /// </summary>
    public bool IsVisible
    {
        get
        {
            if (Container != null)
                return Container.IsVisible;
            return Panel.IsVisible;
        }
        set
        {
            if (Container != null)
                Container.IsVisible = value;
            else
                Panel.IsVisible = value;
        }
    }

}
