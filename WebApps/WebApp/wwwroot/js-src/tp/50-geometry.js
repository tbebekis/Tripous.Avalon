// ● point
/**
 * Represents a two-dimensional point.
 */
tp.Point = class {
    // ● constructor
    /**
     * Creates a point.
     * @param {number} X The X coordinate.
     * @param {number} Y The Y coordinate.
     */
    constructor(X, Y) {
        this.X = tp.ToInt(X);
        this.Y = tp.ToInt(Y);
    }

    // ● public
    /**
     * Clears this instance.
     * @returns {void}
     */
    Clear() {
        this.X = 0;
        this.Y = 0;
    }
    /**
     * Adds values to this instance.
     * @param {number} X The X value to add.
     * @param {number} Y The Y value to add.
     * @returns {void}
     */
    Add(X, Y) {
        this.X += tp.ToInt(X);
        this.Y += tp.ToInt(Y);
    }
    /**
     * Subtracts values from this instance.
     * @param {number} X The X value to subtract.
     * @param {number} Y The Y value to subtract.
     * @returns {void}
     */
    Subtract(X, Y) {
        this.X -= tp.ToInt(X);
        this.Y -= tp.ToInt(Y);
    }
    /**
     * Returns true when this instance equals the specified coordinates.
     * @param {number} X The X coordinate.
     * @param {number} Y The Y coordinate.
     * @returns {boolean} Returns true when this instance equals the specified coordinates.
     */
    Equals(X, Y) {
        return this.X === tp.ToInt(X) && this.Y === tp.ToInt(Y);
    }
    /**
     * Returns true when this point is greater than or equal to the specified coordinates.
     * @param {number} X The X coordinate.
     * @param {number} Y The Y coordinate.
     * @returns {boolean} Returns true when this point is greater than or equal to the specified coordinates.
     */
    Greater(X, Y) {
        return this.X >= tp.ToInt(X) && this.Y >= tp.ToInt(Y);
    }
    /**
     * Returns true when this point is less than or equal to the specified coordinates.
     * @param {number} X The X coordinate.
     * @param {number} Y The Y coordinate.
     * @returns {boolean} Returns true when this point is less than or equal to the specified coordinates.
     */
    Less(X, Y) {
        return this.X <= tp.ToInt(X) && this.Y <= tp.ToInt(Y);
    }
    /**
     * Returns true when this point is between two points.
     * @param {number} X1 The first point X coordinate.
     * @param {number} Y1 The first point Y coordinate.
     * @param {number} X2 The second point X coordinate.
     * @param {number} Y2 The second point Y coordinate.
     * @returns {boolean} Returns true when this point is between two points.
     */
    IsInBetween(X1, Y1, X2, Y2) {
        return this.Greater(X1, Y1) && this.Less(X2, Y2);
    }
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns a string representation of this instance.
     */
    toString() {
        return "x=" + this.X + ", y=" + this.Y;
    }
};
/**
 * Returns true when a point is contained by a rectangle.
 * @param {{X: number, Y: number}} Point The point to test.
 * @param {{X: number, Y: number, Width: number, Height: number}} Rect The rectangle to check.
 * @returns {boolean} Returns true when the point is contained by the rectangle.
 */
tp.Point.PointInRect = function (Point, Rect) {
    return Point.X >= Rect.X && Point.X <= Rect.X + Rect.Width && Point.Y >= Rect.Y && Point.Y <= Rect.Y + Rect.Height;
};

// ● size
/**
 * Represents a two-dimensional size.
 */
