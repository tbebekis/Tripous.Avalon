/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// The base <see cref="DataModule"/> class for all modules of this application.
/// </summary>
public class AppDataModule: DataModule
{
    protected AppDefaultProperties AppDefaultProperties;
    
    // ● overrides
    protected override void LoadDefaultValues()
    {
        base.LoadDefaultValues();
        AppDefaultProperties = Config.GetObjectValue<AppDefaultProperties>(DataLib.SAppDefaultProperties);
    }
 
    // ● construction
    public AppDataModule()
    {
    }
}