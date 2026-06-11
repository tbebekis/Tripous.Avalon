/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Base class for application settings stored in JSON files.
/// </summary>
public abstract class SettingsBase
{
    // ● protected
    /// <summary>
    /// Gets the settings file name.
    /// </summary>
    protected virtual string FileName => $"{this.GetType().Name}.json";
    /// <summary>
    /// Called before loading settings from disk.
    /// </summary>
    protected virtual void LoadBefore()
    {
    }
    /// <summary>
    /// Called after settings have been loaded from disk.
    /// </summary>
    protected virtual void LoadAfter()
    {
    }
    /// <summary>
    /// Called before saving settings to disk.
    /// </summary>
    protected virtual void SaveBefore()
    {
    }
    /// <summary>
    /// Called after settings have been saved to disk.
    /// </summary>
    protected virtual void SaveAfter()
    {
    }
    /// <summary>
    /// Returns the full path of the settings file.
    /// </summary>
    protected virtual string GetFilePath() => Path.Combine(SysConfig.AppFolderPath, FileName);
    
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public SettingsBase()
    {
    }
    
    // ● public
    /// <summary>
    /// Loads settings from disk.
    /// </summary>
    public virtual void Load()
    {
        LoadBefore();
            
        if (!File.Exists(SettingsFilePath))
            return;

        string JsonText = File.ReadAllText(SettingsFilePath);
        Json.PopulateObject(this, JsonText);
            
        IsLoaded = true;
        LoadAfter();
    }
    /// <summary>
    /// Saves settings to disk.
    /// </summary>
    public virtual void Save()
    {
        SaveBefore();
            
        string DirectoryPath = Path.GetDirectoryName(SettingsFilePath);

        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);
        
        string JsonText = Json.Serialize(this);
        File.WriteAllText(SettingsFilePath, JsonText);
            
        SaveAfter();
    }
    
    // ● properties
    /// <summary>
    /// Gets the full path of the settings file.
    /// </summary>
    [JsonIgnore]
    public string SettingsFilePath => GetFilePath();
    /// <summary>
    /// Gets a value indicating whether the settings have been loaded.
    /// </summary>
    [JsonIgnore]
    public virtual bool IsLoaded { get; protected set; }
}