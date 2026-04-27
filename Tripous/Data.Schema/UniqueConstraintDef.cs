namespace Tripous.Data;

/// <summary>
/// For table-wise unique constraints, possibly on multiple fields.
/// </summary>
public class UniqueConstraintDef
{
    string fFieldNames;

    /// <summary>
    /// Constructor
    /// </summary>
    public UniqueConstraintDef()
    {
    }

    /// <summary>
    /// The constraint name
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// A proper string, e.g. <code>Field1, Field2</code>
    /// </summary>
    public string FieldNames
    {
        get { return fFieldNames; }
        set
        {
            if (fFieldNames != value)
            {
                if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(Name))
                    Name = "UC_" + Sys.GenerateRandomString(DataTableDef.IdentifierMaxLength - 3);

                fFieldNames = value;
            }
        }
    }


    /// <summary>
    /// Returns the definition text.
    /// </summary>
    public string GetDefText()
    { 
        string sName = DataTableDef.EnsureIdentifierValidLength(this.Name);
        string Result = $"constraint {sName} unique ({this.Name})";
        return Result;
    }
}