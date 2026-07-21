// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.Charts;

/// <summary>
/// Provides the application entry point.
/// </summary>
static public class Program
{
    // ● static public
    /// <summary>
    /// Application entry point.
    /// </summary>
    /// <param name="Args">Command line arguments.</param>
    [STAThread]
    static public void Main(string[] Args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(Args);
    /// <summary>
    /// Builds the Avalonia application.
    /// </summary>
    /// <returns>The app builder.</returns>
    static public AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
