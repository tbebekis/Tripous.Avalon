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
