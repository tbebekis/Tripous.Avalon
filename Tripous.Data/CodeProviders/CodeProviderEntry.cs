/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a code provider entry, usually loaded from the NumberSeries table.
/// </summary>
public class CodeProviderEntry
{
        
    // ● private
    /// <summary>
    /// Parses the pattern
    /// </summary>
    void Parse()
    {
        NumericDigits = Pattern.Count(c => c == 'X');
        MaxNumber = NumericDigits <= 0? 0: (int)Math.Pow(10, NumericDigits) - 1;
    }
    /// <summary>
    /// Validates the pattern syntax and reset period compatibility.
    /// </summary>
    void Validate()
    {
        ValidatePattern();
        ValidateResetPeriod();
    }

    /// <summary>
    /// Validates reset period compatibility with the pattern.
    /// </summary>
    void ValidateResetPeriod()
    {
        bool HasYear = Pattern.Contains("YYYY") || Pattern.Contains("YY");
        bool HasMonth = Pattern.Contains("MM");
        bool HasDay = Pattern.Contains("DD");
        bool HasQuarter = Pattern.Contains("Q");
        bool HasSemester = Pattern.Contains("S");
        bool HasWeek = Pattern.Contains("WW");

        switch (ResetPeriod)
        {
            case ResetPeriod.None:
                break;

            case ResetPeriod.Year:
                if (!HasYear)
                    throw new TripousDataException("Year reset requires YYYY or YY token.");
                break;

            case ResetPeriod.Semester:
                if (!HasYear || !HasSemester)
                    throw new TripousDataException("Semester reset requires YYYY/YY and S tokens.");
                break;

            case ResetPeriod.Quarter:
                if (!HasYear || !HasQuarter)
                    throw new TripousDataException("Quarter reset requires YYYY/YY and Q tokens.");
                break;

            case ResetPeriod.Month:
                if (!HasYear || !HasMonth)
                    throw new TripousDataException("Month reset requires YYYY/YY and MM tokens.");
                break;

            case ResetPeriod.Week:
                if (!HasYear || !HasWeek)
                    throw new TripousDataException("Week reset requires YYYY/YY and WW tokens.");
                break;

            case ResetPeriod.Day:
                if (!HasYear || !HasMonth || !HasDay)
                    throw new TripousDataException("Day reset requires YYYY/YY, MM and DD tokens.");
                break;
        }
    }
    void ValidatePattern()
    {
        if (string.IsNullOrWhiteSpace(Pattern))
            throw new TripousDataException("Code provider pattern is empty.");
        if (NumericDigits <= 0)
            throw new TripousDataException("Code provider pattern must contain at least one X token.");
    }
    void ValidateNumber(int Number)
    {
        if (Number <= 0)
            throw new TripousDataException("Code provider number must be greater than zero.");
        if (Number > MaxNumber)
            throw new TripousDataException("Code provider number exceeds pattern capacity.");
    }
    
    int GetQuarter(DateTime Date) => ((Date.Month - 1) / 3) + 1;
    int GetSemester(DateTime Date) => Date.Month <= 6 ? 1 : 2;
    int GetIsoWeek(DateTime Date) => System.Globalization.ISOWeek.GetWeekOfYear(Date);
    
    string ReplaceFirst(string Text, char OldChar, char NewChar)
    {
        int Index = Text.IndexOf(OldChar);
        if (Index < 0)
            return Text;

        return Text.Remove(Index, 1).Insert(Index, NewChar.ToString());
    }
    
    // ● construction
    public CodeProviderEntry()
    {
    }
    public CodeProviderEntry(DataRow Row)
    {
        LoadForm(Row);
    }
    
