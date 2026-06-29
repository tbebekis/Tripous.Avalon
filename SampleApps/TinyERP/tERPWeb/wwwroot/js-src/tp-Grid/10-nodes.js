// ● node type
/**
 * Indicates the type of a grid node.
 * @enum {number}
 */
tp.GridNodeType = {
    None: 0,
    Group: 1,
    Row: 2,
    Footer: 4
};
Object.freeze(tp.GridNodeType);

// ● node
/**
 * Internal grid node.
 */
tp.GridNode = class {
    // ● constructor
    /**
     * Creates a grid node.
     * @param {tp.Grid} Grid The owner grid.
     * @param {tp.GridNode|null} Parent The parent node.
     * @param {number} Type A tp.GridNodeType value.
     * @param {tp.DataRow|null} Row The data row.
     */
    constructor(Grid, Parent, Type, Row) {
        this.Grid = Grid;
        this.Parent = Parent;
        this.Type = Type;
        this.Row = Row;
        this.tpClass = "tp.GridNode";
        this.IsRoot = false;
        this.IsGroup = false;
        this.IsRow = false;
        this.IsFooter = false;
        this.IsExpanded = true;
        this.Level = Parent === null ? -1 : Parent.Level + 1;
        if (Type === tp.GridNodeType.Group)
            this.IsGroup = true;
        else if (Type === tp.GridNodeType.Row)
            this.IsRow = true;
        else if (Type === tp.GridNodeType.Footer)
            this.IsFooter = true;
        this.List = [];
        this.Aggregates = [];
        this.Footer = this.IsGroup ? new tp.GridNode(Grid, this, tp.GridNodeType.Footer, Row) : null;
    }

    // ● protected
    /**
     * Creates and adds a child group node.
     * @protected
     * @param {*} Key The group key.
     * @returns {tp.GridNode} Returns the new group node.
     */
    AddGroup(Key) {
        var Result = new tp.GridNode(this.Grid, this, tp.GridNodeType.Group, null);
        this.List.push(Result);
        Result.Key = Key;
        return Result;
    }
    /**
     * Adds data rows as child row nodes.
     * @protected
     * @param {tp.DataRow[]} RowList The row list.
     * @returns {void}
     */
    AddRowList(RowList) {
        var Index;
        var Child;
        for (Index = 0; Index < RowList.length; Index++) {
            Child = new tp.GridNode(this.Grid, this, tp.GridNodeType.Row, RowList[Index]);
            this.List.push(Child);
        }
    }

    // ● public
    /**
     * Expands or collapses this group node.
     * @param {boolean} Flag True to expand.
     * @returns {boolean} Returns true when the operation was valid and changed state.
     */
    Expand(Flag) {
        var Result = false;
        if (!this.IsRoot && this.IsGroup) {
            Flag = Flag === true;
            if (Flag !== this.IsExpanded) {
                this.IsExpanded = Flag;
                Result = true;
            }
        }
        if (Result)
            this.Grid.ToggleNode(this);
        return Result;
    }
    /**
     * Toggles this group node.
     * @returns {void}
     */
    Toggle() {
        this.Expand(!this.IsExpanded);
    }
    /**
     * Returns the number of visible nodes under this node.
     * @returns {number} Returns the visible node count.
     */
    GetNodeListCount() {
        var Result = 0;
        var Index;
        if (this.IsGroup) {
            if (!this.IsRoot)
                Result = 1;
            if (this.IsExpanded) {
                if (this.List.length > 0) {
                    if (this.List[0].IsGroup) {
                        for (Index = 0; Index < this.List.length; Index++)
                            Result += this.List[Index].GetNodeListCount();
                    } else {
                        Result += this.List.length;
                    }
                }
                if (!this.IsRoot && this.Grid.GroupFooterVisible)
                    Result++;
            }
        }
        return Result;
    }
    /**
     * Recreates the flat visible node list.
     * @returns {void}
     */
    UpdateNodeList() {
        var Index;
        var Child;
        if (this.IsGroup) {
            if (!this.IsRoot)
                this.Grid.NodeList.push(this);
            if (this.IsExpanded) {
                if (this.List.length > 0) {
                    if (this.List[0].IsGroup) {
                        for (Index = 0; Index < this.List.length; Index++) {
                            Child = this.List[Index];
                            Child.UpdateNodeList();
                        }
                    } else {
                        for (Index = 0; Index < this.List.length; Index++) {
                            Child = this.List[Index];
                            this.Grid.NodeList.push(Child);
                        }
                    }
                }
                if (!this.IsRoot && this.Grid.GroupFooterVisible)
                    this.Grid.NodeList.push(this.Footer);
            }
        }
    }
    /**
     * Returns an aggregate value for a column.
     * @param {tp.GridColumn} Column The grid column.
     * @param {number|null|undefined} AggregateType A tp.AggregateType value.
     * @returns {*} Returns the aggregate value.
     */
    GetAggregateValue(Column, AggregateType) {
        var Index;
        var Value;
        var Result;
        AggregateType = AggregateType || Column.fAggregate;
        switch (AggregateType) {
            case tp.AggregateType.Count:
                if (this.IsRow)
                    return 1;
                if (this.IsRoot || this.IsGroup) {
                    Result = 0;
                    for (Index = 0; Index < this.List.length; Index++)
                        Result += this.List[Index].GetAggregateValue(Column, AggregateType);
                    return Result;
                }
                break;
            case tp.AggregateType.Sum:
                if (this.IsRow)
                    return this.Row.Get(Column.DataColumn);
                if (this.IsRoot || this.IsGroup) {
                    Result = 0;
                    for (Index = 0; Index < this.List.length; Index++)
                        Result += this.List[Index].GetAggregateValue(Column, AggregateType);
                    return Result;
                }
                break;
            case tp.AggregateType.Min:
                if (this.IsRow)
                    return this.Row.Get(Column.DataColumn);
                if (this.IsRoot || this.IsGroup) {
                    Result = null;
                    for (Index = 0; Index < this.List.length; Index++) {
                        Value = this.List[Index].GetAggregateValue(Column, AggregateType);
                        if (!tp.IsEmpty(Value)) {
                            if (tp.IsEmpty(Result))
                                Result = Value;
                            else if (Value < Result)
                                Result = Value;
                        }
                    }
                    return Result;
                }
                break;
            case tp.AggregateType.Max:
                if (this.IsRow)
                    return this.Row.Get(Column.DataColumn);
                if (this.IsRoot || this.IsGroup) {
                    Result = null;
                    for (Index = 0; Index < this.List.length; Index++) {
                        Value = this.List[Index].GetAggregateValue(Column, AggregateType);
                        if (!tp.IsEmpty(Value)) {
                            if (tp.IsEmpty(Result))
                                Result = Value;
                            else if (Value > Result)
                                Result = Value;
                        }
                    }
                    return Result;
                }
                break;
            case tp.AggregateType.Avg:
                Result = this.GetAggregateValue(Column, tp.AggregateType.Sum);
                if (this.IsRoot || this.IsGroup) {
                    Value = this.GetAggregateValue(Column, tp.AggregateType.Count);
                    if (!tp.IsEmpty(Value) && Value > 0)
                        Result = Math.ceil(Result / Value);
                }
                return Result;
        }
        return null;
    }
    /**
     * Returns aggregate display text for a column.
     * @param {tp.GridColumn} Column The grid column.
     * @returns {string} Returns the aggregate text.
     */
    GetAggregateText(Column) {
        var Value;
        var Result = "";
        if (Column.fAggregate !== tp.AggregateType.None) {
            Value = this.GetAggregateValue(Column);
            switch (Column.fAggregate) {
                case tp.AggregateType.Count:
                    Result = "count=" + (Value ? Value.toString() : "0");
                    break;
                case tp.AggregateType.Sum:
                    Result = "sum=" + Column.Format(Value);
                    break;
                case tp.AggregateType.Min:
                    Result = "min=" + Column.Format(Value);
                    break;
                case tp.AggregateType.Max:
                    Result = "max=" + Column.Format(Value);
                    break;
                case tp.AggregateType.Avg:
                    Result = "avg=" + Column.Format(Value);
                    break;
            }
        }
        return Result;
    }

    // ● properties
    /**
     * Returns the first child node.
     * @returns {tp.GridNode|null} Returns the first child node.
     */
    get First() {
        return this.List.length > 0 ? this.List[0] : null;
    }
    /**
     * Returns the last child node.
     * @returns {tp.GridNode|null} Returns the last child node.
     */
    get Last() {
        return this.List.length > 0 ? this.List[this.List.length - 1] : null;
    }
    /**
     * Returns true if this is the first child in its parent.
     * @returns {boolean} Returns true when first.
     */
    get IsFirst() {
        return this.Parent ? this === this.Parent.First : false;
    }
    /**
     * Returns true if this is the last child in its parent.
     * @returns {boolean} Returns true when last.
     */
    get IsLast() {
        return this.Parent ? this === this.Parent.Last : false;
    }
    /**
     * Returns true if this is the last group node in its parent.
     * @returns {boolean} Returns true when last group.
     */
    get IsLastGroup() {
        return (this.IsRoot && this.Grid.GroupColumnCount === 0)
            || (this.IsGroup && this.Grid.GroupColumnCount === this.Level - 1);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.GridNode.prototype.tpClass = "tp.GridNode";

// ● root node
/**
 * Internal grid root node.
 */
tp.GridRootNode = class extends tp.GridNode {
    // ● constructor
    /**
     * Creates a grid root node.
     * @param {tp.Grid} Grid The owner grid.
     */
    constructor(Grid) {
        super(Grid, null, tp.GridNodeType.Group, null);
        this.tpClass = "tp.GridRootNode";
        this.IsRoot = true;
    }

    // ● public
    /**
     * Clears child nodes and aggregates.
     * @returns {void}
     */
    Clear() {
        if (this.List)
            this.List.length = 0;
        if (this.Aggregates)
            this.Aggregates.length = 0;
    }
    /**
     * Recreates the flat visible node list.
     * @returns {void}
     */
    UpdateNodeList() {
        this.Grid.NodeList.length = 0;
        super.UpdateNodeList();
    }
    /**
     * Builds groups and nodes.
     * @param {tp.DataRow[]} RowList The row list.
     * @returns {void}
     */
    BuildGroups(RowList) {
        var Self = this;
        var Index;
        var Props = [];
        var InitializeNodeList = function () {
            var RowIndex;
            var Node;
            for (RowIndex = 0; RowIndex < RowList.length; RowIndex++) {
                Node = new tp.GridNode(Self.Grid, Self, tp.GridNodeType.Row, RowList[RowIndex]);
                Self.Grid.NodeList.push(Node);
                Self.List.push(Node);
            }
        };
        var GroupBy = function (ParentNode, DataList, PropIndex) {
            var Prop = Props[PropIndex];
            var RowIndex;
            var Data;
            var Key;
            var Node;
            var Groups = {};
            var Keys;
            for (RowIndex = 0; RowIndex < DataList.length; RowIndex++) {
                Data = DataList[RowIndex];
                Key = Data.Data[Prop];
                if (Key in Groups === false) {
                    Groups[Key] = {
                        Key: Key,
                        List: []
                    };
                }
                Groups[Key].List.push(Data);
            }
            Keys = Object.keys(Groups);
            for (RowIndex = 0; RowIndex < Keys.length; RowIndex++) {
                Key = Keys[RowIndex];
                Node = ParentNode.AddGroup(Groups[Key].Key);
                if (PropIndex >= Props.length - 1)
                    Node.AddRowList(Groups[Key].List);
            }
            if (PropIndex < Props.length - 1) {
                for (RowIndex = 0; RowIndex < ParentNode.List.length; RowIndex++) {
                    Node = ParentNode.List[RowIndex];
                    GroupBy(Node, Groups[Node.Key].List, PropIndex + 1);
                }
            }
        };
        this.List.length = 0;
        this.Grid.NodeList.length = 0;
        if (this.Grid.GroupColumnCount === 0) {
            InitializeNodeList();
        } else {
            for (Index = 0; Index < this.Grid.GroupColumnCount; Index++)
                Props.push(this.Grid.GroupColumnByIndex(Index).DataIndex);
            GroupBy(this, RowList, 0);
            this.UpdateNodeList();
        }
    }
};