tp.Size = class {
    // ● constructor
    /**
     * Creates a size.
     * @param {number} Width The width.
     * @param {number} Height The height.
     */
    constructor(Width, Height) {
        this.Width = tp.ToInt(Width);
        this.Height = tp.ToInt(Height);
    }

    // ● public
    /**
     * Clears this instance.
     * @returns {void}
     */
    Clear() {
        this.Width = 0;
        this.Height = 0;
    }
    /**
     * Adds values to this instance.
     * @param {number} Width The width to add.
     * @param {number} Height The height to add.
     * @returns {void}
     */
    Add(Width, Height) {
        this.Width += tp.ToInt(Width);
        this.Height += tp.ToInt(Height);
    }
    /**
     * Subtracts values from this instance.
     * @param {number} Width The width to subtract.
     * @param {number} Height The height to subtract.
     * @returns {void}
     */
    Subtract(Width, Height) {
        this.Width -= tp.ToInt(Width);
        this.Height -= tp.ToInt(Height);
    }
    /**
     * Returns true when this instance equals the specified size.
     * @param {number} Width The width.
     * @param {number} Height The height.
     * @returns {boolean} Returns true when this instance equals the specified size.
     */
    Equals(Width, Height) {
        return this.Width === tp.ToInt(Width) && this.Height === tp.ToInt(Height);
    }
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns a string representation of this instance.
     */
    toString() {
        return "width=" + this.Width + ", height=" + this.Height;
    }
};

// ● rect
/**
 * Represents a rectangle.
 */
