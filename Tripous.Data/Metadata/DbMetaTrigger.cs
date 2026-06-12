/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Represents metadata for a database trigger.
/// </summary>
public class DbMetaTrigger : DbMetaObject
{
    // ● properties
    /// <summary>
    /// Gets or sets the name of the table associated with this trigger.
    /// </summary>
    public string TableName { get; set; }
    /// <summary>
    /// Gets or sets the trigger execution type and event timing (e.g., BEFORE INSERT, AFTER UPDATE).
    /// </summary>
    public string TriggerType { get; set; }
    /// <summary>
    /// Gets a descriptive, formatted string representation of the trigger metadata.
    /// </summary>
    public override string DisplayText
    {
        get
        {
            string Result = Name;
            if (!string.IsNullOrWhiteSpace(TriggerType))
                Result += $" {TriggerType}";
            return Result;
        }
    }
}