

namespace Tripous
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    
    public abstract class SettingsBase
    {
        protected abstract string FileName { get; }

        protected virtual void LoadBefore()
        {
        }
        protected virtual void LoadAfter()
        {
        }
        protected virtual void SaveBefore()
        {
        }
        protected virtual void SaveAfter()
        {
        }
    
        protected virtual string GetFilePath() => Path.Combine(SysConfig.AppFolderPath, FileName);
    
        // ● construction
        /// <summary>
        /// Constructor
        /// </summary>
        public SettingsBase()
        {
        }
    
        // ● public
        /// <summary>
        /// Loads the security settings from disk.
        /// </summary>
        public virtual void Load()
        {
            LoadBefore();
            
            if (!File.Exists(FilePath))
                return;

            string JsonText = File.ReadAllText(FilePath);
            Json.PopulateObject(this, JsonText);
            
            IsLoaded  = true;
            LoadAfter();
        }
        /// <summary>
        /// Saves the security settings to disk.
        /// </summary>
        public virtual void Save()
        {
            SaveBefore();
            
            string DirectoryPath = Path.GetDirectoryName(FilePath);

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
        
            string JsonText = Json.Serialize(this);
            File.WriteAllText(FilePath, JsonText);
            
            SaveAfter();
        }
    
        // ● properties
        /// <summary>
        /// The full path where this instance is saved.
        /// </summary>
        [JsonIgnore]
        public string FilePath => GetFilePath();
        [JsonIgnore] 
        public virtual bool IsLoaded { get; protected set; }
    }    
    
}

