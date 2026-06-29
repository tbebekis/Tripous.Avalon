// ● colors
/**
 * Static helper for color constants and color utility methods.
 * @type {object}
 */
tp.Colors = {
    AliceBlue: "#F0F8FF",
    AntiqueWhite: "#FAEBD7",
    Aqua: "#00FFFF",
    Aquamarine: "#7FFFD4",
    Azure: "#F0FFFF",
    Beige: "#F5F5DC",
    Bisque: "#FFE4C4",
    Black: "#000000",
    BlanchedAlmond: "#FFEBCD",
    Blue: "#0000FF",
    BlueViolet: "#8A2BE2",
    Brown: "#A52A2A",
    BurlyWood: "#DEB887",
    CadetBlue: "#5F9EA0",
    Chartreuse: "#7FFF00",
    Chocolate: "#D2691E",
    Coral: "#FF7F50",
    CornflowerBlue: "#6495ED",
    Cornsilk: "#FFF8DC",
    Crimson: "#DC143C",
    Cyan: "#00FFFF",
    DarkBlue: "#00008B",
    DarkCyan: "#008B8B",
    DarkGoldenRod: "#B8860B",
    DarkGray: "#A9A9A9",
    DarkGreen: "#006400",
    DarkKhaki: "#BDB76B",
    DarkMagenta: "#8B008B",
    DarkOliveGreen: "#556B2F",
    DarkOrange: "#FF8C00",
    DarkOrchid: "#9932CC",
    DarkRed: "#8B0000",
    DarkSalmon: "#E9967A",
    DarkSeaGreen: "#8FBC8F",
    DarkSlateBlue: "#483D8B",
    DarkSlateGray: "#2F4F4F",
    DarkTurquoise: "#00CED1",
    DarkViolet: "#9400D3",
    DeepPink: "#FF1493",
    DeepSkyBlue: "#00BFFF",
    DimGray: "#696969",
    DodgerBlue: "#1E90FF",
    FireBrick: "#B22222",
    FloralWhite: "#FFFAF0",
    ForestGreen: "#228B22",
    Fuchsia: "#FF00FF",
    Gainsboro: "#DCDCDC",
    GhostWhite: "#F8F8FF",
    Gold: "#FFD700",
    GoldenRod: "#DAA520",
    Gray: "#808080",
    Green: "#008000",
    GreenYellow: "#ADFF2F",
    HoneyDew: "#F0FFF0",
    HotPink: "#FF69B4",
    IndianRed: "#CD5C5C",
    Indigo: "#4B0082",
    Ivory: "#FFFFF0",
    Khaki: "#F0E68C",
    Lavender: "#E6E6FA",
    LavenderBlush: "#FFF0F5",
    LawnGreen: "#7CFC00",
    LemonChiffon: "#FFFACD",
    LightBlue: "#ADD8E6",
    LightCoral: "#F08080",
    LightCyan: "#E0FFFF",
    LightGoldenRodYellow: "#FAFAD2",
    LightGray: "#D3D3D3",
    LightGreen: "#90EE90",
    LightPink: "#FFB6C1",
    LightSalmon: "#FFA07A",
    LightSeaGreen: "#20B2AA",
    LightSkyBlue: "#87CEFA",
    LightSlateGray: "#778899",
    LightSteelBlue: "#B0C4DE",
    LightYellow: "#FFFFE0",
    Lime: "#00FF00",
    LimeGreen: "#32CD32",
    Linen: "#FAF0E6",
    Magenta: "#FF00FF",
    Maroon: "#800000",
    MediumAquaMarine: "#66CDAA",
    MediumBlue: "#0000CD",
    MediumOrchid: "#BA55D3",
    MediumPurple: "#9370DB",
    MediumSeaGreen: "#3CB371",
    MediumSlateBlue: "#7B68EE",
    MediumSpringGreen: "#00FA9A",
    MediumTurquoise: "#48D1CC",
    MediumVioletRed: "#C71585",
    MidnightBlue: "#191970",
    MintCream: "#F5FFFA",
    MistyRose: "#FFE4E1",
    Moccasin: "#FFE4B5",
    NavajoWhite: "#FFDEAD",
    Navy: "#000080",
    OldLace: "#FDF5E6",
    Olive: "#808000",
    OliveDrab: "#6B8E23",
    Orange: "#FFA500",
    OrangeRed: "#FF4500",
    Orchid: "#DA70D6",
    PaleGoldenRod: "#EEE8AA",
    PaleGreen: "#98FB98",
    PaleTurquoise: "#AFEEEE",
    PaleVioletRed: "#DB7093",
    PapayaWhip: "#FFEFD5",
    PeachPuff: "#FFDAB9",
    Peru: "#CD853F",
    Pink: "#FFC0CB",
    Plum: "#DDA0DD",
    PowderBlue: "#B0E0E6",
    Purple: "#800080",
    Red: "#FF0000",
    RosyBrown: "#BC8F8F",
    RoyalBlue: "#4169E1",
    SaddleBrown: "#8B4513",
    Salmon: "#FA8072",
    SandyBrown: "#F4A460",
    SeaGreen: "#2E8B57",
    SeaShell: "#FFF5EE",
    Sienna: "#A0522D",
    Silver: "#C0C0C0",
    SkyBlue: "#87CEEB",
    SlateBlue: "#6A5ACD",
    SlateGray: "#708090",
    Snow: "#FFFAFA",
    SpringGreen: "#00FF7F",
    SteelBlue: "#4682B4",
    Tan: "#D2B48C",
    Teal: "#008080",
    Thistle: "#D8BFD8",
    Tomato: "#FF6347",
    Turquoise: "#40E0D0",
    Violet: "#EE82EE",
    Wheat: "#F5DEB3",
    White: "#FFFFFF",
    WhiteSmoke: "#F5F5F5",
    Yellow: "#FFFF00",
    YellowGreen: "#9ACD32",
    /**
     * Returns the color list as objects suitable for combo boxes and list boxes.
     * @returns {{Text: string, Value: string}[]} Returns the color option list.
     */
    ToOptionList: function () {
        var Result = [];
        var Name;
        for (Name in tp.Colors) {
            if (Object.prototype.propertyIsEnumerable.call(tp.Colors, Name) && !tp.IsFunction(tp.Colors[Name]))
                Result.push({ Text: Name, Value: tp.Colors[Name] });
        }
        return Result;
    },
    /**
     * Shades a hex color by a percentage.
     * @param {string} Color A hex color with a leading #.
     * @param {number} Percent A number from -100 to 100. Negative values darken the color.
     * @returns {string} Returns the shaded color.
     */
    Shade: function (Color, Percent) {
        var Hex = tp.Colors.NormalizeHex(Color);
        var Amount = Math.round(2.55 * tp.StrToFloat(Percent, 0));
        var Value = parseInt(Hex.slice(1), 16);
        var R = (Value >> 16) + Amount;
        var G = (Value >> 8 & 0x00FF) + Amount;
        var B = (Value & 0x0000FF) + Amount;
        return tp.Colors.FromRgb(R, G, B);
    },
    /**
     * Shades a hex color by blending it toward black or white.
     * @param {string} Color A hex color with a leading #.
     * @param {number} Percent A number from -100 to 100. Negative values darken the color.
     * @returns {string} Returns the shaded color.
     */
    Shade2: function (Color, Percent) {
        var Ratio = tp.StrToFloat(Percent, 0) / 100;
        var Hex = tp.Colors.NormalizeHex(Color);
        var Value = parseInt(Hex.slice(1), 16);
        var Target = Ratio < 0 ? 0 : 255;
        var PositiveRatio = Ratio < 0 ? Ratio * -1 : Ratio;
        var R = Value >> 16;
        var G = Value >> 8 & 0x00FF;
        var B = Value & 0x0000FF;
        return tp.Colors.FromRgb(
            Math.round((Target - R) * PositiveRatio) + R,
            Math.round((Target - G) * PositiveRatio) + G,
            Math.round((Target - B) * PositiveRatio) + B
        );
    },
    /**
     * Returns a vertical gradient CSS value and optionally applies it to an element.
     * @param {string} Color A base color.
     * @param {HTMLElement|string|null|undefined} ElementOrSelector The optional element or selector.
     * @returns {string} Returns a CSS linear-gradient() value.
     */
    SetGradientStyle: function (Color, ElementOrSelector) {
        var BaseColor = tp.Colors.NormalizeHex(Color);
        var Stops = [22, 35, 42, 52];
        var Parts = [BaseColor + " 0%"];
        var Index;
        var Shade;
        var Element;
        for (Index = 0; Index < Stops.length; Index++) {
            Shade = tp.Colors.Shade(BaseColor, -(Index + 1) * 3);
            Parts.push(Shade + " " + Stops[Index] + "%");
        }
        var Result = "linear-gradient(to bottom, " + Parts.join(", ") + ")";
        Element = tp.IsString(ElementOrSelector) ? tp(ElementOrSelector) : ElementOrSelector;
        if (tp.IsHTMLElement(Element))
            Element.style.setProperty("background-image", Result);
        return Result;
    },
    /**
     * Normalizes a hex color to #RRGGBB.
     * @param {string} Color The color to normalize.
     * @returns {string} Returns a normalized color.
     */
    NormalizeHex: function (Color) {
        var Text = tp.IsString(Color) ? Color.trim() : "";
        if (Text.charAt(0) !== "#")
            Text = "#" + Text;
        if (/^#[0-9a-fA-F]{3}$/.test(Text))
            Text = "#" + Text.charAt(1) + Text.charAt(1) + Text.charAt(2) + Text.charAt(2) + Text.charAt(3) + Text.charAt(3);
        if (!/^#[0-9a-fA-F]{6}$/.test(Text))
            return "#000000";
        return Text.toUpperCase();
    },
    /**
     * Converts RGB component values to a hex color.
     * @param {number} Red The red component.
     * @param {number} Green The green component.
     * @param {number} Blue The blue component.
     * @returns {string} Returns a hex color.
     */
    FromRgb: function (Red, Green, Blue) {
        var R = tp.Colors.ClampColor(Red);
        var G = tp.Colors.ClampColor(Green);
        var B = tp.Colors.ClampColor(Blue);
        return "#" + (0x1000000 + R * 0x10000 + G * 0x100 + B).toString(16).slice(1).toUpperCase();
    },
    /**
     * Clamps a value to the 0..255 color component range.
     * @param {number} Value The value to clamp.
     * @returns {number} Returns the clamped value.
     */
    ClampColor: function (Value) {
        Value = Math.round(tp.StrToFloat(Value, 0));
        if (Value < 0)
            return 0;
        if (Value > 255)
            return 255;
        return Value;
    }
};
Object.freeze(tp.Colors);
