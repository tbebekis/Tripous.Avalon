namespace PasswordManager;

/// <summary>
/// Small hidden owner window used during startup.
/// </summary>
public class HiddenMainWindow : Window
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="HiddenMainWindow"/> class.
    /// </summary>
    public HiddenMainWindow()
    {
        Width = 1;
        Height = 1;
        WindowStartupLocation = WindowStartupLocation.Manual;
        Position = new PixelPoint(9000, 100);
        ShowInTaskbar = false;
        CanResize = false;
        Title = "Password Manager Startup";
    }
}
