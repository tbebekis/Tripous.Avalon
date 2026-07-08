// ● string checks
/**
 * Returns true when a value is null, undefined, or an empty trimmed string.
 * Unlike tp.IsBlank(), this function returns false for non-string values.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a blank string.
 */
tp.IsBlankString = function (Value) {
    return tp.IsNil(Value) || (tp.IsString(Value) && Value.trim().length === 0);
};
/**
 * Returns true when a character is a whitespace character.
 * @param {string} Value The character to check.
 * @returns {boolean} Returns true when the character is whitespace.
 */
tp.IsWhitespaceChar = function (Value) {
    return tp.IsString(Value) && Value.length > 0 && Value.charCodeAt(0) <= 32;
};
/**
 * Returns true when text looks like HTML markup.
 * @see {@link https://stackoverflow.com/questions/15458876/check-if-a-string-is-html-or-not|StackOverflow}
 * @param {string} Text The text to check.
 * @returns {boolean} Returns true when text looks like HTML markup.
 */
tp.IsHtml = function (Text) {
    return tp.IsString(Text) && /<[a-z][\s\S]*>/i.test(Text);
};
/**
 * Returns true when two strings are equal case-insensitively.
 * @param {string} A The first string.
 * @param {string} B The second string.
 * @returns {boolean} Returns true when the two strings are equal case-insensitively.
 */
tp.IsSameText = function (A, B) {
    return tp.IsString(A) && tp.IsString(B) && A.toUpperCase() === B.toUpperCase();
};
/**
 * Returns true when a string contains a substring.
 * @param {string} Text The text to search in.
 * @param {string} SubText The text to search for.
 * @param {boolean|null|undefined} CaseInsensitive True for case-insensitive search.
 * @returns {boolean} Returns true when the substring is contained.
 */
tp.ContainsText = function (Text, SubText, CaseInsensitive) {
    if (!tp.IsString(Text) || !tp.IsString(SubText))
        return false;
    CaseInsensitive = CaseInsensitive !== false;
    return CaseInsensitive ? Text.toLowerCase().includes(SubText.toLowerCase()) : Text.includes(SubText);
};
/**
 * Returns true when a string starts with a substring.
 * @param {string} Text The text to check.
 * @param {string} SubText The starting text.
 * @param {boolean|null|undefined} CaseInsensitive True for case-insensitive check.
 * @returns {boolean} Returns true when the text starts with the substring.
 */
tp.StartsWith = function (Text, SubText, CaseInsensitive) {
    if (!tp.IsString(Text) || !tp.IsString(SubText) || SubText.length === 0)
        return false;
    CaseInsensitive = CaseInsensitive !== false;
    return CaseInsensitive ? Text.toUpperCase().startsWith(SubText.toUpperCase()) : Text.startsWith(SubText);
};
/**
 * Returns true when a string ends with a substring.
 * @param {string} Text The text to check.
 * @param {string} SubText The ending text.
 * @param {boolean|null|undefined} CaseInsensitive True for case-insensitive check.
 * @returns {boolean} Returns true when the text ends with the substring.
 */
tp.EndsWith = function (Text, SubText, CaseInsensitive) {
    if (!tp.IsString(Text) || !tp.IsString(SubText) || SubText.length === 0)
        return false;
    CaseInsensitive = CaseInsensitive !== false;
    return CaseInsensitive ? Text.toUpperCase().endsWith(SubText.toUpperCase()) : Text.endsWith(SubText);
};
/**
 * Returns true when a string is a valid JavaScript-like identifier.
 * @param {string} Value The string to check.
 * @param {string|null|undefined} PlusValidChars Additional valid characters after the first character.
 * @returns {boolean} Returns true when the value is a valid identifier.
 */
