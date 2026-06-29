import { describe, expect, test } from "vitest";

describe("tp.DataSet", () => {
    test("serializes and deserializes tables", () => {
        const DataSet = new tp.DataSet("Sales");
        const Customers = DataSet.AddTable("Customers");
        const Orders = DataSet.AddTable("Orders");

        Customers.AutoGenerateGuidKeys = false;
        Customers.AddColumn("Id", tp.DataType.String);
        Customers.AddColumn("Name", tp.DataType.String);
        Customers.AddRow(["C-100", "Northwind"]);

        Orders.AutoGenerateGuidKeys = false;
        Orders.AddColumn("Id", tp.DataType.String);
        Orders.AddColumn("CustomerId", tp.DataType.String);
        Orders.AddColumn("Amount", tp.DataType.Decimal);
        Orders.AddRow(["O-100", "C-100", 1280.5]);

        DataSet.AcceptChanges();

        const Copy = new tp.DataSet(JSON.parse(JSON.stringify(DataSet)));

        expect(Copy.Name).toBe("Sales");
        expect(Copy.TableCount).toBe(2);
        expect(Copy.FindTable("Customers").Rows[0].Get("Name")).toBe("Northwind");
        expect(Copy.FindTable("Orders").Rows[0].Get("Amount")).toBe(1280.5);
        expect(Copy.FindTable("Orders").Rows[0].State).toBe(tp.DataRowState.Unchanged);
    });
});
