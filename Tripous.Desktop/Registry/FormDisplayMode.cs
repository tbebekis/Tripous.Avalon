namespace Tripous.Desktop;


/// <summary>
/// Indicates how a form is opened and hosted.
/// </summary>
public enum FormDisplayMode
{
    /// <summary>
    /// Opens the form in the main UI, typically inside a tab.
    /// Full functionality is available.
    /// </summary>
    TabItem,
    /// <summary>
    /// Opens the form in a modal dialog window.
    /// Enables OK/Cancel semantics and result returning.
    /// </summary>
    Dialog,
}