tp.IsValidIdentifier = function (Value, PlusValidChars) {
    var Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    var Numbers = "0123456789";
    var StartLetters = Letters + "_";
    var ValidChars;
    var Index;
    var Char;
    if (!tp.IsString(Value) || tp.IsBlank(Value))
        return false;
    PlusValidChars = tp.IsString(PlusValidChars) ? PlusValidChars : "";
    ValidChars = Letters + Numbers + PlusValidChars + "_";
    for (Index = 0; Index < Value.length; Index++) {
        Char = Value.charAt(Index);
        if (Index === 0 && !StartLetters.includes(Char))
            return false;
        if (!ValidChars.includes(Char))
            return false;
    }
    return true;
};

// ● string transforms
/**
 * Inserts a substring into a string at a specified index.
 * @param {string} SubString The substring to insert.
 * @param {string} Text The target string.
 * @param {number} Index The insert index.
 * @returns {string} Returns the new string.
 */
tp.InsertText = function (SubString, Text, Index) {
    Text = tp.IsNil(Text) ? "" : String(Text);
    SubString = tp.IsNil(SubString) ? "" : String(SubString);
    Index = Number.isFinite(Number(Index)) ? Math.trunc(Number(Index)) : 0;
    Index = Math.max(0, Math.min(Index, Text.length));
    return Text.slice(0, Index) + SubString + Text.slice(Index);
};
/**
 * Escapes a string for use in a regular expression.
 * @see {@link https://stackoverflow.com/questions/3446170/escape-string-for-use-in-javascript-regex|StackOverflow}
 * @param {string} Value The value to escape.
 * @returns {string} Returns the escaped string.
 */
tp.RegExEscape = function (Value) {
    return tp.IsBlank(Value) ? "" : String(Value).replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
};
/**
 * Replaces the first occurrence of a substring.
 * @param {string} Value The text to operate on.
 * @param {string} OldValue The text to replace.
 * @param {string} NewValue The replacement text.
 * @returns {string} Returns the new string.
 */
tp.Replace = function (Value, OldValue, NewValue) {
    return tp.IsNil(Value) ? "" : String(Value).replace(OldValue, NewValue);
};
/**
 * Replaces all occurrences of a substring.
 * @param {string} Value The text to operate on.
 * @param {string} OldValue The text to replace.
 * @param {string} NewValue The replacement text.
 * @param {boolean|null|undefined} CaseInsensitive True for case-insensitive replacement.
 * @returns {string} Returns the new string.
 */
tp.ReplaceAll = function (Value, OldValue, NewValue, CaseInsensitive) {
    var Flags;
    if (tp.IsNil(Value))
        return "";
    if (tp.IsBlank(OldValue))
        return String(Value);
    Flags = CaseInsensitive === false ? "g" : "gi";
    return String(Value).replace(new RegExp(tp.RegExEscape(OldValue), Flags), tp.IsNil(NewValue) ? "" : String(NewValue));
};
/**
 * Replaces a character at a specified index.
 * @param {string} Value The text to operate on.
 * @param {string} NewChar The replacement character.
 * @param {number} Index The character index.
 * @returns {string} Returns the new string.
 */
tp.ReplaceCharAt = function (Value, NewChar, Index) {
    Value = tp.IsNil(Value) ? "" : String(Value);
    NewChar = tp.IsNil(NewChar) ? "" : String(NewChar);
    Index = Number.isFinite(Number(Index)) ? Math.trunc(Number(Index)) : 0;
    if (Index < 0 || Index >= Value.length)
        return Value;
    return Value.slice(0, Index) + NewChar + Value.slice(Index + 1);
};
/**
 * Converts a dashed CSS name to camel case.
 * @param {string} Value The value to convert.
 * @returns {string} Returns the camel-cased value.
 */
tp.DashToCamelCase = function (Value) {
    if (tp.IsBlank(Value))
        return "";
    Value = String(Value);
    if (Value.length > 1 && Value.charAt(0) === "-")
        Value = Value.substring(1);
    return Value.replace(/-([\da-z])/gi, function (Match, Char) {
        return Char.toUpperCase();
    });
};

// ● url handling
/**
 * Combines two url path parts and ensures that a single slash joins them.
 * @param {string} A The first url part.
 * @param {string} B The second url part.
 * @returns {string} Returns the combined url path.
 */
