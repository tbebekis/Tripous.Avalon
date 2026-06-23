// ● selection
/**
 * Selects and returns an element, if any; otherwise, null.
 * When only one argument is passed, the document is used as parent.
 * @param {string|Element|Document|null|undefined} ParentElementOrSelector The optional parent element or selector.
 * @param {string|Element|null|undefined} ElementOrSelector The child element or selector.
 * @returns {Element|Document|null} Returns the selected element, document, or null.
 */
tp.Select = function (ParentElementOrSelector, ElementOrSelector) {
    var Parent;
    var Element;
    if (arguments.length === 2) {
        Parent = tp.IsString(ParentElementOrSelector) ? document.querySelector(ParentElementOrSelector) : ParentElementOrSelector;
        Element = ElementOrSelector;
    } else {
        Parent = document;
        Element = ParentElementOrSelector;
    }
    if (tp.IsNodeSelector(Parent) && tp.IsString(Element))
        Element = Parent.querySelector(Element);
    return tp.IsElement(Element) || (typeof Document !== "undefined" && Element instanceof Document) ? Element : null;
};
/**
 * Selects and returns all elements matching a selector.
 * When only one argument is passed, the document is used as parent.
 * @param {string|Element|Document|null|undefined} ParentElementOrSelector The optional parent element or selector.
 * @param {string|null|undefined} Selectors The selector list.
 * @returns {Element[]} Returns the selected elements.
 */
tp.SelectAll = function (ParentElementOrSelector, Selectors) {
    var Parent;
    var SelectorText;
    if (arguments.length === 2) {
        Parent = tp.IsString(ParentElementOrSelector) ? document.querySelector(ParentElementOrSelector) : ParentElementOrSelector;
        SelectorText = Selectors;
    } else {
        Parent = document;
        SelectorText = ParentElementOrSelector;
    }
    if (tp.IsNodeSelector(Parent) && tp.IsString(SelectorText))
        return tp.ToArray(Parent.querySelectorAll(SelectorText));
    return [];
};
/**
 * Returns the closest ancestor that matches a selector.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/Element/closest|MDN Element.closest}
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {string} Selector The selector to match.
 * @returns {HTMLElement|null} Returns the closest HTMLElement or null.
 */
tp.Closest = function (ElementOrSelector, Selector) {
    var Element = tp.Select(ElementOrSelector);
    var Result;
    if (tp.IsElement(Element) && tp.IsString(Selector)) {
        Result = Element.closest(Selector);
        return tp.IsHTMLElement(Result) ? Result : null;
    }
    return null;
};
/**
 * Returns the first text node of an element, if any.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @returns {Text|null} Returns the first text node or null.
 */
tp.FindTextNode = function (ElementOrSelector) {
    var Element = tp.Select(ElementOrSelector);
    var List;
    var Index;
    if (tp.IsElement(Element) && Element.hasChildNodes()) {
        List = Element.childNodes;
        for (Index = 0; Index < List.length; Index++) {
            if (List[Index].nodeType === Node.TEXT_NODE)
                return List[Index];
        }
    }
    return null;
};
/**
 * Returns the first HTMLElement whose id ends with a specified string.
 * @param {string} IdEnding The id ending.
 * @param {Element|Document|null|undefined} ParentElement The optional parent element or document.
 * @returns {HTMLElement|null} Returns the found element or null.
 */
tp.FindElementWithIdEnding = function (IdEnding, ParentElement) {
    var Parent = ParentElement || document;
    var List;
    var Index;
    var Element;
    if (tp.IsBlank(IdEnding) || !tp.IsFunction(Parent.getElementsByTagName))
        return null;
    List = Parent.getElementsByTagName("*");
    for (Index = 0; Index < List.length; Index++) {
        Element = List[Index];
        if (tp.IsHTMLElement(Element) && tp.EndsWith(Element.id || "", IdEnding, false))
            return Element;
    }
    return null;
};

