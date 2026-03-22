using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Tripous.Data;
 
public class SqlFilterDef
{
    private string fRawTag;
    private string fStatement;
    private readonly StringBuilder sbErrors = new();
    

    // ● private
    private void Parse()
    {
 
        try
        {
            sbErrors.Clear();

            SetDefaults();

            if (string.IsNullOrWhiteSpace(RawTag))
                return;

            string[] parts = Tokenize(RawTag);
            if (parts.Length == 0)
                return;

            // Shorthand: [[Label]]
            if (parts.Length == 1)
            {
                Type = SqlFilterType.String;
                Label = parts[0];

                if (string.IsNullOrWhiteSpace(Label))
                    AddError("Missing label.");

                return;
            }

            if (!TryParseFilterType(parts[0], out SqlFilterType filterType))
            {
                AddError($"Unknown filter type '{parts[0]}'.");
                return;
            }

            Type = filterType;

            switch (Type)
            {
                case SqlFilterType.Date:
                    ParseDate(parts);
                    break;

                case SqlFilterType.Lookup:
                    ParseLookup(parts);
                    break;

                case SqlFilterType.Enum:
                    ParseEnum(parts);
                    break;

                case SqlFilterType.String:
                case SqlFilterType.Integer:
                case SqlFilterType.Decimal:
                    ParseSimple(parts);
                    break;

                default:
                    AddError($"Unsupported filter type '{Type}'.");
                    break;
            }
        }
        catch (Exception e)
        {
            sbErrors.AppendLine(e.ToString());
        }
 

    }

    private void SetDefaults()
    {
        Type = SqlFilterType.String;
        IsNumeric = false;
        IsMultiple = false;
        DateRange = DateRange.Custom;
        Text = string.Empty;
        Label = string.Empty;
        Value = string.Empty;
    }

    private string[] Tokenize(string rawTag)
    {
        string s = rawTag.Trim();

        if (!s.StartsWith("[[") || !s.EndsWith("]]"))
        {
            AddError("Invalid tag format. Expected [[...]].");
            return Array.Empty<string>();
        }

        s = s.Substring(2, s.Length - 4).Trim();

        if (string.IsNullOrWhiteSpace(s))
        {
            AddError("Empty tag.");
            return Array.Empty<string>();
        }

        return s
            .Split(':', StringSplitOptions.None)
            .Select(x => x.Trim())
            .ToArray();
    }

    private void ParseSimple(string[] parts)
    {
        // Supported:
        // [[Label]]            // default, is string
        // [[string:Label]]
        // [[int:Label]]
        // [[dec:Label]]

        if (parts.Length != 2)
        {
            AddError($"{Type} filter requires exactly one label.");
            return;
        }

        Label = parts[1];

        if (string.IsNullOrWhiteSpace(Label))
        {
            AddError("Missing label.");
            return;
        }

        IsNumeric = Type == SqlFilterType.Integer || Type == SqlFilterType.Decimal;
    }

    private void ParseDate(string[] parts)
    {
        // Supported:
        // Manual entry:  Date >= [[date:Custom:From Date]]
        // Auto-computed: Date = [[date:Today]]

        DateRange range;
        
        if (!TryParseDateRange(parts[1], out range))
        {
            AddError($"Date with invalid range: {parts[1]}");
            return;
        }
        
        this.DateRange = range;
        
        // [[date:Range]]
        if (parts.Length == 2)
        {
            if (DateRange == DateRange.Custom)
                AddError("Custom date requires 3 parts, where the third is a Label.");
 
            return;
        }

        // [[date:Custom:Label]]
        if (parts.Length >= 3)
        {
            if (DateRange != DateRange.Custom)
            {
                AddError("A Date with 3 parts, should be defined as Custom.");
                return;
            }
 
            Label = parts[2];

            if (string.IsNullOrWhiteSpace(Label))
            {
                AddError("Missing label in Custom Date.");
                return;
            }

            if (parts.Length > 3)
            {
                AddError("A Custom Date cannot have more than 3 parts.");
            }

            return;
        }

         
    }