tp.Rect = class {
    // ● constructor
    /**
     * Creates a rectangle.
     * @param {number} X The left coordinate.
     * @param {number} Y The top coordinate.
     * @param {number} Width The width.
     * @param {number} Height The height.
     */
    constructor(X, Y, Width, Height) {
        this.X = tp.ToInt(X);
        this.Y = tp.ToInt(Y);
        this.Width = tp.ToInt(Width);
        this.Height = tp.ToInt(Height);
    }

    // ● properties
    /**
     * Gets or sets the right coordinate.
     * @returns {number} Returns the right coordinate.
     */
    get Right() {
        return this.X + this.Width;
    }
    /**
     * Gets or sets the right coordinate.
     * @param {number} Value The right coordinate.
     * @returns {void}
     */
    set Right(Value) {
        this.Width = tp.ToInt(Value) - this.X;
    }
    /**
     * Gets or sets the bottom coordinate.
     * @returns {number} Returns the bottom coordinate.
     */
    get Bottom() {
        return this.Y + this.Height;
    }
    /**
     * Gets or sets the bottom coordinate.
     * @param {number} Value The bottom coordinate.
     * @returns {void}
     */
    set Bottom(Value) {
        this.Height = tp.ToInt(Value) - this.Y;
    }

    // ● public
    /**
     * Clears this instance.
     * @returns {void}
     */
    Clear() {
        this.X = 0;
        this.Y = 0;
        this.Width = 0;
        this.Height = 0;
    }
    /**
     * Returns true when this instance equals the specified rectangle.
     * @param {number} X The left coordinate.
     * @param {number} Y The top coordinate.
     * @param {number} Width The width.
     * @param {number} Height The height.
     * @returns {boolean} Returns true when this instance equals the specified rectangle.
     */
    Equals(X, Y, Width, Height) {
        return this.X === tp.ToInt(X) && this.Y === tp.ToInt(Y) && this.Width === tp.ToInt(Width) && this.Height === tp.ToInt(Height);
    }
    /**
     * Returns true when this rectangle contains a point, rectangle, or coordinates.
     * @param {...*} Args A point, rectangle, X/Y pair, or X/Y/Width/Height values.
     * @returns {boolean} Returns true when this rectangle contains the specified value.
     */
    Contains() {
        var X;
        var Y;
        var Width;
        var Height;
        var Value;
        if (arguments.length === 1) {
            Value = arguments[0];
            if (tp.IsNil(Value))
                return false;
            if ("Width" in Value) {
                return this.X <= Value.X && this.Y <= Value.Y && this.Right >= Value.X + Value.Width && this.Bottom >= Value.Y + Value.Height;
            }
            return Value.X >= this.X && Value.X <= this.Right && Value.Y >= this.Y && Value.Y <= this.Bottom;
        }
        if (arguments.length === 2) {
            X = tp.ToInt(arguments[0]);
            Y = tp.ToInt(arguments[1]);
            return X >= this.X && X <= this.Right && Y >= this.Y && Y <= this.Bottom;
        }
        if (arguments.length === 4) {
            X = tp.ToInt(arguments[0]);
            Y = tp.ToInt(arguments[1]);
            Width = tp.ToInt(arguments[2]);
            Height = tp.ToInt(arguments[3]);
            return this.X <= X && this.Y <= Y && this.Right >= X + Width && this.Bottom >= Y + Height;
        }
        return false;
    }
    /**
     * Inflates this rectangle.
     * @param {number} Width The width to inflate.
     * @param {number} Height The height to inflate.
     * @returns {void}
     */
    Inflate(Width, Height) {
        Width = tp.ToInt(Width);
        Height = tp.ToInt(Height);
        this.X -= Width;
        this.Y -= Height;
        this.Width += 2 * Width;
        this.Height += 2 * Height;
    }
    /**
     * Returns true when this rectangle intersects another rectangle.
     * @param {...*} Args A rectangle or X/Y/Width/Height values.
     * @returns {boolean} Returns true when the rectangles intersect.
     */
    IntersectsWith() {
        var R = tp.Rect.FromArguments(arguments);
        return R.Right > this.X && this.Right > R.X && R.Bottom > this.Y && this.Bottom > R.Y;
    }
    /**
     * Replaces this rectangle with its intersection with another rectangle.
     * @param {...*} Args A rectangle or X/Y/Width/Height values.
     * @returns {void}
     */
    Intersect() {
        var R = tp.Rect.FromArguments(arguments);
        var X1 = Math.max(this.X, R.X);
        var Y1 = Math.max(this.Y, R.Y);
        var X2 = Math.min(this.Right, R.Right);
        var Y2 = Math.min(this.Bottom, R.Bottom);
        if (X2 >= X1 && Y2 >= Y1) {
            this.X = X1;
            this.Y = Y1;
            this.Width = X2 - X1;
            this.Height = Y2 - Y1;
        } else {
            this.Clear();
        }
    }
    /**
     * Moves this rectangle to a location.
     * @param {...*} Args A point or X/Y values.
     * @returns {void}
     */
    Offset() {
        if (arguments.length === 1) {
            this.X = tp.ToInt(arguments[0].X);
            this.Y = tp.ToInt(arguments[0].Y);
        } else if (arguments.length === 2) {
            this.X = tp.ToInt(arguments[0]);
            this.Y = tp.ToInt(arguments[1]);
        }
    }
    /**
     * Replaces this rectangle with its union with another rectangle.
     * @param {...*} Args A rectangle or X/Y/Width/Height values.
     * @returns {void}
     */
    Union() {
        var R = tp.Rect.FromArguments(arguments);
        var X1 = Math.min(this.X, R.X);
        var Y1 = Math.min(this.Y, R.Y);
        var X2 = Math.max(this.Right, R.Right);
        var Y2 = Math.max(this.Bottom, R.Bottom);
        this.X = X1;
        this.Y = Y1;
        this.Width = X2 - X1;
        this.Height = Y2 - Y1;
    }
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns a string representation of this instance.
     */
    toString() {
        return "x=" + this.X + ", y=" + this.Y + ", width=" + this.Width + ", height=" + this.Height;
    }
};
/**
 * Creates a rectangle from arguments.
 * @param {IArguments|Array} Args The arguments to read.
 * @returns {tp.Rect} Returns a rectangle.
 */
tp.Rect.FromArguments = function (Args) {
    if (Args.length === 1)
        return tp.Rect.FromValue(Args[0]);
    return new tp.Rect(Args[0], Args[1], Args[2], Args[3]);
};
/**
 * Creates a rectangle from a value.
 * @param {*} Value The source value.
 * @returns {tp.Rect} Returns a rectangle.
 */
