// ● units
/**
 * CSS unit suffix constants.
 * @see {@link https://developer.mozilla.org/en-US/docs/Learn/CSS/Building_blocks/Values_and_units|MDN CSS values and units}
 * @type {object}
 */
tp.UnitMap = {
    /**
     * Pixel unit.
     * @type {string}
     */
    pixel: "px",
    /**
     * Percent unit.
     * @type {string}
     */
    percent: "%",
    /**
     * Inch unit.
     * @type {string}
     */
    inch: "in",
    /**
     * Centimeter unit.
     * @type {string}
     */
    cm: "cm",
    /**
     * Millimeter unit.
     * @type {string}
     */
    mm: "mm",
    /**
     * Point unit.
     * @type {string}
     */
    point: "pt",
    /**
     * Pica unit.
     * @type {string}
     */
    pica: "pc",
    /**
     * Font-relative em unit.
     * @type {string}
     */
    em: "em",
    /**
     * Font-relative ex unit.
     * @type {string}
     */
    ex: "ex"
};
Object.freeze(tp.UnitMap);
/**
 * Extracts the unit suffix from a CSS value.
 * @param {string|number|null|undefined} Value The CSS value, e.g. 10px or 50%.
 * @returns {string} Returns the unit suffix, defaulting to px for numeric strings.
 */
tp.ExtractUnit = function (Value) {
    var Match;
    if (tp.IsNumber(Value))
        return tp.UnitMap.pixel;
    if (tp.IsString(Value)) {
        Match = Value.trim().match(/[a-z%]+$/i);
        return Match ? Match[0] : tp.UnitMap.pixel;
    }
    return "";
};
/**
 * Extracts the numeric part from a CSS value.
 * @param {string|number|null|undefined} Value The CSS value, e.g. 10px or 50%.
 * @returns {number} Returns the numeric part.
 */
tp.ExtractNumber = function (Value) {
    if (tp.IsNumber(Value))
        return Value;
    return tp.IsBlank(Value) ? 0 : Number(String(Value).replace(/[^\d.\-]/g, "")) || 0;
};
/**
 * Returns true when a value uses pixels.
 * @param {string|number|null|undefined} Value The value to check.
 * @returns {boolean} Returns true when the value uses pixels.
 */
tp.IsPixel = function (Value) {
    return tp.ExtractUnit(Value) === tp.UnitMap.pixel;
};
/**
 * Returns true when a value uses em.
 * @param {string|number|null|undefined} Value The value to check.
 * @returns {boolean} Returns true when the value uses em.
 */
tp.IsEm = function (Value) {
    return tp.ExtractUnit(Value) === tp.UnitMap.em;
};
/**
 * Returns true when a value uses percent.
 * @param {string|number|null|undefined} Value The value to check.
 * @returns {boolean} Returns true when the value uses percent.
 */
tp.IsPercent = function (Value) {
    return tp.ExtractUnit(Value) === tp.UnitMap.percent;
};
/**
 * Converts a value to a pixel CSS value.
 * @param {string|number|null|undefined} Value The value to convert.
 * @returns {string} Returns the pixel CSS value.
 */
tp.px = function (Value) {
    return tp.ExtractNumber(Value).toString() + tp.UnitMap.pixel;
};