tp.UrlCombine = function (A, B) {
    A = tp.IsNil(A) ? "" : String(A);
    B = tp.IsNil(B) ? "" : String(B);
    if (tp.IsBlank(A))
        return B;
    if (tp.IsBlank(B))
        return A;
    if (tp.EndsWith(A, "/", false) && tp.StartsWith(B, "/", false))
        return A + B.substring(1);
    if (!tp.EndsWith(A, "/", false) && !tp.StartsWith(B, "/", false))
        return A + "/" + B;
    return A + B;
};
/**
 * Navigates to a specified url.
 * @param {string} Url The url to navigate to.
 * @returns {void}
 */
tp.NavigateTo = function (Url) {
    if (!tp.IsBlank(Url) && typeof window !== "undefined")
        window.location.href = Url;
};
/**
 * Returns the base url, e.g. http://server.com/.
 * @returns {string} Returns the base url.
 */
tp.GetBaseUrl = function () {
    if (typeof window === "undefined")
        return "";
    return window.location.protocol + "//" + window.location.host + "/";
};
/**
 * Returns a URL object.
 * @param {string|null|undefined} Url The url. When null, the current browser url is used.
 * @returns {URL|null} Returns a URL object or null.
 */
tp.GetUrl = function (Url) {
    var Base;
    if (tp.IsBlank(Url) && typeof window !== "undefined")
        Url = window.location.href;
    if (tp.IsBlank(Url) || typeof URL === "undefined")
        return null;
    try {
        Base = typeof window !== "undefined" ? window.location.href : "http://localhost/";
        return new URL(Url, Base);
    } catch (e) {
        return null;
    }
};
/**
 * Returns a query string parameter by name, if any; otherwise, null.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/URLSearchParams|MDN URLSearchParams}
 * @param {string} Name The parameter name.
 * @param {string|null|undefined} Url The optional url. When null, the current browser url is used.
 * @returns {string|null} Returns the parameter value or null.
 */
tp.ParamByName = function (Name, Url) {
    var UrlObject;
    if (tp.IsBlank(Name))
        return null;
    UrlObject = tp.GetUrl(Url);
    if (!UrlObject)
        return null;
    return UrlObject.searchParams.has(Name) ? UrlObject.searchParams.get(Name) : null;
};
/**
 * Returns a plain object where each property is a query string parameter.
 * Repeated parameter names become arrays.
 * @param {string|null|undefined} Url The optional url. When null, the current browser url is used.
 * @returns {object} Returns a plain object with query string parameters.
 */
tp.GetParams = function (Url) {
    var Result = {};
    var UrlObject = tp.GetUrl(Url);
    if (!UrlObject)
        return Result;
    UrlObject.searchParams.forEach(function (Value, Key) {
        if (Object.prototype.hasOwnProperty.call(Result, Key)) {
            if (!tp.IsArray(Result[Key]))
                Result[Key] = [Result[Key]];
            Result[Key].push(Value);
        } else {
            Result[Key] = Value;
        }
    });
    return Result;
};
/**
 * Encodes an argument key/value pair for query string or form-url-encoded data.
 * @param {string} Key The argument key.
 * @param {*} Value The argument value.
 * @returns {string} Returns the encoded argument.
 */
tp.EncodeArg = function (Key, Value) {
    Value = tp.IsEmpty(Value) ? "" : Value;
    return encodeURIComponent(Key).replace(/%20/g, "+") + "=" + encodeURIComponent(Value).replace(/%20/g, "+");
};
/**
 * Encodes arguments for query string or form-url-encoded data.
 * The value may be a plain object, an array of values, or an array of DOM elements.
 * @param {object|Array|HTMLElement[]} Value The value to encode.
 * @returns {string} Returns the encoded argument string.
 */
