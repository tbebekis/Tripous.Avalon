/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Normalizes and parses date text using ISO format and culture short-date format.
/// <para>ISO-like input is detected when the text contains a dash or starts with a four-digit part.</para>
/// <para>Examples: <c>2030</c> becomes <c>2030-01-01</c>, <c>2030-03</c> becomes <c>2030-03-01</c>, and <c>2030-03-10</c> remains <c>2030-03-10</c>.</para>
/// <para>Culture input follows <see cref="DateTimeFormatInfo.ShortDatePattern"/>. For a DMY culture, <c>10/3</c> becomes <c>10/03/current-year</c> and <c>10/3/30</c> becomes <c>10/03/2030</c>.</para>
/// </summary>
static public class DateTextNormalizer
{
    // ● private
    static char GetDateSeparator(string Pattern)
    {
        foreach (char C in Pattern)
            if (!char.IsLetter(C) && C != '\'' && C != '"')
                return C;

        return '/';
    }
    static string[] GetPatternParts(string Pattern, char Separator)
    {
        return Pattern
            .Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.Trim('\'', '"'))
            .ToArray();
    }
    static string[] GetInputParts(string Text, char Separator)
    {
        Text = (Text ?? string.Empty).Trim().Trim(Separator);
        return Text.Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
    static bool IsIsoLikeInput(string Text)
    {
        string S = (Text ?? string.Empty).Trim();
        if (S.Contains('-'))
            return true;

        string FirstPart = Regex.Split(S, @"[^\d]+").FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        return FirstPart != null && FirstPart.Length == 4;
    }
    static bool TryGetPartValue(string[] InputParts, string[] PatternParts, char Token, out string Value)
    {
        Value = string.Empty;

        for (int i = 0; i < PatternParts.Length && i < InputParts.Length; i++)
        {
            string Part = PatternParts[i];
            if (Part.Length > 0 && char.ToUpperInvariant(Part[0]) == Token)
            {
                Value = InputParts[i];
                return !string.IsNullOrWhiteSpace(Value);
            }
        }

        return false;
    }
    static bool IsValidInput(string Text, char Separator)
    {
        string ValidChars = "0123456789 " + Separator;

        foreach (char C in Text)
            if (ValidChars.IndexOf(C) < 0)
                return false;

        return true;
    }
    static string CompleteYear(string Text, DateTime Today)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return Today.Year.ToString(CultureInfo.InvariantCulture);

        int Year = int.Parse(Text, CultureInfo.InvariantCulture);
        if (Text.Length >= 4)
            return Year.ToString("0000", CultureInfo.InvariantCulture);

        string CurrentYear = Today.Year.ToString(CultureInfo.InvariantCulture);
        string Prefix = CurrentYear.Substring(0, CurrentYear.Length - Text.Length);
        return Prefix + Text;
    }
    static string GetDefaultPartValue(char Token, bool HasAnyExplicitPart, DateTime Today)
    {
        switch (char.ToUpperInvariant(Token))
        {
            case 'D':
                return HasAnyExplicitPart ? "1" : Today.Day.ToString(CultureInfo.InvariantCulture);
            case 'M':
                return HasAnyExplicitPart ? "1" : Today.Month.ToString(CultureInfo.InvariantCulture);
            case 'Y':
                return Today.Year.ToString(CultureInfo.InvariantCulture);
            default:
                return string.Empty;
        }
    }
    static bool TryNormalizeWithPattern(string Text, string Pattern, out string NormalizedText, out DateTime Date)
    {
        NormalizedText = string.Empty;
        Date = default;

        if (string.IsNullOrWhiteSpace(Text) || string.IsNullOrWhiteSpace(Pattern))
            return false;

        Text = Text.Trim();
        char Separator = GetDateSeparator(Pattern);
        Text = Text.Trim(Separator);
        if (!IsValidInput(Text, Separator))
            return false;

        string[] PatternParts = GetPatternParts(Pattern, Separator);
        string[] InputParts = GetInputParts(Text, Separator);

        if (InputParts.Length == 0)
            return false;

        DateTime Today = DateTime.Today;
        bool HasExplicitDay = TryGetPartValue(InputParts, PatternParts, 'D', out string DayPart);
        bool HasExplicitMonth = TryGetPartValue(InputParts, PatternParts, 'M', out string MonthPart);
        bool HasExplicitYear = TryGetPartValue(InputParts, PatternParts, 'Y', out string YearPart);
        bool HasAnyExplicitPart = HasExplicitDay || HasExplicitMonth || HasExplicitYear;

        string DayText = HasExplicitDay ? DayPart : GetDefaultPartValue('D', HasAnyExplicitPart, Today);
        string MonthText = HasExplicitMonth ? MonthPart : GetDefaultPartValue('M', HasAnyExplicitPart, Today);
        string YearText = HasExplicitYear ? YearPart : GetDefaultPartValue('Y', HasAnyExplicitPart, Today);

        if (!int.TryParse(DayText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Day))
            return false;
        if (!int.TryParse(MonthText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Month))
            return false;

        YearText = CompleteYear(YearText, Today);
        if (!int.TryParse(YearText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Year))
            return false;

        try
        {
            Date = new DateTime(Year, Month, Day);
        }
        catch
        {
            return false;
        }

        List<string> ResultParts = [];
        foreach (string PatternPart in PatternParts)
        {
            if (PatternPart.StartsWith("d", StringComparison.OrdinalIgnoreCase))
                ResultParts.Add(Day.ToString("00", CultureInfo.InvariantCulture));
            else if (PatternPart.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                ResultParts.Add(Month.ToString("00", CultureInfo.InvariantCulture));
            else if (PatternPart.StartsWith("y", StringComparison.OrdinalIgnoreCase))
                ResultParts.Add(Year.ToString("0000", CultureInfo.InvariantCulture));
        }

        if (ResultParts.Count != 3)
            return false;

        NormalizedText = string.Join(Separator, ResultParts);
        return true;
    }

    // ● public
    /// <summary>
    /// Tries to normalize date text using the current culture.
    /// <para>Examples: <c>2030</c> becomes <c>2030-01-01</c>; in a DMY culture, <c>10/3</c> becomes <c>10/03/current-year</c>.</para>
    /// </summary>
    static public bool TryNormalize(string Text, out string NormalizedText)
    {
        bool Result = TryNormalize(Text, CultureInfo.CurrentCulture, out NormalizedText);
        return Result;
    }
    /// <summary>
    /// Tries to normalize date text using the current culture and returns the parsed date.
    /// <para>Examples: <c>2030-</c> becomes <c>2030-01-01</c>; <c>2030-03-</c> becomes <c>2030-03-01</c>.</para>
    /// </summary>
    static public bool TryNormalize(string Text, out string NormalizedText, out DateTime Date)
    {
        bool Result = TryNormalize(Text, CultureInfo.CurrentCulture, out NormalizedText, out Date);
        return Result;
    }
    /// <summary>
    /// Tries to parse date text using the current culture.
    /// <para>Examples: <c>2030</c> parses as January 1st, 2030; culture input such as <c>10/3/30</c> parses according to the current culture pattern.</para>
    /// </summary>
    static public bool TryParse(string Text, out DateTime Date)
    {
        bool Result = TryParse(Text, CultureInfo.CurrentCulture, out Date);
        return Result;
    }
    
    /// <summary>
    /// Tries to normalize date text.
    /// <para>ISO-like input is completed as year-month-day. Culture input is completed according to <paramref name="CultureInfo"/>.</para>
    /// </summary>
    static public bool TryNormalize(string Text, CultureInfo CultureInfo, out string NormalizedText)
    {
        bool Result = TryNormalize(Text, CultureInfo, out NormalizedText, out _);
        return Result;
    }
    /// <summary>
    /// Tries to normalize date text and returns the parsed date.
    /// <para>Missing ISO month or day becomes <c>01</c>. Missing culture year becomes the current year.</para>
    /// </summary>
    static public bool TryNormalize(string Text, CultureInfo CultureInfo, out string NormalizedText, out DateTime Date)
    {
        CultureInfo ??= CultureInfo.CurrentCulture;

        string Pattern = CultureInfo.DateTimeFormat.ShortDatePattern;
        bool IsIsoLike = IsIsoLikeInput(Text);

        if (IsIsoLike)
        {
            if (TryNormalizeWithPattern(Text, "yyyy-MM-dd", out NormalizedText, out Date))
                return true;
            if (!Pattern.IsSameText("yyyy-MM-dd") && TryNormalizeWithPattern(Text, Pattern, out NormalizedText, out Date))
                return true;
        }
        else
        {
            if (TryNormalizeWithPattern(Text, Pattern, out NormalizedText, out Date))
                return true;
            if (!Pattern.IsSameText("yyyy-MM-dd") && TryNormalizeWithPattern(Text, "yyyy-MM-dd", out NormalizedText, out Date))
                return true;
        }

        NormalizedText = string.Empty;
        Date = default;
        return false;
    }
    /// <summary>
    /// Tries to parse date text.
    /// <para>Uses ISO completion first for ISO-like input, otherwise uses the supplied culture first.</para>
    /// </summary>
    static public bool TryParse(string Text, CultureInfo CultureInfo, out DateTime Date)
    {
        bool Result = TryNormalize(Text, CultureInfo, out _, out Date);
        return Result;
    }
}
