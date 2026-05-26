/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace SingleContextFile;

public class Settings
{
    readonly string FolderPath = AppContext.BaseDirectory;
    readonly string FileName = "Settings.json";
    string FilePath;
    TripousList<SingleFileSet> fFileSets;
    
    public Settings()
    {
        FilePath = Path.Combine(FolderPath, FileName);
    }

    public void Load() => Json.LoadFromFile(this, FilePath);
    public void Save() => Json.SaveToFile(this, FilePath);
 
    public string StartFolderPath { get; set; } = "";
 

    public TripousList<SingleFileSet> FileSets
    {
        get => fFileSets ??= new();
        set => fFileSets = value;
    }
}

public class SingleFileSet
{
    public SingleFileSet()
    {
    }

    public override string ToString() => FileName;
 

    public string FileName { get; set; } = "";
    public string Text  { get; set; } = "";
}