tp.EncodeArgs = function (Value) {
    var Data = [];
    var Index;
    var Name;
    var ItemValue;
    var Prop;
    if (tp.IsArray(Value) && Value.length > 0) {
        if (tp.IsHTMLElement(Value[0])) {
            for (Index = 0; Index < Value.length; Index++) {
                if ("name" in Value[Index] && "value" in Value[Index]) {
                    Name = Value[Index].name;
                    ItemValue = Value[Index].value;
                    Data.push(tp.EncodeArg(Name, ItemValue));
                }
            }
        } else {
            for (Index = 0; Index < Value.length; Index++)
                Data.push(tp.EncodeArg("v" + Index.toString(), Value[Index]));
        }
    } else if (tp.IsPlainObject(Value)) {
        for (Prop in Value) {
            if (Object.prototype.propertyIsEnumerable.call(Value, Prop))
                Data.push(tp.EncodeArg(Prop, Value[Prop]));
        }
    }
    return Data.join("&");
};
/**
 * Trims a string.
 * @param {string} Value The string to trim.
 * @returns {string} Returns the trimmed string.
 */
tp.Trim = function (Value) {
    return tp.IsBlank(Value) ? "" : String(Value).trim();
};
/**
 * Trims the start of a string.
 * @param {string} Value The string to trim.
 * @returns {string} Returns the trimmed string.
 */
tp.TrimStart = function (Value) {
    return tp.IsBlank(Value) ? "" : String(Value).trimStart();
};
/**
 * Trims the end of a string.
 * @param {string} Value The string to trim.
 * @returns {string} Returns the trimmed string.
 */
tp.TrimEnd = function (Value) {
    return tp.IsBlank(Value) ? "" : String(Value).trimEnd();
};
/**
 * Removes a trailing comma after trimming a string.
 * @param {string} Value The string to operate on.
 * @returns {string} Returns the result string.
 */
tp.RemoveLastComma = function (Value) {
    Value = tp.Trim(Value);
    if (Value.length > 0 && tp.EndsWith(Value, ","))
        Value = Value.substring(0, Value.length - 1);
    return Value;
};
/**
 * Places single or double quotes around a string.
 * @param {string} Value The string to quote.
 * @param {boolean|null|undefined} DoubleQuotes True to use double quotes; false to use single quotes.
 * @returns {string} Returns the quoted string.
 */
