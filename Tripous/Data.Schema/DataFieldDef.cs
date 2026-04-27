namespace Tripous.Data;

/// <summary>
/// Database table field definition
/// </summary>
public class DataFieldDef
{

    bool fRequired;
    DataFieldType fDataType;
    int fLength;
    string fName;
    string fTitleKey;
    bool fUnique;
    string fForeignKey;


    // ●  construction 
    /// <summary>
    /// Constructor
    /// </summary>
    public DataFieldDef()
    {
    }

    // ●  public  
    /// <summary>
    /// Returns the string representation of a field data type.
    /// </summary>
    static public string DataTypeToString(DataFieldType Type)
    {
        switch (Type)
        {
            case DataFieldType.String: return SqlTypeTokens.CNVARCHAR;
            case DataFieldType.Integer: return "integer";
            case DataFieldType.Double: return SqlTypeTokens.CFLOAT;
            case DataFieldType.Decimal: return SqlTypeTokens.CDECIMAL;
            case DataFieldType.Decimal_: return SqlTypeTokens.CDECIMAL;
            case DataFieldType.Date: return SqlTypeTokens.CDATE;
            case DataFieldType.DateTime: return SqlTypeTokens.CDATE_TIME;
            case DataFieldType.Boolean: return "integer";
            case DataFieldType.Blob: return SqlTypeTokens.CBLOB;
            case DataFieldType.TextBlob: return SqlTypeTokens.CBLOB_TEXT;
        }

        throw new ApplicationException($"DataType not supported in Field definition: {Type}");
    }

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString()
    {
        return this.Name;
    }

    /// <summary>
    /// Returns the string representation of a field data type.
    /// </summary>
    public string DataTypeToString()
    {
        return DataTypeToString(this.DataType);
    }

    /// <summary>
    /// Returns the definition text for the data-type, e.g. @NVARCHAR(96)
    /// </summary>
    public string GetDataTypeDefText()
    {
        string sDataType;
        if (IsPrimaryKey)
        {
            sDataType = DataType == DataFieldType.String ? $"@NVARCHAR({Length}) @NOT_NULL primary key" : "@PRIMARY_KEY";
        }
        else if (DataType == DataFieldType.String)
        {
            sDataType = $"{SqlTypeTokens.CNVARCHAR}({Length})";
        }
        else if (DataType == DataFieldType.Decimal_)
        {
            sDataType = $"{SqlTypeTokens.CDECIMAL_}({DecimalPart})";
        }
        else
        {
            sDataType = DataTypeToString(DataType);
        }

        return sDataType;
    }
    /// <summary>
    /// Returns the definition text for the null/not null constraint, e.g. not null
    /// </summary>
    public string GetNullDefText()
    {
        string sNull = string.Empty;
        if (!IsPrimaryKey)
            sNull = Required ? SqlTypeTokens.CNOT_NULL : SqlTypeTokens.CNULL;

        return sNull;
    }
    /// <summary>
    /// Returns the definition text for the default constraint, e.g. '' or 0
    /// </summary>
    public string GetDefaultDefText()
    {
        string sDefault = !string.IsNullOrWhiteSpace(DefaultExpression) ? $"default {DefaultExpression}" : string.Empty;
        return sDefault;
    }

    /// <summary>
    /// Returns the definition text of the unique constraint, if defined, else empty string.
    /// <para>CAUTION: For use with a CREATE TABLE statement only. </para>
    /// <para>NOTE: All databases support unique constraint in the CREATE TABLE statement.</para>
    /// </summary>
    public string GetUniqueConstraintDefText()
    {
        string Result = "";
        if (this.Unique)
        {
            string sName = DataTableDef.EnsureIdentifierValidLength(this.UniqueConstraintName);
            Result = $"constraint {sName} unique ({this.Name})";
        }
        return Result;         
    }
    /// <summary>
    /// Returns the definition text of the foreign key constraint, if defined, else empty string.
    /// <para>CAUTION: For use with a CREATE TABLE statement only.</para>
    /// <para>NOTE: All databases support foreign key constraint in the CREATE TABLE statement.</para>
    /// </summary>
    public string GetForeignKeyConstraintDefText()
    {
        string Result = "";
        if (!string.IsNullOrWhiteSpace(this.ForeignKey))
        {
            string ForeignTableName;
            string ForeignFieldName;
            DataTableDef.SplitForeignKey(this.ForeignKey, out ForeignTableName, out ForeignFieldName);

            string sName = DataTableDef.EnsureIdentifierValidLength(this.ForeignKeyConstraintName);
            Result = $"constraint {sName} foreign key ({this.Name}) references {ForeignTableName} ({ForeignFieldName})";
        }
        return Result;
    }

    /// <summary>
    /// Returns the field definition text.
    /// <para>WARNING: The returned text must be passed through <see cref="SqlProvider.ReplaceDataTypePlaceholders"/> method, for the final result.</para>
    /// </summary>
    public string GetDefText(bool IncludeName)
    {
        string sDataType = GetDataTypeDefText();
        string sNull = GetNullDefText();
        string sDefault = GetDefaultDefText();
        string Result = IncludeName? $"{Name} {sDataType} {sDefault} {sNull}" : $"{sDataType} {sDefault} {sNull}";
        return Result;
    }

