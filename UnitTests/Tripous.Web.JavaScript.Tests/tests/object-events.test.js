import { describe, expect, test } from "vitest";

describe("tp.Object events", () => {
    test("Trigger preserves the custom Tripous event name", () => {
        const Sender = new tp.Object();
        let CapturedArgs = null;

        Sender.On("CustomEvent", Args => {
            CapturedArgs = Args;
        });

        const Result = Sender.Trigger("CustomEvent", { Value: 42 });

        expect(Result).toBe(CapturedArgs);
        expect(CapturedArgs.EventName).toBe("CustomEvent");
        expect(CapturedArgs.Sender).toBe(Sender);
        expect(CapturedArgs.Value).toBe(42);
    });
    test("Once removes the listener after the first trigger", () => {
        const Sender = new tp.Object();
        let Count = 0;

        Sender.Once("Ping", () => {
            Count++;
        });

        Sender.Trigger("Ping");
        Sender.Trigger("Ping");

        expect(Count).toBe(1);
        expect(Sender.HasListeners("Ping")).toBe(false);
    });
    test("EventsEnabled suppresses and restores event dispatch", () => {
        const Sender = new tp.Object();
        let Count = 0;

        Sender.On("Ping", () => {
            Count++;
        });

        Sender.EventsEnabled = false;
        Sender.Trigger("Ping");
        Sender.EventsEnabled = true;
        Sender.Trigger("Ping");

        expect(Count).toBe(1);
    });
});
