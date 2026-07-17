// ● create params
/**
 * Represents initialization options passed to a tp.Component constructor.
 * This object is intentionally open-ended: derived component classes may add
 * any extra create-param properties they support.
 */
tp.CreateParams = class {
    // ● constructor
    /**
     * Creates a new create params instance by merging all source properties into this instance.
     * @param {object|null|undefined} Source The optional source object.
     */
    constructor(Source) {
        if (tp.IsObject(Source))
            tp.MergePropsShallow(this, Source);
    }
};

// ● prototype
/**
 * Gets or sets the component element, selector, or tag name.
 * When it is an HTMLElement, or a selector that resolves to an HTMLElement, the component wraps the existing element and FromMarkup returns true.
 * When it is a standard tag name, a new element of that type is created.
 * When it is null, a new div element is created.
 * @type {HTMLElement|string|null}
 */
tp.CreateParams.prototype.ElementOrSelector = null;
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
 * Gets or sets the component innerHTML.
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
 * Gets or sets the component tab index.
 * @type {number|null}
 */
tp.CreateParams.prototype.TabIndex = null;
/**
 * Gets or sets a user-defined value.
 * @type {*}
 */
tp.CreateParams.prototype.Tag = null;
/**
 * When true the constructor does not create the handle.
 * @type {boolean}
 */
tp.CreateParams.prototype.DeferHandleCreation = false;

// ● component
/**
 * Represents an HTML element wrapper without data binding.
 *
 * Initialization call order:
 * - constructor(CreateParams)
 * - CreateHandle()
 * - OnHandleCreated()
 * - InitializeFields()
 * - OnFieldsInitialized()
 * - ApplyCreateParams(Params)
 *
 * constructor(CreateParams):
 * - Do: normalize input parameters into a params object and call super(Params).
 * - Do: add any extra derived-class create params to that same params object before
 *   calling super(Params), so the base construction cycle can preserve and apply them.
 * - Do: set defaults that are needed before the handle is created, such as ElementOrSelector.
 * - Avoid: initializing instance fields after super() when those fields are needed by
 *   InitializeFields(), OnFieldsInitialized(), or ApplyCreateParams(). Those methods have
 *   already run by the time super() returns.
 * - Avoid: applying custom component options after super(). Use ApplyCreateParams() instead.
 *
 * CreateHandle():
 * - Do: let the base implementation resolve ElementOrSelector.
 * - Do: let the base implementation read data-setup and merge it over the same params object.
 * - Do: override only with great care. Most classes should use the hooks below instead.
 * - Avoid: duplicating data-setup or create-param processing in derived classes.
 *
 * OnHandleCreated():
 * - Do: call super.OnHandleCreated() first.
 * - Do: apply CSS classes, simple handle attributes, and handle-only setup.
 * - Avoid: using fields that should be initialized in InitializeFields(), because this
 *   method runs before InitializeFields().
 * - Avoid: applying create params here. data-setup has been merged, but ApplyCreateParams()
 *   is the place that consumes params.
 *
 * InitializeFields():
 * - Do: call super.InitializeFields() first.
 * - Do: initialize all per-instance fields, flags, arrays, handlers, and default values.
 * - Do: create bound handler functions such as this.FuncBind(...).
 * - Avoid: reading DOM layout or creating child DOM that depends on field values from
 *   derived constructors.
 * - Avoid: applying create params here.
 *
 * OnFieldsInitialized():
 * - Do: call super.OnFieldsInitialized() first.
 * - Do: create inner DOM, child controls, event listeners, and helper objects that must
 *   exist before create params are applied.
 * - Do: build markup that ApplyCreateParams() may target, for example label elements,
 *   inner inputs, drop-down boxes, or scrollers.
 * - Avoid: applying constructor params or data-setup params here. ApplyCreateParams()
 *   runs next and owns that work.
 *
 * ApplyCreateParams(Params):
 * - Do: call super.ApplyCreateParams(Params) first.
 * - Do: explicitly apply every supported create-param property of the current class.
 * - Do: treat Params as the final initialization object: derived constructor params plus data-setup.
 * - Do: support every property that server-side markup is allowed to place in data-setup.
 * - Avoid: relying on this.CreateParams inside constructors for custom derived params.
 * - Avoid: generic assignment of all params. Explicit assignment keeps initialization
 *   visible and makes unsupported params obvious.
 *
 * The explicit ApplyCreateParams() step replaces the old generic prototype-field-driven
 * ProcessCreateParams() pattern. See ApplyCreateParams() for the migration note.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 */
