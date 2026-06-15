/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */



namespace Tripous.Desktop;

/// <summary>
/// Converts lookup values to display text.
/// </summary>
public class LookupDisplayConverter: IValueConverter
{
    // ● private fields
    /// <summary>
    /// The lookup definition.
    /// </summary>
    readonly LookupDef fLookupDef;
    /// <summary>
    /// The lookup source.
    /// </summary>
    LookupSource LookupSource;

    // ● constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="LookupDisplayConverter"/> class.
    /// </summary>
    /// <param name="LookupDef">The lookup definition.</param>
    public LookupDisplayConverter(LookupDef LookupDef)
    {
        fLookupDef = LookupDef ?? throw new TripousArgumentNullException(nameof(LookupDef));
        LookupSource = fLookupDef.Create();
    }

    // ● public methods
    /// <summary>
    /// Converts a lookup value to display text.
    /// </summary>
    /// <param name="Value">The lookup value.</param>
    /// <param name="TargetType">The target type.</param>
    /// <param name="Parameter">The converter parameter.</param>
    /// <param name="Culture">The culture.</param>
    /// <returns>The lookup display text.</returns>
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

    /// <summary>
    /// Converts a value back to the source type.
    /// </summary>
    /// <param name="Value">The value to convert.</param>
    /// <param name="TargetType">The target type.</param>
    /// <param name="Parameter">The converter parameter.</param>
    /// <param name="Culture">The culture.</param>
    /// <returns>A binding operation value.</returns>
    public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
    {
        return Avalonia.Data.BindingOperations.DoNothing;
    }
}