// ● traversal
/**
 * Returns the index of an element in its parent's children collection.
 * @param {Element|string} ParentElementOrSelector The parent element or selector.
 * @param {Element|string} ElementOrSelector The child element or selector.
 * @returns {number} Returns the child index or -1.
 */
tp.ChildIndex = function (ParentElementOrSelector, ElementOrSelector) {
    var Parent = tp.Select(ParentElementOrSelector);
    var Element = tp.Select(ElementOrSelector);
    var List;
    var Index;
    if (tp.IsElement(Parent) && tp.IsElement(Element)) {
        List = Parent.children;
        for (Index = 0; Index < List.length; Index++) {
            if (List[Index] === Element)
                return Index;
        }
    }
    return -1;
};
/**
 * Returns direct HTMLElement children of a specified element.
 * HTMLElement.children returns Element nodes and may include SVG elements, so this function filters to HTMLElement.
 * @param {Element|string} ElementOrSelector The parent element or selector.
 * @returns {HTMLElement[]} Returns direct HTMLElement children.
 */
tp.ChildHTMLElements = function (ElementOrSelector) {
    var Element = tp.Select(ElementOrSelector);
    var Result = [];
    var List;
    var Index;
    if (tp.IsHTMLElement(Element)) {
        List = Element.children;
        for (Index = 0; Index < List.length; Index++) {
            if (tp.IsHTMLElement(List[Index]))
                Result.push(List[Index]);
        }
    }
    return Result;
};
/**
 * Returns direct HTMLElement children of a specified element.
 * @param {Element|string} ElementOrSelector The parent element or selector.
 * @returns {HTMLElement[]} Returns direct HTMLElement children.
 */
tp.GetElementList = function (ElementOrSelector) {
    return tp.ChildHTMLElements(ElementOrSelector);
};
/**
 * Returns true when an element is directly or indirectly contained by a parent element.
 * @param {Element|Document} Parent The parent element or document.
 * @param {Element} Element The element to check.
 * @returns {boolean} Returns true when the element is contained.
 */
tp.ContainsElement = function (Parent, Element) {
    var Node;
    if (tp.IsValid(Parent) && tp.IsFunction(Parent.contains))
        return Parent.contains(Element);
    if (tp.IsValid(Element)) {
        Node = Element.parentNode;
        while (!tp.IsNil(Node)) {
            if (Node === Parent)
                return true;
            Node = Node.parentNode;
        }
    }
    return false;
};
/**
 * Returns true when an event target is an element or is contained by that element.
 * @param {HTMLElement} Element The container element.
 * @param {EventTarget} Target The event target.
 * @returns {boolean} Returns true when the target belongs to the element.
 */
tp.ContainsEventTarget = function (Element, Target) {
    return Element === Target || tp.IsHTMLElement(Target) && tp.ContainsElement(Element, Target);
};

// ● creation and removal
/**
 * Creates an element, appends it to a parent element, and returns it.
 * @param {Element|Document|string} ParentOrSelector The parent element, document, or selector.
 * @param {string|null|undefined} TagName The tag name. Defaults to div.
 * @returns {HTMLElement|null} Returns the created element or null.
 */
tp.el = function (ParentOrSelector, TagName) {
    var Parent = tp.Select(ParentOrSelector);
    var Result;
    TagName = tp.IsBlank(TagName) ? "div" : TagName;
    if (tp.IsElement(Parent) || (typeof Document !== "undefined" && Parent instanceof Document)) {
        Result = Parent.ownerDocument ? Parent.ownerDocument.createElement(TagName) : Parent.createElement(TagName);
        Parent.appendChild(Result);
        return Result;
    }
    return null;
};
/**
 * Creates and appends a div element.
 * @param {Element|Document|string} ParentOrSelector The parent element, document, or selector.
 * @returns {HTMLDivElement|null} Returns the created div or null.
 */
tp.Div = function (ParentOrSelector) {
    return tp.el(ParentOrSelector, "div");
};
/**
 * Creates and appends a span element.
 * @param {Element|Document|string} ParentOrSelector The parent element, document, or selector.
 * @returns {HTMLSpanElement|null} Returns the created span or null.
 */
