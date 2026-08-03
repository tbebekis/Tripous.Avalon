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
	/// <summary>
	/// Defines a simple highlighting definition.
	/// </summary>
	class SimpleHighlightingDefinition: IHighlightingDefinition
	{
		// ● private fields
		readonly Dictionary<string, HighlightingColor> fColors;
		readonly Dictionary<string, HighlightingRuleSet> fRuleSets;

		// ● construction
		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleHighlightingDefinition"/> class.
		/// </summary>
		public SimpleHighlightingDefinition(string Name, HighlightingRuleSet MainRuleSet, IEnumerable<HighlightingColor> Colors)
		{
			this.Name = Name;
			this.MainRuleSet = MainRuleSet;
			fColors = Colors.ToDictionary(Item => Item.Name, StringComparer.OrdinalIgnoreCase);
			fRuleSets = new Dictionary<string, HighlightingRuleSet>(StringComparer.OrdinalIgnoreCase);
			if (!string.IsNullOrWhiteSpace(MainRuleSet.Name))
				fRuleSets[MainRuleSet.Name] = MainRuleSet;
		}

		// ● public
		/// <summary>
		/// Gets a named highlighting rule set.
		/// </summary>
		/// <param name="Name">The rule set name.</param>
		/// <returns>The rule set, if any; otherwise, null.</returns>
		public HighlightingRuleSet GetNamedRuleSet(string Name) => !string.IsNullOrWhiteSpace(Name) && fRuleSets.TryGetValue(Name, out HighlightingRuleSet Result) ? Result : null;
		/// <summary>
		/// Gets a named highlighting color.
		/// </summary>
		/// <param name="Name">The color name.</param>
		/// <returns>The color, if any; otherwise, null.</returns>
		public HighlightingColor GetNamedColor(string Name) => !string.IsNullOrWhiteSpace(Name) && fColors.TryGetValue(Name, out HighlightingColor Result) ? Result : null;

		// ● properties
		/// <summary>
		/// Gets the highlighting definition name.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Gets the main rule set.
		/// </summary>
		public HighlightingRuleSet MainRuleSet { get; }
		/// <summary>
		/// Gets the named highlighting colors.
		/// </summary>
		public IEnumerable<HighlightingColor> NamedHighlightingColors => fColors.Values;
		/// <summary>
		/// Gets custom highlighting properties.
		/// </summary>
		public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();
	}
	/// <summary>
	/// A highlighting brush that resolves its color from the active editor theme.
	/// </summary>
	class ThemeHighlightingBrush: HighlightingBrush
	{
		// ● private fields
		readonly Color fLightColor;
		readonly Color fDarkColor;
		readonly IBrush fLightBrush;
		readonly IBrush fDarkBrush;

		// ● construction
		/// <summary>
		/// Initializes a new instance of the <see cref="ThemeHighlightingBrush"/> class.
		/// </summary>
		public ThemeHighlightingBrush(Color LightColor, Color DarkColor)
		{
			fLightColor = LightColor;
			fDarkColor = DarkColor;
			fLightBrush = new SolidColorBrush(LightColor);
			fDarkBrush = new SolidColorBrush(DarkColor);
		}

		// ● public
		/// <summary>
		/// Gets the brush for the current text run construction context.
		/// </summary>
		/// <param name="Context">The text run construction context.</param>
		/// <returns>The brush.</returns>
		public override IBrush GetBrush(AvaloniaEdit.Rendering.ITextRunConstructionContext Context)
		{
			ThemeVariant Theme = Context?.TextView?.ActualThemeVariant ?? GetActualThemeVariant();
			return IsDarkTheme(Theme) ? fDarkBrush : fLightBrush;
		}
		/// <summary>
		/// Gets the color for the current text run construction context.
		/// </summary>
		/// <param name="Context">The text run construction context.</param>
		/// <returns>The color.</returns>
		public override Color? GetColor(AvaloniaEdit.Rendering.ITextRunConstructionContext Context)
		{
			ThemeVariant Theme = Context?.TextView?.ActualThemeVariant ?? GetActualThemeVariant();
			return IsDarkTheme(Theme) ? fDarkColor : fLightColor;
		}
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
	/// <summary>
	/// The SQL keyword regular expression text.
	/// </summary>
	static readonly string SqlKeywordPattern = @"\b(ADD|ALL|ALTER|AND|ANY|AS|ASC|BEGIN|BETWEEN|BY|CASE|CAST|CHECK|COLUMN|COMMIT|CONSTRAINT|CREATE|CROSS|DATABASE|DEFAULT|DELETE|DESC|DISTINCT|DROP|ELSE|END|EXCEPT|EXEC|EXISTS|FOREIGN|FROM|FULL|GROUP|HAVING|IN|INDEX|INNER|INSERT|INTERSECT|INTO|IS|JOIN|KEY|LEFT|LIKE|LIMIT|NOT|NULL|ON|OR|ORDER|OUTER|PRIMARY|PROCEDURE|REFERENCES|RIGHT|ROLLBACK|SELECT|SET|TABLE|THEN|TOP|TRAN|TRANSACTION|UNION|UNIQUE|UPDATE|VALUES|VIEW|WHEN|WHERE|WITH)\b";
	/// <summary>
	/// The light theme SQL highlighting definition.
	/// </summary>
	static readonly IHighlightingDefinition SqlLightDefinition = CreateSqlDefinition(false);
	/// <summary>
	/// The dark theme SQL highlighting definition.
	/// </summary>
	static readonly IHighlightingDefinition SqlDarkDefinition = CreateSqlDefinition(true);
	/// <summary>
	/// The theme-aware SQL highlighting definition.
	/// </summary>
	static readonly IHighlightingDefinition SqlThemeDefinition = CreateSqlDefinition(null);
	/// <summary>
	/// The light theme Markdown highlighting definition.
	/// </summary>
	static readonly IHighlightingDefinition MarkdownLightDefinition = CreateMarkdownDefinition(false);
	/// <summary>
	/// The dark theme Markdown highlighting definition.
	/// </summary>
	static readonly IHighlightingDefinition MarkdownDarkDefinition = CreateMarkdownDefinition(true);
	/// <summary>
	/// The theme-aware Markdown highlighting definition.
	/// </summary>
	static readonly IHighlightingDefinition MarkdownThemeDefinition = CreateMarkdownDefinition(null);

	// ● private methods
	static HighlightingColor CreateColor(string Name, string ColorText, string DarkColorText = null, FontWeight? FontWeight = null, FontStyle? FontStyle = null)
	{
		return new HighlightingColor
		{
			Name = Name,
			Foreground = !string.IsNullOrWhiteSpace(DarkColorText)
				? new ThemeHighlightingBrush(Color.Parse(ColorText), Color.Parse(DarkColorText))
				: new SimpleHighlightingBrush(Color.Parse(ColorText)),
			FontWeight = FontWeight,
			FontStyle = FontStyle
		};
	}
	static HighlightingColor CreateSqlColor(string Name, bool? IsDark, string LightColorText, string DarkColorText, FontWeight? FontWeight = null, FontStyle? FontStyle = null)
	{
		if (IsDark == true)
			return CreateColor(Name, DarkColorText, FontWeight: FontWeight, FontStyle: FontStyle);
		if (IsDark == false)
			return CreateColor(Name, LightColorText, FontWeight: FontWeight, FontStyle: FontStyle);
		return CreateColor(Name, LightColorText, DarkColorText, FontWeight, FontStyle);
	}
	static HighlightingColor CreateThemeColor(string Name, bool? IsDark, string LightColorText, string DarkColorText, FontWeight? FontWeight = null, FontStyle? FontStyle = null)
	{
		if (IsDark == true)
			return CreateColor(Name, DarkColorText, FontWeight: FontWeight, FontStyle: FontStyle);
		if (IsDark == false)
			return CreateColor(Name, LightColorText, FontWeight: FontWeight, FontStyle: FontStyle);
		return CreateColor(Name, LightColorText, DarkColorText, FontWeight, FontStyle);
	}
	static HighlightingRule CreateRule(string Pattern, HighlightingColor Color, RegexOptions Options = RegexOptions.None)
	{
		return new HighlightingRule
		{
			Regex = new Regex(Pattern, RegexOptions.CultureInvariant | Options),
			Color = Color
		};
	}
	static HighlightingSpan CreateSpan(string StartPattern, string EndPattern, HighlightingColor Color, bool IncludesStart = true, bool IncludesEnd = true)
	{
		return new HighlightingSpan
		{
			StartExpression = new Regex(StartPattern, RegexOptions.CultureInvariant),
			EndExpression = new Regex(EndPattern, RegexOptions.CultureInvariant),
			SpanColor = Color,
			SpanColorIncludesStart = IncludesStart,
			SpanColorIncludesEnd = IncludesEnd
		};
	}
	static IHighlightingDefinition CreateSqlDefinition(bool? IsDark)
	{
		HighlightingColor KeywordColor = CreateSqlColor("Keyword", IsDark, "#0050A4", "#8AB4F8", FontWeight.Bold);
		HighlightingColor StringColor = CreateSqlColor("String", IsDark, "#A31515", "#CE9178");
		HighlightingColor CommentColor = CreateSqlColor("Comment", IsDark, "#008000", "#7FB069", FontStyle: FontStyle.Italic);
		HighlightingColor NumberColor = CreateSqlColor("Number", IsDark, "#098658", "#B5CEA8");
		HighlightingColor ParameterColor = CreateSqlColor("Parameter", IsDark, "#795E26", "#DCDCAA");
		HighlightingColor OperatorColor = CreateSqlColor("Operator", IsDark, "#5F6368", "#C8C8C8");
		HighlightingColor FilterExpressionColor = CreateSqlColor("FilterExpression", IsDark, "#7A5C00", "#D7BA7D", FontWeight.SemiBold);

		HighlightingRuleSet RuleSet = new() { Name = "Main" };
		RuleSet.Spans.Add(CreateSpan(@"/\*", @"\*/", CommentColor));
		RuleSet.Rules.Add(CreateRule(@"--.*$", CommentColor, RegexOptions.Multiline));
		RuleSet.Rules.Add(CreateRule(@"\[\[[^\]]+\]\]", FilterExpressionColor));
		RuleSet.Rules.Add(CreateRule(@"N?'([^']|'')*'", StringColor, RegexOptions.IgnoreCase));
		RuleSet.Rules.Add(CreateRule(SqlKeywordPattern, KeywordColor, RegexOptions.IgnoreCase));
		RuleSet.Rules.Add(CreateRule(@"\b\d+(\.\d+)?\b", NumberColor));
		RuleSet.Rules.Add(CreateRule(@"[:@][A-Za-z_][A-Za-z0-9_]*", ParameterColor));
		RuleSet.Rules.Add(CreateRule(@"[<>!=+\-*/%&|^~.,;()[\]]", OperatorColor));

		string Name = IsDark == true ? "Tripous SQL Dark" : IsDark == false ? "Tripous SQL Light" : "Tripous SQL";
		return new SimpleHighlightingDefinition(Name, RuleSet, [KeywordColor, StringColor, CommentColor, NumberColor, ParameterColor, OperatorColor, FilterExpressionColor]);
	}
	static IHighlightingDefinition CreateMarkdownDefinition(bool? IsDark)
	{
		HighlightingColor HeadingColor = CreateThemeColor("Heading", IsDark, "#7A3E9D", "#C586C0", FontWeight.SemiBold);
		HighlightingColor LinkColor = CreateThemeColor("Link", IsDark, "#005A9E", "#7EA7D8", FontWeight.SemiBold);
		HighlightingColor CodeColor = CreateThemeColor("Code", IsDark, "#A31515", "#CE9178");
		HighlightingColor QuoteColor = CreateThemeColor("Quote", IsDark, "#5A6773", "#9DA7B1", FontStyle: FontStyle.Italic);
		HighlightingColor EmphasisColor = CreateThemeColor("Emphasis", IsDark, "#333333", "#D2D7DE", FontWeight.SemiBold);
		HighlightingColor MarkerColor = CreateThemeColor("Marker", IsDark, "#6B7280", "#8A929C");

		HighlightingRuleSet RuleSet = new() { Name = "Main" };
		RuleSet.Rules.Add(CreateRule(@"^#{1,6}\s+.*$", HeadingColor, RegexOptions.Multiline));
		RuleSet.Rules.Add(CreateRule(@"!?\[[^\]\r\n]+\]\([^\)\r\n]+\)", LinkColor));
		RuleSet.Rules.Add(CreateRule(@"`[^`\r\n]+`", CodeColor));
		RuleSet.Rules.Add(CreateRule(@"^>\s?.*$", QuoteColor, RegexOptions.Multiline));
		RuleSet.Rules.Add(CreateRule(@"(\*\*|__)[^\r\n]+?(\*\*|__)", EmphasisColor));
		RuleSet.Rules.Add(CreateRule(@"(^|\s)([*_])[^*_]+?\2", EmphasisColor));
		RuleSet.Rules.Add(CreateRule(@"^\s*([-*+]|\d+\.)\s+", MarkerColor, RegexOptions.Multiline));
		RuleSet.Rules.Add(CreateRule(@"^\s*[-*_]{3,}\s*$", MarkerColor, RegexOptions.Multiline));

		string Name = IsDark == true ? "Tripous Markdown Dark" : IsDark == false ? "Tripous Markdown Light" : "Tripous Markdown";
		return new SimpleHighlightingDefinition(Name, RuleSet, [HeadingColor, LinkColor, CodeColor, QuoteColor, EmphasisColor, MarkerColor]);
	}
	static ThemeVariant GetActualThemeVariant(TextEditor Editor = null)
	{
		ThemeVariant Result = Editor?.ActualThemeVariant ?? Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;
		if (Result == ThemeVariant.Default)
			Result = Application.Current?.RequestedThemeVariant ?? ThemeVariant.Default;
		return Result;
	}
	static bool IsDarkTheme(ThemeVariant Theme)
	{
		return Theme == ThemeVariant.Dark
			|| string.Equals(Theme?.Key?.ToString(), ThemeVariant.Dark.Key.ToString(), StringComparison.OrdinalIgnoreCase);
	}

	// ● static public
	/// <summary>
	/// Finds a highlighting definition by mode.
	/// </summary>
	/// <param name="Mode">The highlight mode.</param>
	/// <returns>The highlighting definition, if any; otherwise, null.</returns>
	static public IHighlightingDefinition Find(HighlightMode Mode)
	{
		if (Mode == HighlightMode.SQL)
			return SqlThemeDefinition;
		if (Mode == HighlightMode.Markdown)
			return MarkdownThemeDefinition;
		return Find(Mode, GetActualThemeVariant());
	}
	/// <summary>
	/// Finds a highlighting definition by mode and theme variant.
	/// </summary>
	/// <param name="Mode">The highlight mode.</param>
	/// <param name="Theme">The theme variant.</param>
	/// <returns>The highlighting definition, if any; otherwise, null.</returns>
	static public IHighlightingDefinition Find(HighlightMode Mode, ThemeVariant Theme)
	{
		IHighlightingDefinition Result = null;

		if (Mode == HighlightMode.SQL)
			return IsDarkTheme(Theme) ? SqlDarkDefinition : SqlLightDefinition;
		if (Mode == HighlightMode.Markdown)
			return IsDarkTheme(Theme) ? MarkdownDarkDefinition : MarkdownLightDefinition;

		if (Items.ContainsKey(Mode))
		{
			HighlighterItem Item = Items[Mode];
			Result = HighlightingManager.Instance.GetDefinition(Item.Key);
		}

		return Result;
	}
	/// <summary>
	/// Applies a highlighting mode to a text editor.
	/// </summary>
	/// <param name="Editor">The text editor.</param>
	/// <param name="Mode">The highlight mode.</param>
	static public void Apply(TextEditor Editor, HighlightMode Mode)
	{
		if (Editor != null)
			Editor.SyntaxHighlighting = Mode == HighlightMode.SQL ? SqlThemeDefinition : Mode == HighlightMode.Markdown ? MarkdownThemeDefinition : Find(Mode, GetActualThemeVariant(Editor));
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
						Result = Find(Entry.Key);
						break;
					}
				}
			}
		}
			
		return Result;	
	}
}