tp.Quote = function (Value, DoubleQuotes) {
    if (tp.IsNil(Value))
        return Value;
    Value = String(Value);
    DoubleQuotes = DoubleQuotes !== false;
    if (DoubleQuotes)
        return "\"" + Value.replace(/"/g, "\\\"") + "\"";
    return "'" + Value.replace(/'/g, "\\'") + "'";
};
/**
 * Removes surrounding single or double quotes from a string.
 * @param {string} Value The string to unquote.
 * @returns {string} Returns the unquoted string.
 */
tp.Unquote = function (Value) {
    if (!tp.IsString(Value) || Value.length < 2)
        return Value;
    if ((Value.charAt(0) === "\"" && Value.charAt(Value.length - 1) === "\"") || (Value.charAt(0) === "'" && Value.charAt(Value.length - 1) === "'"))
        return Value.substring(1, Value.length - 1);
    return Value;
};
/**
 * Truncates a string to a specified length.
 * @param {string} Value The string to operate on.
 * @param {number} NewLength The desired length.
 * @returns {string} Returns the truncated string.
 */
tp.SetLength = function (Value, NewLength) {
    if (tp.IsBlank(Value))
        return "";
    Value = String(Value);
    NewLength = Number.isFinite(Number(NewLength)) ? Math.trunc(Number(NewLength)) : 0;
    return Value.length > NewLength ? Value.slice(0, NewLength) : Value;
};

// ● splitting and joining
/**
 * Splits a string into chunks according to a specified chunk size.
 * @param {string} Value The string to split.
 * @param {number} ChunkSize The chunk size.
 * @returns {string[]} Returns the chunks.
 */
tp.Chunk = function (Value, ChunkSize) {
    var RegEx;
    Value = tp.IsNil(Value) ? "" : String(Value);
    ChunkSize = Number.isFinite(Number(ChunkSize)) ? Math.trunc(Number(ChunkSize)) : 0;
    if (ChunkSize <= 0)
        return [];
    RegEx = new RegExp(".{1," + ChunkSize + "}", "g");
    return Value.match(RegEx) || [];
};
/**
 * Splits a string.
 * @param {string} Value The string to split.
 * @param {string|RegExp|null|undefined} Separator The separator.
 * @param {boolean|null|undefined} RemoveEmptyEntries True to remove empty entries.
 * @returns {string[]} Returns the split parts.
 */
tp.Split = function (Value, Separator, RemoveEmptyEntries) {
    var Parts;
    var Result = [];
    var Index;
    Value = tp.IsNil(Value) ? "" : String(Value);
    Separator = tp.IsNil(Separator) ? " " : Separator;
    RemoveEmptyEntries = RemoveEmptyEntries !== false;
    Parts = Value.split(Separator);
    if (!RemoveEmptyEntries)
        return Parts;
    for (Index = 0; Index < Parts.length; Index++) {
        if (!tp.IsBlank(Parts[Index]))
            Result.push(Parts[Index]);
    }
    return Result;
};
/**
 * Splits a PascalCase string into words.
 * @param {string} Value The string to split.
 * @returns {string} Returns the split string.
 */
tp.SplitOnUpperCase = function (Value) {
    var Match;
    if (!tp.IsString(Value))
        return "";
    Match = Value.match(/[A-Z][a-z]+|[A-Z]+(?![a-z])/g);
    return Match ? Match.join(" ") : Value;
};
/**
 * Splits descriptor text of the form Key:Value; Key2:Value2 into an object.
 * @param {string} Value The descriptor text.
 * @returns {object} Returns the descriptor object.
 */
tp.SplitDescriptor = function (Value) {
    var Result = {};
    var Lines;
    var Parts;
    var Index;
    var Key;
    var ItemValue;
    if (!tp.IsString(Value))
        return Result;
    Lines = tp.Split(Value, ";", true);
    for (Index = 0; Index < Lines.length; Index++) {
        Parts = tp.Split(Lines[Index], ":", false);
        if (Parts.length === 2) {
            Key = tp.Unquote(tp.Trim(Parts[0]));
            ItemValue = tp.Unquote(tp.Trim(Parts[1]));
            if (Key.length > 0)
                Result[Key] = ItemValue;
        }
    }
    return Result;
};
/**
 * Joins values with a separator.
 * @param {string} Separator The separator.
 * @param {...*} Values The values to join.
 * @returns {string} Returns the joined string.
 */
tp.Join = function (Separator, ...Values) {
    return Values.join(Separator || "");
};
/**
 * Joins values with comma and space.
 * @param {...*} Values The values to join.
 * @returns {string} Returns the joined string.
 */
tp.CommaText = function (...Values) {
    return Values.join(", ");
};
/**
 * Splits a string into lines.
 * @param {string} Value The string to split.
 * @returns {string[]} Returns the lines.
 */
tp.ToLines = function (Value) {
    if (tp.IsBlank(Value))
        return [];
    return String(Value).replace(/\r\n/g, "\n").replace(/\r/g, "\n").split("\n");
};
/**
 * Replaces line breaks with a separator.
 * @param {string} Value The string to operate on.
 * @param {string} Separator The separator that replaces line breaks.
 * @returns {string} Returns the result string.
 */
tp.ReplaceLineBreaks = function (Value, Separator) {
    if (tp.IsBlank(Value))
        return "";
    return String(Value).replace(/\r\n|\r|\n/g, tp.IsNil(Separator) ? "" : String(Separator));
};
/**
 * Replaces line breaks with <br /> tags.
 * @param {string} Value The string to operate on.
 * @returns {string} Returns the HTML string.
 */
tp.LineBreaksToHtml = function (Value) {
    return tp.ReplaceLineBreaks(Value, "<br />");
};
/**
 * Encodes a value as HTML text.
 * @param {*} Value The value to encode.
 * @returns {string} Returns the encoded text.
 */
tp.EncodeHtml = function (Value) {
    return tp.IsNil(Value) ? "" : String(Value)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");
};

// ● padding and generation
/**
 * Repeats a string.
 * @param {string} Value The string to repeat.
 * @param {number} Count The repeat count.
 * @returns {string} Returns the repeated string.
 */
tp.Repeat = function (Value, Count) {
    Value = tp.IsNil(Value) ? "" : String(Value);
    Count = Number.isFinite(Number(Count)) ? Math.trunc(Number(Count)) : 0;
    return Count > 0 ? Value.repeat(Count) : "";
};
/**
 * Pads a string from the left.
 * @param {string} Value The string to pad.
 * @param {string} PadText The pad text.
 * @param {number} TotalLength The desired total length.
 * @returns {string} Returns the padded string.
 */
tp.PadLeft = function (Value, PadText, TotalLength) {
    if (tp.IsNil(Value))
        return Value;
    Value = String(Value);
    PadText = tp.IsBlank(PadText) ? " " : String(PadText);
    TotalLength = Number.isFinite(Number(TotalLength)) ? Math.trunc(Number(TotalLength)) : 0;
    while (Value.length < TotalLength)
        Value = PadText + Value;
    return Value.length > TotalLength ? Value.slice(Value.length - TotalLength) : Value;
};
/**
 * Pads a string from the right.
 * @param {string} Value The string to pad.
 * @param {string} PadText The pad text.
 * @param {number} TotalLength The desired total length.
 * @returns {string} Returns the padded string.
 */
tp.PadRight = function (Value, PadText, TotalLength) {
    if (tp.IsNil(Value))
        return Value;
    Value = String(Value);
    PadText = tp.IsBlank(PadText) ? " " : String(PadText);
    TotalLength = Number.isFinite(Number(TotalLength)) ? Math.trunc(Number(TotalLength)) : 0;
    while (Value.length < TotalLength)
        Value += PadText;
    return Value.length > TotalLength ? Value.slice(0, TotalLength) : Value;
};
/**
 * Returns a new GUID string.
 * @see {@link https://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript|StackOverflow}
 * @param {boolean|null|undefined} UseBrackets True to wrap the GUID in brackets.
 * @returns {string} Returns a GUID string.
 */
tp.Guid = function (UseBrackets) {
    var Result;
    if (typeof crypto !== "undefined" && tp.IsFunction(crypto.randomUUID))
        Result = crypto.randomUUID().toUpperCase();
    else {
        Result = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (Char) {
            var Random = Math.random() * 16 | 0;
            var Value = Char === "x" ? Random : (Random & 0x3 | 0x8);
            return Value.toString(16).toUpperCase();
        });
    }
    return UseBrackets === true ? "{" + Result + "}" : Result;
};
/**
 * Creates a random string.
 * @param {number} Length The string length.
 * @param {string|null|undefined} CharSet The source character set.
 * @returns {string} Returns the random string.
 */