tp.Span = function (ParentOrSelector) {
    return tp.el(ParentOrSelector, "span");
};
/**
 * Creates and appends a paragraph element.
 * @param {Element|Document|string} ParentOrSelector The parent element, document, or selector.
 * @param {string|null|undefined} Text The paragraph text.
 * @returns {HTMLParagraphElement|null} Returns the created paragraph or null.
 */
tp.Paragraph = function (ParentOrSelector, Text) {
    var Element = tp.el(ParentOrSelector, "p");
    if (Element && !tp.IsBlank(Text))
        Element.innerText = Text;
    return Element;
};
/**
 * Appends a line break element to a parent.
 * @param {Element|string} ParentOrSelector The parent element or selector.
 * @returns {HTMLBRElement|null} Returns the created br element or null.
 */
tp.Break = function (ParentOrSelector) {
    return tp.el(ParentOrSelector, "br");
};
/**
 * Removes an HTMLElement from the DOM.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @returns {void}
 */
tp.Remove = function (ElementOrSelector) {
    var Element = tp.Select(ElementOrSelector);
    if (tp.IsHTMLElement(Element) && Element.parentNode)
        Element.parentNode.removeChild(Element);
};
/**
 * Removes all child nodes from a parent element.
 * @param {Element|string} ParentOrSelector The parent element or selector.
 * @returns {void}
 */
tp.RemoveChildren = function (ParentOrSelector) {
    var Parent = tp.Select(ParentOrSelector);
    if (tp.IsElement(Parent)) {
        while (Parent.firstChild)
            Parent.removeChild(Parent.lastChild);
    }
};
/**
 * Appends an element, node, or HTML markup to a parent element.
 * @param {Element|Document|string} ParentOrSelector The parent element, document, or selector.
 * @param {Node|string} ElementOrHtml The node or HTML markup.
 * @returns {Node|null} Returns the appended node or null.
 */
tp.Append = function (ParentOrSelector, ElementOrHtml) {
    var Parent = tp.Select(ParentOrSelector);
    if (tp.IsElement(Parent) || (typeof Document !== "undefined" && Parent instanceof Document)) {
        if (tp.IsNode(ElementOrHtml))
            return Parent.appendChild(ElementOrHtml);
        if (tp.IsString(ElementOrHtml) && !tp.IsBlank(ElementOrHtml)) {
            Parent.insertAdjacentHTML("beforeend", ElementOrHtml);
            return Parent.childNodes[Parent.childNodes.length - 1] || null;
        }
    }
    return null;
};
/**
 * Prepends an element, node, or HTML markup to a parent element.
 * @param {Element|string} ParentOrSelector The parent element or selector.
 * @param {Node|string} ElementOrHtml The node or HTML markup.
 * @returns {Node|null} Returns the prepended node or null.
 */
tp.Prepend = function (ParentOrSelector, ElementOrHtml) {
    var Parent = tp.Select(ParentOrSelector);
    if (tp.IsElement(Parent)) {
        if (tp.IsNode(ElementOrHtml)) {
            if (Parent.childNodes.length === 0)
                return Parent.appendChild(ElementOrHtml);
            return Parent.insertBefore(ElementOrHtml, Parent.childNodes[0]);
        }
        if (tp.IsString(ElementOrHtml) && !tp.IsBlank(ElementOrHtml)) {
            Parent.insertAdjacentHTML("afterbegin", ElementOrHtml);
            return Parent.childNodes[0] || null;
        }
    }
    return null;
};
/**
 * Creates an element of a specified tag name and appends it to a parent.
 * @param {Element|Document|string} ParentOrSelector The parent element, document, or selector.
 * @param {string} TagName The tag name.
 * @returns {Element|null} Returns the created element or null.
 */
tp.AppendElement = function (ParentOrSelector, TagName) {
    return tp.el(ParentOrSelector, TagName);
};
/**
 * Creates an element and inserts it at a specified child element index.
 * @param {Element|string} ParentOrSelector The parent element or selector.
 * @param {number} Index The child element index.
 * @param {string} TagName The tag name.
 * @returns {Element|null} Returns the created element or null.
 */
