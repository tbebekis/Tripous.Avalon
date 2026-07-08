// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Normalizes and parses short date text for group grid date editors.
/// </summary>
static internal class GroupGridDateTextNormalizer
{
    // ● private methods
    static char GetDateSeparator(string Pattern)
    {
        foreach (char Character in Pattern)
            if (!char.IsLetter(Character) && Character != '\'' && Character != '"')
                return Character;

        return '/';
    }
    static string[] GetPatternParts(string Pattern, char Separator)
    {
        return Pattern
            .Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Item => Item.Trim('\'', '"'))
            .ToArray();
    }
    static string[] GetInputParts(string Text, char Separator)
    {
        Text = (Text ?? string.Empty).Trim().Trim(Separator);
        return Text.Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
    static bool IsIsoLikeInput(string Text)
    {
        string Value = (Text ?? string.Empty).Trim();
        string FirstPart = Regex.Split(Value, @"[^\d]+").FirstOrDefault(Item => !string.IsNullOrWhiteSpace(Item));
        return FirstPart != null && FirstPart.Length == 4;
    }
    static string[] GetNumericParts(string Text)
    {
        return Regex.Split(Text ?? string.Empty, @"[^\d]+")
            .Where(Item => !string.IsNullOrWhiteSpace(Item))
            .ToArray();
    }
    static bool TryGetPartValue(string[] InputParts, string[] PatternParts, char Token, out string Value)
    {
        Value = string.Empty;

        for (int Index = 0; Index < PatternParts.Length && Index < InputParts.Length; Index++)
        {
            string Part = PatternParts[Index];
            if (Part.Length > 0 && char.ToUpperInvariant(Part[0]) == Token)
            {
                Value = InputParts[Index];
                return !string.IsNullOrWhiteSpace(Value);
            }
        }

        return false;
    }
    static bool IsValidInput(string Text, char Separator)
    {
        string ValidChars = "0123456789 " + Separator;
        foreach (char Character in Text)
            if (ValidChars.IndexOf(Character) < 0)
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
        }

        return string.Empty;
    }
    static bool TryNormalizeWithPattern(string Text, string Pattern, out DateTime Date)
    {
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
            return true;
        }
        catch
        {
            return false;
        }
    }
    static bool TryNormalizePartialDate(string Text, out DateTime Date)
    {
        Date = default;
        string[] Parts = GetNumericParts(Text);
        if (Parts.Length == 0 || Parts.Length > 3)
            return false;

        DateTime Today = DateTime.Today;
        if (!int.TryParse(Parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int Day))
            return false;

        int Month = Today.Month;
        if (Parts.Length >= 2 && !int.TryParse(Parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out Month))
            return false;

        string YearText = Parts.Length >= 3 ? Parts[2] : Today.Year.ToString(CultureInfo.InvariantCulture);
        YearText = CompleteYear(YearText, Today);
        if (!int.TryParse(YearText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int Year))
            return false;

        try
        {
            Date = new DateTime(Year, Month, Day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ● public methods
    /// <summary>
    /// Tries to parse date text using ISO completion or the specified culture short date pattern.
    /// </summary>
    /// <param name="Text">The input text.</param>
    /// <param name="CultureInfo">The culture.</param>
    /// <param name="Date">The parsed date.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    static public bool TryParse(string Text, CultureInfo CultureInfo, out DateTime Date)
    {
        CultureInfo ??= CultureInfo.CurrentCulture;
        string Pattern = CultureInfo.DateTimeFormat.ShortDatePattern;
        bool IsIsoLike = IsIsoLikeInput(Text);
        if (IsIsoLike)
        {
            if (TryNormalizeWithPattern(Text, "yyyy-MM-dd", out Date))
                return true;
            if (!string.Equals(Pattern, "yyyy-MM-dd", StringComparison.OrdinalIgnoreCase) && TryNormalizeWithPattern(Text, Pattern, out Date))
                return true;
        }
        else
        {
            if (TryNormalizePartialDate(Text, out Date))
                return true;
            if (TryNormalizeWithPattern(Text, Pattern, out Date))
                return true;
            if (!string.Equals(Pattern, "yyyy-MM-dd", StringComparison.OrdinalIgnoreCase) && TryNormalizeWithPattern(Text, "yyyy-MM-dd", out Date))
                return true;
        }

        Date = default;
        return false;
    }
}
