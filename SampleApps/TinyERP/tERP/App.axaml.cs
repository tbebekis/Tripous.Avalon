/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace tERP;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = AppHost.StartupMainWindow; //new MainWindow();
            desktop.MainWindow.Opened += async (s, e) =>
            {
                await Dispatcher.UIThread.InvokeAsync(async () => await AppHost.Start(desktop), DispatcherPriority.Background);
            };
        }
    }
}