tp.InsertElement = function (ParentOrSelector, Index, TagName) {
    var Parent = tp.Select(ParentOrSelector);
    var Result;
    var List;
    if (tp.IsElement(Parent) && tp.IsString(TagName)) {
        Result = Parent.ownerDocument.createElement(TagName);
        List = tp.ChildHTMLElements(Parent);
        if (List.length === 0 || Index >= List.length)
            Parent.appendChild(Result);
        else
            Parent.insertBefore(Result, List[Index]);
        return Result;
    }
    return null;
};
/**
 * Appends a node to a parent node.
 * @param {Element|Document|string} ParentOrSelector The parent element, document, or selector.
 * @param {Node} Node The node to append.
 * @returns {void}
 */
tp.AppendNode = function (ParentOrSelector, Node) {
    var Parent = tp.Select(ParentOrSelector);
    if (tp.IsNode(Parent) && tp.IsNode(Node))
        Parent.appendChild(Node);
};
/**
 * Inserts a node at a specified child node index.
 * @param {Element|string} ParentOrSelector The parent element or selector.
 * @param {number} Index The child node index.
 * @param {Node} Node The node to insert.
 * @returns {void}
 */
tp.InsertNode = function (ParentOrSelector, Index, Node) {
    var Parent = tp.Select(ParentOrSelector);
    var List;
    if (tp.IsNode(Parent) && tp.IsNode(Node)) {
        List = Parent.childNodes;
        if (List.length === 0 || Index >= List.length)
            Parent.appendChild(Node);
        else
            Parent.insertBefore(Node, List[Index]);
    }
};

// ● attributes and data
/**
 * Sets multiple attributes of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {object} Values The attribute values.
 * @returns {void}
 */
tp.SetAttributes = function (ElementOrSelector, Values) {
    var Element = tp.Select(ElementOrSelector);
    var Name;
    if (tp.IsHTMLElement(Element) && tp.IsPlainObject(Values)) {
        for (Name in Values) {
            if (Object.prototype.propertyIsEnumerable.call(Values, Name) && !tp.IsFunction(Values[Name]))
                tp.Attribute(Element, Name, Values[Name]);
        }
    }
};
/**
 * Gets or sets an attribute of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {string} Name The attribute name.
 * @param {*} Value The optional value to set.
 * @returns {*} Returns the attribute value when getting; otherwise, returns the assigned value.
 */
tp.Attribute = function (ElementOrSelector, Name, Value) {
    var Element = tp.Select(ElementOrSelector);
    if (!tp.IsHTMLElement(Element) || !tp.IsString(Name))
        return null;
    if (arguments.length < 3)
        return Element.getAttribute(Name);
    if (Name in Element)
        Element[Name] = Value;
    else
        Element.setAttribute(Name, Value);
    return Value;
};
/**
 * Removes an attribute from an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {string} Name The attribute name.
 * @returns {void}
 */
tp.RemoveAttribute = function (ElementOrSelector, Name) {
    var Element = tp.Select(ElementOrSelector);
    if (tp.IsHTMLElement(Element) && Element.hasAttribute(Name))
        Element.removeAttribute(Name);
};
/**
 * Returns true when an element has an attribute.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {string} Name The attribute name.
 * @returns {boolean} Returns true when the element has the attribute.
 */
tp.HasAttribute = function (ElementOrSelector, Name) {
    var Element = tp.Select(ElementOrSelector);
    return tp.IsHTMLElement(Element) && Element.hasAttribute(Name);
};
/**
 * Gets or sets data-* attributes.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {string|object} NameOrValues The data name or an object with data values.
 * @param {*} Value The optional value to set.
 * @returns {string} Returns the data value when getting; otherwise, returns an empty string.
 */
