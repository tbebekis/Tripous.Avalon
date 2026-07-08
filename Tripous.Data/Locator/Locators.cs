namespace Tripous.Data;

/// <summary>
/// Static locator service.
/// </summary>
static public class Locators
{
    // ● public
    /// <summary>
    /// Executes a locator request.
    /// </summary>
    static public LocatorResult Execute(LocatorRequest Request)
    {
        if (Request == null)
            throw new TripousArgumentNullException(nameof(Request));
        if (Request.Context == null)
            throw new TripousDataException($"{nameof(LocatorRequest)} has no {nameof(LocatorRequest.Context)}.");
        if (string.IsNullOrWhiteSpace(Request.Context.LocatorName))
            throw new TripousDataException($"{nameof(LocatorRequest)} has no {nameof(LocatorContext.LocatorName)}.");

        LocatorDef LocatorDef = DataRegistry.GetLocator(Request.Context.LocatorName);
        LocatorDef.CheckDescriptor();

        Locator Locator = TypeStore.CreateInstance<Locator>(LocatorDef.ClassName);
        return Locator.Execute(LocatorDef, Request);
    }
}
