// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.PivotGrid;

/// <summary>
/// Provides the demo application entry point.
/// </summary>
class Program
{
    // ● public
    /// <summary>
    /// Starts the demo application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    /// <summary>
    /// Builds the Avalonia application.
    /// </summary>
    /// <returns>The application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