tp.Data = function (ElementOrSelector, NameOrValues, Value) {
    var Element = tp.Select(ElementOrSelector);
    var Name;
    if (!tp.IsHTMLElement(Element))
        return "";
    if (arguments.length < 3 && tp.IsString(NameOrValues))
        return Element.getAttribute("data-" + NameOrValues);
    if (tp.IsString(NameOrValues)) {
        Element.setAttribute("data-" + NameOrValues, Value);
    } else if (tp.IsPlainObject(NameOrValues)) {
        for (Name in NameOrValues) {
            if (Object.prototype.propertyIsEnumerable.call(NameOrValues, Name) && !tp.IsFunction(NameOrValues[Name]))
                Element.setAttribute("data-" + Name, NameOrValues[Name]);
        }
    }
    return "";
};

// ● values
/**
 * Indicates which element member stores its value.
 * @type {object}
 */
tp.ElementValueType = {
    Unknown: 0,
    Value: 1,
    Checked: 2,
    InnerHtml: 4,
    TextContent: 8,
    SelectedIndex: 0x10
};
Object.freeze(tp.ElementValueType);
/**
 * Returns the value type of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @returns {number} Returns a tp.ElementValueType value.
 */
tp.GetElementValueType = function (ElementOrSelector) {
    var Element = tp.Select(ElementOrSelector);
    var NodeName;
    if (!Element)
        return tp.ElementValueType.Unknown;
    NodeName = Element.nodeName.toLowerCase();
    if (NodeName === "input")
        return Element.type === "checkbox" || Element.type === "radio" ? tp.ElementValueType.Checked : tp.ElementValueType.Value;
    if (NodeName === "textarea")
        return tp.ElementValueType.Value;
    if (NodeName === "button")
        return tp.ElementValueType.InnerHtml;
    if (NodeName === "select")
        return tp.ElementValueType.SelectedIndex;
    if ("textContent" in Element)
        return tp.ElementValueType.TextContent;
    return tp.ElementValueType.InnerHtml;
};
/**
 * Gets or sets the value of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {*} Value The optional value to set.
 * @returns {*} Returns the element value when getting; otherwise, returns the assigned value.
 */
tp.val = function (ElementOrSelector, Value) {
    var Element = tp.Select(ElementOrSelector);
    var ValueType = tp.GetElementValueType(Element);
    var Index;
    if (!Element || ValueType === tp.ElementValueType.Unknown)
        return null;
    if (arguments.length < 2) {
        switch (ValueType) {
            case tp.ElementValueType.Value: return Element.value;
            case tp.ElementValueType.Checked: return Element.checked;
            case tp.ElementValueType.InnerHtml: return Element.innerHTML;
            case tp.ElementValueType.TextContent: return Element.textContent;
            case tp.ElementValueType.SelectedIndex: return tp.InRange(Element.options, Element.selectedIndex) ? Element.options[Element.selectedIndex].value : null;
        }
    } else {
        switch (ValueType) {
            case tp.ElementValueType.Value:
                Element.value = Value;
                break;
            case tp.ElementValueType.Checked:
                Element.checked = Value === true || Value === "true" || Value === 1;
                break;
            case tp.ElementValueType.InnerHtml:
                Element.innerHTML = tp.IsNil(Value) ? "" : String(Value);
                break;
            case tp.ElementValueType.TextContent:
                Element.textContent = tp.IsNil(Value) ? "" : String(Value);
                break;
            case tp.ElementValueType.SelectedIndex:
                for (Index = 0; Index < Element.options.length; Index++) {
                    if (Element.options[Index].value === Value) {
                        Element.selectedIndex = Index;
                        return Value;
                    }
                }
                if (tp.IsNumber(Value) && tp.InRange(Element.options, Value))
                    Element.selectedIndex = Value;
                break;
        }
        return Value;
    }
    return null;
};
/**
 * Clears the value of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @returns {void}
 */