tp.GenerateRandomString = function (Length, CharSet) {
    var Result = [];
    var Index;
    var i;
    Length = Number.isFinite(Number(Length)) ? Math.trunc(Number(Length)) : 0;
    CharSet = tp.IsBlank(CharSet) ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : String(CharSet);
    for (i = 0; i < Length; i++) {
        Index = Math.floor(Math.random() * CharSet.length);
        Result.push(CharSet.charAt(Index));
    }
    return Result.join("");
};

// ● encoding
/**
 * Encodes a string as Base64.
 * @see {@link https://developer.mozilla.org/en-US/docs/Glossary/Base64|MDN Base64}
 * @param {string} Value The value to encode.
 * @returns {string} Returns the Base64 string.
 */
tp.ToBase64 = function (Value) {
    Value = tp.IsNil(Value) ? "" : String(Value);
    if (typeof btoa === "function")
        return btoa(unescape(encodeURIComponent(Value)));
    if (typeof Buffer !== "undefined")
        return Buffer.from(Value, "utf8").toString("base64");
    return "";
};
/**
 * Decodes a Base64 string.
 * @see {@link https://developer.mozilla.org/en-US/docs/Glossary/Base64|MDN Base64}
 * @param {string} Value The Base64 string.
 * @returns {string} Returns the decoded string.
 */
