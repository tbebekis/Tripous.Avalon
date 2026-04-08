namespace Tripous;

public class CodePage
{
    // ● construction
    public CodePage() { }
    public CodePage(Encoding Encoding)
    {
        Name = Encoding.WebName;
        DisplayName = Encoding.EncodingName;
    }

    // ● static public methods
    /// <summary>
    /// Returns all supported CodePages of the system.
    /// Assumes that Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    /// </summary>
    static public List<CodePage> GetSupportedCodePagesAll()
    {
        return Encoding.GetEncodings()
            .Select(Info => new CodePage(Info.GetEncoding()))
            .OrderBy(Item => Item.DisplayName)
            .ToList();
    }
    static public List<CodePage> GetSupportedCodePages()
    {
        int[] Selected = { 65001, 1253, 1252, 1200, 737, 28597 }; // UTF-8, Greek, Western, UTF-16, DOS Greek, ISO Greek
        return Selected
            .Select(CP => new CodePage(Encoding.GetEncoding(CP)))
            .OrderBy(Item => Item.DisplayName)
            .ToList();
    }
    
    // ● public methods
    public override string ToString() => DisplayName;

    // ● properties
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public Encoding Encoding => Encoding.GetEncoding(Name);
}