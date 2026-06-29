// ● cursors
/**
 * CSS cursor values.
 * See: https://developer.mozilla.org/en-US/docs/Web/CSS/cursor
 * @type {object}
 */
tp.Cursors = {
    Default: "default",
    Pointer: "pointer",
    Text: "text",
    VerticalText: "vertical-text",
    Help: "help",
    Move: "move",
    Wait: "wait",
    Progress: "progress",
    CrossHair: "crosshair",
    ResizeN: "n-resize",
    ResizeE: "e-resize",
    ResizeW: "w-resize",
    ResizeS: "s-resize",
    ResizeNE: "ne-resize",
    ResizeNW: "nw-resize",
    ResizeSE: "se-resize",
    ResizeSW: "sw-resize",
    ResizeCol: "col-resize",
    ResizeRow: "row-resize",
    AllScroll: "all-scroll",
    NotAllowed: "not-allowed",
    NoDrop: "no-drop",
    Auto: "auto",
    Inherit: "inherit"
};
Object.freeze(tp.Cursors);

// ● style
/**
 * Returns the computed style of an element.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/Window/getComputedStyle
 * @param {Element|string} Selector The target selector or element.
 * @returns {CSSStyleDeclaration|null} Returns the computed style or null.
 */
tp.GetComputedStyle = function (Selector) {
    var Element = tp(Selector);
    if (tp.IsElement(Element))
        return Element.ownerDocument.defaultView.getComputedStyle(Element, "");
    return null;
};
/**
 * Gets or sets a style property of an element.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/CSSStyleDeclaration
 * @param {Element|string} Selector The target selector or element.
 * @param {string} Name The style property name.
 * @param {*} Value The optional value to set.
 * @returns {*} Returns the property value when getting; otherwise, returns the assigned value.
 */
tp.StyleProp = function (Selector, Name, Value) {
    var Element = tp(Selector);
    var Style;
    if (!tp.IsElement(Element) || !tp.IsString(Name) || tp.IsBlank(Name))
        return null;
    if (arguments.length < 3) {
        Style = tp.GetComputedStyle(Element);
        if (!Style)
            return null;
        return Name in Style ? Style[Name] : Style.getPropertyValue(Name);
    }
    if (Name in Element.style)
        Element.style[Name] = Value;
    else
        Element.style.setProperty(Name, Value);
    return Value;
};
/**
 * Sets multiple inline style properties of an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {object|null|undefined} Values The style property values.
 * @returns {void}
 */
tp.SetStyle = function (Selector, Values) {
    var Element = tp(Selector);
    var Name;
    if (tp.IsElement(Element) && tp.IsObject(Values)) {
        for (Name in Values) {
            if (Object.prototype.propertyIsEnumerable.call(Values, Name) && !tp.IsFunction(Values[Name]))
                tp.StyleProp(Element, Name, Values[Name]);
        }
    }
};
/**
 * Gets or sets the inline CSS text of an element.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/CSSStyleDeclaration/cssText
 * @param {Element|string} Selector The target selector or element.
 * @param {string} Value The optional CSS text to set.
 * @returns {string} Returns the CSS text when getting; otherwise, returns the assigned CSS text.
 */
tp.StyleText = function (Selector, Value) {
    var Element = tp(Selector);
    if (!tp.IsElement(Element))
        return "";
    if (arguments.length < 2)
        return Element.style.cssText;
    Element.style.cssText = tp.IsNil(Value) ? "" : String(Value);
    return Element.style.cssText;
};
/**
 * Gets or sets the display style property of an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {string} Value The optional display value to set.
 * @returns {string} Returns the display value when getting; otherwise, returns the assigned display value.
 */
tp.Display = function (Selector, Value) {
    var Element = tp(Selector);
    if (!tp.IsElement(Element))
        return "";
    if (arguments.length < 2)
        return tp.GetComputedStyle(Element).display;
    Element.style.display = tp.IsNil(Value) ? "" : String(Value);
    return Element.style.display;
};
/**
 * Gets or sets element visibility through the CSS visibility property.
 * @param {Element|string} Selector The target selector or element.
 * @param {boolean} Value The optional visibility flag.
 * @returns {boolean} Returns true when the element is visible.
 */
