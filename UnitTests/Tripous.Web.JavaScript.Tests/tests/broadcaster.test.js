import { beforeEach, describe, expect, test } from "vitest";

describe("tp.Broadcaster", () => {
    beforeEach(() => {
        tp.Broadcaster.fListeners.length = 0;
    });

    test("sends event arguments to registered listeners", () => {
        const Events = [];
        const Listener = {
            BroadcasterFunc(Args) {
                Events.push({
                    eventName: Args.EventName,
                    sender: Args.Sender,
                    value: Args.Value,
                    isBroadcasterMessage: Args.IsBroadcasterMessage
                });
            }
        };
        const Sender = { Name: "Sender" };

        tp.Broadcaster.Add(Listener);
        tp.Broadcaster.Send("Refresh", Sender, { Value: 123 });

        expect(Events).toEqual([
            {
                eventName: "Refresh",
                sender: Sender,
                value: 123,
                isBroadcasterMessage: true
            }
        ]);
    });
    test("does not add duplicate listeners and supports removal", () => {
        let Count = 0;
        const Listener = {
            BroadcasterFunc() {
                Count++;
            }
        };

        tp.Broadcaster.Add(Listener);
        tp.Broadcaster.Add(Listener);
        tp.Broadcaster.Send("Ping");
        tp.Broadcaster.Remove(Listener);
        tp.Broadcaster.Send("Ping");

        expect(Count).toBe(1);
    });
});
