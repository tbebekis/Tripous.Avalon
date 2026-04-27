using Avalonia;
using System;

namespace TestApp;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    /*
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    */
    
    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

        if (OperatingSystem.IsLinux())
        {
            // for X11
            builder.With(new X11PlatformOptions 
            { 
                UseDBusMenu = false 
            });

            // for Wayland
            /*
            builder.With(new WaylandPlatformOptions
            {
                // Similar philosophy, although Wayland has
                // different handling for DBus services
                UseDBusMenu = false
            });
            */
        }

        return builder;
    }
}