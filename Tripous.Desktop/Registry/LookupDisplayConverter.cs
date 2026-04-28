

namespace Tripous.Desktop;

public class LookupDisplayConverter: IValueConverter
{
    // ● private fields
    readonly LookupSource fSource;

    // ● constructors
    public LookupDisplayConverter(LookupSource Source)
    {
        fSource = Source ?? throw new ArgumentNullException(nameof(Source));
    }

    // ● public methods
    public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        foreach (LookupItem Item in fSource.List)
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