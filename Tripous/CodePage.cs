/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Represents a text encoding (code page) supported by the system.
/// </summary>
public class CodePage
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public CodePage() { }
    /// <summary>
    /// Constructs an instance from a text encoding.
    /// </summary>
    public CodePage(Encoding Encoding)
    {
        Name = Encoding.WebName;
        DisplayName = Encoding.EncodingName;
    }

    // ● static public methods
    /// <summary>
    /// Returns all code pages supported by the system.
    ///
    /// Assumes that:
    /// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    ///
    /// has already been called.
    /// </summary>
    static public List<CodePage> GetSupportedCodePagesAll()
    {
        return Encoding.GetEncodings()
            .Select(Info => new CodePage(Info.GetEncoding()))
            .OrderBy(Item => Item.DisplayName)
            .ToList();
    }
    /// <summary>
    /// Returns a predefined list of commonly used code pages.
    /// </summary>
    static public List<CodePage> GetSupportedCodePages()
    {
        int[] Selected = { 65001, 1253, 1252, 1200, 737, 28597 }; // UTF-8, Greek, Western, UTF-16, DOS Greek, ISO Greek
        return Selected
            .Select(CP => new CodePage(Encoding.GetEncoding(CP)))
            .OrderBy(Item => Item.DisplayName)
            .ToList();
    }
    
    // ● public methods
    /// <summary>
    /// Returns the display name of this code page.
    /// </summary>
    public override string ToString() => DisplayName;

    // ● properties
    /// <summary>
    /// Gets or sets the encoding web name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the user-friendly encoding name.
    /// </summary>
    public string DisplayName { get; set; }
    /// <summary>
    /// Gets the corresponding text encoding.
    /// </summary>
    public Encoding Encoding => Encoding.GetEncoding(Name);
}