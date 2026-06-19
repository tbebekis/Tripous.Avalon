namespace TemplateApp;

/// <summary>
/// Represents the Avalonia application.
/// </summary>
public partial class App : Application
{
    // ● public methods
    /// <summary>
    /// Initializes application resources.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    /// <summary>
    /// Completes application startup.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime Desktop)
        {
            Desktop.MainWindow = AppHost.HiddenMainWindow;
            Desktop.MainWindow.Opened += async (Sender, Args) => await AppHost.Start(Desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
