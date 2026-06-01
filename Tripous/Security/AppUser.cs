namespace Tripous;


/// <summary>
/// Represents a user of this application
/// </summary>
public sealed class AppUser
{
    // ● properties
    /// <summary>
    /// User id.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// User name.
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// User full name.
    /// </summary>
    public string FullName { get; set; }
    /// <summary>
    /// User level.
    /// </summary>
    public UserLevel UserLevel { get; set; }
    /// <summary>
    /// User email.
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// User phone.
    /// </summary>
    public string Phone { get; set; }
    /// <summary>
    /// Last login date and time.
    /// </summary>
    public DateTime LastLoginAt { get; set; }
    /// <summary>
    /// True when the user account is active.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Preferred culture code, e.g. en-US, el-GR.
    /// </summary>
    public string CultureCode { get; set; }
    /// <summary>
    /// Preferred language code, e.g. en, el.
    /// </summary>
    public string LanguageCode { get; set; }
    /// <summary>
    /// User remarks.
    /// </summary>
    public string Remarks { get; set; }
    /// <summary>
    /// True when user has administrator privileges.
    /// </summary>
    public bool IsAdmin => (UserLevel & UserLevel.Admin) == UserLevel.Admin;
    /// <summary>
    /// True when user has god privileges.
    /// </summary>
    public bool IsGod => (UserLevel & UserLevel.God) == UserLevel.God;
    /// <summary>
    /// A bag of custom properties.
    /// </summary>
    public Dictionary<string, object> Properties { get; } = new();
}