    private void ParseLookup(string[] parts)
    {
        // Expected:
        // [[lookup:datatype:Label]]
        // [[lookup:datatype:Label:Text]]
        // [[lookup:datatype:multi:Label]]
        // [[lookup:datatype:multi:Label:Text]]

        if (parts.Length < 3)
        {
            AddError("Lookup requires datatype and label.");
            return;
        }

        // datatype
        if (!TryParseDataType(parts[1], out bool isNumeric))
        {
            AddError($"Invalid datatype '{parts[1]}'.");
            return;
        }

        IsNumeric = isNumeric;

        int index = 2;

        // multi (optional, but ONLY here)
        if (index < parts.Length && EqualsIgnoreCase(parts[index], "multi"))
        {
            IsMultiple = true;
            index++;
        }

        if (index >= parts.Length)
        {
            AddError("Missing label.");
            return;
        }

        Label = parts[index];
        index++;

        if (string.IsNullOrWhiteSpace(Label))
            AddError("Missing label.");

        if (index < parts.Length)
        {
            Text = string.Join(":", parts.Skip(index));
        }
        
        if (string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(Statement))
            AddError($"{Type} filter requires a SELECT statement either inline or not.");
    }

    private void ParseEnum(string[] parts)
    {
        // Expected:
        // [[enum:datatype:Label]]
        // [[enum:datatype:Label:Text]]
        // [[enum:datatype:multi:Label]]
        // [[enum:datatype:multi:Label:Text]]

        if (parts.Length < 3)
        {
            AddError("Enum requires datatype and label.");
            return;
        }

        // datatype (REQUIRED)
        if (!TryParseDataType(parts[1], out bool isNumeric))
        {
            AddError($"Invalid datatype '{parts[1]}'.");
            return;
        }

        IsNumeric = isNumeric;

        int index = 2;

        // multi (ONLY here)
        if (index < parts.Length && EqualsIgnoreCase(parts[index], "multi"))
        {
            IsMultiple = true;
            index++;
        }

        if (index >= parts.Length)
        {
            AddError("Missing label.");
            return;
        }

        Label = parts[index];
        index++;

        if (string.IsNullOrWhiteSpace(Label))
            AddError("Missing label.");

        if (index < parts.Length)
        {
            Text = string.Join(":", parts.Skip(index));
        }
        
        if (string.IsNullOrWhiteSpace(Text) && string.IsNullOrWhiteSpace(Statement))
            AddError($"{Type} filter requires a list of constant values either inline or not.");
            
    }

    private bool TryParseFilterType(string token, out SqlFilterType filterType)
    {
        filterType = SqlFilterType.String;

        if (EqualsIgnoreCase(token, "string"))
        {
            filterType = SqlFilterType.String;
            return true;
        }

        if (EqualsIgnoreCase(token, "date"))
        {
            filterType = SqlFilterType.Date;
            return true;
        }

        if (EqualsIgnoreCase(token, "int") || EqualsIgnoreCase(token, "integer"))
        {
            filterType = SqlFilterType.Integer;
            return true;
        }

        if (EqualsIgnoreCase(token, "dec") || EqualsIgnoreCase(token, "decimal"))
        {
            filterType = SqlFilterType.Decimal;
            return true;
        }

        if (EqualsIgnoreCase(token, "lookup"))
        {
            filterType = SqlFilterType.Lookup;
            return true;
        }

        if (EqualsIgnoreCase(token, "enum"))
        {
            filterType = SqlFilterType.Enum;
            return true;
        }

        return false;
    }

    private bool TryParseDataType(string token, out bool isNumeric)
    {
        isNumeric = false;

        if (EqualsIgnoreCase(token, "string"))
        {
            isNumeric = false;
            return true;
        }

        if (EqualsIgnoreCase(token, "int") || EqualsIgnoreCase(token, "integer"))
        {
            isNumeric = true;
            return true;
        }

        if (EqualsIgnoreCase(token, "dec") || EqualsIgnoreCase(token, "decimal"))
        {
            isNumeric = true;
            return true;
        }

        return false;
    }

