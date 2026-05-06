namespace Tripous;

/// <summary>
/// An observable list of <see cref="IDef"/> items.
/// </summary>
public class DefList<T> : TripousList<T>, IJsonLoadable where T : IDef
{
    bool fAllowDuplicateNames;
    
    // ● protected  
    protected override void CheckAdding(T Def)
    {
        base.CheckAdding(Def);
        
        if (string.IsNullOrWhiteSpace(Def.Name))
            throw new TripousArgumentNullException(nameof(Def.Name));

        if (!AllowDuplicateNames && Contains(Def.Name))
            throw new TripousException($"{nameof(Def)} '{Def.Name}' is already registered.");
    }
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public DefList()
    {
    }
    
    // ● public
    /// <summary>
    /// Returns true if a specified item is in the list.
    /// </summary>
    public bool Contains(string Name) => this.Any(x => x.Name.IsSameText(Name));
    /// <summary>
    /// Returns an item, if any, else null.
    /// </summary>
    public T Find(string Name) => Items.FirstOrDefault(x => Sys.IsSameText(Name, x.Name));
    /// <summary>
    /// Returns an item, if any, else exception.
    /// </summary>
    public T Get(string Name)
    {
        T Result  = Items.FirstOrDefault(x => Sys.IsSameText(Name, x.Name));
        if (Result == null)
            throw new TripousException($"{typeof(T)} not found: {Name}");
        return Result;
    }
 
    /// <summary>
    /// Finds an item by name. It adds a new item if the item is not found.
    /// </summary>
    public T FindOrdAdd(string Name)
    {
        T Result = Find(Name);
        if (Result == null)
        {
            Result = Activator.CreateInstance<T>();
            Result.Name = Name;
            Items.Add(Result);
        }

        return Result;
    }
    /// <summary>
    /// Finds items by name. It adds a new item if an item is not found.
    /// </summary>
    public List<T> FindOrAddRange(string[] Names)
    {
        List<T> Result = [];
        foreach (string Name in Names)
            Result.Add(FindOrdAdd(Name));
        return Result;
    }
    
    /// <summary>
    /// Returns the index of an item or -1 if not found.
    /// </summary>
    public int IndexOf(string Name)
    {
        T Def = Find(Name);
        if (Def != null)
            return Items.IndexOf(Def);
        return -1;
    }
    /// <summary>
    /// Inserts an item before another, spefified by its name.
    /// </summary>
    public bool InsertBefore(string BeforeName, T Def)
    {
        T Other = Find(BeforeName);
        if (Other != null)
        {
            int Index = Items.IndexOf(Other);
            if (Index != -1)
            {
                Index--;
                if (Index < 0)
                    Index = 0;
                Items.Insert(Index, Def);
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Inserts an item after another, spefified by its name.
    /// </summary>
    public bool InsertAfter(string AfterName, T Def)
    {
        T Other = Find(AfterName);
        if (Other != null)
        {
            int Index = Items.IndexOf(Other);
            if (Index != -1)
            {
                Index++;
                if (Index > Items.Count - 1)
                    Items.Add(Def);
                else
                    Items.Insert(Index, Def);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public virtual void CheckDescriptors()
    {
        foreach (T Item in Items)
            Item.CheckDescriptor();
    }
    /// <summary>
    /// Updates references such as when an instance has references to other instances, e.g. tables of a module definition.
    /// </summary>
    public virtual void UpdateReferences()
    {
        foreach (T Item in Items)
            Item.UpdateReferences();
    }
    /// <summary>
    /// Called by the <see cref="Json"/> after deserializing an item.
    /// </summary>
    public virtual void JsonLoaded() => UpdateReferences();
    
    // ● properties
    /// <summary>
    /// Indexer
    /// </summary>
    [JsonIgnore]
    public T this[string Name] => Get(Name);
    /// <summary>
    /// The list of items.
    /// </summary>
    [JsonIgnore]
    public ReadOnlyCollection<T> ReadOnlyList => Items.AsReadOnly();
    /// <summary>
    /// In some cases, e.g. SQL WHERE filters, we may need the same name twice when constructing something like
    /// <para><c> MyDate &gt;= Value1 and MyDate &lt;= Value2</c></para>
    /// </summary>
    public bool AllowDuplicateNames 
    {
        get => fAllowDuplicateNames;
        set { if (fAllowDuplicateNames != value) { fAllowDuplicateNames = value; OnPropertyChanged(nameof(AllowDuplicateNames)); } }
    }
}

 

