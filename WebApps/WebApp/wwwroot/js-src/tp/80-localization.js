// ● language
/**
 * Stores string resources for a language.
 *
 * A language is identified by a two-letter ISO 639-1 language code.
 * @see {@link https://en.wikipedia.org/wiki/ISO_639-1|ISO 639-1}
 * @see {@link https://www.rfc-editor.org/rfc/bcp/bcp47.txt|BCP 47}
 */
tp.Language = class {
    // ● constructor
    /**
     * Creates a language.
     * @param {string} Name The language name.
     * @param {string} Code The two-letter language code, e.g. en, el, it.
     * @param {string} CultureCode The culture code associated with this language, e.g. en-US, el-GR.
     */
    constructor(Name, Code, CultureCode) {
        this.fName = Name || "";
        this.fCode = Code || "";
        this.fCultureCode = CultureCode || "";
        this.fItems = new tp.Dictionary();
    }

    // ● properties
    /**
     * Gets the two-letter language code.
     * @type {string}
     */
    get Code() {
        return this.fCode;
    }
    /**
     * Gets the language name.
     * @type {string}
     */
    get Name() {
        return this.fName;
    }
    /**
     * Gets the culture code associated with this language.
     * @type {string}
     */
    get CultureCode() {
        return this.fCultureCode;
    }
    /**
     * Gets the string resource dictionary.
     * @type {tp.Dictionary}
     */
    get Items() {
        return this.fItems;
    }

    // ● public
    /**
     * Adds a source string list to this language.
     * @param {object|null|undefined} Source The source object to copy strings from.
     * @returns {void}
     */
    AddStringList(Source) {
        var Key;
        if (!Source)
            return;
        for (Key in Source) {
            if (Object.prototype.propertyIsEnumerable.call(Source, Key))
                this.SetLocalizationEntry(Key, Source[Key]);
        }
    }
    /**
     * Sets a localization entry.
     * @param {string} Key The resource key.
     * @param {string} Value The resource value.
     * @returns {void}
     */
    SetLocalizationEntry(Key, Value) {
        if (!tp.IsBlank(Key))
            this.Items.Set(String(Key).toLowerCase(), Value);
    }
    /**
     * Returns a localized string based on a resource key.
     * @param {string} Key The resource key.
     * @param {string|null|undefined} Default The default value when the key is not found.
     * @returns {string} Returns the localized string, the default value, or the key.
     */
    Localize(Key, Default) {
        var Value;
        if (tp.IsBlank(Key))
            return tp.IsString(Default) ? Default : "";
        Value = this.Items.Get(String(Key).toLowerCase());
        if (tp.IsString(Value))
            return Value;
        if (tp.IsString(Default))
            return Default;
        return Key;
    }
    /**
     * Returns a string representation of this language.
     * @returns {string} Returns a string representation of this language.
     */
    toString() {
        return tp.Format("{0} - {1}", this.Code, this.Name);
    }
};
/**
 * The two-letter language code.
 * @type {string}
 */
tp.Language.prototype.fCode = "";
/**
 * The language name.
 * @type {string}
 */
tp.Language.prototype.fName = "";
/**
 * The associated culture code.
 * @type {string}
 */
tp.Language.prototype.fCultureCode = "";
/**
 * The string resource dictionary.
 * @type {tp.Dictionary|null}
 */
tp.Language.prototype.fItems = null;

// ● languages
/**
 * Static language registry.
 * @type {object}
 */
