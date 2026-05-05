namespace Tripous.Desktop;

public class SqlFilterInfo
{
    // ● construction
    public SqlFilterInfo()
    {
    }
    
    // ● properties
    public SqlFilterDef FilterDef { get; set; }
    public Control Control  { get; set; }
    public Control Control2  { get; set; }
    public ComboBox BoolOpCombo { get; set; }
    public ComboBox ConditionOpCombo { get; set; }
}