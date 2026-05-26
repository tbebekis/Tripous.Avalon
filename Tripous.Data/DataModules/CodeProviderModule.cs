/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// The <see cref="CodeProviderDef"/> module
/// </summary>
public class CodeProviderModule: DataModule
{
    protected override void Commited(bool Reselect, object RowId)
    {
        base.Commited(Reselect, RowId);
        CodeProviderEntries.Clear();
    }

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public CodeProviderModule()
    {
    }
    
 }