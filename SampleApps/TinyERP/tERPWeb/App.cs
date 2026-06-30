/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb;

/// <summary>
/// Provides application initialization for tERPWeb.
/// </summary>
static public class App
{
    static readonly object fLock = new();
    static bool fInitialized;

    // ● private
    /// <summary>
    /// Initializes global Tripous configuration.
    /// </summary>
    static void InitializeConfigs()
    {
        SysConfig.ApplicationMode = ApplicationMode.Web;
        SysConfig.MainAssembly = typeof(App).Assembly;
    }
    /// <summary>
    /// Forces application libraries to load.
    /// </summary>
    static void LoadLibraries()
    {
        CommonLib.Load();
        DataLib.Load();
    }
    /// <summary>
    /// Registers discoverable types.
    /// </summary>
    static void RegisterTypes()
    {
        TypeStore.RegisterLoadedAssemblies();
    }
    /// <summary>
    /// Registers application descriptors.
    /// </summary>
    static void RegisterDescriptors()
    {
        Registry.RegisterDescriptors();
    }
    /// <summary>
    /// Registers Ajax request handlers.
    /// </summary>
    static void RegisterAjaxHandlers()
    {
        AjaxRequestHandlers.RegisterApplicationAssemblies();
    }
    /// <summary>
    /// Initializes application libraries.
    /// </summary>
    static void InitializeLibraries()
    {
        CommonLib.Initialize();
        DataLib.Initialize();
    }

    // ● static public
    /// <summary>
    /// Initializes the tERPWeb application.
    /// </summary>
    static public void Initialize(WebApplicationBuilder Builder)
    {
        lock (fLock)
        {
            if (fInitialized)
                return;

            InitializeConfigs();
            LoadLibraries();
            RegisterTypes();
            RegisterDescriptors();
            RegisterAjaxHandlers();
            InitializeLibraries();

            fInitialized = true;
        }
    }
}
