namespace MiniCrm;

/// <summary>
/// Provides the application entry point.
/// </summary>
public class Program
{
    // ● static public
    /// <summary>
    /// Starts the application.
    /// </summary>
    /// <param name="Args">The command line arguments.</param>
    [STAThread]
    static public void Main(string[] Args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(Args);
    /// <summary>
    /// Creates and configures the Avalonia application builder.
    /// </summary>
    /// <returns>The configured application builder.</returns>
    static public AppBuilder BuildAvaloniaApp()
    {
        var Builder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

        if (OperatingSystem.IsLinux())
        {
            // ● Keep the menu inside the application window on Linux.
            Builder.With(new X11PlatformOptions { UseDBusMenu = false });
        }

        return Builder;
    }
}