tp.Visibility = function (Selector, Value) {
    var Element = tp(Selector);
    if (!tp.IsElement(Element))
        return false;
    if (arguments.length < 2)
        return tp.GetComputedStyle(Element).visibility === "visible";
    Element.style.visibility = Value === true ? "visible" : "hidden";
    return Value === true;
};
/**
 * Gets or sets element visibility through the CSS display property.
 * @param {Element|string} Selector The target selector or element.
 * @param {boolean} Value The optional visibility flag.
 * @returns {boolean} Returns true when the element is visible.
 */
tp.Visible = function (Selector, Value) {
    var Element = tp(Selector);
    if (!tp.IsElement(Element))
        return false;
    if (arguments.length < 2)
        return tp.GetComputedStyle(Element).display !== "none";
    Element.style.display = Value === true ? "" : "none";
    return Value === true;
};
/**
 * Returns the height of a text line based on the font size of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {number} Factor The optional multiplication factor. Defaults to 1.8.
 * @returns {number} Returns the calculated line height.
 */
tp.GetLineHeight = function (ElementOrSelector, Factor) {
    var Element = tp.Select(ElementOrSelector);
    var FontSize;
    var BodyFontSize;
    var Result;
    if (!tp.IsHTMLElement(Element))
        return 24;
    Factor = tp.IsNumber(Factor) && Factor > 0 ? Factor : 1.8;
    FontSize = tp.StyleProp(Element, "font-size");
    if (tp.IsEm(FontSize)) {
        BodyFontSize = tp.StyleProp(Element.ownerDocument.body, "font-size");
        if (!tp.IsPixel(BodyFontSize))
            tp.Throw("document.body font-size is not defined in pixels.");
        FontSize = tp.ExtractNumber(FontSize) * tp.ExtractNumber(BodyFontSize);
    } else {
        FontSize = tp.ExtractNumber(FontSize);
    }
    Result = Math.ceil(FontSize * Factor);
    return Result > 0 ? Result : 24;
};

// ● z-index
/**
 * Gets or sets the z-index of an element.
 * See: http://philipwalton.com/articles/what-no-one-told-you-about-z-index/
 * See: https://www.w3.org/TR/CSS2/visuren.html#z-index
 * @param {Element|string} Selector The target selector or element.
 * @param {string|number|null|undefined} Value The optional z-index value to set.
 * @returns {number} Returns the numeric z-index when getting; otherwise, returns the assigned numeric z-index.
 */
tp.ZIndex = function (Selector, Value) {
    var Element = tp(Selector);
    var Style;
    if (!tp.IsElement(Element))
        return 0;
    if (arguments.length < 2 || tp.IsNil(Value)) {
        Style = tp.GetComputedStyle(Element);
        return Style ? tp.StrToInt(Style.zIndex, 0) : 0;
    }
    Value = tp.ToInt(Value);
    Element.style.zIndex = String(Value);
    return Value;
};
/**
 * Returns the maximum computed z-index under a container element.
 * @param {Element|Document|string|null|undefined} Container The optional container. Defaults to document.
 * @returns {number} Returns the maximum z-index.
 */
tp.MaxZIndexOf = function (Container) {
    var Parent = tp.IsNil(Container) ? document : tp(Container);
    var Result = 0;
    var List;
    var Index;
    var Element;
    var Value;
    if (!tp.IsNodeSelector(Parent))
        Parent = document;
    List = Parent.querySelectorAll("*");
    for (Index = 0; Index < List.length; Index++) {
        Element = List[Index];
        Value = Element.ownerDocument.defaultView.getComputedStyle(Element, "").getPropertyValue("z-index");
        if (Value === "auto")
            Value = Index;
        Value = tp.ExtractNumber(Value);
        Result = Math.max(Result, Value);
    }
    return Result;
};
/**
 * Returns the minimum computed z-index under a container element.
 * @param {Element|Document|string|null|undefined} Container The optional container. Defaults to document.
 * @returns {number} Returns the minimum z-index.
 */
tp.MinZIndexOf = function (Container) {
    var Parent = tp.IsNil(Container) ? document : tp(Container);
    var Result = 0;
    var List;
    var Index;
    var Element;
    var Value;
    if (!tp.IsNodeSelector(Parent))
        Parent = document;
    List = Parent.querySelectorAll("*");
    for (Index = 0; Index < List.length; Index++) {
        Element = List[Index];
        Value = Element.ownerDocument.defaultView.getComputedStyle(Element, "").getPropertyValue("z-index");
        if (Value === "auto")
            Value = Index;
        Value = tp.ExtractNumber(Value);
        Result = Math.min(Result, Value);
    }
    return Result;
};
/**
 * Brings an element in front of all siblings and returns the assigned z-index.
 * @param {Element|string} Selector The target selector or element.
 * @returns {number} Returns the assigned z-index.
 */