tp.Component = class extends tp.Object {
    // ● private
    /**
     * Creates component create parameters from an element, selector, tag name, plain object, or tp.CreateParams instance.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} Value The source value.
     * @returns {tp.CreateParams} Returns create parameters.
     */
    static CreateParams(Value) {
        if (Value instanceof tp.CreateParams)
            return Value;
        if (tp.IsString(Value) || tp.IsHTMLElement(Value))
            return new tp.CreateParams({ ElementOrSelector: Value });
        return new tp.CreateParams(Value);
    }

    // ● constructor
    /**
     * Creates a new component.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The component creation parameters, element, selector, or tag name.
     */
    constructor(CreateParams) {
        super();
        this.CreateParams = tp.Component.CreateParams(CreateParams);
        if (this.CreateParams.DeferHandleCreation !== true)
            this.CreateHandle();
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.tpClass = "tp.Component";
        this.fElementType = "div";
        this.fElementSubType = "";
    }
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
     * @param {HTMLElement|string|null|undefined} ElementOrSelector The element, selector, or tag name.
     * @returns {void}
     */
    CreateHandle(ElementOrSelector) {
        var Element;
        var Params;
        var Source;
        if (this.fHandle instanceof HTMLElement)
            tp.Throw("Component handle is already assigned.");
        Params = this.CreateParams || {};
        Source = !tp.IsNil(ElementOrSelector) ? ElementOrSelector : Params.ElementOrSelector;
        this.fFromMarkup = false;
        if (tp.IsHTMLElement(Source)) {
            Element = Source;
            this.fFromMarkup = true;
        } else if (tp.IsString(Source) && !tp.IsBlank(Source)) {
            if (this.IsStandardNodeType(Source)) {
                Element = this.Document.createElement(Source.toLowerCase());
            } else {
                Element = this.ResolveHandle(Source);
                this.fFromMarkup = Element instanceof HTMLElement;
            }
        }
        if (!(Element instanceof HTMLElement))
            Element = this.Document.createElement(this.ElementType);
        if ((Element instanceof HTMLInputElement || Element instanceof HTMLButtonElement || Element instanceof HTMLSelectElement) && !tp.IsBlank(this.ElementSubType))
            Element.type = this.ElementSubType;
        this.fHandle = Element;
        this.fDocument = Element.ownerDocument;
        // Keep the same params object and merge data-setup over it.
        // Derived constructors may have added custom properties before calling super().
        tp.MergePropsShallow(Params, tp.GetDataSetupObject(Element));
        this.CreateParams = Params;
        tp.SetObject(Element, this);
        this.OnHandleCreated();
        this.InitializeFields();
        this.OnFieldsInitialized();
        this.ApplyCreateParams(Params);
    }
    /**
     * Initializes per-instance fields before child DOM is created and before create params are applied.
     *
     * Override this method to initialize mutable instance state: fields, flags, arrays,
     * collections, default values, and bound handler functions such as this.FuncBind(...).
     * Always call super.InitializeFields() first.
     *
     * Do not create inner DOM here when that DOM depends on fields from derived classes;
     * use OnFieldsInitialized() for that. Do not consume constructor params or data-setup
     * values here; ApplyCreateParams() owns create-param application.
     *
     * @returns {void}
     */
    InitializeFields() {
        this.fEnabled = true;
        this.fSizeChart = new tp.SizeChart();
    }
    /**
     * Notification called after field initialization and before create params are applied.
     *
     * Override this method to create inner DOM, child controls, event listeners, and helper
     * objects that must exist before ApplyCreateParams() assigns values. Typical examples
     * are inner input elements, labels, buttons, drop-down boxes, scrollers, and markup that
     * later create params will target. Always call super.OnFieldsInitialized() first.
     *
     * Do not consume constructor params or data-setup values here. At this point all fields
     * exist, but create params have not yet been applied; ApplyCreateParams() runs next and
     * owns that work.
     *
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
    }
    /**
     * Applies explicit create params to this component.
     *
     * Override this method to consume constructor params and data-setup values supported by
     * the current class. Always call super.ApplyCreateParams(Params) first, then explicitly
     * apply only the params owned by the derived class.
     *
     * Treat Params as the final initialization object: it contains the normalized constructor
     * params plus any data-setup values merged from the handle. If a property is supported
     * from server-side markup, it must be handled here. Do not apply params in constructors,
     * OnHandleCreated(), InitializeFields(), or OnFieldsInitialized(). Do not generic-assign
     * every param to this; explicit assignment documents the supported initialization surface.
     *
     * Migration note:
     *
     * The old Tripous JavaScript runtime used a generic ProcessCreateParams() method.
     * That method walked through this.CreateParams and tried to assign each entry to
     * a same-named property of the component instance.
     *
     * That old mechanism had an important JavaScript construction-order problem. It
     * was called from inside the base tp.Component constructor, while a derived class
     * constructor had not completed yet. At that point, instance fields declared or
     * assigned by the derived class constructor did not exist. Only members already
     * available through the base class and the prototype chain were visible.
     *
     * This is the main historical reason the old code contains many declarations such
     * as MyControl.prototype.SomeProperty = ... . Those prototype declarations made
     * properties visible early enough for ProcessCreateParams() to see and assign them.
     * In other words, many prototype fields were not primarily a design preference;
     * they were a workaround for generic create-param processing during construction.
     *
     * In the migrated runtime we prefer explicit instance initialization plus
     * class-specific ApplyCreateParams() overrides. This costs a little more code in
     * each class, but it keeps the class easier to read at a glance and avoids adding
     * prototype fields only for construction-time visibility. Derived classes should
     * call super.ApplyCreateParams(Params) first and then explicitly apply only their
     * own supported create-param properties.
     *
     * Prototype defaults are still acceptable for argument/default descriptor classes
     * such as tp.CreateParams and tp.WindowArgs, and for real class-level metadata.
     * For component/control classes, do not add new prototype fields just so create
     * params can see them.
     *
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
        if (!tp.IsNil(Params.TabIndex))
            this.TabIndex = Params.TabIndex;
        if (!tp.IsNil(Params.Tag))
            this.Tag = Params.Tag;
        if (!tp.IsNil(Params.Parent))
            this.Parent = Params.Parent;
        if (!tp.IsNil(Params.Breakpoints))
            this.Breakpoints = Params.Breakpoints;
        if (!tp.IsNil(Params.IsElementResizeListener))
            this.IsElementResizeListener = Params.IsElementResizeListener;
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
     * Returns the top-level nested child components owned by this component.
     * Components inside wrapper elements are included, but descendants of another
     * child component are left to that component's own disposal.
     * @returns {tp.Component[]} Returns the child components.
     */
    GetOwnedComponentList() {
        var List = this.HasHandle ? tp.GetAllComponents(this.Handle) : [];
        var Result = [];
        var Index;
        var Component;
        var Parent;
        var Owner;
        for (Index = 0; Index < List.length; Index++) {
            Component = List[Index];
            if (!(Component instanceof tp.Component) || Component === this || !Component.HasHandle)
                continue;
            Parent = Component.Handle.parentElement;
            Owner = null;
            while (Parent instanceof HTMLElement && Parent !== this.Handle) {
                Owner = tp.GetComponent(Parent);
                if (Owner instanceof tp.Component)
                    break;
                Parent = Parent.parentElement;
            }
            if (Owner === null)
                Result.push(Component);
        }
        return Result;
    }
    /**
     * Disposes all child components owned by this component.
     * @returns {void}
     */
    DisposeChildComponents() {
        var List = this.GetOwnedComponentList();
        var Index;
        var Component;
        for (Index = 0; Index < List.length; Index++) {
            Component = List[Index];
            if (Component instanceof tp.Component && !Component.IsDisposed)
                Component.Dispose();
        }
    }
    /**
     * Destroys the component handle and releases resources.
     * @returns {void}
     */
    DoDispose() {
        var Element = this.fHandle;
        this.DisposeChildComponents();
        if (this.fResizeDetector) {
            this.fResizeDetector.Dispose();
            this.fResizeDetector = null;
        }
        if (Element instanceof HTMLElement) {
            if (tp.GetObject(Element) === this)
                tp.SetObject(Element, null);
            if (Element.parentNode)
                Element.parentNode.removeChild(Element);
        }
        this.fHandle = null;
        this.fIsDisposed = true;
    }
    /**
     * Notification called immediately after the component handle has been created or resolved.
     *
     * Override this method for handle-only setup: CSS classes, simple attributes, ARIA
     * attributes, handle event listeners, and DOM state that needs only this.Handle.
     * Always call super.OnHandleCreated() first.
     *
     * At this point this.Handle and this.Document are valid, and data-setup has already been
     * merged into the create params object. InitializeFields() has not run yet, so do not
     * rely on fields, handlers, child controls, or helper objects initialized there. Do not
     * apply create params here; ApplyCreateParams() owns that work.
     *
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
    /**
     * Notification sent by tp.ResizeDetector when this component size changes.
     * This method is called only when IsElementResizeListener is true.
     * @param {object|null|undefined} ResizeInfo The resize info object.
     * @returns {void}
     */
    OnElementSizeChanged(ResizeInfo) {
        this.Trigger("ElementSizeChanged", ResizeInfo || {});
        if (this.fSizeChart && this.HasHandle && this.fSizeChart.IsModeChange(this.Handle.offsetWidth))
            this.OnSizeModeChanged();
    }
    /**
     * Notification called when SizeMode changes.
     * @returns {void}
     */
    OnSizeModeChanged() {
        var List;
        this.Trigger("SizeModeChanged", { SizeMode: this.SizeMode });
        if (tp.DebugMode === true && this.HasHandle) {
            tp.RemoveClasses(this.Handle, tp.SizeModes);
            tp.AddClass(this.Handle, this.SizeMode);
        }
        List = this.GetComponentList();
        List.forEach(function (Component) {
            Component.ParentSizeModeChanged(this.SizeMode);
        }, this);
    }
    /**
     * Notification called by a parent component when its SizeMode changes.
     * @param {string} ParentSizeMode A tp.SizeMode value.
     * @returns {void}
     */
    ParentSizeModeChanged(ParentSizeMode) {
    }
    /**
     * Broadcasts SizeModeChanged to this component and nested listening components.
     * @returns {void}
     */
    BroadcastSizeModeChanged() {
        var List;
        this.OnSizeModeChanged();
        List = this.GetAllComponents();
        List.forEach(function (Component) {
            if (Component.IsElementResizeListener === true)
                Component.OnSizeModeChanged();
        });
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
     * Returns true when the handle comes from existing markup.
     * @returns {boolean} Returns true when the handle comes from existing markup.
     */
    get FromMarkup() {
        return this.fFromMarkup === true;
    }
    /**
     * Returns true when the handle comes from existing markup.
     * @returns {boolean} Returns true when the handle comes from existing markup.
     */
    get FormMarkup() {
        return this.FromMarkup;
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
     * Gets the default element tag name for this component class.
     * @returns {string} Returns the default element tag name.
     */
    get ElementType() {
        return this.fElementType;
    }
    /**
     * Gets the input/button type used when creating applicable elements.
     * @returns {string} Returns the element subtype.
     */
    get ElementSubType() {
        return this.fElementSubType;
    }
    /**
     * Returns true when this component has focus.
     * @returns {boolean} Returns true when focused.
     */
    get IsFocused() {
        return this.HasHandle && tp.IsFocused(this.Handle);
    }
    /**
     * Returns true when this component or one of its children has focus.
     * @returns {boolean} Returns true when this component contains focus.
     */
    get HasFocused() {
        return this.HasHandle && tp.HasFocused(this.Handle);
    }
    /**
     * Gets or sets the component tab index.
     * A negative value keeps the element programmatically focusable without adding it to the tab order.
     * @returns {number} Returns the tab index.
     */
    get TabIndex() {
        return this.HasHandle ? this.Handle.tabIndex : 0;
    }
    /**
     * Gets or sets the component tab index.
     * @param {number|string} Value The tab index.
     * @returns {void}
     */
    set TabIndex(Value) {
        if (this.HasHandle)
            this.Handle.tabIndex = tp.StrToInt(Value, 0);
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
        return this.ParentHandle instanceof HTMLElement ? tp.GetComponent(this.ParentHandle) : null;
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
        return this.HasHandle ? tp.val(this.Handle) : "";
    }
    /**
     * Sets text or value depending on the handle.
     * @param {*} Value The value to set.
     * @returns {void}
     */
    set Text(Value) {
        if (this.HasHandle)
            tp.val(this.Handle, Value);
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
        if (this.HasHandle) {
            if ("disabled" in this.Handle)
                this.Handle.disabled = !this.fEnabled;
            this.Handle.classList.toggle("tp-Disabled", !this.fEnabled);
        }
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
     * Gets or sets a value indicating whether this component listens to element resize changes.
     * @returns {boolean} Returns true when this component listens to element resize changes.
     */
    get IsElementResizeListener() {
        return this.fResizeDetector instanceof tp.ResizeDetector && this.fResizeDetector.Observing === true;
    }
    /**
     * Gets or sets a value indicating whether this component listens to element resize changes.
     * @param {boolean} Value True to start listening; false to stop.
     * @returns {void}
     */
    set IsElementResizeListener(Value) {
        Value = Value === true;
        if (this.HasHandle && Value !== this.IsElementResizeListener) {
            if (Value) {
                if (!this.fResizeDetector)
                    this.fResizeDetector = new tp.ResizeDetector(this.Handle, this.OnElementSizeChanged, this, true);
                else
                    this.fResizeDetector.Start();
                this.OnElementSizeChanged();
            } else if (this.fResizeDetector) {
                this.fResizeDetector.Stop();
            }
        }
    }
    /**
     * Gets the current size mode.
     * @returns {string} Returns a tp.SizeMode value.
     */
    get SizeMode() {
        return this.fSizeChart ? this.fSizeChart.Mode : tp.SizeMode.None;
    }
    /**
     * Gets or sets size mode breakpoint values.
     * @returns {number[]} Returns the breakpoint values.
     */
    get Breakpoints() {
        return this.fSizeChart ? this.fSizeChart.Breakpoints : [];
    }
    /**
     * Gets or sets size mode breakpoint values.
     * @param {number[]} Value The breakpoint values.
     * @returns {void}
     */
    set Breakpoints(Value) {
        if (this.fSizeChart)
            this.fSizeChart.Assign(Value);
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
    /**
     * Makes the handle of this component the focused element.
     * @returns {void}
     */
    Focus() {
        if (this.HasHandle)
            this.Handle.focus();
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
            Child = new tp.Component({ ElementOrSelector: Element });
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
            Child = new tp.Component({ ElementOrSelector: Element });
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
    /**
     * Returns all nested component children.
     * @returns {tp.Component[]} Returns all nested components.
     */
    GetAllComponents() {
        return this.HasHandle ? tp.GetAllComponents(this.Handle) : [];
    }
    /**
     * Returns all direct component children.
     * @returns {tp.Component[]} Returns all direct child components.
     */
    GetComponentList() {
        return this.HasHandle ? tp.GetComponentList(this.Handle) : [];
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
 * Gets the resize detector field.
 * @type {tp.ResizeDetector|null}
 */
tp.Component.prototype.fResizeDetector = null;
/**
 * Gets the size chart field.
 * @type {tp.SizeChart|null}
 */
tp.Component.prototype.fSizeChart = null;
/**
 * Default element tag name.
 * @type {string}
 */
tp.Component.prototype.fElementType = "div";
/**
 * Default input/button subtype.
 * @type {string}
 */
tp.Component.prototype.fElementSubType = "";
/**
 * Gets or sets a user-defined value.
 * @type {*}
 */
tp.Component.prototype.Tag = null;
/**
 * True when the handle comes from existing markup.
 * @type {boolean}
 */
tp.Component.prototype.fFromMarkup = false;
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
 * Returns the Tripous object associated with an element.
 * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
 * @returns {*|null} Returns the associated object or null.
 */
tp.GetObject = function (ElementOrSelector) {
    var Element = tp.Select(ElementOrSelector);
    return Element instanceof HTMLElement && !tp.IsNil(Element.__tpObject) ? Element.__tpObject : null;
};
/**
 * Associates an element with a Tripous object.
 * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
 * @param {*|null} Value The object to associate.
 * @returns {void}
 */
tp.SetObject = function (ElementOrSelector, Value) {
    var Element = tp.Select(ElementOrSelector);
    if (!(Element instanceof HTMLElement))
        return;
    if (!tp.IsNil(Value)) {
        Element.__tpObject = Value;
        tp.AddClass(Element, "tp-Object");
    } else {
        delete Element.__tpObject;
        tp.RemoveClass(Element, "tp-Object");
    }
};
/**
 * Returns true when an element is associated with a Tripous object.
 * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
 * @returns {boolean} Returns true when the element is associated with a Tripous object.
 */
tp.HasObject = function (ElementOrSelector) {
    return tp.GetObject(ElementOrSelector) !== null;
};
/**
 * Returns all nested Tripous objects of a parent element.
 * @param {HTMLElement|string|Document|null|undefined} ParentElementOrSelector The parent element or selector.
 * @returns {object[]} Returns all nested objects.
 */
tp.GetAllObjects = function (ParentElementOrSelector) {
    var Parent = tp.Select(ParentElementOrSelector) || document;
    var Result = [];
    var List;
    var Index;
    var Value;
    if (tp.IsNodeSelector(Parent)) {
        List = Parent.querySelectorAll(".tp-Object");
        for (Index = 0; Index < List.length; Index++) {
            Value = tp.GetObject(List[Index]);
            if (Value !== null)
                Result.push(Value);
        }
    }
    return Result;
};
/**
 * Returns the first container object of a specified class, searching upward from an element.
 * @param {HTMLElement|string|null|undefined} ElementOrSelector The starting element or selector.
 * @param {Function} ObjectClass The object class to match.
 * @returns {*|null} Returns the matched object or null.
 */
tp.GetContainerByClass = function (ElementOrSelector, ObjectClass) {
    var Element = tp.Select(ElementOrSelector);
    var Value;
    while (Element instanceof HTMLElement) {
        Value = tp.GetObject(Element);
        if (tp.IsFunction(ObjectClass) && Value instanceof ObjectClass)
            return Value;
        if (Element === Element.ownerDocument.body)
            return null;
        Element = Element.parentElement;
    }
    return null;
};
/**
 * Returns the component associated with an element.
 * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
 * @returns {tp.Component|null} Returns the associated component or null.
 */
tp.GetComponent = function (ElementOrSelector) {
    var Value = tp.GetObject(ElementOrSelector);
    return Value instanceof tp.Component ? Value : null;
};
/**
 * Returns true when an element is associated with a component.
 * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
 * @returns {boolean} Returns true when the element is associated with a component.
 */
tp.HasComponent = function (ElementOrSelector) {
    return tp.GetComponent(ElementOrSelector) instanceof tp.Component;
};
/**
 * Returns all nested component children of a parent element.
 * @param {HTMLElement|string|null|undefined} ParentElementOrSelector The parent element or selector.
 * @returns {tp.Component[]} Returns all nested components.
 */
tp.GetAllComponents = function (ParentElementOrSelector) {
    var List = tp.GetAllObjects(ParentElementOrSelector);
    var Result = [];
    var Index;
    var Component;
    for (Index = 0; Index < List.length; Index++) {
        Component = List[Index];
        if (Component instanceof tp.Component)
            Result.push(Component);
    }
    return Result;
};
/**
 * Returns all direct component children of a parent element.
 * @param {HTMLElement|string|null|undefined} ParentElementOrSelector The parent element or selector.
 * @returns {tp.Component[]} Returns all direct child components.
 */
tp.GetComponentList = function (ParentElementOrSelector) {
    var Result = [];
    var List = tp.GetElementList(ParentElementOrSelector);
    var Index;
    var Component;
    for (Index = 0; Index < List.length; Index++) {
        Component = tp.GetComponent(List[Index]);
        if (Component instanceof tp.Component)
            Result.push(Component);
    }
    return Result;
};
