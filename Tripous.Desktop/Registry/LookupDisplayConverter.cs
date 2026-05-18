

namespace Tripous.Desktop;

public class LookupDisplayConverter: IValueConverter
{
    // ● private fields
    readonly LookupDef fLookupDef;
    LookupSource LookupSource;

    // ● constructors
    public LookupDisplayConverter(LookupDef LookupDef)
    {
        fLookupDef = LookupDef ?? throw new TripousArgumentNullException(nameof(LookupDef));
        LookupSource = fLookupDef.Create();
    }

    // ● public methods
    public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        foreach (LookupItem Item in LookupSource.GetList())
        {
            if (Item.IsNullItem)
                continue;

            if (Equals(Item.Value, Value))
                return Item.DisplayText ?? string.Empty;
        }

        return string.Empty;
    }

    public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
    {
        return Avalonia.Data.BindingOperations.DoNothing;
    }
}