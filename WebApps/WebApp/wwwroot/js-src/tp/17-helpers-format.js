// ● locale helpers
/**
 * Gets a culture code for formatting.
 * @param {string|null|undefined} CultureCode The optional culture code.
 * @returns {string} Returns a culture code.
 */
tp.GetCultureCode = function (CultureCode) {
    if (tp.IsString(CultureCode) && !tp.IsBlank(CultureCode))
        return CultureCode;
    if (tp.IsString(tp.CultureCode) && !tp.IsBlank(tp.CultureCode))
        return tp.CultureCode;
    if (typeof navigator !== "undefined" && tp.IsString(navigator.language) && !tp.IsBlank(navigator.language))
        return navigator.language;
    return "en-US";
};
/**
 * Returns the decimal separator of a culture.
 * @param {string|null|undefined} CultureCode The optional culture code.
 * @returns {string} Returns the decimal separator.
 */
tp.GetDecimalSeparator = function (CultureCode) {
    return (1.1).toLocaleString(tp.GetCultureCode(CultureCode)).substring(1, 2);
};
/**
 * Returns the thousand separator of a culture.
 * @param {string|null|undefined} CultureCode The optional culture code.
 * @returns {string} Returns the thousand separator.
 */
tp.GetThousandSeparator = function (CultureCode) {
    return (1000).toLocaleString(tp.GetCultureCode(CultureCode)).substring(1, 2);
};
/**
 * Returns the date separator of a culture.
 * @param {string|null|undefined} CultureCode The optional culture code.
 * @returns {string} Returns the date separator.
 */
tp.GetDateSeparator = function (CultureCode) {
    var Text = new Date(2000, 9, 15).toLocaleDateString(tp.GetCultureCode(CultureCode));
    if (Text.indexOf("/") !== -1)
        return "/";
    if (Text.indexOf(".") !== -1)
        return ".";
    return "-";
};
/**
 * Returns the date format of a culture, e.g. dd/MM/yyyy or MM/dd/yyyy.
 * @param {string|null|undefined} CultureCode The optional culture code.
 * @returns {string} Returns the date format.
 */
tp.GetDateFormat = function (CultureCode) {
    var DateSeparator;
    var Text;
    var Parts;
    var Index;
    if (CultureCode === "ISO")
        return tp.DateFormatISO;
    CultureCode = tp.GetCultureCode(CultureCode);
    DateSeparator = tp.GetDateSeparator(CultureCode);
    Text = new Date(2000, 9, 15).toLocaleDateString(CultureCode, { year: "numeric", month: "2-digit", day: "2-digit" });
    Parts = Text.split(DateSeparator);
    for (Index = 0; Index < Parts.length; Index++) {
        Parts[Index] = Parts[Index].trim();
        if (Parts[Index] === "2000")
            Parts[Index] = "yyyy";
        else if (Parts[Index] === "10")
            Parts[Index] = "MM";
        else if (Parts[Index] === "15")
            Parts[Index] = "dd";
    }
    return Parts.join(DateSeparator);
};
/**
 * Returns a tp.DatePattern value by analyzing a date format string.
 * @param {string} DateFormat The date format string.
 * @returns {number} Returns a tp.DatePattern value.
 */
tp.GetDatePattern = function (DateFormat) {
    var First;
    if (tp.IsBlank(DateFormat))
        return tp.DatePattern.DMY;
    First = DateFormat.trim().charAt(0).toUpperCase();
    if (First === "Y")
        return tp.DatePattern.YMD;
    if (First === "M")
        return tp.DatePattern.MDY;
    return tp.DatePattern.DMY;
};
/**
 * The ISO date format.
 * @type {string}
 */
tp.DateFormatISO = "yyyy-MM-dd";

// ● format
/**
 * Formats a string the C# way.
 * Number and Date values should be passed as already formatted strings.
 * @example
 * var Text = tp.Format("String: {0}, Number: {1}", "tripous", 789);
 * @param {string} Text The format string.
 * @param {...*} Values The values for the format string.
 * @returns {string} Returns the formatted string.
 */