    /// <summary>
    /// Defines a foreign key upon this field and a foreign table and field specified by string of the form <code>TableName.ColumnName</code>. Returns this.
    /// </summary>
    public DataFieldDef SetForeignKey(string Value, string ConstraintName = "")
    {
        this.ForeignKey = Value;
        if (!string.IsNullOrWhiteSpace(ConstraintName))
            this.ForeignKeyConstraintName = ConstraintName; 
        return this;
    }
    /// <summary>
    /// Sets the value of a property and returns this instance.
    /// </summary>
    public DataFieldDef SetUnique(bool Value, string ConstraintName = "")
    {
        this.Unique = Value;
        if (!string.IsNullOrWhiteSpace(ConstraintName))
            this.UniqueConstraintName = ConstraintName; 
        return this;
    }
    /// <summary>
    /// Sets the value of a property and returns this instance.
    /// </summary>
    public DataFieldDef SetRequired(bool Value = true)
    {
        this.Required = Value;
        return this;
    }
    /// <summary>
    /// Sets the value of a property and returns this instance.
    /// </summary>
    public DataFieldDef SetDefaultExpression(string Value = null)
    {
        this.DefaultExpression = Value;
        return this;
    }
    /// <summary>
    /// Sets the value of a property and returns this instance.
    /// </summary>
    public DataFieldDef SetTitleKey(string Value = "")
    {
        this.TitleKey = Value;
        return this;
    }
    /// <summary>
    /// Sets the value of a property and returns this instance.
    /// </summary>
    public DataFieldDef SetLength(int Value = 0)
    {
        this.Length = Value;
        return this;
    }

    // ●  properties  
    /// <summary>
    /// A GUID string. We need this in comparing two instances of <see cref="DataTableDef"/> 
    /// for fields with the same Id but different Name, for column renaming.
    /// </summary>
    public string Id { get; set; } = Sys.GenId();
    /// <summary>
    /// A name unique among all instances of this type
    /// </summary>
    public string Name
    {
        get { return fName; }
        set
        {
            DataTableDef.CheckIsValidDatabaseObjectIdentifier(value);
            fName = value;
        }
    }
    /// <summary>
    /// Gets or sets a resource Key used in returning a localized version of Title
    /// </summary>
    public string TitleKey
    {
        get { return !string.IsNullOrWhiteSpace(fTitleKey) ? fTitleKey : Name; }
        set { fTitleKey = value; }
    }
    /// <summary>
    /// Gets the Title of this instance, used for display purposes. 
    /// <para>NOTE: The setter is fake. Do NOT use it.</para>
    /// </summary>    
    public string Title
    {
        get { return !string.IsNullOrWhiteSpace(TitleKey) ? Texts.L(TitleKey, TitleKey) : Name; }
        set { }
    }

    /// <summary>
    /// True when the field is a primary key
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// The data-type of the field. One of the <see cref="DataFieldType"/> constants.
    /// </summary>
    public DataFieldType DataType
    {
        get
        {
            if (IsPrimaryKey)
                return SysConfig.GuidOids ? DataFieldType.String : DataFieldType.Integer;

            return fDataType;
        }
        set { fDataType = value; }
    }
    /// <summary>
    /// Field length. Applicable to varchar fields only.
    /// </summary>
    public int Length
    {
        get
        {
            if (IsPrimaryKey && SysConfig.GuidOids)
                return fLength <= 40 ? 40 : fLength;
            if (DataType == DataFieldType.String)
                return fLength;
            return 0;
        }
        set { fLength = value; }

    }
    /// <summary>
    /// Defines the Precision and Scale explicitly of a decimal field. 
    /// <para>Used with <see cref="DataFieldType.Decimal_"/> only.</para>
    /// <para>The user provides the Precision and Scale explicitly <strong>without parentheses.</strong>.</para>
    /// <para>Example: <c>@DECIMAL_(10, 2)</c> becomes <c>decimal(10, 2)</c></para>
    /// </summary>
    public string DecimalPart { get; set; } = "18, 4";
    /// <summary>
    /// True when the field is NOT nullable 
    /// <para>NOTE: when true then produces 'not null'</para>
    /// </summary>
    public bool Required
    {
        get { return IsPrimaryKey ? true : fRequired; }
        set { fRequired = value; }
    }
    /// <summary>
    /// The default expression, if any. E.g. 0, or ''. Defaults to null.
    /// <para>NOTE:  e.g. produces default 0, or default '' </para>
    /// </summary>
    public string DefaultExpression { get; set; }


    /// <summary>
    /// When true indicates that the field has a unique constraint.
    /// </summary>
    public bool Unique 
    {
        get { return fUnique; }
        set
        {
            if (fUnique != value)
            {
                if (value && string.IsNullOrWhiteSpace(UniqueConstraintName))
                    UniqueConstraintName = "UC_" + Sys.GenerateRandomString(DataTableDef.IdentifierMaxLength - 3);

                fUnique = value;
            }
        }
    }
    /// <summary>
    /// The unique constraint name to create when <see cref="Unique"/> is set to true.
    /// </summary>
    public string UniqueConstraintName { get; set; }

    /// <summary>
    /// A string of the form <code>TableName.ColumnName</code> for creating a foreign key constraint on this field.
    /// </summary>
    public string ForeignKey
    {
        get { return fForeignKey; }
        set
        {
            if (fForeignKey != value )
            {
                if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(ForeignKeyConstraintName))
                    ForeignKeyConstraintName = "FC_" + Sys.GenerateRandomString(DataTableDef.IdentifierMaxLength - 3);

                fForeignKey = value;
            }
        }
    }

    /// <summary>
    /// When not null/empty indicates that this field has a foreign key constraint.
    /// </summary>
    public string ForeignKeyConstraintName { get; set; }
}
 
