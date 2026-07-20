// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Demo00.PivotGrid;

/// <summary>
/// Provides the demo application object.
/// </summary>
public partial class App: Application
{
    // ● public
    /// <inheritdoc />
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}
