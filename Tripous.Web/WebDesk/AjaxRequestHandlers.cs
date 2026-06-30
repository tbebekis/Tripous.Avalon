/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Registry and dispatcher for Ajax request handlers.
/// </summary>
static public class AjaxRequestHandlers
{
    static readonly object syncLock = new();
    static readonly List<IAjaxRequestHandler> fHandlers = new();

    // ● static public
    /// <summary>
    /// Finds and returns a registered handler by name, if any; otherwise null.
    /// </summary>
    static public IAjaxRequestHandler Find(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            return null;

        lock (syncLock)
            return fHandlers.FirstOrDefault(x => Sys.IsSameText(x.Name, Name));
    }
    /// <summary>
    /// Returns true when a handler with a specified name is already registered.
    /// </summary>
    static public bool Contains(string Name) => Find(Name) != null;
    /// <summary>
    /// Registers a handler.
    /// </summary>
    static public void Register(IAjaxRequestHandler Handler)
    {
        if (Handler == null)
            throw new TripousArgumentNullException(nameof(Handler));
        if (string.IsNullOrWhiteSpace(Handler.Name))
            throw new TripousException($"{nameof(IAjaxRequestHandler)} has no {nameof(IAjaxRequestHandler.Name)}.");

        lock (syncLock)
        {
            if (fHandlers.Any(x => Sys.IsSameText(x.Name, Handler.Name)))
                return;

            fHandlers.Insert(0, Handler);
        }
    }
    /// <summary>
    /// Registers all handlers found in a specified assembly.
    /// </summary>
    static public void Register(Assembly Assembly)
    {
        if (Assembly == null)
            throw new TripousArgumentNullException(nameof(Assembly));

        Type InterfaceType = typeof(IAjaxRequestHandler);
        Type[] Types = Assembly.GetTypesSafe();

        foreach (Type Type in Types)
        {
            if (!Type.IsClass || Type.IsAbstract)
                continue;
            if (!Type.ImplementsInterface(InterfaceType))
                continue;
            if (!Type.HasDefaultConstructor())
                continue;

            IAjaxRequestHandler Handler = Activator.CreateInstance(Type) as IAjaxRequestHandler;
            Register(Handler);
        }
    }
    /// <summary>
    /// Registers all handlers found in application assemblies.
    /// </summary>
    static public void RegisterApplicationAssemblies()
    {
        foreach (Assembly Assembly in AppAssemblies.GetApplicationAssemblies())
            Register(Assembly);
    }
    /// <summary>
    /// Unregisters a handler.
    /// </summary>
    static public void Unregister(IAjaxRequestHandler Handler)
    {
        if (Handler == null)
            return;

        lock (syncLock)
            fHandlers.Remove(Handler);
    }
    /// <summary>
    /// Unregisters a handler by name.
    /// </summary>
    static public void Unregister(string Name)
    {
        IAjaxRequestHandler Handler = Find(Name);
        if (Handler != null)
            Unregister(Handler);
    }
    /// <summary>
    /// Handles a specified request using the first registered handler that returns a response.
    /// </summary>
    static public AjaxResponse Handle(AjaxRequest Request, IViewToStringConverter ViewToStringConverter)
    {
        lock (syncLock)
        {
            foreach (IAjaxRequestHandler Handler in fHandlers)
            {
                AjaxResponse Result = Handler.Handle(Request, ViewToStringConverter);
                if (Result != null)
                    return Result;
            }
        }

        return null;
    }
    /// <summary>
    /// Gets the registered handlers.
    /// </summary>
    static public IAjaxRequestHandler[] Items
    {
        get
        {
            lock (syncLock)
                return fHandlers.ToArray();
        }
    }
}
