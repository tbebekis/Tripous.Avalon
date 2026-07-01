/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Registry for WebDesk form providers.
/// </summary>
static public class WebFormProviders
{
    // ● private fields
    static readonly object fSyncLock = new();
    static readonly List<WebFormProvider> fProviders = new();

    // ● static public
    /// <summary>
    /// Finds and returns a registered provider by web form name, if any; otherwise null.
    /// </summary>
    static public WebFormProvider Find(string WebFormName)
    {
        if (string.IsNullOrWhiteSpace(WebFormName))
            return null;

        lock (fSyncLock)
            return fProviders.FirstOrDefault(x => Sys.IsSameText(x.Name, WebFormName));
    }
    /// <summary>
    /// Returns true when a provider with a specified web form name is already registered.
    /// </summary>
    static public bool Contains(string WebFormName) => Find(WebFormName) != null;
    /// <summary>
    /// Registers a provider.
    /// </summary>
    static public void Register(WebFormProvider Provider)
    {
        if (Provider == null)
            throw new TripousArgumentNullException(nameof(Provider));
        if (string.IsNullOrWhiteSpace(Provider.Name))
            throw new TripousException($"{nameof(WebFormProvider)} has no {nameof(WebFormProvider.Name)}.");

        lock (fSyncLock)
        {
            if (fProviders.Any(x => Sys.IsSameText(x.Name, Provider.Name)))
                throw new TripousException($"Duplicate WebForm provider: {Provider.Name}");

            fProviders.Add(Provider);
        }
    }
    /// <summary>
    /// Registers all providers found in a specified assembly.
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
            if (!typeof(WebFormProvider).IsAssignableFrom(Type))
                continue;
            if (Type.GetCustomAttribute<WebFormProviderAttribute>() == null)
                continue;
            if (!Type.HasDefaultConstructor())
                continue;

            WebFormProvider Provider = Activator.CreateInstance(Type) as WebFormProvider;
            Register(Provider);
        }
    }
    /// <summary>
    /// Registers all providers found in application assemblies.
    /// </summary>
    static public void RegisterApplicationAssemblies()
    {
        foreach (Assembly Assembly in AppAssemblies.GetApplicationAssemblies())
            Register(Assembly);
    }
    /// <summary>
    /// Unregisters a provider.
    /// </summary>
    static public void Unregister(WebFormProvider Provider)
    {
        if (Provider == null)
            return;

        lock (fSyncLock)
            fProviders.Remove(Provider);
    }
    /// <summary>
    /// Unregisters a provider by web form name.
    /// </summary>
    static public void Unregister(string WebFormName)
    {
        WebFormProvider Provider = Find(WebFormName);
        if (Provider != null)
            Unregister(Provider);
    }
    /// <summary>
    /// Returns a provider for a specified form.
    /// </summary>
    static public WebFormProvider GetProvider(WebFormDef Form)
    {
        if (Form == null)
            throw new TripousArgumentNullException(nameof(Form));
        return Find(Form.Name) ?? new StandardWebFormProvider();
    }
    /// <summary>
    /// Executes a provider for a specified form.
    /// </summary>
    static public WebFormProviderPacket Execute(AjaxRequest Request, WebFormDef Form, AjaxOperationContext Context)
    {
        WebFormProvider Provider = GetProvider(Form);
        WebFormProviderContext ProviderContext = new(Request, Form, Context);
        return Provider.Execute(ProviderContext);
    }
    /// <summary>
    /// Gets the registered providers.
    /// </summary>
    static public WebFormProvider[] Items
    {
        get
        {
            lock (fSyncLock)
                return fProviders.ToArray();
        }
    }
}
