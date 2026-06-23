// ● create params
/**
 * Represents initialization options passed to a tp.Component constructor.
 */
tp.CreateParams = class {
    // ● constructor
    /**
     * Creates a new create params instance.
     * @param {object|null|undefined} Source The optional source object.
     */
    constructor(Source) {
        if (tp.IsObject(Source))
            tp.Assign(this, Source);
    }
};

// ● prototype
/**
 * Gets or sets the component handle.
 * @type {HTMLElement|string|null}
 */
tp.CreateParams.prototype.Handle = null;
/**
 * Gets or sets the component parent.
 * @type {HTMLElement|tp.Component|string|null}
 */
tp.CreateParams.prototype.Parent = null;
/**
 * Gets or sets the component id.
 * @type {string}
 */
tp.CreateParams.prototype.Id = "";
/**
 * Gets or sets the component name.
 * @type {string}
 */
tp.CreateParams.prototype.Name = "";
/**
 * Gets or sets the component HTML.
 * @type {string}
 */
tp.CreateParams.prototype.Html = "";
/**
 * Gets or sets the component text.
 * @type {string}
 */
tp.CreateParams.prototype.Text = "";
/**
 * Gets or sets the component tooltip.
 * @type {string}
 */
tp.CreateParams.prototype.ToolTip = "";
/**
 * Gets or sets the component CSS classes.
 * @type {string}
 */
tp.CreateParams.prototype.CssClasses = "";
/**
 * Gets or sets the component inline CSS text.
 * @type {string}
 */
tp.CreateParams.prototype.CssText = "";
/**
 * Gets or sets a user-defined value.
 * @type {*}
 */
tp.CreateParams.prototype.Tag = null;

// ● component
/**
 * Represents an HTML element wrapper without data binding.
 */