tp.Languages = {
    /**
     * Finds a language by code.
     * @param {string} Code The two-letter language code.
     * @returns {tp.Language|null} Returns the language or null.
     */
    Find: function (Code) {
        return tp.FirstOrDefault(this.Items, function (Item) {
            return tp.IsSameText(Code, Item.Code);
        });
    },
    /**
     * Returns true when a language exists.
     * @param {string} Code The two-letter language code.
     * @returns {boolean} Returns true when the language exists.
     */
    Exists: function (Code) {
        return this.Find(Code) !== null;
    },
    /**
     * Adds or returns a language.
     * @param {string} Name The language name.
     * @param {string} Code The two-letter language code.
     * @param {string} CultureCode The associated culture code.
     * @returns {tp.Language} Returns the language.
     */
    Add: function (Name, Code, CultureCode) {
        var Result = this.Find(Code);
        if (tp.IsNil(Result)) {
            Result = new tp.Language(Name, Code, CultureCode);
            this.Items.push(Result);
        }
        return Result;
    },
    /**
     * Initializes the default languages.
     * @returns {void}
     */
    Initialize: function () {
        var CultureCode = tp.GetInitialCultureCode();
        var LanguageCode = CultureCode.split("-")[0];
        var Language;
        this.En = this.Add("English", "en", "en-US");
        this.Gr = this.Add("Greek", "el", "el-GR");
        Language = this.Find(LanguageCode);
        if (!Language)
            Language = this.Add("unknown", LanguageCode, CultureCode);
        this.fCurrent = Language;
    },
    /**
     * Gets or sets the current language.
     * @type {tp.Language|null}
     */
    get Current() {
        if (this.fCurrent instanceof tp.Language)
            return this.fCurrent;
        if (this.Items.length > 0)
            return this.Items[0];
        return null;
    },
    set Current(v) {
        if (v instanceof tp.Language)
            this.fCurrent = v;
    },
    /**
     * The registered languages.
     * @type {tp.Language[]}
     */
    Items: [],
    /**
     * The current language.
     * @type {tp.Language|null}
     */
    fCurrent: null,
    /**
     * The English language.
     * @type {tp.Language|null}
     */
    En: null,
    /**
     * The Greek language.
     * @type {tp.Language|null}
     */
    Gr: null
};
/**
 * Returns a localized string from the current language.
 * @param {string} Key The resource key.
 * @param {string|null|undefined} Default The default value when the key is not found.
 * @returns {string} Returns the localized string, the default value, or the key.
 */
tp._L = function (Key, Default) {
    return tp.Languages.Current ? tp.Languages.Current.Localize(Key, Default) : (tp.IsString(Default) ? Default : Key);
};

// ● string resources
/**
 * Static string resource helper.
 * @type {object}
 */
tp.Res = {
    /**
     * Gets a string resource and returns it through a callback.
     * @param {string} Key The resource key.
     * @param {Function} ResultFunc The callback function.
     * @param {string|null|undefined} Default The default value when the key is not found.
     * @param {object|null|undefined} Context The callback context.
     * @param {*} UserTag A user-defined value.
     * @returns {void}
     */
    GS: function (Key, ResultFunc, Default, Context, UserTag) {
        var Value;
        if (!tp.IsFunction(ResultFunc))
            return;
        UserTag = tp.IsNil(UserTag) ? Key : UserTag;
        Value = tp._L(Key, Default);
        tp.Call(ResultFunc, Context, Value, UserTag);
    },
    /**
     * Gets a string resource and returns it through a callback.
     * @param {string} Key The resource key.
     * @param {Function} ResultFunc The callback function.
     * @param {string|null|undefined} Default The default value when the key is not found.
     * @param {object|null|undefined} Context The callback context.
     * @param {*} UserTag A user-defined value.
     * @returns {void}
     */
    GetString: function (Key, ResultFunc, Default, Context, UserTag) {
        tp.Res.GS(Key, ResultFunc, Default, Context, UserTag);
    }
};

// ● culture
/**
 * Provides culture information regarding dates, numbers, and currency.
 */
tp.Culture = class {
    // ● constructor
    /**
     * Creates a culture.
     * @param {object|null|undefined} Source The optional source object.
     */
    constructor(Source) {
        if (Source)
            tp.Assign(this, Source);
    }
};
/**
 * Country name, in English.
 * @type {string}
 */
tp.Culture.prototype.Country = "";
/**
 * Culture name, in English.
 * @type {string}
 */
tp.Culture.prototype.Name = "";
/**
 * Culture code, e.g. en-US.
 * @type {string}
 */
tp.Culture.prototype.Code = "";
/**
 * Full date-time format pattern.
 * @type {string}
 */
tp.Culture.prototype.FullDateTimeFormat = "";
/**
 * Date format pattern.
 * @type {string}
 */
tp.Culture.prototype.DateFormat = "";
/**
 * Date separator.
 * @type {string}
 */
tp.Culture.prototype.DateSeparator = "";
/**
 * The first day of the week, in English.
 * @type {string}
 */
tp.Culture.prototype.FirstDayOfWeek = "";
/**
 * Time format pattern.
 * @type {string}
 */
tp.Culture.prototype.TimeFormat = "";
/**
 * Time separator.
 * @type {string}
 */
tp.Culture.prototype.TimeSeparator = "";
/**
 * The PM designator.
 * @type {string}
 */
tp.Culture.prototype.PM = "";
/**
 * The AM designator.
 * @type {string}
 */
