// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.GroupGrid;

public partial class App : Application
{
    // ● public
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}
