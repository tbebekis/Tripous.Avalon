namespace Tripous.Data;

/// <summary>
/// Represents the full group of DML statements 
/// </summary>
public class TableSqls
{
    /* construction */
    /// <summary>
    /// Constructor
    /// </summary>
    public TableSqls()
    {
    }


    /* public */
    /// <summary>
    /// Empties all statements
    /// </summary>
    public void Clear()
    {
        SelectSql = "";
        DeleteSql = "";

        SelectByMasterIdSql = "";

        SelectRowSql = "";
        InsertRowSql = "";
        UpdateRowSql = "";
        DeleteRowSql = "";
    }
    
    /// <summary>
    /// Returns true if <see cref="FieldTitleKeys"/> is valid and contains values.
    /// </summary>
    public bool HasTitleKeys() { return FieldTitleKeys != null && FieldTitleKeys.Count > 0; } 
    /// <summary>
    /// Loads <see cref="FieldTitleKeys"/> from a specified text.
    /// <para>NOTE: The specified text must contain string lines separated by <see cref="Environment.NewLine"/> 
    /// where ecah line contains an equal sign character, e.g. FIELD_NAME=TitleKey.</para>
    /// </summary>
    /// <param name="Text"></param>
    public void LoadFieldTitleKeysFromText(string Text)
    {
        if (!string.IsNullOrWhiteSpace(Text))
        {
            if (FieldTitleKeys == null)
                FieldTitleKeys = new List<string>();

            FieldTitleKeys.Clear();

            string[] Lines = Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
            if (Lines != null && Lines.Length > 0)
                FieldTitleKeys.AddRange(Lines);
        }
    }


    /// <summary>
    /// SELECT statement, e.g. select * from TABLE_NAME
    /// </summary>
    public string SelectSql { get; set; }
    /// <summary>
    /// DELETE statement, e.g. delete from TABLE_NAME
    /// </summary>
    public string DeleteSql { get; set; }

    /// <summary>
    /// SELECT statement for a detail table, e.g. select * from TABLE_NAME  where FOREIGN_KEY_FIELD = :SomeValue
    /// </summary>
    public string SelectByMasterIdSql { get; set; }

    /// <summary>
    /// SELECT a row statement, e.g. select * from TABLE_NAME where Id = :Id
    /// </summary>
    public string SelectRowSql { get; set; }
    /// <summary>
    /// INSERT a row statement, e.g. insert into  TABLE_NAME (FIELD_0, FIELD_N, ...) values (:FIELD_0, :FIELD_N, ...)
    /// </summary>
    public string InsertRowSql { get; set; }
    /// <summary>
    /// UPDATE a row statement, e.g. update TABLE_NAME set FIELD_= :FIELD_0, FIELD_N = :FIELD_N where Id = :Id
    /// </summary>
    public string UpdateRowSql { get; set; }
    /// <summary>
    /// UPDATE a row statement, e.g. delete from TABLE_NAME where Id = :Id
    /// </summary>
    public string DeleteRowSql { get; set; }

    /// <summary>
    /// To be used with <see cref="SelectSql"/>
    /// <para>A string list, where each string  has the format FIELD_NAME=TitleKey.</para> 
    /// <para>Determines the visibility of the fields in the drop-down grids: 
    /// if it is empty then all fields are visible  
    /// else only the included fields are visible  
    /// </para>
    /// </summary>
    public List<string> FieldTitleKeys { get; set; } = new List<string>();
}