tp.Culture.prototype.AM = "";
/**
 * Day names.
 * @type {string[]}
 */
tp.Culture.prototype.DayNames = [];
/**
 * Abbreviated day names.
 * @type {string[]}
 */
tp.Culture.prototype.AbbreviatedDayNames = [];
/**
 * Month names.
 * @type {string[]}
 */
tp.Culture.prototype.MonthNames = [];
/**
 * Abbreviated month names.
 * @type {string[]}
 */
tp.Culture.prototype.AbbreviatedMonthNames = [];
/**
 * Decimal separator.
 * @type {string}
 */
tp.Culture.prototype.DecimalSeparator = "";
/**
 * Thousand separator.
 * @type {string}
 */
tp.Culture.prototype.ThousandSeparator = "";
/**
 * Currency name, in English.
 * @type {string}
 */
tp.Culture.prototype.CurrencyName = "";
/**
 * Currency code, e.g. USD or EUR.
 * @type {string}
 */
tp.Culture.prototype.CurrencyCode = "";
/**
 * Currency symbol, e.g. $ or €.
 * @type {string}
 */
tp.Culture.prototype.CurrencySymbol = "";
/**
 * Default decimal places for a currency value.
 * @type {number}
 */
tp.Culture.prototype.CurrencyDecimals = 0;

// ● cultures
/**
 * Static culture registry.
 * @type {object}
 */
tp.Cultures = {
    /**
     * Finds a culture by code.
     * @param {string} Code The culture code.
     * @returns {tp.Culture|null} Returns the culture or null.
     */
    Find: function (Code) {
        return tp.FirstOrDefault(this.Items, function (Item) {
            return tp.IsSameText(Code, Item.Code);
        });
    },
    /**
     * Adds a culture.
     * @param {tp.Culture} Culture The culture to add.
     * @returns {tp.Culture|null} Returns the culture or null.
     */
    Add: function (Culture) {
        if (Culture instanceof tp.Culture) {
            if (!this.Find(Culture.Code))
                this.Items.push(Culture);
            return Culture;
        }
        return null;
    },
    /**
     * Sorts cultures by property.
     * @param {string|null|undefined} PropName The property name. Defaults to Code.
     * @returns {void}
     */
    Sort: function (PropName) {
        PropName = tp.IsBlank(PropName) ? "Code" : PropName;
        this.Items.sort(function (A, B) {
            return String(A[PropName] || "").localeCompare(String(B[PropName] || ""));
        });
    },
    /**
     * Ensures a culture exists and returns it.
     * @param {string} CultureCode The culture code.
     * @returns {tp.Culture} Returns a culture.
     */
    Ensure: function (CultureCode) {
        var Result;
        var Base;
        CultureCode = tp.IsBlank(CultureCode) ? "en-US" : CultureCode;
        Result = this.Find(CultureCode);
        if (Result)
            return Result;
        Base = this.Find("en-US") || this.Items[0] || new tp.Culture();
        Result = new tp.Culture(Base);
        Result.Code = CultureCode;
        Result.DecimalSeparator = tp.GetDecimalSeparator(CultureCode);
        Result.ThousandSeparator = tp.GetThousandSeparator(CultureCode);
        Result.DateSeparator = tp.GetDateSeparator(CultureCode);
        Result.DateFormat = tp.GetDateFormat(CultureCode);
        this.Add(Result);
        return Result;
    },
    /**
     * Initializes the default cultures.
     * @returns {void}
     */
    Initialize: function () {
        this.Add(new tp.Culture({
            Country: "United States",
            Name: "English (United States)",
            Code: "en-US",
            FullDateTimeFormat: "dddd, MMMM d, yyyy h:mm:ss tt",
            DateFormat: "M/d/yyyy",
            DateSeparator: "/",
            FirstDayOfWeek: "Sunday",
            TimeFormat: "h:mm tt",
            TimeSeparator: ":",
            PM: "PM",
            AM: "AM",
            DayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
            AbbreviatedDayNames: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
            MonthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
            AbbreviatedMonthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            DecimalSeparator: ".",
            ThousandSeparator: ",",
            CurrencyName: "US Dollar",
            CurrencyCode: "USD",
            CurrencySymbol: "$",
            CurrencyDecimals: 2
        }));
        this.Add(new tp.Culture({
            Country: "Greece",
            Name: "Greek (Greece)",
            Code: "el-GR",
            FullDateTimeFormat: "dddd, d MMMM yyyy h:mm:ss tt",
            DateFormat: "d/M/yyyy",
            DateSeparator: "/",
            FirstDayOfWeek: "Monday",
            TimeFormat: "h:mm tt",
            TimeSeparator: ":",
            PM: "μμ",
            AM: "πμ",
            DayNames: ["Κυριακή", "Δευτέρα", "Τρίτη", "Τετάρτη", "Πέμπτη", "Παρασκευή", "Σάββατο"],
            AbbreviatedDayNames: ["Κυρ", "Δευ", "Τρι", "Τετ", "Πεμ", "Παρ", "Σαβ"],
            MonthNames: ["Ιανουάριος", "Φεβρουάριος", "Μάρτιος", "Απρίλιος", "Μάιος", "Ιούνιος", "Ιούλιος", "Αύγουστος", "Σεπτέμβριος", "Οκτώβριος", "Νοέμβριος", "Δεκέμβριος"],
            AbbreviatedMonthNames: ["Ιαν", "Φεβ", "Μαρ", "Απρ", "Μαϊ", "Ιουν", "Ιουλ", "Αυγ", "Σεπ", "Οκτ", "Νοε", "Δεκ"],
            DecimalSeparator: ",",
            ThousandSeparator: ".",
            CurrencyName: "Euro",
            CurrencyCode: "EUR",
            CurrencySymbol: "€",
            CurrencyDecimals: 2
        }));
        this.fCurrent = this.Ensure(tp.GetInitialCultureCode());
    },
    /**
     * Gets or sets the current culture.
     * @type {tp.Culture|null}
     */
    get Current() {
        if (this.fCurrent instanceof tp.Culture)
            return this.fCurrent;
        if (this.Items.length > 0)
            return this.Items[0];
        return null;
    },
    set Current(v) {
        if (v instanceof tp.Culture)
            this.fCurrent = v;
    },
    /**
     * The current culture.
     * @type {tp.Culture|null}
     */
    fCurrent: null,
    /**
     * The registered cultures.
     * @type {tp.Culture[]}
     */
    Items: []
};