tp.Format = function (Text, ...Values) {
    var Index;
    if (!tp.IsString(Text))
        return Text;
    for (Index = 0; Index < Values.length; Index++)
        Text = Text.replace(new RegExp("\\{" + Index + "\\}", "g"), tp.IsNil(Values[Index]) ? "" : String(Values[Index]));
    return Text;
};
/**
 * Short alias for tp.Format.
 * @param {string} Text The format string.
 * @param {...*} Values The values for the format string.
 * @returns {string} Returns the formatted string.
 */
tp._F = function (Text, ...Values) {
    return tp.Format.apply(tp, [Text].concat(Values));
};
/**
 * Formats a number using standard or simple custom .NET-like format strings.
 * Supports D, F, N, C and simple custom patterns such as 0.00 or #,##0.00.
 * @see {@link https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings|.NET standard numeric format strings}
 * @see {@link https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings|.NET custom numeric format strings}
 * @param {number} Value The number to format.
 * @param {string|null|undefined} Format The format string.
 * @param {string|null|undefined} CultureCode The optional culture code.
 * @returns {string} Returns the formatted number.
 */
tp.FormatNumber = function (Value, Format, CultureCode) {
    var Options = { style: "decimal" };
    var Decimals;
    var Parts;
    var IntegerPart;
    var DecimalPart;
    var UseGroups;
    if (!tp.IsNumber(Value))
        return "";
    CultureCode = tp.GetCultureCode(CultureCode);
    Format = tp.IsBlank(Format) ? "G" : String(Format);
    if (Format.toUpperCase() === "G")
        return String(Value);
    Format = Format.toUpperCase();
    if (["C", "D", "F", "N"].indexOf(Format.charAt(0)) !== -1) {
        Decimals = Format.length > 1 ? tp.StrToInt(Format.substring(1), 0) : 0;
        if (Format.charAt(0) === "C") {
            Options.style = "currency";
            Options.useGrouping = true;
            Options.currency = tp.CurrencyCode || "USD";
            Options.currencyDisplay = "symbol";
            Options.minimumFractionDigits = Decimals > 0 ? Decimals : 2;
            Options.maximumFractionDigits = Options.minimumFractionDigits;
        } else if (Format.charAt(0) === "D") {
            Options.useGrouping = false;
            Options.minimumIntegerDigits = Decimals > 0 ? Decimals : 1;
            Options.minimumFractionDigits = 0;
            Options.maximumFractionDigits = 0;
        } else {
            Options.useGrouping = Format.charAt(0) === "N";
            Options.minimumFractionDigits = Decimals > 0 ? Decimals : 2;
            Options.maximumFractionDigits = Options.minimumFractionDigits;
        }
    } else {
        Parts = Format.split(".");
        IntegerPart = Parts[0];
        DecimalPart = Parts.length > 1 ? Parts[1] : "";
        UseGroups = IntegerPart.indexOf(",") !== -1;
        IntegerPart = IntegerPart.replace(/,/g, "");
        Options.useGrouping = UseGroups;
        Options.minimumIntegerDigits = tp.StartsWith(IntegerPart, "0", false) ? IntegerPart.length : 1;
        Options.minimumFractionDigits = DecimalPart.length;
        Options.maximumFractionDigits = DecimalPart.length;
    }
    return Value.toLocaleString(CultureCode, Options);
};
/**
 * Formats a number using explicit separators.
 * @param {number} Value The number to format.
 * @param {number|null|undefined} Decimals The number of decimal places.
 * @param {string|null|undefined} DecimalSep The decimal separator.
 * @param {string|null|undefined} ThousandSep The thousand separator.
 * @returns {string} Returns the formatted number.
 */
