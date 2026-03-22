using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Tripous.Data;

public class SelectDef
{
    StringBuilder sbErrors = new();
    
    // ● private
    private ObservableCollection<SqlFilterDef> fFilters;
    
    // ● construction
    public SelectDef()
    {
    }

    // ● public
    public override string ToString()
    {
        return !string.IsNullOrWhiteSpace(Name) ? Name : base.ToString();
    }
    public void Load(string FilePath  = null)
    {
       if (string.IsNullOrWhiteSpace(FilePath))
           FilePath = GetFilePath();
       
        string JsonText = File.ReadAllText(FilePath);
        Json.PopulateObject(this, JsonText);
    }
    public void Save(string FilePath = null)
    {
        if (string.IsNullOrWhiteSpace(FilePath))
            FilePath = GetFilePath();
        
        string DirectoryPath = Path.GetDirectoryName(FilePath);

        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);
        
        string JsonText = Json.Serialize(this);
        File.WriteAllText(FilePath, JsonText);
    }
 
    public string GetFilePath()
    {
        string Temp = Path.Combine(SysConfig.AppFolderPath, ConnectionName);
        Temp = Path.Combine(Temp, "SelectDefs");
        Temp = Path.Combine(Temp, Name + ".json");
        return Temp;
    }
    
    /// <summary>
    /// If a SourceList element exists in Filters, then is updated.
    /// Elements that exist in Filters but not in SourceList, are deleted from Filters
    /// </summary>
    public void ReplaceFilters(ICollection<SqlFilterDef> SourceList)
    {
        // 1. Αφαιρούμε φίλτρα που δεν υπάρχουν πια στο SQL κείμενο
        var toRemove = Filters.Where(old => !SourceList.Any(src => src.RawTag == old.RawTag)).ToList();
        foreach (var item in toRemove)
        {
            Filters.Remove(item);
        }

        // 2. Επεξεργαζόμαστε τη νέα λίστα
        foreach (var src in SourceList)
        {
            var existing = Filters.FirstOrDefault(f => f.RawTag == src.RawTag);

            if (existing != null)
            {
                // Αν υπάρχει ήδη, το RawTag Setter έχει ήδη ενημερώσει τα Metadata (Label, Type κλπ)
                // απλώς σιγουρευόμαστε ότι το existing αντικείμενο συγχρονίστηκε με το src 
                // σε περίπτωση που ο χρήστης άλλαξε το RawTag στο SQL (π.χ. από [[Label]] σε [[int:Label]])
                existing.RawTag = src.RawTag;
            
                // ΣΗΜΑΝΤΙΚΟ: Το .Text (Lookup SQL, Enum κλπ) και το .Value (Runtime) 
                // παραμένουν ως έχουν για να μη χαθεί η δουλειά του χρήστη.
            }
            else
            {
                // Αν είναι νέο tag, το προσθέτουμε στη συλλογή
                Filters.Add(src);
            }
        }
    }
    public bool ParseSqlText()
    {
        sbErrors.Clear();
        
        if (string.IsNullOrWhiteSpace(SqlText))
        {
            Filters.Clear();
            sbErrors.AppendLine("No SELECT provided");
            return false;
        }

        List<SqlFilterDef> sourceList = new List<SqlFilterDef>();

        // Εντοπίζουμε όλα τα [[...]] tags
        var matches = Regex.Matches(SqlText, @"\[\[.*?\]\]");

        foreach (Match m in matches)
        {
            string rawTag = m.Value;

            // Δημιουργούμε το φίλτρο. 
            // Ο setter του RawTag θα καλέσει την Parse() εσωτερικά 
            // και θα γεμίσει αυτόματα τα Label, Type, IsMultiple, DateRange.
            var newFilter = new SqlFilterDef { RawTag = rawTag };

            if (newFilter.HasErrors)
            {
                sbErrors.AppendLine(newFilter.Errors);
                return false;
            }
            
            // Αν για κάποιο λόγο το label βγήκε κενό, το αγνοούμε
            if (string.IsNullOrEmpty(newFilter.Label))
            {
                sbErrors.AppendLine($"Filter {rawTag} has no Label.");
                return false;
            }

            // Αποφεύγουμε τα διπλότυπα tags στο ίδιο query
            if (!sourceList.Any(f => f.RawTag == rawTag))
            {
                sourceList.Add(newFilter);
            }
        }

        // Συγχρονισμός με την υπάρχουσα λίστα
        ReplaceFilters(sourceList);
        return true;
    }

    /// <summary>
    /// Applies the SqlFilterDef.Value(s) to the appropriate raw tags in the SELECT text and returns the text.
    /// </summary>
    public string ApplyFiltersToSqlText()
    {
        string finalSql = SqlText;

        foreach (var filter in Filters)
        {
            string replacement;

            if (filter.IsMultiple)
            {
                var parts = filter.Value?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            
                // Αν είναι numeric, τα βάζουμε χύμα. Αν όχι, τα βάζουμε σε quotes.
                if (filter.IsNumeric)
                    replacement = parts.Length > 0 ? string.Join(", ", parts) : "NULL";
                else
                    replacement = parts.Length > 0 ? string.Join(", ", parts.Select(v => $"'{v}'")) : "''";
            }
            else
            {
                if (filter.IsNumeric)
                    replacement = string.IsNullOrWhiteSpace(filter.Value) ? "0" : filter.Value;
                else
                    replacement = $"'{filter.Value ?? ""}'";
            }

            finalSql = finalSql.Replace(filter.RawTag, replacement);
        }

        return finalSql;
    }
 
    public DbConnectionInfo GetConnectionInfo()
    {
        return Db.Connections.Find(ConnectionName);
    }

    public DbServerType GetServerType()
    {
        return GetConnectionInfo().DbServerType;
    }

    public SqlStore GetSqlStore()
    {
        return new SqlStore(GetConnectionInfo());
    }
    
    // ● properties
    public string ConnectionName { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }

    public string SqlText { get; set; }
    public string Description { get; set; }

    public ObservableCollection<SqlFilterDef> Filters
    {
        get
        {
            if (fFilters == null)
                fFilters = new();
            return fFilters;
        }
        set => fFilters = value;
    }
    
    [JsonIgnore] 
    public bool HasErrors => sbErrors.Length > 0;
    [JsonIgnore] 
    public string Errors => sbErrors.ToString();
}