/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.RegBuilder;

/// <summary>
/// Parser message type.
/// </summary>
public enum ParsingErrorType
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Error
    /// </summary>
    Error = 1,
    /// <summary>
    /// Warning
    /// </summary>
    Warning = 2
}

/// <summary>
/// A parser validation or informational message.
/// </summary>
public class ParsingMessage
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ParsingMessage()
    {
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString()
    {
        string Result = @$"Type: {MessageType}
Code: {Code}
ErrorText: {Text}
";
        return Result;
    }
    
    // ● properties
    /// <summary>
    /// Message type.
    /// </summary>
    public ParsingErrorType MessageType { get; set; }
    /// <summary>
    /// Message code.
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// Message text.
    /// </summary>
    public string Text { get; set; }
}

/// <summary>
/// Result of parsing a Tripous schema registration script.
/// </summary>
public class SchemaParserResult
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public SchemaParserResult()
    {
    }

    // ● public
    /// <summary>
    /// Returns the errors of the result.
    /// </summary>
    public string GetErrors()
    {
        StringBuilder SB = new();
        foreach (var Msg in Messages)
            if (Msg.MessageType == ParsingErrorType.Error)
                SB.AppendLine(Msg.ToString());
        return SB.ToString();
    }
    /// <summary>
    /// Returns the warnings of the result.
    /// </summary>
    public string GetWarnings()
    {
        StringBuilder SB = new();
        foreach (var Msg in Messages)
            if (Msg.MessageType == ParsingErrorType.Warning)
                SB.AppendLine(Msg.ToString());
        return SB.ToString();
    }
    
    /// <summary>
    /// Generates source code for a static method that returns the code provider patterns dictionary.
    /// </summary>
    public string GenerateCodeProviderPatternsMethod()
    {
        StringBuilder SB = new();

        SB.AppendLine("static public Dictionary<string, string> GetCodeProviderPatterns()");
        SB.AppendLine("{");
        SB.AppendLine("    Dictionary<string, string> Result = [];");
        SB.AppendLine();

        foreach (var Pair in CodeProviderPatterns.OrderBy(item => item.Key))
            SB.AppendLine($"    Result[\"{Sys.EscapeText(Pair.Key)}\"] = \"{Sys.EscapeText(Pair.Value)}\";");

        SB.AppendLine();
        SB.AppendLine("    return Result;");
        SB.AppendLine("}");

        return SB.ToString();
    }

    // ● internal
    /// <summary>
    /// Parsed schema script.
    /// </summary>
    internal object Script { get; set; }
    /// <summary>
    /// Namespace name used by generated C# source.
    /// </summary>
    internal string NamespaceName { get; set; }
    
    // ● properties
    /// <summary>
    /// Ordered schema SQL text.
    /// </summary>
    public string SchemaSql { get; set; }
    /// <summary>
    /// Source code that registers create table SQL statements.
    /// </summary>
    public string CreateTablesSourceCode { get; set; }
    /// <summary>
    /// Source code that registers module definitions.
    /// </summary>
    public string ModuleDefsSourceCode { get; set; }
    /// <summary>
    /// Source code that registers form definitions.
    /// </summary>
    public string FormDefsSourceCode { get; set; }
    /// <summary>
    /// Source code that registers lookup definitions.
    /// </summary>
    public string LookupDefsSourceCode { get; set; }
    /// <summary>
    /// Source code that registers locator definitions.
    /// </summary>
    public string LocatorDefsSourceCode { get; set; }
    /// <summary>
    /// Source code that registers code provider definitions.
    /// </summary>
    public string CodeProviderDefsSourceCode { get; set; }
    /// <summary>
    /// Source code for the registry version root partial class.
    /// </summary>
    public string RegistryVersionSourceCode { get; set; }
    /// <summary>
    /// A dictionary of CodeProviderName=Pattern
    /// </summary>
    public Dictionary<string, string> CodeProviderPatterns { get; set; } = [];

    /// <summary>
    /// Parser messages.
    /// </summary>
    public List<ParsingMessage> Messages { get; set; } = new();
    /// <summary>
    /// True when parser contains errors.
    /// </summary>
    public bool HasErrors => Messages.Any(x => x.MessageType == ParsingErrorType.Error);
    /// <summary>
    /// True when parser contains warnings.
    /// </summary>
    public bool HasWarnings => Messages.Any(x => x.MessageType == ParsingErrorType.Warning);
}