tp.Rect.FromValue = function (Value) {
    if (Value instanceof tp.Rect)
        return new tp.Rect(Value.X, Value.Y, Value.Width, Value.Height);
    if (!tp.IsNil(Value) && "X" in Value && "Y" in Value && "Width" in Value && "Height" in Value)
        return new tp.Rect(Value.X, Value.Y, Value.Width, Value.Height);
    return new tp.Rect();
};
/**
 * Creates a rectangle from an element or DOMRect.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/DOMRect
 * @param {Element|DOMRect|object} Value The element or rectangle-like value.
 * @returns {tp.Rect} Returns a rectangle.
 */
tp.Rect.FromClientRect = function (Value) {
    var Rect;
    if (tp.IsElement(Value)) {
        Rect = Value.getBoundingClientRect();
        return new tp.Rect(Rect.left, Rect.top, Rect.width, Rect.height);
    }
    if (!tp.IsNil(Value) && "left" in Value && "top" in Value && "width" in Value && "height" in Value)
        return new tp.Rect(Value.left, Value.top, Value.width, Value.height);
    return new tp.Rect();
};

// ● edge
/**
 * Edge constants and helpers for resize hit-testing.
 * @type {object}
 */
tp.Edge = {
    None: 0,
    N: 1,
    E: 2,
    W: 4,
    S: 8,
    NE: 0x10,
    NW: 0x20,
    SE: 0x40,
    SW: 0x80,
    /**
     * Gets all edge flags.
     * @returns {number} Returns all edge flags.
     */
    get All() {
        return tp.Edge.N | tp.Edge.E | tp.Edge.W | tp.Edge.S | tp.Edge.NE | tp.Edge.NW | tp.Edge.SE | tp.Edge.SW;
    },
    /**
     * Gets the flags affecting height.
     * @returns {number} Returns the height edge flags.
     */
    get Height() {
        return tp.Bf.Subtract(tp.Edge.All, tp.Edge.E | tp.Edge.W);
    },
    /**
     * Gets the flags affecting width.
     * @returns {number} Returns the width edge flags.
     */
    get Width() {
        return tp.Bf.Subtract(tp.Edge.All, tp.Edge.N | tp.Edge.S);
    },
    /**
     * Gets the left-side edge flags.
     * @returns {number} Returns the left-side edge flags.
     */
    get Left() {
        return tp.Edge.NW | tp.Edge.W | tp.Edge.SW;
    },
    /**
     * Gets the top-side edge flags.
     * @returns {number} Returns the top-side edge flags.
     */
    get Top() {
        return tp.Edge.NW | tp.Edge.N | tp.Edge.NE;
    },
    /**
     * Gets the right-side edge flags.
     * @returns {number} Returns the right-side edge flags.
     */
    get Right() {
        return tp.Edge.NE | tp.Edge.E | tp.Edge.SE;
    },
    /**
     * Gets the bottom-side edge flags.
     * @returns {number} Returns the bottom-side edge flags.
     */
    get Bottom() {
        return tp.Edge.SW | tp.Edge.S | tp.Edge.SE;
    },
    /**
     * Returns true when an edge affects height.
     * @param {number} Value The edge value.
     * @returns {boolean} Returns true when the edge affects height.
     */
    IsHeight: function (Value) {
        return tp.Bf.In(Value, tp.Edge.Height);
    },
    /**
     * Returns true when an edge affects height.
     * @param {number} Value The edge value.
     * @returns {boolean} Returns true when the edge affects height.
     */
    IsHeigth: function (Value) {
        return tp.Edge.IsHeight(Value);
    },
    /**
     * Returns true when an edge affects width.
     * @param {number} Value The edge value.
     * @returns {boolean} Returns true when the edge affects width.
     */
    IsWidth: function (Value) {
        return tp.Bf.In(Value, tp.Edge.Width);
    },
    /**
     * Returns true when an edge is on the left side.
     * @param {number} Value The edge value.
     * @returns {boolean} Returns true when the edge is on the left side.
     */
    IsLeft: function (Value) {
        return tp.Bf.In(Value, tp.Edge.Left);
    },
    /**
     * Returns true when an edge is on the top side.
     * @param {number} Value The edge value.
     * @returns {boolean} Returns true when the edge is on the top side.
     */
    IsTop: function (Value) {
        return tp.Bf.In(Value, tp.Edge.Top);
    },
    /**
     * Returns true when an edge is on the right side.
     * @param {number} Value The edge value.
     * @returns {boolean} Returns true when the edge is on the right side.
     */
    IsRight: function (Value) {
        return tp.Bf.In(Value, tp.Edge.Right);
    },
    /**
     * Returns true when an edge is on the bottom side.
     * @param {number} Value The edge value.
     * @returns {boolean} Returns true when the edge is on the bottom side.
     */
    IsBottom: function (Value) {
        return tp.Bf.In(Value, tp.Edge.Bottom);
    },
    /**
     * Converts an edge value to a CSS cursor value.
     * @param {number} Value The edge value.
     * @returns {string} Returns a tp.Cursors value.
     */
    ToCursor: function (Value) {
        switch (Value) {
            case tp.Edge.NE: return tp.Cursors.ResizeNE;
            case tp.Edge.NW: return tp.Cursors.ResizeNW;
            case tp.Edge.SE: return tp.Cursors.ResizeSE;
            case tp.Edge.SW: return tp.Cursors.ResizeSW;
            case tp.Edge.N: return tp.Cursors.ResizeN;
            case tp.Edge.E: return tp.Cursors.ResizeE;
            case tp.Edge.W: return tp.Cursors.ResizeW;
            case tp.Edge.S: return tp.Cursors.ResizeS;
            default: return tp.Cursors.Default;
        }
    },
    /**
     * Performs resize edge hit-testing on an element.
     * See: https://developer.mozilla.org/en-US/docs/Web/API/Element/getBoundingClientRect
     * @param {MouseEvent|PointerEvent} e The mouse or pointer event.
     * @param {Element|string} Selector The target selector or element.
     * @param {number} HandleSize The resize handle size in pixels.
     * @returns {number} Returns one of the tp.Edge constants.
     */
    ResizeHitTest: function (e, Selector, HandleSize) {
        var Element = tp(Selector);
        var Size = tp.IsNumber(HandleSize) && HandleSize > 0 ? HandleSize : 8;
        var Rect;
        var X;
        var Y;
        var InLeft;
        var InRight;
        var InTop;
        var InBottom;
        if (!tp.IsElement(Element) || tp.IsNil(e) || !tp.IsNumber(e.clientX) || !tp.IsNumber(e.clientY))
            return tp.Edge.None;
        Rect = Element.getBoundingClientRect();
        X = e.clientX - Rect.left;
        Y = e.clientY - Rect.top;
        if (X < 0 || Y < 0 || X > Rect.width || Y > Rect.height)
            return tp.Edge.None;
        InLeft = X <= Size;
        InRight = X >= Rect.width - Size;
        InTop = Y <= Size;
        InBottom = Y >= Rect.height - Size;
        if (InTop && InRight)
            return tp.Edge.NE;
        if (InTop && InLeft)
            return tp.Edge.NW;
        if (InBottom && InRight)
            return tp.Edge.SE;
        if (InBottom && InLeft)
            return tp.Edge.SW;
        if (InTop)
            return tp.Edge.N;
        if (InRight)
            return tp.Edge.E;
        if (InLeft)
            return tp.Edge.W;
        if (InBottom)
            return tp.Edge.S;
        return tp.Edge.None;
    }
};
Object.freeze(tp.Edge);