tp.FormatNumber2 = function (Value, Decimals, DecimalSep, ThousandSep) {
    var Text;
    var Parts;
    var NumPart;
    var DecPart;
    if (!tp.IsNumber(Value))
        return "";
    Decimals = tp.IsNullOrUndefined(Decimals) ? 0 : tp.ToInt(Decimals);
    DecimalSep = tp.IsNullOrUndefined(DecimalSep) ? tp.GetDecimalSeparator() : DecimalSep;
    ThousandSep = tp.IsNullOrUndefined(ThousandSep) ? tp.GetThousandSeparator() : ThousandSep;
    Text = Value.toFixed(Decimals);
    Parts = Text.split(".");
    NumPart = Parts[0];
    DecPart = Parts[1] ? DecimalSep + Parts[1] : "";
    return NumPart.replace(/(\d)(?=(?:\d{3})+$)/g, "$1" + ThousandSep) + DecPart;
};
/**
 * Formats a Date value based on a format string pattern.
 * @param {Date} Value The Date value to format.
 * @param {string|null|undefined} Format The format string. Defaults to the current culture date format.
 * @returns {string} Returns the formatted date.
 */
tp.FormatDateTime = function (Value, Format) {
    var Pad;
    var Parts;
    var Result = [];
    var IsMatch;
    var FormatPart;
    var ResultPart;
    var i;
    if (!tp.IsValidDate(Value))
        return "";
    Format = tp.IsBlank(Format) ? tp.GetDateFormat() : Format;
    Pad = function (NumberValue, Length) {
        var Negative = NumberValue < 0 ? "-" : "";
        var Zeros = "0";
        for (var Index = 2; Index < Length; Index++)
            Zeros += "0";
        return Negative + (Zeros + Math.abs(NumberValue).toString()).slice(-Length);
    };
    Parts = {
        yyyy: function () { return Value.getFullYear(); },
        yy: function () { return Value.getFullYear() % 100; },
        MM: function () { return Pad(Value.getMonth() + 1, 2); },
        M: function () { return Value.getMonth() + 1; },
        dd: function () { return Pad(Value.getDate(), 2); },
        d: function () { return Value.getDate(); },
        HH: function () { return Pad(Value.getHours(), 2); },
        H: function () { return Value.getHours(); },
        hh: function () {
            var Hour = Value.getHours();
            if (Hour > 12)
                Hour -= 12;
            else if (Hour < 1)
                Hour = 12;
            return Pad(Hour, 2);
        },
        h: function () {
            var Hour = Value.getHours();
            if (Hour > 12)
                Hour -= 12;
            else if (Hour < 1)
                Hour = 12;
            return Hour;
        },
        mm: function () { return Pad(Value.getMinutes(), 2); },
        m: function () { return Value.getMinutes(); },
        ss: function () { return Pad(Value.getSeconds(), 2); },
        s: function () { return Value.getSeconds(); },
        fff: function () { return Pad(Value.getMilliseconds(), 3); },
        ff: function () { return Pad(Math.floor(Value.getMilliseconds() / 10), 2); },
        f: function () { return Math.floor(Value.getMilliseconds() / 100); },
        zzzz: function () { return Pad(Math.floor(-Value.getTimezoneOffset() / 60), 2) + ":" + Pad(-Value.getTimezoneOffset() % 60, 2); },
        zzz: function () { return Math.floor(-Value.getTimezoneOffset() / 60) + ":" + Pad(-Value.getTimezoneOffset() % 60, 2); },
        zz: function () { return Pad(Math.floor(-Value.getTimezoneOffset() / 60), 2); },
        z: function () { return Math.floor(-Value.getTimezoneOffset() / 60); }
    };
    while (Format.length > 0) {
        IsMatch = false;
        for (i = Format.length; i > 0; i--) {
            FormatPart = Format.substring(0, i);
            if (FormatPart in Parts) {
                ResultPart = Parts[FormatPart]();
                Result.push(ResultPart);
                Format = Format.substring(i);
                IsMatch = true;
                break;
            }
        }
        if (!IsMatch) {
            Result.push(Format[0]);
            Format = Format.substring(1);
        }
    }
    return Result.join("");
};
