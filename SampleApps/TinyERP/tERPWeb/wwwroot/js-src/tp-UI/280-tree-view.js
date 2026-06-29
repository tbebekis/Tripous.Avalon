// ● tree view event args
/**
 * Event arguments for tp.TreeView node events.
 */
tp.TreeViewEventArgs = class extends tp.EventArgs {
    // ● constructor
    /**
     * Creates the event arguments.
     * @param {tp.TreeNode|null|undefined} Node The tree node.
     */
    constructor(Node) {
        super("");
        this.Node = Node instanceof tp.TreeNode ? Node : null;
    }
};

// ● tree node
/**
 * Represents a tp.TreeView node.
 *
 * Example markup:
 * <pre>
 *     <div>Node
 *         <div>Leaf</div>
 *     </div>
 * </pre>
 */
tp.TreeNode = class extends tp.Object {
    // ● constructor
    /**
     * Creates a tree node.
     * @param {HTMLElement} Handle The element representing the node.
     */
    constructor(Handle) {
        super();
        if (!(Handle instanceof HTMLElement))
            throw new Error("Can not create a TreeNode without handle (HTMLElement).");
        this.fHandle = Handle;
        this.fStripElement = null;
        this.fPlusMinusElement = null;
        this.fImageElement = null;
        this.fTextElement = null;
        this.fItemsElement = null;
        this.fParentTreeNode = null;
        this.fItems = [];
        this.fIcoClasses = "";
        this.fImageUrl = "";
        this.Tag = null;
        this.NormalizeHandle();
    }

    // ● protected
    /**
     * Normalizes the node markup.
     * @protected
     * @returns {void}
     */
    NormalizeHandle() {
        var TextNode = tp.FindTextNode(this.Handle);
        var Text = TextNode ? TextNode.nodeValue || "" : "";
        var List = tp.ChildHTMLElements(this.Handle);
        var Params;
        var i;
        if (TextNode)
            TextNode.nodeValue = "";
        Text = tp.Trim(Text);
        tp.SetElementInfo(this.Handle, this, "__TreeNode");
        for (i = 0; i < List.length; i++)
            this.Handle.removeChild(List[i]);
        this.CreateElements(Text);
        for (i = 0; i < List.length; i++)
            this.Add(new tp.TreeNode(List[i]));
        tp.AddClass(this.Handle, this.Count > 0 ? tp.Classes.Node : tp.Classes.Leaf);
        if (this.IsNode)
            this.fPlusMinusElement.textContent = tp.TreeNode.CollapseSymbol;
        Params = tp.GetDataSetupObject(this.Handle);
        if (Params) {
            this.Handle.removeAttribute("data-setup");
            if (!tp.IsNil(Params.Text))
                this.Text = Params.Text;
            if (!tp.IsNil(Params.IcoClasses))
                this.IcoClasses = Params.IcoClasses;
            if (!tp.IsNil(Params.ImageUrl))
                this.ImageUrl = Params.ImageUrl;
            if (!tp.IsNil(Params.Url))
                this.Url = Params.Url;
            if (!tp.IsNil(Params.ToolTip))
                this.ToolTip = Params.ToolTip;
            if (!tp.IsNil(Params.Tag))
                this.Tag = Params.Tag;
        }
        this.IcoChanged();
        if (tp.IsBlank(this.ToolTip))
            this.ToolTip = Text;
    }
    /**
     * Creates the node inner elements.
     * @protected
     * @param {string} Text The display text.
     * @returns {void}
     */
    CreateElements(Text) {
        this.fStripElement = this.Handle.ownerDocument.createElement("div");
        this.fStripElement.className = tp.Classes.Strip;
        this.Handle.appendChild(this.fStripElement);
        this.fPlusMinusElement = this.Handle.ownerDocument.createElement("div");
        this.fStripElement.appendChild(this.fPlusMinusElement);
        this.fImageElement = this.Handle.ownerDocument.createElement("div");
        this.fStripElement.appendChild(this.fImageElement);
        this.fTextElement = this.Handle.ownerDocument.createElement("a");
        this.fTextElement.href = "javascript:void(0);";
        this.fStripElement.appendChild(this.fTextElement);
        if (!tp.IsBlank(Text))
            this.fTextElement.innerHTML = Text;
    }
    /**
     * Updates image element visibility after icon changes.
     * @protected
     * @returns {void}
     */
    IcoChanged() {
        if (this.fImageElement instanceof HTMLElement)
            this.fImageElement.style.display = tp.IsBlank(this.IcoClasses) && tp.IsBlank(this.ImageUrl) ? "none" : "";
    }
    /**
     * Triggers the Collapsing event through the owning tree view.
     * @protected
     * @returns {void}
     */
    OnCollapsing() {
        var Tree = this.TreeView;
        if (Tree)
            Tree.OnCollapsing(this);
    }
    /**
     * Triggers the Collapsed event through the owning tree view.
     * @protected
     * @returns {void}
     */
    OnCollapsed() {
        var Tree = this.TreeView;
        if (Tree)
            Tree.OnCollapsed(this);
    }
    /**
     * Triggers the Expanding event through the owning tree view.
     * @protected
     * @returns {void}
     */
    OnExpanding() {
        var Tree = this.TreeView;
        if (Tree)
            Tree.OnExpanding(this);
    }
    /**
     * Triggers the Expanded event through the owning tree view.
     * @protected
     * @returns {void}
     */
    OnExpanded() {
        var Tree = this.TreeView;
        if (Tree)
            Tree.OnExpanded(this);
    }

    // ● public
    /**
     * Removes all child nodes from this node.
     * @returns {void}
     */
    Clear() {
        var i;
        tp.RemoveClass(this.Handle, tp.Classes.Expanded);
        tp.RemoveClass(this.Handle, tp.Classes.Node);
        tp.AddClass(this.Handle, tp.Classes.Leaf);
        this.fPlusMinusElement.textContent = "";
        if (this.fItemsElement)
            tp.RemoveChildren(this.fItemsElement);
        for (i = 0; i < this.fItems.length; i++)
            this.fItems[i].fParentTreeNode = null;
        this.fItems.length = 0;
        super.Clear();
    }
    /**
     * Returns true when this node contains a specified child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {boolean} Returns true when the child node exists.
     */
    Contains(Node) {
        return this.IndexOf(Node) >= 0;
    }
    /**
     * Returns the index of a child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {number} Returns the index or -1.
     */
    IndexOf(Node) {
        return this.fItems.indexOf(Node);
    }
    /**
     * Returns a child node by index.
     * @param {number} Index The child index.
     * @returns {tp.TreeNode|null} Returns the child node or null.
     */
    ByIndex(Index) {
        return tp.InRange(this.fItems, Index) ? this.fItems[Index] : null;
    }
    /**
     * Adds a child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {void}
     */
    Add(Node) {
        this.Insert(this.Count, Node);
    }
    /**
     * Inserts a child node at an index.
     * @param {number} Index The child index.
     * @param {tp.TreeNode} Node The child node.
     * @returns {void}
     */
    Insert(Index, Node) {
        var WasLeaf;
        var RefNode;
        if (!(Node instanceof tp.TreeNode) || this.Contains(Node))
            return;
        Index = Math.max(0, Math.min(tp.ToInt(Index), this.Count));
        WasLeaf = this.IsLeaf;
        if (Node.ParentTreeNode)
            Node.ParentTreeNode.Remove(Node);
        if (!this.fItemsElement) {
            this.fItemsElement = this.Handle.ownerDocument.createElement("div");
            this.Handle.appendChild(this.fItemsElement);
        }
        if (Index >= this.Count) {
            this.fItems.push(Node);
            this.fItemsElement.appendChild(Node.Handle);
        } else if (Index >= 0) {
            RefNode = this.fItems[Index];
            tp.ListInsert(this.fItems, Index, Node);
            this.fItemsElement.insertBefore(Node.Handle, RefNode.Handle);
        }
        Node.fParentTreeNode = this;
        tp.RemoveClass(this.Handle, tp.Classes.Leaf);
        tp.AddClass(this.Handle, tp.Classes.Node);
        if (WasLeaf)
            this.fPlusMinusElement.textContent = tp.TreeNode.CollapseSymbol;
    }
    /**
     * Removes a child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {void}
     */
    Remove(Node) {
        if (!this.Contains(Node))
            return;
        if (Node.Handle.parentNode)
            Node.Handle.parentNode.removeChild(Node.Handle);
        tp.ListRemove(this.fItems, Node);
        Node.fParentTreeNode = null;
        if (this.Count === 0) {
            tp.RemoveClass(this.Handle, tp.Classes.Expanded);
            tp.RemoveClass(this.Handle, tp.Classes.Node);
            tp.AddClass(this.Handle, tp.Classes.Leaf);
            this.fPlusMinusElement.textContent = "";
        }
    }
    /**
     * Removes a child node by index.
     * @param {number} Index The child index.
     * @returns {void}
     */
    RemoveAt(Index) {
        if (tp.InRange(this.fItems, Index))
            this.Remove(this.fItems[Index]);
    }
    /**
     * Creates and adds a child node.
     * @param {string} Text The node display text.
     * @returns {tp.TreeNode} Returns the new node.
     */
    AddNode(Text) {
        return this.InsertNode(this.Count, Text);
    }
    /**
     * Creates and inserts a child node.
     * @param {number} Index The child index.
     * @param {string} Text The node display text.
     * @returns {tp.TreeNode} Returns the new node.
     */
    InsertNode(Index, Text) {
        var Element = this.Handle.ownerDocument.createElement("div");
        var Result = new tp.TreeNode(Element);
        Result.Text = Text;
        this.Insert(Index, Result);
        return Result;
    }
    /**
     * Collapses this node.
     * @returns {void}
     */
    Collapse() {
        if (this.IsNode) {
            this.fPlusMinusElement.textContent = tp.TreeNode.ExpandSymbol;
            if (this.IsExpanded) {
                this.OnCollapsing();
                tp.RemoveClass(this.Handle, tp.Classes.Expanded);
                this.OnCollapsed();
            }
        }
    }
    /**
     * Expands this node.
     * @returns {void}
     */
    Expand() {
        if (this.IsNode) {
            this.fPlusMinusElement.textContent = tp.TreeNode.CollapseSymbol;
            if (this.IsExpanded !== true) {
                this.OnExpanding();
                tp.AddClass(this.Handle, tp.Classes.Expanded);
                this.OnExpanded();
            }
        }
    }
    /**
     * Toggles this node.
     * @returns {void}
     */
    Toggle() {
        if (this.IsExpanded)
            this.Collapse();
        else
            this.Expand();
    }
    /**
     * Collapses all descendant nodes and this node.
     * @returns {void}
     */
    CollapseAll() {
        var i;
        if (this.IsNode) {
            for (i = 0; i < this.fItems.length; i++)
                this.fItems[i].CollapseAll();
            this.Collapse();
        }
    }
    /**
     * Expands all descendant nodes and this node.
     * @returns {void}
     */
    ExpandAll() {
        var i;
        if (this.IsNode) {
            for (i = 0; i < this.fItems.length; i++)
                this.fItems[i].ExpandAll();
            this.Expand();
        }
    }

    // ● properties
    /**
     * Gets the owning tree view.
     * @returns {tp.TreeView|null} Returns the owning tree view.
     */
    get TreeView() {
        var Current = this;
        while (Current && !(Current instanceof tp.TreeView))
            Current = Current.ParentTreeNode;
        return Current instanceof tp.TreeView ? Current : null;
    }
    /**
     * Gets or sets the display text.
     * @returns {string} Returns the display text.
     */
    get Text() {
        return this.fTextElement instanceof HTMLElement ? this.fTextElement.innerHTML : "";
    }
    /**
     * Gets or sets the display text.
     * @param {string|null|undefined} Value The display text.
     * @returns {void}
     */
    set Text(Value) {
        if (this.fTextElement instanceof HTMLElement)
            this.fTextElement.innerHTML = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the tooltip.
     * @returns {string} Returns the tooltip.
     */
    get ToolTip() {
        return this.Handle ? this.Handle.title || "" : "";
    }
    /**
     * Gets or sets the tooltip.
     * @param {string|null|undefined} Value The tooltip.
     * @returns {void}
     */
    set ToolTip(Value) {
        if (this.Handle)
            this.Handle.title = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the anchor URL.
     * @returns {string} Returns the URL.
     */
    get Url() {
        var Value = this.fTextElement instanceof HTMLAnchorElement ? this.fTextElement.getAttribute("href") || "" : "";
        return Value === "javascript:void(0);" ? "" : Value;
    }
    /**
     * Gets or sets the anchor URL.
     * @param {string|null|undefined} Value The URL.
     * @returns {void}
     */
    set Url(Value) {
        if (this.fTextElement instanceof HTMLAnchorElement) {
            if (tp.IsBlank(Value)) {
                this.fTextElement.href = "javascript:void(0);";
                this.fTextElement.removeAttribute("target");
                this.fTextElement.removeAttribute("rel");
            } else {
                this.fTextElement.href = String(Value);
                this.fTextElement.target = "_blank";
                this.fTextElement.rel = "noopener noreferrer";
            }
        }
    }
    /**
     * Gets or sets icon CSS classes.
     * @returns {string} Returns icon CSS classes.
     */
    get IcoClasses() {
        return this.fIcoClasses;
    }
    /**
     * Gets or sets icon CSS classes.
     * @param {string|null|undefined} Value The icon CSS classes.
     * @returns {void}
     */
    set IcoClasses(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fImageElement instanceof HTMLElement) {
            tp.RemoveClasses(this.fImageElement, this.fIcoClasses);
            this.fImageElement.style.background = "";
            this.fImageUrl = "";
            tp.AddClasses(this.fImageElement, Value);
        }
        this.fIcoClasses = Value;
        this.IcoChanged();
    }
    /**
     * Gets or sets image URL.
     * @returns {string} Returns the image URL.
     */
    get ImageUrl() {
        return this.fImageUrl;
    }
    /**
     * Gets or sets image URL.
     * @param {string|null|undefined} Value The image URL.
     * @returns {void}
     */
    set ImageUrl(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (this.fImageElement instanceof HTMLElement) {
            tp.RemoveClasses(this.fImageElement, this.fIcoClasses);
            this.fIcoClasses = "";
            this.fImageElement.style.background = "";
            if (!tp.IsBlank(Value)) {
                tp.SetStyle(this.fImageElement, {
                    backgroundImage: "url(\"" + Value + "\")",
                    backgroundRepeat: "no-repeat",
                    backgroundPosition: "center center",
                    backgroundSize: "75%"
                });
            }
        }
        this.fImageUrl = Value;
        this.IcoChanged();
    }
    /**
     * Gets or sets the parent tree node or tree view.
     * @returns {tp.TreeNode|tp.TreeView|null} Returns the parent.
     */
    get ParentTreeNode() {
        return this.fParentTreeNode;
    }
    /**
     * Gets or sets the parent tree node or tree view.
     * @param {tp.TreeNode|tp.TreeView|null} Value The parent.
     * @returns {void}
     */
    set ParentTreeNode(Value) {
        this.fParentTreeNode = Value instanceof tp.TreeNode || Value instanceof tp.TreeView ? Value : null;
    }
    /**
     * Gets the node handle.
     * @returns {HTMLElement} Returns the handle.
     */
    get Handle() {
        return this.fHandle;
    }
    /**
     * Gets the node text element.
     * @returns {HTMLAnchorElement|null} Returns the text element.
     */
    get TextElement() {
        return this.fTextElement instanceof HTMLAnchorElement ? this.fTextElement : null;
    }
    /**
     * Gets the child node count.
     * @returns {number} Returns the child node count.
     */
    get Count() {
        return this.fItems.length;
    }
    /**
     * Returns true when this node has children.
     * @returns {boolean} Returns true when this node has children.
     */
    get HasChildren() {
        return this.Count > 0;
    }
    /**
     * Returns true when this node is a tree root.
     * @returns {boolean} Returns true for a root.
     */
    get IsRoot() {
        return tp.HasClass(this.Handle, tp.Classes.TreeView);
    }
    /**
     * Returns true when this node has children.
     * @returns {boolean} Returns true for a node.
     */
    get IsNode() {
        return !this.IsRoot && this.Count > 0;
    }
    /**
     * Returns true when this node has no children.
     * @returns {boolean} Returns true for a leaf.
     */
    get IsLeaf() {
        return !this.IsRoot && this.Count === 0;
    }
    /**
     * Returns true when this node is expanded.
     * @returns {boolean} Returns true when expanded.
     */
    get IsExpanded() {
        return tp.HasClass(this.Handle, tp.Classes.Expanded);
    }
    /**
     * Gets the node level. Root is level 0.
     * @returns {number} Returns the node level.
     */
    get Level() {
        return this.ParentTreeNode ? this.ParentTreeNode.Level + 1 : 0;
    }
    /**
     * Gets the node index in its parent.
     * @returns {number} Returns the node index or -1.
     */
    get Index() {
        return this.ParentTreeNode ? this.ParentTreeNode.IndexOf(this) : -1;
    }
};

/**
 * The expand symbol.
 * @type {string}
 */
tp.TreeNode.ExpandSymbol = "\u25B8";
/**
 * The collapse symbol.
 * @type {string}
 */
tp.TreeNode.CollapseSymbol = "\u25BE";

// ● tree view
/**
 * A tree-view control.
 *
 * Example markup:
 * <pre>
 *     <div>
 *         <div>Leaf</div>
 *         <div>Node
 *             <div>Leaf</div>
 *         </div>
 *     </div>
 * </pre>
 *
 * Events:
 * - NodeClick
 * - Collapsing
 * - Collapsed
 * - Expanding
 * - Expanded
 */
tp.TreeView = class extends tp.Component {
    // ● private
    /**
     * Creates tree-view create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateTreeViewParams(CreateParams) {
        var Args = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "div";
        return Args;
    }

    // ● constructor
    /**
     * Creates a tree view.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.TreeView.CreateTreeViewParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fItemsElement = null;
        this.fItems = [];
        this.fClickHandler = this.FuncBind(this.HandleClick);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.NormalizeNodes();
        this.CollapseAll();
        this.Handle.addEventListener("click", this.fClickHandler, false);
    }
    /**
     * Applies explicit create params to this tree view.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Width))
            this.Width = Params.Width;
        if (!tp.IsNil(Params.Height))
            this.Height = Params.Height;
        if (!tp.IsNil(Params.Expanded) && Params.Expanded === true)
            this.ExpandAll();
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.TreeView);
        this.Handle.tabIndex = 0;
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.Handle && this.fClickHandler)
            this.Handle.removeEventListener("click", this.fClickHandler, false);
        this.fClickHandler = null;
        this.fItemsElement = null;
        this.fItems = null;
        super.DoDispose();
    }
    /**
     * Handles tree clicks.
     * @protected
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleClick(e) {
        var Strip;
        var Node;
        if (!(e.target instanceof HTMLElement))
            return;
        Strip = tp.Closest(e.target, "." + tp.Classes.Strip);
        if (!Strip || !Strip.parentNode)
            return;
        Node = tp.GetElementInfo(Strip.parentNode, "__TreeNode");
        if (Node instanceof tp.TreeNode)
            this.OnNodeClick(Node);
    }
    /**
     * Converts direct child elements into tree nodes.
     * @protected
     * @returns {void}
     */
    NormalizeNodes() {
        var List = tp.ChildHTMLElements(this.Handle);
        var i;
        for (i = 0; i < List.length; i++)
            this.Handle.removeChild(List[i]);
        for (i = 0; i < List.length; i++)
            this.Add(new tp.TreeNode(List[i]));
    }
    /**
     * Sets the focused node.
     * @protected
     * @param {tp.TreeNode|null|undefined} Node The node to focus.
     * @returns {void}
     */
    SetFocusedNode(Node) {
        var List = tp.SelectAll(this.Handle, "." + tp.Classes.Focused);
        var i;
        for (i = 0; i < List.length; i++)
            tp.RemoveClass(List[i], tp.Classes.Focused);
        if (Node instanceof tp.TreeNode)
            tp.AddClass(Node.Handle, tp.Classes.Focused);
    }

    // ● public
    /**
     * Removes all child nodes.
     * @returns {void}
     */
    Clear() {
        var i;
        tp.RemoveClass(this.Handle, tp.Classes.Expanded);
        if (this.fItemsElement)
            tp.RemoveChildren(this.fItemsElement);
        for (i = 0; i < this.fItems.length; i++)
            this.fItems[i].ParentTreeNode = null;
        this.fItems.length = 0;
    }
    /**
     * Returns true when this tree contains a child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {boolean} Returns true when found.
     */
    Contains(Node) {
        return this.IndexOf(Node) >= 0;
    }
    /**
     * Returns the index of a child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {number} Returns the index or -1.
     */
    IndexOf(Node) {
        return this.fItems.indexOf(Node);
    }
    /**
     * Returns a child node by index.
     * @param {number} Index The child index.
     * @returns {tp.TreeNode|null} Returns the child node or null.
     */
    ByIndex(Index) {
        return tp.InRange(this.fItems, Index) ? this.fItems[Index] : null;
    }
    /**
     * Adds a child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {void}
     */
    Add(Node) {
        this.Insert(this.Count, Node);
    }
    /**
     * Inserts a child node at an index.
     * @param {number} Index The child index.
     * @param {tp.TreeNode} Node The child node.
     * @returns {void}
     */
    Insert(Index, Node) {
        var RefNode;
        if (!(Node instanceof tp.TreeNode) || this.Contains(Node))
            return;
        Index = Math.max(0, Math.min(tp.ToInt(Index), this.Count));
        if (Node.ParentTreeNode)
            Node.ParentTreeNode.Remove(Node);
        if (!this.fItemsElement) {
            this.fItemsElement = this.Handle.ownerDocument.createElement("div");
            this.Handle.appendChild(this.fItemsElement);
        }
        if (Index >= this.Count) {
            this.fItems.push(Node);
            this.fItemsElement.appendChild(Node.Handle);
        } else if (Index >= 0) {
            RefNode = this.fItems[Index];
            tp.ListInsert(this.fItems, Index, Node);
            this.fItemsElement.insertBefore(Node.Handle, RefNode.Handle);
        }
        Node.ParentTreeNode = this;
    }
    /**
     * Removes a child node.
     * @param {tp.TreeNode} Node The child node.
     * @returns {void}
     */
    Remove(Node) {
        if (!this.Contains(Node))
            return;
        if (Node.Handle.parentNode)
            Node.Handle.parentNode.removeChild(Node.Handle);
        tp.ListRemove(this.fItems, Node);
        Node.ParentTreeNode = null;
        if (this.Count === 0)
            tp.RemoveClass(this.Handle, tp.Classes.Expanded);
    }
    /**
     * Removes a child node by index.
     * @param {number} Index The child index.
     * @returns {void}
     */
    RemoveAt(Index) {
        if (tp.InRange(this.fItems, Index))
            this.Remove(this.fItems[Index]);
    }
    /**
     * Creates and adds a child node.
     * @param {string} Text The display text.
     * @returns {tp.TreeNode} Returns the new node.
     */
    AddNode(Text) {
        return this.InsertNode(this.Count, Text);
    }
    /**
     * Creates and inserts a child node.
     * @param {number} Index The child index.
     * @param {string} Text The display text.
     * @returns {tp.TreeNode} Returns the new node.
     */
    InsertNode(Index, Text) {
        var Element = this.Handle.ownerDocument.createElement("div");
        var Result = new tp.TreeNode(Element);
        Result.Text = Text;
        this.Insert(Index, Result);
        return Result;
    }
    /**
     * Collapses root nodes.
     * @returns {void}
     */
    Collapse() {
        var i;
        for (i = 0; i < this.fItems.length; i++)
            this.fItems[i].Collapse();
        tp.RemoveClass(this.Handle, tp.Classes.Expanded);
    }
    /**
     * Expands root nodes.
     * @returns {void}
     */
    Expand() {
        var i;
        for (i = 0; i < this.fItems.length; i++)
            this.fItems[i].Expand();
        if (this.HasChildren)
            tp.AddClass(this.Handle, tp.Classes.Expanded);
    }
    /**
     * Toggles root nodes.
     * @returns {void}
     */
    Toggle() {
        if (this.IsExpanded)
            this.Collapse();
        else
            this.Expand();
    }
    /**
     * Collapses all nodes.
     * @returns {void}
     */
    CollapseAll() {
        var i;
        for (i = 0; i < this.fItems.length; i++)
            this.fItems[i].CollapseAll();
        tp.RemoveClass(this.Handle, tp.Classes.Expanded);
    }
    /**
     * Expands all nodes.
     * @returns {void}
     */
    ExpandAll() {
        var i;
        for (i = 0; i < this.fItems.length; i++)
            this.fItems[i].ExpandAll();
        if (this.HasChildren)
            tp.AddClass(this.Handle, tp.Classes.Expanded);
    }

    // ● properties
    /**
     * Gets null. Tree view is the root.
     * @returns {null} Returns null.
     */
    get ParentTreeNode() {
        return null;
    }
    /**
     * Ignores parent assignment. Tree view is the root.
     * @param {*} Value Ignored.
     * @returns {void}
     */
    set ParentTreeNode(Value) {
    }
    /**
     * Gets the child count.
     * @returns {number} Returns child count.
     */
    get Count() {
        return this.fItems.length;
    }
    /**
     * Returns true when this tree has children.
     * @returns {boolean} Returns true when this tree has children.
     */
    get HasChildren() {
        return this.Count > 0;
    }
    /**
     * Returns true. Tree view is the root.
     * @returns {boolean} Returns true.
     */
    get IsRoot() {
        return true;
    }
    /**
     * Returns false. Tree view is not a normal node.
     * @returns {boolean} Returns false.
     */
    get IsNode() {
        return false;
    }
    /**
     * Returns false. Tree view is not a leaf.
     * @returns {boolean} Returns false.
     */
    get IsLeaf() {
        return false;
    }
    /**
     * Returns true when the tree root is expanded.
     * @returns {boolean} Returns true when expanded.
     */
    get IsExpanded() {
        return tp.HasClass(this.Handle, tp.Classes.Expanded);
    }
    /**
     * Gets the root level.
     * @returns {number} Returns zero.
     */
    get Level() {
        return 0;
    }
    /**
     * Gets the root index.
     * @returns {number} Returns -1.
     */
    get Index() {
        return -1;
    }
    /**
     * Gets or sets the tree width.
     * @returns {string} Returns the CSS width.
     */
    get Width() {
        return this.Handle instanceof HTMLElement ? this.Handle.style.width || "" : "";
    }
    /**
     * Gets or sets the tree width.
     * @param {string|number|null|undefined} Value The width.
     * @returns {void}
     */
    set Width(Value) {
        if (this.Handle instanceof HTMLElement)
            this.Handle.style.width = tp.IsNumber(Value) ? tp.px(Value) : tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the tree height.
     * @returns {string} Returns the CSS height.
     */
    get Height() {
        return this.Handle instanceof HTMLElement ? this.Handle.style.height || "" : "";
    }
    /**
     * Gets or sets the tree height.
     * @param {string|number|null|undefined} Value The height.
     * @returns {void}
     */
    set Height(Value) {
        if (this.Handle instanceof HTMLElement)
            this.Handle.style.height = tp.IsNumber(Value) ? tp.px(Value) : tp.IsNil(Value) ? "" : String(Value);
    }

    // ● event triggers
    /**
     * Triggers NodeClick and toggles node expansion.
     * @param {tp.TreeNode} Node The clicked node.
     * @returns {void}
     */
    OnNodeClick(Node) {
        var Args;
        if (Node instanceof tp.TreeNode) {
            this.SetFocusedNode(Node);
            Args = new tp.TreeViewEventArgs(Node);
            Args.EventName = "NodeClick";
            this.Trigger("NodeClick", Args);
            if (Node.IsNode)
                Node.Toggle();
        }
    }
    /**
     * Triggers Collapsing.
     * @param {tp.TreeNode} Node The node.
     * @returns {void}
     */
    OnCollapsing(Node) {
        var Args = new tp.TreeViewEventArgs(Node);
        Args.EventName = "Collapsing";
        this.Trigger("Collapsing", Args);
    }
    /**
     * Triggers Collapsed.
     * @param {tp.TreeNode} Node The node.
     * @returns {void}
     */
    OnCollapsed(Node) {
        var Args = new tp.TreeViewEventArgs(Node);
        Args.EventName = "Collapsed";
        this.Trigger("Collapsed", Args);
    }
    /**
     * Triggers Expanding.
     * @param {tp.TreeNode} Node The node.
     * @returns {void}
     */
    OnExpanding(Node) {
        var Args = new tp.TreeViewEventArgs(Node);
        Args.EventName = "Expanding";
        this.Trigger("Expanding", Args);
    }
    /**
     * Triggers Expanded.
     * @param {tp.TreeNode} Node The node.
     * @returns {void}
     */
    OnExpanded(Node) {
        var Args = new tp.TreeViewEventArgs(Node);
        Args.EventName = "Expanded";
        this.Trigger("Expanded", Args);
    }
};

tp.Ui.RegisterType(["TreeView", "tp-TreeView"], tp.TreeView);
