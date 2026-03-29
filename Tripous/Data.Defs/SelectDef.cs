using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Tripous.Data;

public class SelectDef
{
    private string fSqlText;
    StringBuilder sbErrors = new();
    private bool Parsing = false;
    
    // ● private
    private ObservableCollection<SqlFilterDef> fFilters;
    private ObservableCollection<GridViewDef> fGridViews;
    
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
    /// Parses the SELECT statement and generates <see cref="SqlFilterDef"/> items.
    /// <para>Returns true when there are no errors in the <see cref="Errors"/>.</para>
    /// <para>WARNING: Existing <see cref="SqlFilterDef"/> items are discarded. </para>
    /// </summary>
    public bool Parse()
    {
        if (!Parsing)
        {
            Parsing = true;
            try
            {
                sbErrors.Clear();
        
                if (string.IsNullOrWhiteSpace(SqlText))
                {
                    Filters.Clear();
                    sbErrors.AppendLine("No SELECT provided");
                    return false;
                }

                List<SqlFilterDef> NewFilterList = new List<SqlFilterDef>();

                // Εντοπίζουμε όλα τα [[...]] tags
                var matches = Regex.Matches(SqlText, @"\[\[.*?\]\]");

                foreach (Match m in matches)
                {
                    string rawTag = m.Value;

                    // create the filter, the RawTag setter calls the Parse()
                    var newFilter = new SqlFilterDef { RawTag = rawTag };
            
                    if (string.IsNullOrEmpty(newFilter.Label))
                    {
                        sbErrors.AppendLine($"Filter {rawTag} has no Label.");
                        return false;
                    }

                    var Old = NewFilterList.FirstOrDefault(x => x.Label.IsSameText(newFilter.Label));
                    if (Old != null)
                    {
                        sbErrors.AppendLine($"Label {Old.Label} already exists.");
                        return false;
                    }    
  
                    NewFilterList.Add(newFilter);
                }

                // keep the old statements, i.e. SELECT and Constant Lists
                UpdateFilterListFrom(NewFilterList);
            }
            finally
            {
                Parsing = false;
            }
        }
        
        return !HasErrors;
    }
    /// <summary>
    /// If a SourceList element exists in Filters, then is updated its <see cref="SqlFilterDef.RawTag"/> and <see cref="SqlFilterDef.Statement"/>.
    /// Elements that exist in Filters but not in SourceList, are deleted from Filters
    /// </summary>
    void UpdateFilterListFrom(ICollection<SqlFilterDef> SourceList)
    {
        // removed non existed filters
        var toRemove = Filters.Where(old => !SourceList.Any(src => src.RawTag == old.RawTag)).ToList();
        foreach (var item in toRemove)
        {
            Filters.Remove(item);
        }
 
        foreach (var src in SourceList)
        {
            var existing = Filters.FirstOrDefault(f => f.RawTag == src.RawTag);

            if (existing != null)
            {
                // update existing filter from old filter
                existing.RawTag = src.RawTag;
                //existing.Statement = src.Statement;
            }
            else
            {
                Filters.Add(src);
            }
        }
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
    /// <summary>
    /// The SELECT statement text.
    /// <para>NOTE: Assigning this text automatically calls the <see cref="Parse"/>() which creates the new <see cref="SqlFilterDef"/> items.</para>
    /// </summary>
    public string SqlText
    {
        get => fSqlText;
        set
        {
            fSqlText = value;
            Parse();
        }
    }
    public string Description { get; set; }
    public string LastSelectSqlText { get; set; }

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
    public ObservableCollection<GridViewDef> GridViews
    {
        get
        {
            if (fGridViews == null)
                fGridViews = new();
            return fGridViews;
        }
        set => fGridViews = value;
    }

    [JsonIgnore] 
    public bool HasErrors => !string.IsNullOrWhiteSpace(Errors);
    [JsonIgnore] 
    public string Errors  
    {
        get
        {
            StringBuilder SB = new();
            if (sbErrors.Length > 0)
                SB.AppendLine(sbErrors.ToString());
            
            foreach (var Item in Filters)
            {
                if (Item.HasErrors)
                    SB.AppendLine(Item.Errors);
            }

            return SB.ToString();
        }
    }
}