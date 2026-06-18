namespace ToDo;

/// <summary>
/// Hidden startup window used while the application initializes.
/// </summary>
public class HiddenMainWindow : Window
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="HiddenMainWindow"/> class.
    /// </summary>
    public HiddenMainWindow()
    {
        Width = 5;
        Height = 5;
        Position = new PixelPoint(10000, 10000);
        ShowInTaskbar = false;
    }
}
