namespace PasswordManager.Services;

/// <summary>
/// DTO used by the encrypted credential import/export sample service.
/// </summary>
public class CredentialTransferRow
{
    // ● properties
    /// <summary>
    /// Gets or sets the row id.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Gets or sets the category id.
    /// </summary>
    public int CategoryId { get; set; }
    /// <summary>
    /// Gets or sets the credential title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// Gets or sets the encrypted password value.
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// Gets or sets the encrypted notes value.
    /// </summary>
    public string Notes { get; set; }
    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Gets or sets the update date.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
