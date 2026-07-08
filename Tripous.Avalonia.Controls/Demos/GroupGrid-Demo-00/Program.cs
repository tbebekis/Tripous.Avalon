// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.GroupGrid;

class Program
{
    // ● public
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
