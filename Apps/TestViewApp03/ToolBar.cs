using System;
using System.Collections;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Tripous.Avalon;

public class ToolBar
{
    protected StackPanel fPanel;
    protected virtual void SetupButton(Button Button, string ImageFileName = null, string ToolTipText = null)
    {
        Uri Uri = Ui.FindImageUri(ImageFileName);
        if (Uri != null)
            Button.Content = new Image() { Source = new Bitmap(AssetLoader.Open(Uri)) };
        else if (!string.IsNullOrWhiteSpace(ToolTipText))
            Button.Content = ToolTipText;
        
        if (!string.IsNullOrWhiteSpace(ToolTipText))
            ToolTip.SetTip(Button, ToolTipText);
    }

    protected virtual void PanelChanging()
    {
        RemoveAll();
    }
    protected virtual void PanelChanged()
    {
    }
    protected virtual void RemovingAll()
    {
    }
    protected virtual void RemovedAll()
    {
    }
    
    // ● construction
    public ToolBar()
    {
    }
    
    // ● public
    public virtual void RemoveAll()
    {
        if (fPanel != null)
        {
            RemovingAll();
            fPanel.Children.Clear();
            RemovedAll();
        }
    }
    
    public Border AddSeparator(string Name = null)
    {
        Border Result = new Border();

        if (!string.IsNullOrWhiteSpace(Name))
            Result.Name = Name;

        Result.Width = 1;
        Result.Height = 20;
        Result.Margin = new Thickness(4, 0, 4, 0);
        Result.Background = Brushes.Gray;

        Panel.Children.Add(Result);

        return Result;
    }
    public Button AddButton(string ImageFileName = null, string ToolTipText = null, EventHandler<RoutedEventArgs> OnClick = null)
    {
        Button Result = new Button();

        SetupButton(Result, ImageFileName, ToolTipText);

        if (OnClick != null)
            Result.Click += (Sender, Args) => OnClick(Sender, Args);
 
        Panel.Children.Add(Result);

        return Result;
    }
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
    public TextBlock AddTextBlock(string Text = null)
    {
        TextBlock Result = new TextBlock();

        if (!string.IsNullOrWhiteSpace(Text))
            Result.Text = Text;

        Panel.Children.Add(Result);

        return Result;
    }
    public Label AddLabel(string Text = null)
    {
        Label Result = new Label();

        if (!string.IsNullOrWhiteSpace(Text))
            Result.Content = Text;

        Panel.Children.Add(Result);

        return Result;
    }
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
    
    // ● properties
    public virtual StackPanel Panel
    {
        get => fPanel;
        set
        {
            if (fPanel != value)
            {
                PanelChanging();
                fPanel = value;
                PanelChanged();
            }
        }
    }
}