namespace PasswordManager;

/// <summary>
/// Provides the Avalonia application object.
/// </summary>
public partial class App : Application
{
    // ● public
    /// <summary>
    /// Initializes Avalonia XAML.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    /// <summary>
    /// Handles framework initialization.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime Desktop)
        {
            Desktop.MainWindow = AppHost.HiddenMainWindow;
            AppHost.HiddenMainWindow.Opened += async (Sender, Args) => await AppHost.Start(Desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
