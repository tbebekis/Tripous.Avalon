using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Tripous.Desktop;

namespace HelloTripous;

/// <summary>
/// Represents the Avalonia application.
/// </summary>
public partial class App : Application
{
    // ● public
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
            // ● Create the main window.
            var Window = new MainWindow();
            // ● Let Tripous.Desktop helpers know which window owns dialogs.
            Ui.MainWindow = Window;
            // ● Give Avalonia the main window of this desktop application.
            Desktop.MainWindow = Window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