tp.Component = class extends tp.Object {
    // ● constructor
    /**
     * Creates a new component.
     * @param {tp.CreateParams|object} CreateParams The component creation parameters. A valid Handle is required.
     */
    constructor(CreateParams) {
        super();
        this.CreateParams = CreateParams instanceof tp.CreateParams ? CreateParams : new tp.CreateParams(CreateParams);
        this.CreateHandle(this.CreateParams.Handle);
        this.ApplyCreateParams(this.CreateParams);
    }

    // ● protected
    /**
     * Resolves a value to an HTMLElement.
     * @param {HTMLElement|string|null|undefined} Value The value to resolve.
     * @returns {HTMLElement|null} Returns the resolved element or null.
     */
    ResolveHandle(Value) {
        var Element = tp(Value);
        return Element instanceof HTMLElement ? Element : null;
    }
    /**
     * Creates the component handle.
     * @param {HTMLElement|string|null|undefined} Handle The handle or selector.
     * @returns {void}
     */
    CreateHandle(Handle) {
        var Element;
        if (this.fHandle instanceof HTMLElement)
            tp.Throw("Component handle is already assigned.");
        Element = this.ResolveHandle(Handle);
        if (!(Element instanceof HTMLElement))
            tp.Throw("tp.Component requires a valid HTMLElement handle.");
        this.fHandle = Element;
        this.fDocument = Element.ownerDocument;
        tp.Component.SetComponent(Element, this);
        this.OnHandleCreated();
    }
    /**
     * Applies explicit create params to this component.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        if (!Params)
            return;
        if (!tp.IsBlank(Params.Id))
            this.Id = Params.Id;
        if (!tp.IsBlank(Params.Name))
            this.Name = Params.Name;
        if (!tp.IsBlank(Params.Html))
            this.Html = Params.Html;
        if (!tp.IsBlank(Params.Text))
            this.Text = Params.Text;
        if (!tp.IsBlank(Params.ToolTip))
            this.ToolTip = Params.ToolTip;
        if (!tp.IsBlank(Params.CssClasses))
            this.CssClasses = Params.CssClasses;
        if (!tp.IsBlank(Params.CssText))
            this.CssText = Params.CssText;
        if (!tp.IsNil(Params.Tag))
            this.Tag = Params.Tag;
        if (!tp.IsNil(Params.Parent))
            this.Parent = Params.Parent;
    }
    /**
     * Resolves a parent value to an HTMLElement.
     * @param {HTMLElement|tp.Component|string|null|undefined} Parent The parent to resolve.
     * @returns {HTMLElement|null} Returns the parent element or null.
     */
    ResolveParent(Parent) {
        if (Parent instanceof tp.Component)
            return Parent.Handle;
        return this.ResolveHandle(Parent);
    }
    /**
     * Returns true when a tag name is a standard component node type.
     * @param {string} TagName The tag name to check.
     * @returns {boolean} Returns true when the tag name is standard.
     */
    IsStandardNodeType(TagName) {
        return tp.IsString(TagName) && tp.Component.StandardNodeTypes.indexOf(TagName.toLowerCase()) !== -1;
    }
    /**
     * Resolves an element child value.
     * @param {HTMLElement|string|null|undefined} Child The child to resolve.
     * @returns {HTMLElement|null} Returns the child element or null.
     */
    ResolveElementChild(Child) {
        if (Child instanceof HTMLElement)
            return Child;
        if (this.IsStandardNodeType(Child))
            return this.Document.createElement(Child.toLowerCase());
        return null;
    }
    /**
     * Resolves a child value to an existing child element.
     * @param {HTMLElement|tp.Component|string|null|undefined} Child The child to resolve.
     * @returns {HTMLElement|null} Returns the child element or null.
     */
    ResolveExistingChild(Child) {
        if (Child instanceof tp.Component)
            return Child.Handle;
        return this.ResolveHandle(Child);
    }
    /**
     * Destroys the component handle and releases resources.
     * @returns {void}
     */
    DoDispose() {
        var Element = this.fHandle;
        if (Element instanceof HTMLElement) {
            if (tp.Component.GetComponent(Element) === this)
                tp.Component.SetComponent(Element, null);
            if (Element.parentNode)
                Element.parentNode.removeChild(Element);
        }
        this.fHandle = null;
        this.fIsDisposed = true;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
    }
    /**
     * Event trigger called before disposal.
     * @returns {void}
     */
    OnDisposing() {
        this.Trigger("Disposing", {});
    }
    /**
     * Event trigger called after disposal.
     * @returns {void}
     */
    OnDisposed() {
        this.Trigger("Disposed", {});
    }
    /**
     * Event trigger called when the parent changes.
     * @returns {void}
     */
    OnParentChanged() {
        this.Trigger("ParentChanged", {});
    }
    /**
     * Event trigger called when Enabled changes.
     * @returns {void}
     */
    OnEnabledChanged() {
        this.Trigger("EnabledChanged", {});
    }
    /**
     * Event trigger called when Visible changes.
     * @returns {void}
     */
    OnVisibleChanged() {
        this.Trigger("VisibleChanged", {});
    }

    // ● properties
    /**
     * Gets the component handle.
     * @returns {HTMLElement|null} Returns the component handle.
     */
    get Handle() {
        return this.fHandle;
    }
    /**
     * Returns true when this component has a handle.
     * @returns {boolean} Returns true when this component has a handle.
     */
    get HasHandle() {
        return this.Handle instanceof HTMLElement;
    }
    /**
     * Gets the document this component belongs to.
     * @returns {Document} Returns the owner document.
     */
    get Document() {
        return this.fDocument || document;
    }
    /**
     * Gets the component node type.
     * @returns {string} Returns the node type.
     */
    get NodeType() {
        return this.HasHandle ? this.Handle.nodeName : "";
    }
    /**
     * Gets or sets the component id.
     * @returns {string} Returns the id.
     */
    get Id() {
        return this.HasHandle ? this.Handle.id || "" : "";
    }
    /**
     * Sets the component id.
     * @param {string} Value The id to set.
     * @returns {void}
     */
    set Id(Value) {
        if (this.HasHandle && tp.IsString(Value))
            this.Handle.id = Value;
    }
    /**
     * Gets or sets the component name.
     * @returns {string} Returns the name.
     */
    get Name() {
        return this.HasHandle && "name" in this.Handle ? this.Handle.name || "" : "";
    }
    /**
     * Sets the component name.
     * @param {string} Value The name to set.
     * @returns {void}
     */
    set Name(Value) {
        if (this.HasHandle && tp.IsString(Value) && "name" in this.Handle)
            this.Handle.name = Value;
    }
    /**
     * Gets or sets the parent component.
     * @returns {tp.Component|null} Returns the parent component or null.
     */
    get Parent() {
        return this.ParentHandle instanceof HTMLElement ? tp.Component.GetComponent(this.ParentHandle) : null;
    }
    /**
     * Sets the parent component or element.
     * @param {tp.Component|HTMLElement|string|null|undefined} Value The parent value.
     * @returns {void}
     */
    set Parent(Value) {
        if (tp.IsNil(Value))
            this.RemoveFromDom();
        else
            this.AppendTo(Value);
    }
    /**
     * Gets or sets the parent element.
     * @returns {HTMLElement|null} Returns the parent element.
     */
    get ParentHandle() {
        return this.HasHandle && this.Handle.parentNode instanceof HTMLElement ? this.Handle.parentNode : null;
    }
    /**
     * Sets the parent element.
     * @param {HTMLElement|string|null|undefined} Value The parent element or selector.
     * @returns {void}
     */
    set ParentHandle(Value) {
        this.Parent = Value;
    }
    /**
     * Gets the number of direct HTMLElement children.
     * @returns {number} Returns the child count.
     */
    get Count() {
        return this.GetElementList().length;
    }
    /**
     * Gets or sets the inner HTML.
     * @returns {string} Returns the inner HTML.
     */
    get Html() {
        return this.HasHandle ? this.Handle.innerHTML : "";
    }
    /**
     * Sets the inner HTML.
     * @param {string} Value The HTML to set.
     * @returns {void}
     */
    set Html(Value) {
        if (this.HasHandle && tp.IsString(Value))
            this.Handle.innerHTML = Value;
    }
    /**
     * Gets or sets text or value depending on the handle.
     * @returns {string} Returns the text.
     */
    get Text() {
        if (!this.HasHandle)
            return "";
        return "value" in this.Handle ? this.Handle.value : this.Handle.textContent;
    }
    /**
     * Sets text or value depending on the handle.
     * @param {*} Value The value to set.
     * @returns {void}
     */
    set Text(Value) {
        if (!this.HasHandle)
            return;
        Value = tp.IsNil(Value) ? "" : String(Value);
        if ("value" in this.Handle)
            this.Handle.value = Value;
        else
            this.Handle.textContent = Value;
    }
    /**
     * Gets or sets the tooltip.
     * @returns {string} Returns the tooltip.
     */
    get ToolTip() {
        return this.HasHandle ? this.Handle.title || "" : "";
    }
    /**
     * Sets the tooltip.
     * @param {string} Value The tooltip to set.
     * @returns {void}
     */
    set ToolTip(Value) {
        if (this.HasHandle && tp.IsString(Value))
            this.Handle.title = Value;
    }
    /**
     * Gets or sets the CSS position.
     * @returns {string} Returns the CSS position.
     */
    get Position() {
        return this.HasHandle ? this.Handle.style.position || "" : "";
    }
    /**
     * Sets the CSS position.
     * @param {string} Value The CSS position to set.
     * @returns {void}
     */
    set Position(Value) {
        if (this.HasHandle && tp.IsString(Value))
            this.Handle.style.position = Value;
    }
    /**
     * Gets or sets a value indicating whether this component is enabled.
     * @returns {boolean} Returns true when enabled.
     */
    get Enabled() {
        return this.fEnabled === true;
    }
    /**
     * Sets a value indicating whether this component is enabled.
     * @param {boolean} Value True to enable; false to disable.
     * @returns {void}
     */
    set Enabled(Value) {
        var OldValue = this.Enabled;
        this.fEnabled = Value === true;
        if (this.HasHandle && "disabled" in this.Handle)
            this.Handle.disabled = !this.fEnabled;
        if (OldValue !== this.Enabled)
            this.OnEnabledChanged();
    }
    /**
     * Gets or sets a value indicating whether this component is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        return this.HasHandle ? this.Handle.style.display !== "none" : false;
    }
    /**
     * Sets a value indicating whether this component is visible.
     * @param {boolean} Value True to show; false to hide.
     * @returns {void}
     */
    set Visible(Value) {
        var OldValue = this.Visible;
        if (this.HasHandle)
            this.Handle.style.display = Value === true ? "" : "none";
        if (OldValue !== this.Visible)
            this.OnVisibleChanged();
    }
    /**
     * Gets or sets the CSS visibility value.
     * @returns {string} Returns the CSS visibility.
     */
    get Visibility() {
        return this.HasHandle ? this.Handle.style.visibility || "" : "";
    }
    /**
     * Sets the CSS visibility value.
     * @param {string} Value The visibility value.
     * @returns {void}
     */
    set Visibility(Value) {
        if (this.HasHandle && tp.IsString(Value))
            this.Handle.style.visibility = Value;
    }
    /**
     * Gets or sets the CSS class names.
     * @returns {string} Returns the CSS class names.
     */
    get CssClasses() {
        return this.HasHandle ? this.Handle.className || "" : "";
    }
    /**
     * Sets the CSS class names.
     * @param {string} Value The CSS class names.
     * @returns {void}
     */
    set CssClasses(Value) {
        if (this.HasHandle && tp.IsString(Value))
            this.Handle.className = Value;
    }
    /**
     * Gets or sets the inline CSS text.
     * @returns {string} Returns the inline CSS text.
     */
    get CssText() {
        return this.HasHandle ? this.Handle.style.cssText || "" : "";
    }
    /**
     * Sets the inline CSS text.
     * @param {string} Value The inline CSS text.
     * @returns {void}
     */
    set CssText(Value) {
        if (this.HasHandle && tp.IsString(Value))
            this.Handle.style.cssText = Value;
    }
    /**
     * Gets the CSS style declaration.
     * @returns {CSSStyleDeclaration|null} Returns the style declaration.
     */
    get Style() {
        return this.HasHandle ? this.Handle.style : null;
    }
    /**
     * Gets a value indicating whether this component is disposed.
     * @returns {boolean} Returns true when disposed.
     */
    get IsDisposed() {
        return this.fIsDisposed === true;
    }

    // ● self handling
    /**
     * Appends this component to a parent.
     * @param {tp.Component|HTMLElement|string} Parent The parent component, element, or selector.
     * @returns {void}
     */
    AppendTo(Parent) {
        var ParentElement = this.ResolveParent(Parent);
        if (this.HasHandle && ParentElement instanceof HTMLElement) {
            ParentElement.appendChild(this.Handle);
            this.OnParentChanged();
        }
    }
    /**
     * Removes this component from DOM.
     * @returns {void}
     */
    RemoveFromDom() {
        if (this.HasHandle && this.ParentHandle) {
            this.ParentHandle.removeChild(this.Handle);
            this.OnParentChanged();
        }
    }
    /**
     * Disposes this component.
     * @returns {void}
     */
    Dispose() {
        if (!this.IsDisposed && this.HasHandle) {
            this.OnDisposing();
            this.DoDispose();
            this.OnDisposed();
        }
    }

    // ● child components
    /**
     * Adds a component child.
     * @param {tp.Component|string} Child The child component or standard node type.
     * @returns {tp.Component|null} Returns the child component or null.
     */
    AddComponent(Child) {
        var Element;
        if (tp.IsString(Child) && this.IsStandardNodeType(Child)) {
            Element = this.Document.createElement(Child.toLowerCase());
            Child = new tp.Component({ Handle: Element });
        }
        if (Child instanceof tp.Component && Child.Handle) {
            this.Handle.appendChild(Child.Handle);
            return Child;
        }
        return null;
    }
    /**
     * Inserts a component child.
     * @param {number|HTMLElement|tp.Component|string} IndexOrNode The target index or reference node.
     * @param {tp.Component|string} Child The child component or standard node type.
     * @returns {tp.Component|null} Returns the child component or null.
     */
    InsertComponent(IndexOrNode, Child) {
        var Element;
        if (tp.IsString(Child) && this.IsStandardNodeType(Child)) {
            Element = this.Document.createElement(Child.toLowerCase());
            Child = new tp.Component({ Handle: Element });
        }
        if (Child instanceof tp.Component && Child.Handle) {
            this.InsertElement(IndexOrNode, Child.Handle);
            return Child;
        }
        return null;
    }
    /**
     * Removes a component child.
     * @param {tp.Component|HTMLElement|string} Child The child component, element, or selector.
     * @returns {void}
     */
    RemoveComponent(Child) {
        this.RemoveElement(Child);
    }

    // ● child elements
    /**
     * Returns direct HTMLElement children.
     * @returns {HTMLElement[]} Returns the direct child elements.
     */
    GetElementList() {
        return this.HasHandle ? tp.ToArray(this.Handle.children) : [];
    }
    /**
     * Returns a direct child element by index.
     * @param {number} Index The child index.
     * @returns {HTMLElement|null} Returns the child element or null.
     */
    GetElementAt(Index) {
        var List = this.GetElementList();
        return Index >= 0 && Index < List.length ? List[Index] : null;
    }
    /**
     * Returns the index of a direct child element.
     * @param {HTMLElement|tp.Component|string} Child The child element, component, or selector.
     * @returns {number} Returns the child index or -1.
     */
    IndexOfElement(Child) {
        var Element = this.ResolveExistingChild(Child);
        return Element instanceof HTMLElement ? this.GetElementList().indexOf(Element) : -1;
    }
    /**
     * Returns true when a value is a direct child element.
     * @param {HTMLElement|tp.Component|string} Child The child element, component, or selector.
     * @returns {boolean} Returns true when the value is a direct child element.
     */
    IsChildElement(Child) {
        return this.IndexOfElement(Child) !== -1;
    }
    /**
     * Adds an element child.
     * @param {HTMLElement|string} Child The child element or standard node type.
     * @returns {HTMLElement|null} Returns the added element or null.
     */
    AddElement(Child) {
        var Element = this.ResolveElementChild(Child);
        if (this.HasHandle && Element instanceof HTMLElement) {
            this.Handle.appendChild(Element);
            return Element;
        }
        return null;
    }
    /**
     * Inserts an element child.
     * @param {number|HTMLElement|tp.Component|string} IndexOrNode The target index or reference node.
     * @param {HTMLElement|string} Child The child element or standard node type.
     * @returns {HTMLElement|null} Returns the inserted element or null.
     */
    InsertElement(IndexOrNode, Child) {
        var Element = this.ResolveElementChild(Child);
        var ReferenceNode = null;
        var List;
        if (!this.HasHandle || !(Element instanceof HTMLElement))
            return null;
        if (tp.IsNumber(IndexOrNode)) {
            List = this.GetElementList();
            ReferenceNode = IndexOrNode >= 0 && IndexOrNode < List.length ? List[IndexOrNode] : null;
        } else {
            ReferenceNode = this.ResolveExistingChild(IndexOrNode);
        }
        if (ReferenceNode instanceof HTMLElement && this.IsChildElement(ReferenceNode))
            this.Handle.insertBefore(Element, ReferenceNode);
        else
            this.Handle.appendChild(Element);
        return Element;
    }
    /**
     * Removes an element child.
     * @param {HTMLElement|tp.Component|string} Child The child element, component, or selector.
     * @returns {void}
     */
    RemoveElement(Child) {
        var Index = this.IndexOfElement(Child);
        if (Index !== -1)
            this.RemoveElementAt(Index);
    }
    /**
     * Removes an element child by index.
     * @param {number} Index The child index.
     * @returns {void}
     */
    RemoveElementAt(Index) {
        var Element = this.GetElementAt(Index);
        if (Element instanceof HTMLElement)
            this.Handle.removeChild(Element);
    }
    /**
     * Removes all element children.
     * @returns {void}
     */
    Clear() {
        while (this.HasHandle && this.Handle.firstChild)
            this.Handle.removeChild(this.Handle.firstChild);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * Treat this as a read-only class field.
 * @type {string}
 */
tp.Component.prototype.tpClass = "tp.Component";
/**
 * Gets the creation parameters.
 * @type {tp.CreateParams|object|null}
 */
tp.Component.prototype.CreateParams = null;
/**
 * Gets the component handle field.
 * @type {HTMLElement|null}
 */
tp.Component.prototype.fHandle = null;
/**
 * Gets the document field.
 * @type {Document|null}
 */
tp.Component.prototype.fDocument = null;
/**
 * Gets a value indicating whether this component is enabled.
 * @type {boolean}
 */
tp.Component.prototype.fEnabled = true;
/**
 * Gets a value indicating whether this component is disposed.
 * @type {boolean}
 */
tp.Component.prototype.fIsDisposed = false;
/**
 * Gets or sets a user-defined value.
 * @type {*}
 */
tp.Component.prototype.Tag = null;
/**
 * Gets standard node types.
 * @type {string[]}
 */
tp.Component.StandardNodeTypes = [
    "main",
    "aside",
    "article",
    "section",
    "header",
    "footer",
    "nav",
    "iframe",
    "div",
    "span",
    "fieldset",
    "a",
    "form",
    "table",
    "label",
    "button",
    "input",
    "select",
    "option",
    "ul",
    "ol",
    "li",
    "img",
    "textarea",
    "progress",
    "video"
];
/**
 * Sets the component associated with an element.
 * @param {HTMLElement} Element The element to mark.
 * @param {tp.Component|null} Component The component to associate.
 * @returns {void}
 */
tp.Component.SetComponent = function (Element, Component) {
    if (!(Element instanceof HTMLElement))
        return;
    if (Component instanceof tp.Component)
        Element.__tpComponent = Component;
    else
        delete Element.__tpComponent;
};
/**
 * Returns the component associated with an element.
 * @param {HTMLElement|null|undefined} Element The element to inspect.
 * @returns {tp.Component|null} Returns the associated component or null.
 */
tp.Component.GetComponent = function (Element) {
    return Element instanceof HTMLElement && Element.__tpComponent instanceof tp.Component ? Element.__tpComponent : null;
};
