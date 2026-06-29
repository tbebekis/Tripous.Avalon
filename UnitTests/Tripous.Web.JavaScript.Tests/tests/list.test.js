import { describe, expect, test } from "vitest";

describe("tp.List", () => {
    test("supports basic list operations", () => {
        const List = new tp.List([1, 2]);

        List.Add(4);
        List.Insert(2, 3);
        List.Remove(1);

        expect(List.Count).toBe(3);
        expect(List.ToArray()).toEqual([2, 3, 4]);
        expect(List.Contains(3)).toBe(true);
        expect(List.IndexOf(4)).toBe(2);
    });
    test("finds and removes items by property value", () => {
        const List = new tp.List([
            { Id: 1, Name: "Northwind" },
            { Id: 2, Name: "Contoso" }
        ]);

        expect(List.FindBy("Id", 2).Name).toBe("Contoso");
        expect(List.ContainsBy("Name", "Northwind")).toBe(true);
        expect(List.RemoveBy("Id", 1)).toBe(true);
        expect(List.ToArray()).toEqual([{ Id: 2, Name: "Contoso" }]);
    });
    test("raises Changing and Changed events when enabled", () => {
        const List = new tp.List();
        const Events = [];

        List.EventsEnabled = true;
        List.On("Changing", Args => Events.push("before:" + Args.Action));
        List.On("Changed", Args => Events.push("after:" + Args.Action));

        List.Add("A");
        List.Remove("A");

        expect(Events).toEqual(["before:Insert", "after:Insert", "before:Remove", "after:Remove"]);
    });
    test("supports classic enumerator", () => {
        const Enumerator = new tp.Enumerator(["A", "B"]);
        const Result = [];

        while (Enumerator.MoveNext())
            Result.push(Enumerator.Current);

        expect(Result).toEqual(["A", "B"]);
        Enumerator.Reset();
        expect(Enumerator.Current).toBeNull();
    });
});