    private bool TryParseDateRange(string token, out DateRange range)
    {
        return Enum.TryParse(token, true, out range);
    }

    private bool IsMultiToken(string token)
    {
        return EqualsIgnoreCase(token, "multi");
    }

    private bool EqualsIgnoreCase(string a, string b)
    {
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }

    private void AddError(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
            sbErrors.AppendLine($"{message} - {RawTag}");
    }

    // ● public
    public override string ToString()
    {
        return $"{Label} - {RawTag}";
    }

    // ● properties
    /// <summary>
    /// The raw tag, e.g. [[date:A_Date]]
    /// </summary>
    public string RawTag
    {
        get => fRawTag;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                SetDefaults();
            }
            else if (fRawTag != value)
            {
                fRawTag = value;
                Parse();
            }
        }
    }

    /// <summary>
    /// When it is a LookUp or Enum filter, this is the SELECT statement or the list of constants the user enters inline in the SELECT statement.
    /// <para>When it is a LookUp filter this is a SELECT statement, e.g. [[lookup:int:Country:select Id from Country]]</para>
    /// <para>When it is an Enum filter, this is a semicolon delimited list of constants, e.g.  [[enum:string:Status:Pending;Completed;Cancelled]]</para>
    /// </summary>
    [JsonIgnore]
    public string Text { get; set; }

    /// <summary>
    /// When it is a LookUp or Enum filter, this is the SELECT statement or the list of constants the user enters in the design-time UI.
    /// <para>When it is a LookUp filter, this is a SELECT statement.</para>
    /// <para>When it is an Enum filter, this is a semicolon delimited list of constants.</para>
    /// </summary>
    public string Statement
    {
        get => fStatement;
        set
        {
            fStatement = value;
            Parse(); 
        }
    }
    [JsonIgnore]
    public string LookUpSelectSqlText
    {
        get
        {
            string Result = "";

            if (Type == SqlFilterType.Lookup)
            {
                if (!string.IsNullOrWhiteSpace(Text) &&
                    Text.Trim().StartsWith("select ", StringComparison.InvariantCultureIgnoreCase))
                    Result = Text;
                else
                    Result = Statement;
            }

            return Result;
        }
 
    }
    
    /// <summary>
    /// The label the user gives to the filter, e.g. A_Date as in [[date:A_Date]]
    /// <para>A label may contain spaces and characters other than english characters.</para>
    /// </summary>
    [JsonIgnore]
    public string Label { get; private set; }

    /// <summary>
    /// The type of the filter, e.g. String, Date, Integer, Decimal, LookUp, Enum
    /// </summary>
    [JsonIgnore]
    public SqlFilterType Type { get; private set; }

    /// <summary>
    /// The date range. Used when it is a Date filter only.
    /// <para>[[date:MyDate]] is a Custom date. The user must enter the date in the runtime UI.</para>
    /// <para>[[date:LastMonth:MyDate]] is a computed date. The system computes the date in the runtime UI.</para>
    /// </summary>
    [JsonIgnore]
    public DateRange DateRange { get; private set; }

    /// <summary>
    /// True when 'multi' is included in the <see cref="RawTag"/>
    /// </summary>
    [JsonIgnore]
    public bool IsMultiple { get; private set; }

    /// <summary>
    /// True when in lookup or enum types a numeric modifier is used.
    /// <para>e.g.</para>
    /// <para> [[lookup:string|int:Label]] </para>
    /// <para> [[enum:string|int|dec:Label]] </para>
    /// </summary>
    [JsonIgnore]
    public bool IsNumeric { get; private set; }

    /// <summary>
    /// The value the user enters in the runtime UI when the SELECT is executed.
    /// <para>When this is a Date filter with a value other than <see cref="DateRange.Custom"/>
    /// then this value is automatically completed in the runtime UI that executes the SELECT.</para>
    /// </summary>
    [JsonIgnore]
    public string Value { get; set; }

    [JsonIgnore]
    public bool HasErrors => sbErrors.Length > 0;

    [JsonIgnore]
    public string Errors => sbErrors.ToString();
}