tp.ClearValue = function (ElementOrSelector) {
    var Element = tp.Select(ElementOrSelector);
    var ValueType = tp.GetElementValueType(Element);
    if (!Element || ValueType === tp.ElementValueType.Unknown)
        return;
    switch (ValueType) {
        case tp.ElementValueType.Value:
            Element.value = "";
            break;
        case tp.ElementValueType.Checked:
            Element.checked = false;
            break;
        case tp.ElementValueType.InnerHtml:
            Element.innerHTML = "";
            break;
        case tp.ElementValueType.TextContent:
            Element.textContent = "";
            break;
        case tp.ElementValueType.SelectedIndex:
            Element.selectedIndex = -1;
            break;
    }
};
/**
 * Gets or sets the inner HTML of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {string} Value The optional HTML value to set.
 * @returns {string} Returns inner HTML when getting; otherwise, returns the assigned HTML.
 */
tp.Html = function (ElementOrSelector, Value) {
    var Element = tp.Select(ElementOrSelector);
    if (!tp.IsElement(Element))
        return "";
    if (arguments.length < 2)
        return Element.innerHTML;
    Element.innerHTML = tp.IsNil(Value) ? "" : String(Value);
    return Element.innerHTML;
};
/**
 * Gets or sets the text content of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {*} Value The optional value to set.
 * @returns {string} Returns text when getting; otherwise, returns the assigned text.
 */
tp.Text = function (ElementOrSelector, Value) {
    var Element = tp.Select(ElementOrSelector);
    if (!Element)
        return "";
    if (arguments.length < 2)
        return Element.textContent;
    Element.textContent = tp.IsNil(Value) ? "" : String(Value);
    return Element.textContent;
};
/**
 * Gets or sets the value of an element.
 * @param {Element|string} ElementOrSelector The element or selector.
 * @param {*} Value The optional value to set.
 * @returns {*} Returns the element value when getting; otherwise, returns the assigned value.
 */
tp.Value = function (ElementOrSelector, Value) {
    return arguments.length < 2 ? tp.val(ElementOrSelector) : tp.val(ElementOrSelector, Value);
};
/**
 * Returns an HTMLElement based on an element, selector, or HTML text. Throws on failure.
 * Used when content must be extracted as an element, for example in dialogs.
 * @param {HTMLElement|string} ElementOrSelectorOrHtmlText The element, selector, or HTML text.
 * @returns {HTMLElement} Returns the extracted HTMLElement.
 */
tp.HtmlToElement = function (ElementOrSelectorOrHtmlText) {
    var Result = null;
    var Template;
    if (tp.IsHTMLElement(ElementOrSelectorOrHtmlText))
        Result = ElementOrSelectorOrHtmlText;
    if (Result === null && !tp.IsBlankString(ElementOrSelectorOrHtmlText)) {
        if (tp.IsHtml(ElementOrSelectorOrHtmlText)) {
            Template = document.createElement("template");
            Template.innerHTML = ElementOrSelectorOrHtmlText.trim();
            Result = Template.content.firstElementChild;
        } else {
            Result = tp.Select(ElementOrSelectorOrHtmlText);
        }
    }
    if (!tp.IsHTMLElement(Result))
        tp.Throw("Can not extract the content element.");
    return Result;
};

// ● events
/**
 * Adds an event listener to an element.
 * @param {Element|Document|Window|string} ElementOrSelector The target selector, element, document, or window.
 * @param {string} EventName The event name.
 * @param {Function} Handler The event handler.
 * @param {object|boolean} Options The optional event listener options.
 * @returns {void}
 */
tp.On = function (ElementOrSelector, EventName, Handler, Options) {
    var Element = tp(ElementOrSelector) || tp.Select(ElementOrSelector);
    if (Element && tp.IsFunction(Element.addEventListener))
        Element.addEventListener(EventName, Handler, Options);
};
/**
 * Removes an event listener from an element.
 * @param {Element|Document|Window|string} ElementOrSelector The target selector, element, document, or window.
 * @param {string} EventName The event name.
 * @param {Function} Handler The event handler.
 * @param {object|boolean} Options The optional event listener options.
 * @returns {void}
 */
tp.Off = function (ElementOrSelector, EventName, Handler, Options) {
    var Element = tp(ElementOrSelector) || tp.Select(ElementOrSelector);
    if (Element && tp.IsFunction(Element.removeEventListener))
        Element.removeEventListener(EventName, Handler, Options);
};
