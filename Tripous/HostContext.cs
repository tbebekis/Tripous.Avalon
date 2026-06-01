namespace Tripous;

 

/// <summary>
/// Provides access to the current application execution context.
/// <para> <see cref="HostContext"/> because the <see cref="AppContext"/> is already taken by .Net.</para>
/// </summary>
public sealed class HostContext
{
     
    
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    internal HostContext()
    {
    }

    // ● properties
    /// <summary>
    /// True when a user is authenticated.
    /// </summary>
    public bool IsAuthenticated => CurrentUser != null;
    /// <summary>
    /// The currently authenticated user.
    /// </summary>
    public AppUser CurrentUser { get; set; }
    /// <summary>
    /// The current UI culture code, e.g. en-US, el-GR.
    /// </summary>
    public string CultureCode => CurrentUser != null ? CurrentUser.CultureCode : "en-US";  
    /// <summary>
    /// The current language code, e.g. en, el.
    /// </summary>
    public string LanguageCode
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(CultureCode))
            {
                string[] Parts = CultureCode.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (Parts.Length > 0)
                    return Parts[0];
            }
                
            return "en";
        }
    }
    /// <summary>
    /// A bag of custom application properties.
    /// </summary>
    public Dictionary<string, object> Properties { get; } = new();
}