import { beforeEach, describe, expect, test } from "vitest";

describe("tp.Local and tp.Session", () => {
    beforeEach(() => {
        window.localStorage.clear();
        window.sessionStorage.clear();
    });

    test("stores and retrieves string values", () => {
        tp.Local.Set("Tripous.Test.Name", "Tripous");

        expect(tp.Local.Available).toBe(true);
        expect(tp.Local.Get("Tripous.Test.Name")).toBe("Tripous");
        expect(tp.Local.Get("Tripous.Test.Missing", "Default")).toBe("Default");

        tp.Local.Remove("Tripous.Test.Name");
        expect(tp.Local.Get("Tripous.Test.Name", "Default")).toBe("Default");
    });
    test("stores and retrieves object values", () => {
        const Value = { Id: 1, Name: "Northwind" };

        tp.Session.SetObject("Tripous.Test.Customer", Value);

        expect(tp.Session.Available).toBe(true);
        expect(tp.Session.GetObject("Tripous.Test.Customer")).toEqual(Value);
        expect(tp.Session.GetObject("Tripous.Test.Missing", { Id: 0 })).toEqual({ Id: 0 });
    });
});