// ● culture helpers
/**
 * Returns the initial culture code from html lang or navigator language.
 * @returns {string} Returns the initial culture code.
 */
tp.GetInitialCultureCode = function () {
    var Result = "";
    var Html;
    if (typeof document !== "undefined") {
        Html = document.querySelector("html");
        Result = Html ? Html.getAttribute("lang") : "";
    }
    if (tp.IsBlank(Result) && typeof navigator !== "undefined")
        Result = navigator.language;
    if (tp.IsBlank(Result))
        Result = "en-US";
    if (Result.length === 2) {
        if (tp.IsSameText(Result, "el"))
            return "el-GR";
        if (tp.IsSameText(Result, "en"))
            return "en-US";
    }
    return Result;
};
/**
 * Defines a culture property on the tp namespace.
 * @param {string} Name The property name.
 * @param {Function} Getter The getter function.
 * @returns {void}
 */
tp.DefineCultureProperty = function (Name, Getter) {
    Object.defineProperty(tp, Name, {
        configurable: true,
        get: Getter
    });
};
tp.DefineCultureProperty("CurrencySymbol", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.CurrencySymbol : "$";
});
tp.DefineCultureProperty("CurrencyCode", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.CurrencyCode : "USD";
});
tp.DefineCultureProperty("CurrencyDecimals", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.CurrencyDecimals : 2;
});
tp.DefineCultureProperty("DecimalSeparator", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.DecimalSeparator : ".";
});
tp.DefineCultureProperty("ThousandSeparator", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.ThousandSeparator : ",";
});
tp.DefineCultureProperty("DateSeparator", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.DateSeparator : "/";
});
tp.DefineCultureProperty("DateFormat", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.DateFormat : "M/d/yyyy";
});
tp.DefineCultureProperty("DayNames", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.DayNames : [];
});
tp.DefineCultureProperty("MonthNames", function () {
    return tp.Cultures.Current ? tp.Cultures.Current.MonthNames : [];
});
Object.defineProperty(tp, "CultureCode", {
    configurable: true,
    get: function () {
        return tp.Cultures.Current ? tp.Cultures.Current.Code : "en-US";
    },
    set: function (v) {
        if (!tp.IsBlank(v))
            tp.Cultures.Current = tp.Cultures.Ensure(v);
    }
});

tp.Cultures.Initialize();
tp.Languages.Initialize();
