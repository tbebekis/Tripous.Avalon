/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

static public partial class Registry
{
    // ● private
    static List<SchemaVersionDef> SchemaVersionList = [];
    static List<RegistryVersion> RegistryVersionList = [];

    // ● construction
    static Registry()
    {
        SchemaVersionList.AddRange([
            new SchemaVersion1(),
            new SchemaVersion2()
        ]);
        
        RegistryVersionList.AddRange([
            new RegistryVersion1(),
            new RegistryVersion2()
        ]);
    }
    
    // ● public
 
    static public void RegisterSchemas()
    {
        foreach (SchemaVersionDef Version in SchemaVersionList)
            Version.Register();
    }
    /// <summary>
    /// Register descriptors, i.e. commands, lookup sources, locators, modules and forms.
    /// </summary>
    static public void RegisterDescriptors()
    {
        foreach (RegistryVersion Version in RegistryVersionList)
        {
            Version.RegisterLookups();
            Version.RegisterLookupSources();
            Version.RegisterLocators();
            
            Version.RegisterCodeProviders();
            Version.RegisterModules();
            Version.RegisterForms();
        }
 
        RegisterDocumentHandlers();
  
        UpdateLookups();
        UpdateLocators();
        UpdateForms();
        UpdateModules();
        
 
    }
    
}