namespace Tripous.Data;

public class RowFilterDefs : List<RowFilterDef>
{
    public RowFilterDefs()
    {
    }

    public override string ToString() => this.Text;
    public void Check() => ForEach(x => x.Check());

    public RowFilterDef Add(BoolOp BoolOp, ConditionOp ConditionOp, string FieldName, object Value, object Value2 = null)
    {
        RowFilterDef Result = new RowFilterDef(BoolOp, ConditionOp, FieldName, Value, Value2);
        this.Add(Result);
        return Result;
    }

    [JsonIgnore]
    public string Text
    {
        get
        {
            Check();
            StringBuilder SB = new();

            string Text;

            for (int i = 0; i < Count; i++)
            {
                RowFilterDef def = this[i];
                Text = def.Text;

                if (i == 0)
                {
                    if (Text.StartsWith("and "))
                        Text = Text.Remove(0, "and ".Length);
                    else if (Text.StartsWith("or "))
                        Text = Text.Remove(0, "or ".Length);
                }

                SB.Append(Text);
            }

            return SB.ToString();
        }
    }
}