namespace Tripous.Data;


/// <summary>
/// A list of filter items.
/// <para>It can be used to to construct the <see cref="DataView.RowFilter"/> or to construct the <c>WHERE</c> clause of a <c>SELECT</c> statement. </para>
/// </summary>
public class SqlFilterDefs : DefList<SqlFilterDef>
{
    // ● construction
    public SqlFilterDefs()
    {
    }
    
    // ● public
    public SqlFilterDef Add(string Name, string FieldName = null, BoolOp BoolOp = BoolOp.And, ConditionOp ConditionOp = ConditionOp.Equal, string TitleKey = null)
    {
        SqlFilterDef Result = new();
        Result.Name = Name;
        Result.FieldName = FieldName;
        Result.BoolOp = BoolOp;
        Result.ConditionOp = ConditionOp;
        Result.TitleKey = TitleKey;
        this.Add(Result);
        return Result;
    }

    public void CheckDescriptorsWithValues()
    {
        foreach (SqlFilterDef Def in Items)
            Def.CheckDescriptorWithValues();
    }
    
    /// <summary>
    /// Formats a filter definition for the <see cref="DataView.RowFilter"/>.
    /// </summary>
    public string GetDataViewRowFilterText() => DataViewRowFilterFormatter.FormatText(this);
    /// <summary>
    /// Formats SQL WHERE filter definitions as inline SQL text.
    /// </summary>
    public string GetSqlWhereFilterTextInline() => SqlWhereFilterFormatter.FormatInline(this);
    /// <summary>
    /// Formats SQL WHERE filter definitions as parameterized SQL text.
    /// <para>NOTE: The <see cref="Params"/> should be an empty dictionary. The formatter adds Parameter Names and Values to it.</para>
    /// </summary>
    public string GetSqlWhereFilterTextParameterized(IDictionary<string, object> Params) => SqlWhereFilterFormatter.FormatParameterized(this, Params);

 
}