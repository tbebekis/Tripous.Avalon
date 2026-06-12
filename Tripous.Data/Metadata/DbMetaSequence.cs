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
/// Represents metadata for a database sequence or generator.
/// </summary>
public class DbMetaSequence : DbMetaObject
{
    // ● properties
    /// <summary>
    /// Gets or sets the current numeric value of the sequence.
    /// </summary>
    public long CurrentValue { get; set; }
    /// <summary>
    /// Gets or sets the initial or seed value of the sequence.
    /// </summary>
    public long InitialValue { get; set; }
    /// <summary>
    /// Gets or sets the step interval increment value of the sequence.
    /// </summary>
    public long IncrementBy { get; set; }
}