    // ● public
    public virtual void LoadForm(DataRow Row)
    {
        Code = Row.AsString("Code");
        Name = Row.AsString("Name");
        Pattern = Row.AsString("Pattern");
        ResetPeriod = (ResetPeriod)Row.AsInteger("ResetPeriodId");
        NextNumber = Row.AsInteger("NextNumber");
        LastResetValue = Row.AsString("LastResetValue");
        IsActive = Row.AsInteger("IsActive") == 1;
        
        Parse();
        Validate();
    }
    /// <summary>
    /// Returns the current reset value according to the configured reset period.
    /// Examples:
    /// Year      -> 2026
    /// Semester  -> 2026-S1
    /// Quarter   -> 2026-Q2
    /// Month     -> 2026-05
    /// Week      -> 2026-W20
    /// Day       -> 2026-05-18
    /// </summary>
    public string GetResetValue(DateTime Date)
    {
        return ResetPeriod switch
        {
            ResetPeriod.None => string.Empty,
            ResetPeriod.Year => Date.ToString("yyyy"),
            ResetPeriod.Semester => $"{Date:yyyy}-S{GetSemester(Date)}",
            ResetPeriod.Quarter => $"{Date:yyyy}-Q{GetQuarter(Date)}",
            ResetPeriod.Month => Date.ToString("yyyy-MM"),
            ResetPeriod.Week => $"{Date:yyyy}-W{GetIsoWeek(Date):00}",
            ResetPeriod.Day => Date.ToString("yyyy-MM-dd"),
            _ => string.Empty,
        };
    }
    /// <summary>
    /// Formats and returns the final generated code.
    /// Replaces date tokens and numeric X positions.
    /// Example:
    /// Pattern: SO-YYYY-XXXXXX
    /// Result : SO-2026-000123
    /// </summary>
    public string Format(DateTime Date, int Number)
    {
        ValidateNumber(Number);

        string Result = Pattern;

        Result = Result.Replace("YYYY", Date.ToString("yyyy"));
        Result = Result.Replace("YY", Date.ToString("yy"));
        Result = Result.Replace("MM", Date.ToString("MM"));
        Result = Result.Replace("DD", Date.ToString("dd"));
        
        Result = Result.Replace("WW", $"W{GetIsoWeek(Date):00}");
        Result = Result.Replace("Q", $"Q{GetQuarter(Date)}");
        Result = Result.Replace("S", $"S{GetSemester(Date)}");

        string NumberText = Number.ToString().PadLeft(NumericDigits, '0');

        foreach (char C in NumberText)
            Result = ReplaceFirst(Result, 'X', C);

        return Result;
    }

    // ● properties 
    /// <summary>
    /// The unique provider code.
    /// Usually maps to NumberSeries.Code and is used as the lookup key.
    /// Example: SALES_ORDER, CUSTOMER, SUPPLIER.
    /// </summary>
    public string Code { get; private set; }
    /// <summary>
    /// The display name of this provider.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// The code generation pattern.
    /// Example: SO-YYYY-XXXXXX
    /// </summary>
    public string Pattern { get; private set; }
    /// <summary>
    /// Defines the reset period of the generated sequence.
    /// Example: Year, Month, Week.
    /// </summary>
    public ResetPeriod ResetPeriod { get; private set; }
    /// <summary>
    /// The next numeric value to be generated.
    /// </summary>
    public int NextNumber { get; private set; }
    /// <summary>
    /// Stores the last reset marker value.
    /// Example: 2026, 2026-05, 2026-Q2.
    /// Used to determine whether sequence reset is required.
    /// </summary>
    public string LastResetValue { get; private set; }
    /// <summary>
    /// Indicates whether this provider is active.
    /// </summary>
    public bool IsActive { get; private set; }
    
    // ● properties - values derived after parsing
    /// <summary>
    /// Total number of numeric digits defined by X tokens.
    /// All X tokens participate regardless of separators.
    /// Example:
    /// XXX-XXX -> 6
    /// </summary>
    public int NumericDigits { get; private set; }
    /// <summary>
    /// Maximum numeric value supported by the pattern.
    /// Calculated from the total X token count.
    /// Example:
    /// XXXX -> 9999
    /// XXX-XXX -> 999999
    /// </summary>
    public int MaxNumber { get; private set; }
}