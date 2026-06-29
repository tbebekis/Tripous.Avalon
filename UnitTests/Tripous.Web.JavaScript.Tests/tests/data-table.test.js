import { describe, expect, test } from "vitest";

describe("tp.DataTable", () => {
    test("FromObjectList copies matching properties only", () => {
        const Table = new tp.DataTable("Customers");
        const SourceList = [
            { Id: 1, Code: "CUS-100", Name: "Northwind", Balance: 1830.45, IsActive: true, InternalNote: "Gold" },
            { Id: 2, Code: "CUS-200", Name: "Contoso", Balance: 420, IsActive: false, InternalNote: "Inactive" }
        ];

        Table.AutoGenerateGuidKeys = false;
        Table.AddColumn("Id", tp.DataType.Integer);
        Table.AddColumn("Code", tp.DataType.String);
        Table.AddColumn("Name", tp.DataType.String);
        Table.AddColumn("Balance", tp.DataType.Decimal);
        Table.AddColumn("IsActive", tp.DataType.Boolean);
        Table.FromObjectList(SourceList);

        expect(Table.RowCount).toBe(2);
        expect(Table.ColumnCount).toBe(5);
        expect(Table.Rows[0].Get("Code")).toBe("CUS-100");
        expect(Table.Rows[1].Get("Balance")).toBe(420);
        expect(Table.ContainsColumn("InternalNote")).toBe(false);
        expect(Table.ToObjectList()[0].InternalNote).toBeUndefined();
    });
    test("serialization keeps schema once and row values as arrays", () => {
        const Table = new tp.DataTable("Customers");

        Table.AutoGenerateGuidKeys = false;
        Table.KeyField = "Id";
        Table.AddColumn({ Name: "Id", DataType: tp.DataType.String, Flags: tp.FieldFlags.Required });
        Table.AddColumn({ Name: "Name", DataType: tp.DataType.String });
        Table.AddColumn({ Name: "Balance", DataType: tp.DataType.Decimal, Decimals: 2 });
        Table.AddRow(["C-100", "Northwind", 1830.45]);
        Table.AcceptChanges();

        const Json = JSON.parse(JSON.stringify(Table));

        expect(Json.Name).toBe("Customers");
        expect(Json.Columns.map(Column => Column.Name)).toEqual(["Id", "Name", "Balance"]);
        expect(Json.Rows).toEqual([{ State: tp.DataRowState.Unchanged, Data: ["C-100", "Northwind", 1830.45] }]);
        expect(Json.Rows[0].Name).toBeUndefined();

        const Copy = new tp.DataTable(Json);
        expect(Copy.Name).toBe("Customers");
        expect(Copy.RowCount).toBe(1);
        expect(Copy.Rows[0].Get("Name")).toBe("Northwind");
        expect(Copy.Rows[0].State).toBe(tp.DataRowState.Unchanged);
    });
    test("CreateFromList infers columns from a plain object list", () => {
        const Table = tp.DataTable.CreateFromList([
            { Id: 1, Name: "Northwind", Balance: 1830.45, IsActive: true },
            { Id: 2, Name: "Contoso", Balance: 420, IsActive: false }
        ]);

        expect(Table).toBeInstanceOf(tp.DataTable);
        expect(Table.ColumnCount).toBe(4);
        expect(Table.FindColumn("Id").DataType).toBe(tp.DataType.Integer);
        expect(Table.FindColumn("Name").DataType).toBe(tp.DataType.String);
        expect(Table.FindColumn("Balance").DataType).toBe(tp.DataType.Decimal);
        expect(Table.FindColumn("IsActive").DataType).toBe(tp.DataType.Boolean);
        expect(Table.Rows[1].Get("Name")).toBe("Contoso");
    });
});
