/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Defines supported syntax highlighting modes.
/// </summary>
public enum HighlightMode
{
    /// <summary>
    /// No syntax highlighting.
    /// </summary>
    None,
    /// <summary>
    /// C# syntax highlighting.
    /// </summary>
    CSharp,
    /// <summary>
    /// JavaScript syntax highlighting.
    /// </summary>
    Javascript,
    /// <summary>
    /// HTML syntax highlighting.
    /// </summary>
    HTML,
    /// <summary>
    /// Boo syntax highlighting.
    /// </summary>
    Boo,
    /// <summary>
    /// Coco/R syntax highlighting.
    /// </summary>
    Coco,
    /// <summary>
    /// CSS syntax highlighting.
    /// </summary>
    CSS,
    /// <summary>
    /// C++ syntax highlighting.
    /// </summary>
    Cpp,
    /// <summary>
    /// Java syntax highlighting.
    /// </summary>
    Java,
    /// <summary>
    /// PowerShell syntax highlighting.
    /// </summary>
    PowerShell,
    /// <summary>
    /// PHP syntax highlighting.
    /// </summary>
    PHP,
    /// <summary>
    /// Python syntax highlighting.
    /// </summary>
    Python,
    /// <summary>
    /// SQL syntax highlighting.
    /// </summary>
    SQL,
    /// <summary>
    /// Visual Basic syntax highlighting.
    /// </summary>
    VB,
    /// <summary>
    /// XML syntax highlighting.
    /// </summary>
    XML,
    /// <summary>
    /// Markdown syntax highlighting.
    /// </summary>
    Markdown,
    /// <summary>
    /// JSON syntax highlighting.
    /// </summary>
    JSON,
}

/// <summary>
/// Provides syntax highlighter lookup methods.
/// </summary>
static public class Highlighters
{
	/// <summary>
	/// Defines a syntax highlighter registration item.
	/// </summary>
	class HighlighterItem
	{
		/// <summary>
		/// Gets or sets the highlighting definition key.
		/// </summary>
		public string Key { get; set; }
		/// <summary>
		/// Gets or sets the file extensions associated with the highlighter.
		/// </summary>
		public string[] Extensions { get; set; }
	}

	// ● private fields
	/// <summary>
	/// The registered highlighter items.
	/// </summary>
	static private Dictionary<HighlightMode, HighlighterItem> Items = new()
	{
		{ HighlightMode.CSharp, new() { Key = "C#", Extensions = new []{ ".cs" }} },
		{ HighlightMode.Javascript, new() { Key = "JavaScript", Extensions = new []{ ".js" }} },
		{ HighlightMode.HTML, new() { Key = "HTML", Extensions = new []{  ".htm", ".html"  }} },
		{ HighlightMode.Boo, new() { Key = "Boo", Extensions = new []{ ".boo" }} },
		{ HighlightMode.Coco, new() { Key = "Coco", Extensions = new []{ ".atg" }} },
		{ HighlightMode.CSS, new() { Key = "CSS", Extensions = new []{ ".css" }} },
		{ HighlightMode.Cpp, new() { Key = "C++", Extensions = new []{ ".c", ".h", ".cc", ".cpp", ".hpp" }} },
		{ HighlightMode.Java, new() { Key = "Java", Extensions = new []{ ".java" }} },
		{ HighlightMode.PowerShell, new() { Key = "PowerShell", Extensions = new []{  ".ps1", ".psm1", ".psd1"}} },
		{ HighlightMode.PHP, new() { Key = "PHP", Extensions = new []{ ".php" }} },
		{ HighlightMode.Python, new() { Key = "Python", Extensions = new []{ ".py", ".pyw" }} },
		{ HighlightMode.SQL, new() { Key = "TSQL", Extensions = new []{ ".sql" }} },
		{ HighlightMode.VB, new() { Key = "VB", Extensions = new []{ ".vb" }} },
		{ HighlightMode.XML, new() { Key = "XML", Extensions = (".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;" +
		                                                        ".xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;" +
		                                                        ".booproj;.build;.xfrm;.targets;.xaml;.xpt;" +
		                                                        ".xft;.map;.wsdl;.disco;.ps1xml;.nuspec").Split(';')} },
		{ HighlightMode.Markdown, new() { Key = "MarkDownWithFontSize", Extensions = new []{ ".md" }} },
		{ HighlightMode.JSON, new() { Key = "Json", Extensions = new []{ ".json" }} },
	};


	// ● static public
	/// <summary>
	/// Finds a highlighting definition by mode.
	/// </summary>
	/// <param name="Mode">The highlight mode.</param>
	/// <returns>The highlighting definition, if any; otherwise, null.</returns>
	static public IHighlightingDefinition Find(HighlightMode Mode)
	{
		IHighlightingDefinition Result = null;
		
		if (Items.ContainsKey(Mode))
		{
			HighlighterItem Item = Items[Mode];
			Result = HighlightingManager.Instance.GetDefinition(Item.Key);
		}

		return Result;
	}
	/// <summary>
	/// Finds a highlighting definition by file extension.
	/// </summary>
	/// <param name="Ext">The file extension.</param>
	/// <returns>The highlighting definition, if any; otherwise, null.</returns>
	static public IHighlightingDefinition FindByExtension(string Ext)
	{
		IHighlightingDefinition Result = null;

		if (!string.IsNullOrWhiteSpace(Ext))
		{
			if (!Ext.StartsWith('.'))
				Ext = "." + Ext;

			foreach (var Entry in Items)
			{
				HighlighterItem Item = Entry.Value;
				foreach (var S in Item.Extensions)
				{
					if (S.IsSameText(Ext))
					{
						Result = HighlightingManager.Instance.GetDefinition(Item.Key);;
						break;
					}
				}
			}
		}
			
		return Result;	
	}
}
