/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Registry and dispatcher for WebDesk Ajax operations.
/// </summary>
static public class AjaxOperations
{
    static readonly object syncLock = new();
    static readonly List<AjaxOperation> fOperations = new();

    // ● static public
    /// <summary>
    /// Finds and returns a registered operation by name, if any; otherwise null.
    /// </summary>
    static public AjaxOperation Find(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            return null;

        lock (syncLock)
            return fOperations.FirstOrDefault(x => Sys.IsSameText(x.Name, Name));
    }
    /// <summary>
    /// Returns true when an operation with a specified name is already registered.
    /// </summary>
    static public bool Contains(string Name) => Find(Name) != null;
    /// <summary>
    /// Registers an operation.
    /// </summary>
    static public void Register(AjaxOperation Operation)
    {
        if (Operation == null)
            throw new TripousArgumentNullException(nameof(Operation));
        if (string.IsNullOrWhiteSpace(Operation.Name))
            throw new TripousException($"{nameof(AjaxOperation)} has no {nameof(AjaxOperation.Name)}.");

        lock (syncLock)
        {
            if (fOperations.Any(x => Sys.IsSameText(x.Name, Operation.Name)))
                throw new TripousException($"Duplicate Ajax operation: {Operation.Name}");

            fOperations.Add(Operation);
        }
    }
    /// <summary>
    /// Registers all operations found in a specified assembly.
    /// </summary>
    static public void Register(Assembly Assembly)
    {
        if (Assembly == null)
            throw new TripousArgumentNullException(nameof(Assembly));

        Type[] Types = Assembly.GetTypesSafe();

        foreach (Type Type in Types)
        {
            if (!Type.IsClass || Type.IsAbstract)
                continue;
            if (!typeof(AjaxOperation).IsAssignableFrom(Type))
                continue;
            if (Type.GetCustomAttribute<AjaxOperationAttribute>() == null)
                continue;
            if (!Type.HasDefaultConstructor())
                continue;

            AjaxOperation Operation = Activator.CreateInstance(Type) as AjaxOperation;
            Register(Operation);
        }
    }
    /// <summary>
    /// Registers all operations found in application assemblies.
    /// </summary>
    static public void RegisterApplicationAssemblies()
    {
        foreach (Assembly Assembly in AppAssemblies.GetApplicationAssemblies())
            Register(Assembly);
    }
    /// <summary>
    /// Unregisters an operation.
    /// </summary>
    static public void Unregister(AjaxOperation Operation)
    {
        if (Operation == null)
            return;

        lock (syncLock)
            fOperations.Remove(Operation);
    }
    /// <summary>
    /// Unregisters an operation by name.
    /// </summary>
    static public void Unregister(string Name)
    {
        AjaxOperation Operation = Find(Name);
        if (Operation != null)
            Unregister(Operation);
    }
    /// <summary>
    /// Executes a specified request.
    /// </summary>
    static public AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (Request == null)
            throw new TripousArgumentNullException(nameof(Request));
        AjaxOperation Operation = Find(Request.OperationName);
        if (Operation == null)
            return null;
        return Operation.Execute(Request, Context);
    }
    /// <summary>
    /// Gets the registered operations.
    /// </summary>
    static public AjaxOperation[] Items
    {
        get
        {
            lock (syncLock)
                return fOperations.ToArray();
        }
    }
}