tp.BringToFront = function (Selector) {
    var Element = tp(Selector);
    var Max;
    var Current;
    if (tp.IsElement(Element) && tp.IsElement(Element.parentNode)) {
        Max = tp.MaxZIndexOf(Element.parentNode);
        Current = tp.ZIndex(Element);
        if (Current < Max) {
            Max++;
            tp.ZIndex(Element, Max);
            return Max;
        }
        return Current;
    }
    return 0;
};
/**
 * Sends an element behind all siblings and returns the assigned z-index.
 * @param {Element|string} Selector The target selector or element.
 * @returns {number} Returns the assigned z-index.
 */
tp.SendToBack = function (Selector) {
    var Element = tp(Selector);
    var Min;
    if (tp.IsElement(Element) && tp.IsElement(Element.parentNode)) {
        Min = tp.MinZIndexOf(Element.parentNode) - 1;
        tp.ZIndex(Element, Min);
        return Min;
    }
    return 0;
};

// ● css classes
/**
 * Returns an array of CSS class names from a string or array.
 * @param {string|string[]|null|undefined} Names The class names.
 * @returns {string[]} Returns the class names.
 */
tp.GetCssClassList = function (Names) {
    var Result = [];
    var Add = function (Value) {
        if (tp.IsArray(Value)) {
            Value.forEach(Add);
        } else if (tp.IsString(Value)) {
            Value.split(/\s+/).forEach(function (Item) {
                if (!tp.IsBlank(Item))
                    Result.push(Item);
            });
        }
    };
    Add(Names);
    return Result;
};
/**
 * Returns true if an element has a specified CSS class.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/Element/classList
 * @param {Element|string} Selector The target selector or element.
 * @param {string} Name The CSS class name.
 * @returns {boolean} Returns true when the element has the class.
 */
tp.HasClass = function (Selector, Name) {
    var Element = tp(Selector);
    return tp.IsElement(Element) && !tp.IsBlank(Name) && Element.classList.contains(Name);
};
/**
 * Adds one or more CSS classes to an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {string|string[]} Names The CSS class name or names.
 * @returns {void}
 */
tp.AddClass = function (Selector, Names) {
    var Element = tp(Selector);
    if (tp.IsElement(Element)) {
        tp.GetCssClassList(Names).forEach(function (Name) {
            Element.classList.add(Name);
        });
    }
};
/**
 * Removes one or more CSS classes from an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {string|string[]} Names The CSS class name or names.
 * @returns {void}
 */
tp.RemoveClass = function (Selector, Names) {
    var Element = tp(Selector);
    if (tp.IsElement(Element)) {
        tp.GetCssClassList(Names).forEach(function (Name) {
            Element.classList.remove(Name);
        });
    }
};
/**
 * Toggles a CSS class on an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {string} Name The CSS class name.
 * @returns {boolean} Returns true when the class is present after toggling.
 */
tp.ToggleClass = function (Selector, Name) {
    var Element = tp(Selector);
    if (tp.IsElement(Element) && !tp.IsBlank(Name))
        return Element.classList.toggle(Name);
    return false;
};
/**
 * Adds one or more CSS classes to an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {...(string|string[])} Names The CSS class names.
 * @returns {void}
 */
tp.AddClasses = function (Selector) {
    var Args = Array.prototype.slice.call(arguments, 1);
    tp.AddClass(Selector, Args);
};
/**
 * Removes one or more CSS classes from an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {...(string|string[])} Names The CSS class names.
 * @returns {void}
 */
tp.RemoveClasses = function (Selector) {
    var Args = Array.prototype.slice.call(arguments, 1);
    tp.RemoveClass(Selector, Args);
};
/**
 * Clears all CSS classes from an element.
 * @param {Element|string} Selector The target selector or element.
 * @returns {void}
 */
tp.ClearClasses = function (Selector) {
    var Element = tp(Selector);
    if (tp.IsElement(Element))
        Element.className = "";
};
/**
 * Concatenates CSS class names into a single space-delimited string.
 * @param {...(string|string[])} Names The CSS class names.
 * @returns {string} Returns the concatenated CSS class names.
 */
tp.ConcatClasses = function () {
    return tp.GetCssClassList(Array.prototype.slice.call(arguments)).join(" ");
};
