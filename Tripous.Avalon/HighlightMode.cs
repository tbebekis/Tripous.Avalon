namespace Tripous.Avalon;

public enum HighlightMode
{
    None,
    CSharp,
    Javascript,
    HTML,
    Boo,
    Coco,
    CSS,
    Cpp,
    Java,
    PowerShell,
    PHP,
    Python,
    SQL,
    VB,
    XML,
    Markdown,
    JSON,
}

static public class Highlighters
{
	class HighlighterItem
	{
		public string Key { get; set; }
		public string[] Extensions { get; set; }
	}

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

 