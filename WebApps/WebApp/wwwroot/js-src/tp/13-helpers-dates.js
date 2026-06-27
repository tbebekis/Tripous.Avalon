// ● date constants
/**
 * Day of week constants matching JavaScript Date.getDay().
 * @type {object}
 */
tp.Day = {
    /**
     * Sunday.
     * @type {number}
     */
    Sunday: 0,
    /**
     * Monday.
     * @type {number}
     */
    Monday: 1,
    /**
     * Tuesday.
     * @type {number}
     */
    Tuesday: 2,
    /**
     * Wednesday.
     * @type {number}
     */
    Wednesday: 3,
    /**
     * Thursday.
     * @type {number}
     */
    Thursday: 4,
    /**
     * Friday.
     * @type {number}
     */
    Friday: 5,
    /**
     * Saturday.
     * @type {number}
     */
    Saturday: 6
};
Object.freeze(tp.Day);
/**
 * Date format pattern constants.
 * @type {object}
 */
tp.DatePattern = {
    /**
     * Month, day, year.
     * @type {number}
     */
    MDY: 0,
    /**
     * Day, month, year.
     * @type {number}
     */
    DMY: 1,
    /**
     * Year, month, day.
     * @type {number}
     */
    YMD: 2
};
Object.freeze(tp.DatePattern);

// ● date checks
/**
 * Returns true when a value is a valid Date.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a valid Date.
 */
tp.IsValidDate = function (Value) {
    return Value instanceof Date && !Number.isNaN(Value.getTime());
};
/**
 * Returns true when a year is a leap year.
 * @param {number} Year The year to check.
 * @returns {boolean} Returns true when the year is a leap year.
 */
tp.IsLeapYear = function (Year) {
    Year = tp.ToInt(Year);
    return Year % 4 === 0 && Year % 100 !== 0 || Year % 400 === 0;
};

// ● date creation
/**
 * Returns the current date and time.
 * @returns {Date} Returns the current date and time.
 */
tp.Now = function () {
    return new Date();
};
/**
 * Returns the current date with time cleared.
 * @returns {Date} Returns today's date.
 */
tp.Today = function () {
    return tp.ClearTime(new Date());
};
/**
 * Returns a Date containing only the current time.
 * @returns {Date} Returns the current time with date cleared.
 */
tp.Time = function () {
    return tp.ClearDate(new Date());
};
/**
 * Clones a date.
 * @param {Date} Value The date to clone.
 * @returns {Date|null} Returns the cloned date or null.
 */
tp.DateClone = function (Value) {
    return tp.IsValidDate(Value) ? new Date(Value.getTime()) : null;
};

// ● date parts
/**
 * Returns the day of week.
 * @param {Date} Value The date.
 * @returns {number} Returns the day of week, 0..6.
 */
tp.DayOfWeek = function (Value) {
    return tp.IsValidDate(Value) ? Value.getDay() : -1;
};
/**
 * Returns the day of month.
 * @param {Date} Value The date.
 * @returns {number} Returns the day of month, 1..31.
 */
tp.DayOfMonth = function (Value) {
    return tp.IsValidDate(Value) ? Value.getDate() : 0;
};
/**
 * Returns the number of days in a month.
 * @param {number} Year The year.
 * @param {number} Month The zero-based month.
 * @returns {number} Returns the number of days in the month.
 */
