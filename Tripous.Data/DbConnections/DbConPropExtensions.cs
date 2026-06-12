/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Provides extension methods for lists of connection string properties.
/// </summary>
static public class DbConPropExtensions
{
    /// <summary>
    /// Returns true if the specified property exists and has a non-empty value.
    /// </summary>
    static public bool HasProp(this List<DbConProp> List, DbConPropType PropType) => Find(List, PropType) != null && !string.IsNullOrEmpty(Find(List, PropType).Value);

    /// <summary>
    /// Finds and returns a property by type.
    /// </summary>
    static public DbConProp Find(this List<DbConProp> List, DbConPropType PropType) => List.FirstOrDefault(x => x.PropType == PropType);
    /// <summary>
    /// Returns a property by type or raises an exception when not found.
    /// </summary>
    static public DbConProp Get(this List<DbConProp> List, DbConPropType PropType)
    {
        DbConProp Prop = List.FirstOrDefault(x => x.PropType == PropType);
        if (Prop == null)
            throw new TripousDataException($"Connection string property not found: {PropType}");
        return Prop;
    }

    /// <summary>
    /// Returns the value of the specified property.
    /// </summary>
    static public string GetValue(this List<DbConProp> List, DbConPropType PropType) => Get(List, PropType).Value;
    /// <summary>
    /// Sets the value of the specified property.
    /// </summary>
    static public void SetValue(this List<DbConProp> List, DbConPropType PropType, string Value) => Get(List, PropType).Value = Value;
}