tp.FromBase64 = function (Value) {
    Value = tp.IsNil(Value) ? "" : String(Value);
    if (typeof atob === "function")
        return decodeURIComponent(escape(atob(Value)));
    if (typeof Buffer !== "undefined")
        return Buffer.from(Value, "base64").toString("utf8");
    return "";
};

// ● dynamic function
/**
 * Creates a function from source code.
 * @see {@link http://stackoverflow.com/questions/7650071/is-there-a-way-to-create-a-function-from-a-string-with-javascript|StackOverflow}
 * @param {string} Value The function source code.
 * @returns {Function|null} Returns the function or null.
 */
tp.CreateFunction = function (Value) {
    var FuncRegEx;
    var Match;
    var Args;
    if (!tp.IsString(Value))
        return null;
    FuncRegEx = /function *\(([^()]*)\)[ \n\t]*\{(.*)\}/gmi;
    Match = FuncRegEx.exec(Value.replace(/\n/g, " "));
    if (Match) {
        Args = Match[1].split(",");
        Args.push(Match[2]);
        return Function.apply(null, Args);
    }
    return null;
};

// ● string builder
/**
 * Constructs strings incrementally.
 */
tp.StringBuilder = class {
    // ● constructor
    /**
     * Creates a string builder.
     * @param {string|null|undefined} LineBreak The line break to use. Defaults to "\n".
     */
    constructor(LineBreak) {
        this.fData = "";
        this.fLB = LineBreak || "\n";
    }

    // ● properties
    /**
     * Gets the length of the internal string.
     * @returns {number} Returns the length of the internal string.
     */
    get Length() {
        return this.fData.length;
    }
    /**
     * Returns true when the internal string is empty.
     * @returns {boolean} Returns true when the internal string is empty.
     */
    get IsEmpty() {
        return this.fData.length === 0;
    }
    /**
     * Gets or sets the line break.
     * @returns {string} Returns the line break.
     */
    get LineBreak() {
        return this.fLB;
    }
    /**
     * Sets the line break.
     * @param {string} Value The line break.
     * @returns {void}
     */
    set LineBreak(Value) {
        this.fLB = Value || "\n";
    }

    // ● public
    /**
     * Clears the internal string.
     * @returns {void}
     */
    Clear() {
        this.fData = "";
    }
    /**
     * Appends a value.
     * @param {*} Value The value to append.
     * @returns {void}
     */
    Append(Value) {
        if (!tp.IsNil(Value))
            this.fData += String(Value);
    }
    /**
     * Appends a value and a line break.
     * @param {*} Value The optional value to append.
     * @returns {void}
     */
    AppendLine(Value) {
        if (!tp.IsNil(Value))
            this.fData += String(Value);
        this.fData += this.LineBreak;
    }
    /**
     * Inserts a value at an index.
     * @param {number} Index The insert index.
     * @param {*} Value The value to insert.
     * @returns {void}
     */
    Insert(Index, Value) {
        if (!tp.IsNil(Value))
            this.fData = tp.InsertText(String(Value), this.fData, Index);
    }
    /**
     * Replaces a value with another value.
     * @param {string} OldValue The string to replace.
     * @param {string} NewValue The replacement string.
     * @param {boolean|null|undefined} CaseInsensitive True for case-insensitive replacement.
     * @returns {void}
     */
    Replace(OldValue, NewValue, CaseInsensitive) {
        this.fData = tp.ReplaceAll(this.fData, OldValue, NewValue, CaseInsensitive);
    }
    /**
     * Returns the internal string.
     * @returns {string} Returns the internal string.
     */
    ToString() {
        return this.fData;
    }
    /**
     * Returns the internal string.
     * @returns {string} Returns the internal string.
     */
    toString() {
        return this.ToString();
    }
};
/**
 * The internal string.
 * @type {string}
 */
tp.StringBuilder.prototype.fData = "";
/**
 * The line break.
 * @type {string}
 */
tp.StringBuilder.prototype.fLB = "\n";
