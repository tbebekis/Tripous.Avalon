using Avalonia.Controls;

namespace TestApp00;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
 
    
    // ● event handlers
    void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
    }
    
    // ● private
    void WindowInitialize()
    {
        
    }

    void Test()
    {
        
    }

    void Test2()
    {
        
    }
    
    // ● construction
    public MainWindow()
    {
        InitializeComponent();
        
        this.Loaded += (s, e) =>
        {
            if (IsWindowInitialized)
                return;
            WindowInitialize();
            IsWindowInitialized = true;
        };
    }
}