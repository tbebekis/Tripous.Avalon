namespace Tripous;

public class DbMetaColumn : DbMetaObject
{
    public string DataType { get; set; } = string.Empty;   // varchar, integer, timestamp κλπ
    public string DataSubType { get; set; }                // firebird-specific π.χ. VARCHAR → CHARACTER VARYING
    public bool IsNullable { get; set; }
    public int SizeInChars { get; set; }               // CHARACTER_MAXIMUM_LENGTH
    public int SizeInBytes  { get; set; }
    public int Precision { get; set; }
    public int Scale { get; set; }
    public string DefaultValue { get; set; }
    public string Expression { get; set; }
    public bool IsIdentity { get; set; }  
    public bool IsComputed { get; set; }
    public int OrdinalPosition { get; set; }           
    
    public override string DisplayText
    {
       get
       {
          string Result = Name + " " + DataType;
          
          if (!string.IsNullOrWhiteSpace(DataSubType))
             Result += " - " + DataSubType;
          
          if (IsIdentity)
             Result += " IDENTITY";
 
          if (SizeInChars > 0)
             Result += $"({SizeInChars})";
          if ((Precision > 0) && (Scale > 0))
             Result += $"({Precision},{Scale})";
 
          Result += IsNullable ? " null" : " not null";
 
          return Result;
       }
    }
    /*

    
  Result := Name + ' (' + DataType;

       if not Sys.IsEmpty(DataSubType) then
          Result := Result + ' - ' + DataSubType;

       if SizeInChars > 0 then
          Result := Result + '(' + IntToStr(SizeInChars) + ')';

       if (Precision > 0) and (Scale > 0) then
          Result := Result + '(' + IntToStr(Precision) + ', ' + IntToStr(Scale) + ')';

       if IsNullable then
          Result := Result + ', null)'
       else
          Result := Result + ', not null)';     
    */
}