tp.DaysInMonth = function (Year, Month) {
    Year = tp.ToInt(Year);
    Month = tp.ToInt(Month);
    return [31, (tp.IsLeapYear(Year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][Month] || 0;
};

// ● date mutation
/**
 * Adds years to a date.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @param {number} Years The number of years to add.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.AddYears = function (Value, Years) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setFullYear(Value.getFullYear() + tp.ToInt(Years));
    return Value;
};
/**
 * Adds months to a date.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @param {number} Months The number of months to add.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.AddMonths = function (Value, Months) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setMonth(Value.getMonth() + tp.ToInt(Months));
    return Value;
};
/**
 * Adds days to a date.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @param {number} Days The number of days to add.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.AddDays = function (Value, Days) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setDate(Value.getDate() + tp.ToInt(Days));
    return Value;
};
/**
 * Adds weeks to a date.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @param {number} Weeks The number of weeks to add.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.AddWeeks = function (Value, Weeks) {
    return tp.AddDays(Value, tp.ToInt(Weeks) * 7);
};
/**
 * Adds hours to a date.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @param {number} Hours The number of hours to add.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.AddHours = function (Value, Hours) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setTime(Value.getTime() + tp.StrToFloat(Hours, 0) * 60 * 60 * 1000);
    return Value;
};
/**
 * Adds minutes to a date.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @param {number} Minutes The number of minutes to add.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.AddMinutes = function (Value, Minutes) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setTime(Value.getTime() + tp.StrToFloat(Minutes, 0) * 60 * 1000);
    return Value;
};
/**
 * Adds seconds to a date.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @param {number} Seconds The number of seconds to add.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.AddSeconds = function (Value, Seconds) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setTime(Value.getTime() + tp.StrToFloat(Seconds, 0) * 1000);
    return Value;
};
/**
 * Clears the date part of a Date value.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.ClearDate = function (Value) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setFullYear(0);
    Value.setMonth(0);
    Value.setDate(0);
    return Value;
};
/**
 * Clears the time part of a Date value.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.ClearTime = function (Value) {
    if (!tp.IsValidDate(Value))
        return null;
    Value.setHours(0);
    Value.setMinutes(0);
    Value.setSeconds(0);
    Value.setMilliseconds(0);
    return Value;
};

// ● date comparison
/**
 * Compares two Date values.
 * @param {Date|null|undefined} A The first date.
 * @param {Date|null|undefined} B The second date.
 * @returns {number} Returns 1 if A is greater, -1 if A is less, and 0 when equal.
 */
tp.DateCompare = function (A, B) {
    A = tp.IsValidDate(A) ? A.getTime() : null;
    B = tp.IsValidDate(B) ? B.getTime() : null;
    return A > B ? 1 : (A < B ? -1 : 0);
};
/**
 * Returns true when a date is between two dates, inclusive.
 * @param {Date} Value The date to check.
 * @param {Date} A The lower date.
 * @param {Date} B The upper date.
 * @returns {boolean} Returns true when Value is between A and B.
 */
tp.DateBetween = function (Value, A, B) {
    return tp.IsValidDate(Value) && tp.IsValidDate(A) && tp.IsValidDate(B) && Value >= A && Value <= B;
};
/**
 * Returns the start of a day.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.StartOfDay = function (Value) {
    return tp.ClearTime(Value);
};
/**
 * Returns the end of a day.
 * CAUTION: The passed Date value is modified.
 * @param {Date} Value The date to modify.
 * @returns {Date|null} Returns the modified date or null.
 */
tp.EndOfDay = function (Value) {
    if (!tp.IsValidDate(Value))
        return null;
    tp.ClearTime(Value);
    tp.AddDays(Value, 1);
    tp.AddSeconds(Value, -1);
    return Value;
};

// ● date text normalization
/**
 * Creates a date and verifies that date parts did not overflow.
 * @param {number} Year The full year.
 * @param {number} Month The one-based month.
 * @param {number} Day The day of month.
 * @returns {Date|null} Returns a valid date or null.
 */
tp.CreateCheckedDate = function (Year, Month, Day) {
    var Result;
    Year = tp.ToInt(Year);
    Month = tp.ToInt(Month);
    Day = tp.ToInt(Day);
    Result = new Date(Year, Month - 1, Day);
    if (tp.IsValidDate(Result) && Result.getFullYear() === Year && Result.getMonth() === Month - 1 && Result.getDate() === Day)
        return tp.ClearTime(Result);
    return null;
};
/**
 * Completes a one, two, or four digit year using the current century prefix.
 * @param {string|null|undefined} Text The year text.
 * @param {Date} Today Today's date.
 * @returns {string} Returns a four digit year text.
 */
tp.CompleteDateYearText = function (Text, Today) {
    var Year;
    var CurrentYear;
    var Prefix;
    Text = tp.IsBlank(Text) ? String(Today.getFullYear()) : String(Text);
    Year = tp.ToInt(Text);
    if (Text.length >= 4)
        return ("0000" + Year).slice(-4);
    CurrentYear = String(Today.getFullYear());
    Prefix = CurrentYear.substring(0, CurrentYear.length - Text.length);
    return Prefix + Text;
};
/**
 * Returns true when a date text should be treated as ISO-like.
 * @param {string} Text The date text.
 * @returns {boolean} Returns true when ISO-like.
 */
tp.IsIsoLikeDateText = function (Text) {
    var Match;
    Text = tp.Trim(Text);
    Match = /[0-9]+/.exec(Text);
    return !!Match && Match[0].length === 4;
};
/**
 * Normalizes and parses date text using ISO format or the current culture date format.
 * Missing ISO month or day becomes 01. Missing culture year becomes current year. Culture input follows the current culture date pattern.
 * @param {string|Date|null|undefined} Text The source text or date.
 * @param {string|null|undefined} CultureCode Optional culture code.
 * @returns {{Result: boolean, NormalizedText: string, Date: Date|null}} Returns the parse result.
 */
tp.TryNormalizeDateText = function (Text, CultureCode) {
    var TryPattern;
    var TryCulturePattern;
    var CultureFormat = tp.GetDateFormat(CultureCode);
    var Today = tp.Today();
    var EscapeRegExp = function (Value) {
        return String(Value).replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    };
    if (tp.IsValidDate(Text)) {
        return {
            Result: true,
            NormalizedText: tp.FormatDateTime(Text, CultureFormat),
            Date: tp.ClearTime(tp.DateClone(Text))
        };
    }
    Text = tp.Trim(Text);
    TryPattern = function (SourceText, Format, SourceSeparator) {
        var Separator = SourceSeparator || (Format === tp.DateFormatISO ? "-" : tp.GetDateSeparator(CultureCode));
        var Pattern = tp.GetDatePattern(Format);
        var Parts = SourceText.trim();
        var InputParts;
        var Year = null;
        var Month = null;
        var Day = null;
        var DateValue;
        var NormalizedText;
        var GetPart;
        while (Parts.charAt(0) === Separator)
            Parts = Parts.substring(1);
        while (Parts.charAt(Parts.length - 1) === Separator)
            Parts = Parts.substring(0, Parts.length - 1);
        if (tp.IsBlank(Parts))
            return null;
        if (new RegExp("[^0-9" + EscapeRegExp(Separator) + "\\s]").test(Parts))
            return null;
        InputParts = Parts.split(Separator).map(function (Part) { return tp.Trim(Part); }).filter(function (Part) { return !tp.IsBlank(Part); });
        if (InputParts.length === 0 || InputParts.length > 3)
            return null;
        GetPart = function (Index) {
            return Index < InputParts.length ? InputParts[Index] : "";
        };
        if (Pattern === tp.DatePattern.YMD) {
            Year = GetPart(0);
            Month = GetPart(1);
            Day = GetPart(2);
            Month = tp.IsBlank(Month) ? "1" : Month;
            Day = tp.IsBlank(Day) ? "1" : Day;
        } else if (Pattern === tp.DatePattern.MDY) {
            Month = GetPart(0);
            Day = GetPart(1);
            Year = GetPart(2);
            Day = tp.IsBlank(Day) ? "1" : Day;
        } else {
            Day = GetPart(0);
            Month = GetPart(1);
            Year = GetPart(2);
            Month = tp.IsBlank(Month) ? "1" : Month;
        }
        Year = tp.CompleteDateYearText(Year, Today);
        DateValue = tp.CreateCheckedDate(Year, Month, Day);
        if (!DateValue)
            return null;
        NormalizedText = tp.FormatDateTime(DateValue, Format);
        return {
            Result: true,
            NormalizedText: NormalizedText,
            Date: DateValue
        };
    };
    TryCulturePattern = function (SourceText) {
        var Separators = [tp.GetDateSeparator(CultureCode), "/", "-", "."];
        var Separator;
        var Result;
        var i;
        for (i = 0; i < Separators.length; i++) {
            Separator = Separators[i];
            if (Separators.indexOf(Separator) !== i)
                continue;
            Result = TryPattern(SourceText, CultureFormat, Separator);
            if (Result)
                return Result;
        }
        return null;
    };
    if (tp.IsBlank(Text))
        return { Result: false, NormalizedText: "", Date: null };
    if (tp.IsIsoLikeDateText(Text)) {
        return TryPattern(Text, tp.DateFormatISO)
            || TryCulturePattern(Text)
            || { Result: false, NormalizedText: "", Date: null };
    }
    return TryCulturePattern(Text)
        || TryPattern(Text, tp.DateFormatISO)
        || { Result: false, NormalizedText: "", Date: null };
};
/**
 * Parses date text using ISO format or the current culture date format.
 * @param {string|Date|null|undefined} Text The source text or date.
 * @param {string|null|undefined} CultureCode Optional culture code.
 * @returns {Date|null} Returns a date or null.
 */
tp.ParseDateText = function (Text, CultureCode) {
    var Result = tp.TryNormalizeDateText(Text, CultureCode);
    return Result.Result === true ? Result.Date : null;
};
