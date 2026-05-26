/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

public class DbMetaSequence : DbMetaObject
{
    public long CurrentValue { get; set; }
    public long InitialValue { get; set; }
    public long IncrementBy { get; set; }
}

 