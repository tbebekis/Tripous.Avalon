using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;


namespace Tripous.Data;

/// <summary>
/// Specifies pivot axis.
/// </summary>
public enum PivotFieldMode
{
    None,
    Row,
    Column,
    Value
}

/// <summary>
/// Specifies pivot data row type.
/// </summary>
public enum PivotDataRowType
{
    Normal,
    Subtotal,
    GrandTotal
}

/// <summary>
/// Specifies pivot value aggregate type.
/// </summary>
public enum PivotValueAggregateType
{
    None,
    Sum,
    Avg,
    Count,
    Min,
    Max,
    StdDev,
    StdDevP,
    Variance,
    VarianceP,
    CountDistinct,
    Product
}

/// <summary>
/// Specifies pivot data column kind.
/// </summary>
public enum PivotDataColumnKind
{
    None,
    RowHeader,
    Value
}

static public class PivotDataExtensions
{
    static public PivotValueAggregateType[] GetValidPivotAggregates(this Type DataType)
    {
        if (DataType == null)
            return Array.Empty<PivotValueAggregateType>();

        if (DataType.IsNumeric())
        {
            return new[]
            {
                PivotValueAggregateType.Count,
                PivotValueAggregateType.Sum,
                PivotValueAggregateType.Avg,
                PivotValueAggregateType.Min,
                PivotValueAggregateType.Max,
                PivotValueAggregateType.StdDev,
                PivotValueAggregateType.StdDevP,
                PivotValueAggregateType.Variance,
                PivotValueAggregateType.VarianceP,
                PivotValueAggregateType.CountDistinct,
                PivotValueAggregateType.Product
            };
        }

        if (DataType.IsDateTime())
        {
            return new[]
            {
                PivotValueAggregateType.Count,
                PivotValueAggregateType.Min,
                PivotValueAggregateType.Max,
                PivotValueAggregateType.CountDistinct
            };
        }

        return new[]
        {
            PivotValueAggregateType.Count,
            PivotValueAggregateType.CountDistinct
        };
    }
    static public bool IsPivotSupportedType(this Type T)
    {
        Type ActualType = Nullable.GetUnderlyingType(T) ?? T;
        return ActualType.IsString() || ActualType.IsNumeric() || ActualType.IsDateTime();
    }
    
    /// <summary>
    /// Creates a default <see cref="PivotViewDef"/> based on a specified <see cref="DataView"/>.
    /// </summary>
    static public PivotViewDef CreateDefaultPivotDef(this DataView DataView)
    {
        if (DataView == null)
            throw new ArgumentNullException(nameof(DataView));

        PivotViewDef Result = new();

        var All = DataView.Table.Columns.Cast<DataColumn>().ToList();
        var Strings = All.Where(x => x.DataType == typeof(string)).ToList();
        var Dates = All.Where(x => x.DataType == typeof(DateTime)).ToList();
        var Numerics = All.Where(x => x.DataType.IsNumeric()).ToList();

        var Eligible = All
            .Where(x => x.DataType.IsPivotSupportedType())
            .ToList();

        foreach (DataColumn Col in Eligible)
        {
            Result.Fields.Add(new PivotFieldDef
            {
                FieldName = Col.ColumnName,
                Caption = string.IsNullOrWhiteSpace(Col.Caption) ? Col.ColumnName : Col.Caption,
                FieldMode = PivotFieldMode.None,
                ValueAggregateType = PivotValueAggregateType.None,
                SortByValue = true,
                Format = Col.DataType == typeof(DateTime) ? "d" : null
            });
        }

        var FirstRow = Strings.FirstOrDefault() ?? Dates.FirstOrDefault();
        var FirstColumn = Strings.Skip(1).FirstOrDefault() ?? Dates.Skip(1).FirstOrDefault();
        var FirstValue = Numerics.FirstOrDefault();

        if (FirstRow != null)
        {
            PivotFieldDef Field = Result.Fields.FirstOrDefault(x => x.FieldName == FirstRow.ColumnName);
            if (Field != null)
                Field.FieldMode = PivotFieldMode.Row;
        }

        if (FirstColumn != null)
        {
            PivotFieldDef Field = Result.Fields.FirstOrDefault(x => x.FieldName == FirstColumn.ColumnName);
            if (Field != null)
                Field.FieldMode = PivotFieldMode.Column;
        }

        if (FirstValue != null)
        {
            PivotFieldDef Field = Result.Fields.FirstOrDefault(x => x.FieldName == FirstValue.ColumnName);
            if (Field != null)
            {
                Field.FieldMode = PivotFieldMode.Value;
                Field.ValueAggregateType = PivotValueAggregateType.Sum;
            }
        }
        else
        {
            PivotFieldDef Fallback = Result.Fields.FirstOrDefault(x => x.FieldMode == PivotFieldMode.None);

            if (Fallback != null)
            {
                Fallback.FieldMode = PivotFieldMode.Value;
                Fallback.ValueAggregateType = PivotValueAggregateType.Count;
            }
        }

        return Result;
    }
    static public PivotViewDef CreateDefaultPivotDef(this Type T)
    {
        PivotViewDef Result = new();
                
        var All = T.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList(); 
        var Strings = All.Where(x => x.PropertyType == typeof(string)).ToList();
        var Dates = All.Where(x => x.PropertyType == typeof(DateTime)).ToList();
        var Numerics = All.Where(x => x.PropertyType.IsNumeric()).ToList();

        var Eligible = All
            .Where(x => x.PropertyType.IsPivotSupportedType())
            .ToList();

        foreach (PropertyInfo Col in Eligible)
        {
            Result.Fields.Add(new PivotFieldDef
            {
                FieldName = Col.Name,
                FieldMode = PivotFieldMode.None,
                ValueAggregateType = PivotValueAggregateType.None,
                SortByValue = true,
                Format = Col.PropertyType == typeof(DateTime) ? "d" : null
            });
        }

        var FirstRow = Strings.FirstOrDefault() ?? Dates.FirstOrDefault();
        var FirstColumn = Strings.Skip(1).FirstOrDefault() ?? Dates.Skip(1).FirstOrDefault();
        var FirstValue = Numerics.FirstOrDefault();

        if (FirstRow != null)
        {
            PivotFieldDef Field = Result.Fields.FirstOrDefault(x => x.FieldName == FirstRow.Name);
            if (Field != null)
                Field.FieldMode = PivotFieldMode.Row;
        }

        if (FirstColumn != null)
        {
            PivotFieldDef Field = Result.Fields.FirstOrDefault(x => x.FieldName == FirstColumn.Name);
            if (Field != null)
                Field.FieldMode = PivotFieldMode.Column;
        }

        if (FirstValue != null)
        {
            PivotFieldDef Field = Result.Fields.FirstOrDefault(x => x.FieldName == FirstValue.Name);
            if (Field != null)
            {
                Field.FieldMode = PivotFieldMode.Value;
                Field.ValueAggregateType = PivotValueAggregateType.Sum;
            }
        }
        else
        {
            PivotFieldDef Fallback = Result.Fields.FirstOrDefault(x => x.FieldMode == PivotFieldMode.None);

            if (Fallback != null)
            {
                Fallback.FieldMode = PivotFieldMode.Value;
                Fallback.ValueAggregateType = PivotValueAggregateType.Count;
            }
        }


        return Result;
    }
    
    /// <summary>
    /// Returns the supported source columns of a specified <see cref="DataView"/>.
    /// </summary>
    static public List<DataColumn> GetPivotSupportedColumns(this DataView DataView)
    {
        if (DataView == null)
            throw new ArgumentNullException(nameof(DataView));

        return DataView.Table.Columns
            .Cast<DataColumn>()
            .Where(x => x.DataType.IsPivotSupportedType())
            .ToList();
    }
}

/// <summary>
/// Pivot column definition.
/// </summary>
public class PivotFieldDef
{
    private string fCaption;
    private PivotFieldMode fFieldMode;

    private PivotValueAggregateType fValueAggregateType;
 
 
    // ● construction
    public PivotFieldDef()
    {
    }

    // ● static
    static public PivotFieldDef CreateRow(string FieldName, string Caption = null)
    {
        return new PivotFieldDef
        {
            FieldName = FieldName,
            Caption = Caption,
            FieldMode = PivotFieldMode.Row
        };
    }
    static public PivotFieldDef CreateColumn(string FieldName, string Caption = null)
    {
        return new PivotFieldDef
        {
            FieldName = FieldName,
            Caption = Caption,
            FieldMode = PivotFieldMode.Column
        };
    }
    static public PivotFieldDef CreateValue(string FieldName, PivotValueAggregateType AggregateType, string Caption = null, string Format = null)
    {
        return new PivotFieldDef
        {
            FieldName = FieldName,
            Caption = Caption,
            FieldMode = PivotFieldMode.Value,
            ValueAggregateType = AggregateType,
            Format = Format
        };
    }

    // ● public
    public override string ToString() => FieldName; //   $"{FieldName}, Axis: {Axis}, IsValue: {IsValue}";
 
    public void AssignFrom(PivotFieldDef Source)
    {
        FieldName = Source.FieldName;
        fCaption = Source.fCaption;
        fFieldMode = Source.fFieldMode;
        SortDirection = Source.SortDirection;
        Format = Source.Format;
 
        SortByValue = Source.SortByValue;
        ValueAggregateType = Source.ValueAggregateType;

        Tag = Source.Tag;
        DataType = Source.DataType;

    }
    public PivotFieldDef Clone()
    {
        PivotFieldDef Result = new();
        Result.AssignFrom(this);
        return Result;
    }

    // ● properties
    public string FieldName { get; set; }
    public string Caption
    {
        get => !string.IsNullOrWhiteSpace(fCaption) ? fCaption : FieldName;
        set => fCaption = value;
    }
    public PivotFieldMode FieldMode
    {
        get => fFieldMode;
        set
        {
            if (fFieldMode != value)
            {
                fFieldMode = value;
                
                SortByValue = false;
                ValueAggregateType = PivotValueAggregateType.None;
                SortDirection = ListSortDirection.Ascending;

                switch (fFieldMode)
                {
                    case PivotFieldMode.None: break;
                    case PivotFieldMode.Row: break;
                    case PivotFieldMode.Column: break;
                    case PivotFieldMode.Value:
                        SortByValue = true;
                        break;
                }
            }
        }
    }
    public ListSortDirection SortDirection { get; set; } // Asc / Desc
    public string Format { get; set; }
 
    public bool SortByValue { get; set; } = true;

    public PivotValueAggregateType ValueAggregateType
    {
        get => IsValue ? fValueAggregateType : PivotValueAggregateType.None;
        set => fValueAggregateType = value;
    }
    public int OrdinalIndex { get; set; } // the index in Rows, Columns or Values

    [JsonIgnore]
    public object Tag { get; set; }
    [JsonIgnore]
    public Type DataType { get; set; }

    [JsonIgnore]
    public bool IsRow => FieldMode == PivotFieldMode.Row;
    [JsonIgnore]
    public bool IsColumn => FieldMode == PivotFieldMode.Column;
    [JsonIgnore]
    public bool IsValue => FieldMode == PivotFieldMode.Value;
    [JsonIgnore]
    public bool IsNone => !IsRow && !IsColumn && !IsValue;
}

/// <summary>
/// Pivot definition.
/// </summary>
public class PivotViewDef
{
    private string fName;
    
    // ● construction
    public PivotViewDef()
    {
    }

    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(Name) ? Name: base.ToString();
    public string GetDescription()
    {
        // --------------------------------------------
        string ValueFieldDefDescription(PivotFieldDef FieldDef) => 
            FieldDef.FieldName + " " +
            FieldDef.ValueAggregateType.ToString() + " " +
            (FieldDef.SortDirection == ListSortDirection.Ascending ? "" : "desc");
        // --------------------------------------------
 
        string S = string.Empty;
        
        StringBuilder SB = new();
        
        // rows
        S = string.Join(", ", GetRowFields());
        SB.AppendLine($"Rows: {S}");
        
        // columns
        S = string.Join(", ", GetColumnFields());
        SB.AppendLine($"Columns: {S}");
        
        // values
        S = string.Join(", ", GetValueFields()
            .Select(x => ValueFieldDefDescription(x)));
        SB.AppendLine($"Values: {S}");
        
        S = SB.ToString();
        return S;
    }
 
    public void AssignFrom(PivotViewDef Source)
    {
        Fields.Clear();
        
        fName = Source.fName;

        ShowSubtotals = Source.ShowSubtotals;
        ShowGrandTotals = Source.ShowGrandTotals;
        RepeatRowHeaders = Source.RepeatRowHeaders;
        
        Tag = Source.Tag;
        IsNameReadOnly = Source.IsNameReadOnly;
 
        foreach (var SourceField in Source.Fields)
            Fields.Add(SourceField.Clone());
         
    }
    public PivotViewDef Clone()
    {
        PivotViewDef Result = new();
        Result.AssignFrom(this);
        return Result;
    }
    
    public IEnumerable<PivotFieldDef> GetRowFields() => Fields.Where(x => x.IsRow).OrderBy(x => x.OrdinalIndex);
    public IEnumerable<PivotFieldDef> GetColumnFields() => Fields.Where(x => x.IsColumn).OrderBy(x => x.OrdinalIndex);
    public IEnumerable<PivotFieldDef> GetValueFields() => Fields.Where(x => x.IsValue).OrderBy(x => x.OrdinalIndex);

    public PivotFieldDef Find(string FieldName) => Fields.FirstOrDefault(x => FieldName.IsSameText(x.FieldName));
    public bool Contains(string FieldName) => Fields.Any(x => x.FieldName.IsSameText(FieldName));
    public PivotFieldDef Get(string FieldName)
    {
        PivotFieldDef Result = Find(FieldName);
        if (Result == null)
            throw new ApplicationException($"{nameof(PivotFieldDef)} not found: {FieldName}");
        return Result;
    }
    
    PivotFieldDef AddInternal(PivotFieldDef FieldDef)
    {
      
        if (string.IsNullOrWhiteSpace(FieldDef.FieldName)) 
            throw new ApplicationException($"Cannot add a {nameof(PivotFieldDef)} without FieldName");

        if (Contains(FieldDef.FieldName))
            throw new ApplicationException($"{nameof(PivotFieldDef)} already exists in list: {FieldDef.FieldName}");

        Fields.Add(FieldDef);
        return FieldDef;
    }
    public PivotFieldDef AddRow(string FieldName, string Caption = null)
    {
        PivotFieldDef Result = PivotFieldDef.CreateRow(FieldName, Caption);
        AddInternal(Result);
        return Result;
    }
    public PivotFieldDef AddColumn(string FieldName, string Caption = null)
    {
        PivotFieldDef Result = PivotFieldDef.CreateColumn(FieldName, Caption);
        AddInternal(Result);
        return Result;
    }
    public PivotFieldDef AddValue(string FieldName, PivotValueAggregateType AggregateType, string Caption = null,
        string Format = null)
    {
        PivotFieldDef Result = PivotFieldDef.CreateValue(FieldName, AggregateType, Caption, Format);
        AddInternal(Result);
        return Result;
    }

    public bool CanBeRow(PivotFieldDef FieldDef)
    {
        return !FieldDef.IsRow
               && (FieldDef.IsNone
                   || (FieldDef.IsColumn && ColumnCount > 1)
                   || (FieldDef.IsValue && ValueCount > 1));
    }
    public bool CanBeColumn(PivotFieldDef FieldDef)
    {
        return !FieldDef.IsColumn
               && (FieldDef.IsNone
                   || (FieldDef.IsRow && RowCount > 1)
                   || (FieldDef.IsValue && ValueCount > 1));
    }
    public bool CanBeValue(PivotFieldDef FieldDef) 
    {
        return !FieldDef.IsValue
               && (FieldDef.IsNone
                   || (FieldDef.IsRow && RowCount > 1)
                   || (FieldDef.IsColumn && ColumnCount > 1));
    }

    public string GetErrors()
    {
        StringBuilder SB = new();
        
        if (string.IsNullOrWhiteSpace(Name))
            SB.AppendLine($"{nameof(PivotViewDef)} name is empty.");

        if (Fields == null || Fields.Count == 0)
            SB.AppendLine($"{nameof(PivotViewDef)} contains no fields.");

        if (!GetValueFields().Any())
            SB.AppendLine($"{nameof(PivotViewDef)} contains no value fields.");
        
        if (Fields.Any(x => x == null))
            SB.AppendLine($"{nameof(PivotViewDef)} contains null fields.");
        
        if (Fields.Any(x => string.IsNullOrWhiteSpace(x.FieldName)))
            SB.AppendLine($"{nameof(PivotViewDef)} contains field with empty FieldName.");
        
        if (Fields.Any(x => x.DataType == null))
            SB.AppendLine($"{nameof(PivotViewDef)} contains fields with null DataTypes.");

        HashSet<string> FieldNames = new(StringComparer.OrdinalIgnoreCase);

        foreach (PivotFieldDef Field in Fields)
        {
            if (!FieldNames.Add(Field.FieldName))
                SB.AppendLine($"Duplicate field: '{Field.FieldName}'.");

            if (Field.IsValue)
            {
                if (Field.ValueAggregateType == PivotValueAggregateType.None)
                {
                    SB.AppendLine($"Value field with no aggregate: '{Field.FieldName}'.");
                }
                else if (!Field.DataType.GetValidPivotAggregates().ToList().Contains(Field.ValueAggregateType))
                {
                    SB.AppendLine($"Value field with no valid aggregate: '{Field.FieldName}'.");
                }
            }
        }

        return SB.ToString();
    }
    public bool HasErrors() => !string.IsNullOrWhiteSpace(GetErrors());
    public void Check()
    {
        string Errors = GetErrors();
        if (!string.IsNullOrWhiteSpace(Errors))
            throw new ApplicationException(Errors);
    }

    public void UpdateDataTypes(DataView DataView)
    {
        var All = DataView.Table.Columns.Cast<DataColumn>().ToList();
        DataColumn Column;
        foreach (PivotFieldDef FieldDef in Fields)
        {
            Column = All.FirstOrDefault(x => x.ColumnName.IsSameText(FieldDef.FieldName));
            if (Column != null)
                FieldDef.DataType = Column.DataType;
        }
    }
    public void UpdateDataTypes(Type T)
    {
        var All = T.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
        PropertyInfo PropInfo;
        foreach (PivotFieldDef FieldDef in Fields)
        {
            PropInfo = All.FirstOrDefault(x => x.Name.IsSameText(FieldDef.FieldName));
            if (PropInfo != null)
                FieldDef.DataType = PropInfo.PropertyType;
        }
    }

    
    // ● properties
    /// <summary>
    /// The name of this grid view
    /// </summary>
    public string Name
    {
        get => !string.IsNullOrWhiteSpace(fName) ? fName : Sys.GenId();
        set => fName = value;
    }
    
    public ObservableCollection<PivotFieldDef> Fields { get; set; } = new();
    public bool ShowSubtotals { get; set; } = true;
    public bool ShowGrandTotals { get; set; } = true;
    //public bool ShowValuesOnRows { get; set; }
    public bool RepeatRowHeaders { get; set; } = false;

    [JsonIgnore]
    public object Tag { get; set; }
    [JsonIgnore]
    public bool IsNameReadOnly { get; set; }
    public int RowCount => Fields.Where(x => x.IsRow).Count();
    public int ColumnCount => Fields.Where(x => x.IsColumn).Count();
    public int ValueCount => Fields.Where(x => x.IsValue).Count();
}

public class PivotViewDefs
{
    private string fName;

    // ● construction
    public PivotViewDefs()
    {
    }

    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(Name) ? Name: base.ToString();
        
    public void LoadFromFile()
    {
        if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
            throw new ApplicationException($"Cannot load {nameof(PivotViewDefs)}. Invalid file path");
        DefList.Clear();
        Json.LoadFromFile(this, FilePath);
    }
    public void SaveToFile()
    {
        if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
            throw new ApplicationException($"Cannot save {nameof(PivotViewDefs)}. Invalid file path");
        Json.SaveToFile(this, FilePath);
    }
    
    public PivotViewDef Find(string Name) => DefList.FirstOrDefault(x => Name.IsSameText(x.Name));
    public bool Contains(string Name) => DefList.Any(x => x.Name.IsSameText(Name));
    public PivotViewDef Get(string Name)
    {
        PivotViewDef Result = Find(Name);
        if (Result == null)
            throw new ApplicationException($"{nameof(PivotViewDef)} not found: {Name}");
        return Result;
    }

    PivotViewDef AddInternal(PivotViewDef PivotViewDef, string Name = "")
    {
        if (!string.IsNullOrWhiteSpace(Name))
            PivotViewDef.Name = Name;
        
        if (string.IsNullOrWhiteSpace(PivotViewDef.Name)) 
            PivotViewDef.Name = Sys.GenId();
        
        if (DefList.Count == 0)
            PivotViewDef.Name = "Default";

        if (Contains(PivotViewDef.Name))
            throw new ApplicationException($"{nameof(PivotViewDef)} already exists in list: {PivotViewDef.Name}");

        DefList.Add(PivotViewDef);
        return PivotViewDef;
    }
    public PivotViewDef Add(string Name = "")
    {
        PivotViewDef PivotViewDef = new();
        return AddInternal(PivotViewDef, Name);
    }
    public void Remove(PivotViewDef PivotViewDef)
    {
        if (DefList.Contains(PivotViewDef))
        {
            if (DefList.Count == 1)
                throw new ApplicationException($"Cannot delete the last {nameof(PivotViewDef)} from list.");
            if (DefList.Count > 1)
                DefList.Remove(PivotViewDef);
        }
    }
    
    // ● properties
    /// <summary>
    /// The name of this grid view
    /// </summary>
    public string Name
    {
        get => !string.IsNullOrWhiteSpace(fName) ? fName : Sys.GenId();
        set => fName = value;
    }
    public List<PivotViewDef> DefList { get; set; } = new();
    [JsonIgnore] 
    public string FilePath { get; set; }
}

/// <summary>
/// Pivot output data.
/// </summary>
public class PivotData
{
    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    public List<PivotDataColumn> Columns { get; } = new();
    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    public List<PivotDataRow> Rows { get; } = new();
}

/// <summary>
/// A pivot output column.
/// </summary>
public class PivotDataColumn
{
    public PivotDataColumn()
    {
    }

    public override string ToString()
    {
        return $"{Caption} ({Key})";
    }

    public string Key { get; set; }
    public string Caption { get; set; }
    public Type DataType { get; set; }
    public string Format { get; set; }
    public PivotDataColumnKind Kind { get; set; }
    public int RowLevel { get; set; } = -1;
    public PivotFieldDef SourceField { get; set; }
    [JsonIgnore]
    public object Tag { get; set; }
}

/// <summary>
/// A pivot output row.
/// </summary>
public class PivotDataRow
{
    public PivotDataRow()
    {
    }

    public object[] Values { get; set; }
    public PivotDataRowType RowType { get; set; } = PivotDataRowType.Normal;
    public int Level { get; set; } = -1;
    [JsonIgnore]
    public object Tag { get; set; }
}

/// <summary>
/// Pivot engine with:
/// - multi-level row subtotals
/// - multi-level column subtotals
/// - grand total row
/// - grand total columns
/// - aggregate-aware totals everywhere
/// Current limitation:
/// - values are still rendered on columns only
/// </summary>
static public class PivotEngine
{
    private const string KeySeparator = "\u001F";
    private const string NullToken = "\u2400";
    private const string GrandRowKey = "__GRAND_TOTAL__";

    private sealed class AggregateBucket
    {
        private readonly PivotValueAggregateType fAggregateType;
        private int fCount;
        private decimal fSum;
        private decimal fSumSquares;
        private decimal fProduct = 1m;
        private decimal? fMin;
        private decimal? fMax;
        private HashSet<object> fDistinctSet;

        public AggregateBucket(PivotValueAggregateType AggregateType)
        {
            fAggregateType = AggregateType;
        }

        public void Add(object Value)
        {
            switch (fAggregateType)
            {
                case PivotValueAggregateType.Count:
                    fCount++;
                    return;
                case PivotValueAggregateType.CountDistinct:
                    if (Value != null && Value != DBNull.Value)
                    {
                        fDistinctSet ??= new HashSet<object>();
                        fDistinctSet.Add(Value);
                    }
                    return;
            }

            if (Value == null || Value == DBNull.Value)
                return;

            decimal Number = Convert.ToDecimal(Value, CultureInfo.InvariantCulture);

            fCount++;

            switch (fAggregateType)
            {
                case PivotValueAggregateType.None:
                case PivotValueAggregateType.Sum:
                case PivotValueAggregateType.Avg:
                    fSum += Number;
                    break;
                case PivotValueAggregateType.Min:
                    if (!fMin.HasValue || Number < fMin.Value)
                        fMin = Number;
                    break;
                case PivotValueAggregateType.Max:
                    if (!fMax.HasValue || Number > fMax.Value)
                        fMax = Number;
                    break;
                case PivotValueAggregateType.Product:
                    fProduct *= Number;
                    break;
                case PivotValueAggregateType.StdDev:
                case PivotValueAggregateType.StdDevP:
                case PivotValueAggregateType.Variance:
                case PivotValueAggregateType.VarianceP:
                    fSum += Number;
                    fSumSquares += Number * Number;
                    break;
            }
        }
        public object GetResult()
        {
            return fAggregateType switch
            {
                PivotValueAggregateType.None => fCount > 0 ? fSum : null,
                PivotValueAggregateType.Sum => fCount > 0 ? fSum : null,
                PivotValueAggregateType.Avg => fCount > 0 ? fSum / fCount : null,
                PivotValueAggregateType.Count => fCount,
                PivotValueAggregateType.Min => fMin,
                PivotValueAggregateType.Max => fMax,
                PivotValueAggregateType.Product => fCount > 0 ? fProduct : null,
                PivotValueAggregateType.CountDistinct => fDistinctSet?.Count ?? 0,
                PivotValueAggregateType.Variance => fCount > 1 ? ComputeVarianceSample() : null,
                PivotValueAggregateType.VarianceP => fCount > 0 ? ComputeVariancePopulation() : null,
                PivotValueAggregateType.StdDev => fCount > 1 ? SqrtDecimal(ComputeVarianceSample()) : null,
                PivotValueAggregateType.StdDevP => fCount > 0 ? SqrtDecimal(ComputeVariancePopulation()) : null,
                _ => throw new NotSupportedException($"Unsupported aggregate type: {fAggregateType}")
            };
        }

        private decimal ComputeVariancePopulation()
        {
            decimal Count = fCount;
            decimal Mean = fSum / Count;
            decimal Variance = (fSumSquares / Count) - (Mean * Mean);
            return Variance < 0m ? 0m : Variance;
        }
        private decimal ComputeVarianceSample()
        {
            if (fCount <= 1)
                return 0m;

            decimal Count = fCount;
            decimal Numerator = fSumSquares - ((fSum * fSum) / Count);
            decimal Variance = Numerator / (Count - 1m);
            return Variance < 0m ? 0m : Variance;
        }
    }
    private sealed class RowSourceEntry
    {
        public Dictionary<string, object> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
    private sealed class RowOutputEntry
    {
        public string RowKey { get; set; }
        public object[] RowValues { get; set; }
        public PivotDataRowType RowType { get; set; }
        public int Level { get; set; }
    }
    private sealed class ValueColumnEntry
    {
        public string ColumnKey { get; set; }
        public string SourceFieldName { get; set; }
        public string ResultFieldKey { get; set; }
        public PivotValueAggregateType AggregateType { get; set; }
    }
    private sealed class ColumnOutputEntry
    {
        public string ColumnKey { get; set; }
        public object[] ColumnValues { get; set; }
        public bool IsSubtotal { get; set; }
        public bool IsGrandTotal { get; set; }
        public int Level { get; set; } = -1;
    }
    private sealed class GroupInfo
    {
        public object Value { get; set; }
        public List<KeyValuePair<string, object[]>> Items { get; } = new();
        public decimal? SortValue { get; set; }
    }
    private sealed class CollectResult
    {
        public List<RowSourceEntry> RowEntries { get; } = new();
        public Dictionary<string, object[]> DetailRowKeyMap { get; } = new();
        public Dictionary<int, Dictionary<string, object[]>> SubtotalRowKeyMaps { get; } = new();
        public Dictionary<string, object[]> DetailColumnKeyMap { get; } = new();
        public Dictionary<int, Dictionary<string, object[]>> SubtotalColumnKeyMaps { get; } = new();
        public Dictionary<string, AggregateBucket> DetailBuckets { get; } = new();
        public Dictionary<int, Dictionary<string, AggregateBucket>> SubtotalBucketsByRowLevel { get; } = new();
        public Dictionary<int, Dictionary<string, AggregateBucket>> RowSortBucketsByLevel { get; } = new();
        public Dictionary<int, Dictionary<string, AggregateBucket>> ColumnSortBucketsByLevel { get; } = new();
        public Dictionary<string, AggregateBucket> GrandBuckets { get; } = new();
    }
    private sealed class BuildColumnsResult
    {
        public List<ColumnOutputEntry> OutputColumns { get; } = new();
        public List<ValueColumnEntry> ValueColumnEntries { get; } = new();
    }
    private sealed class ObjectArrayComparer : IComparer<object[]>
    {
        public int Compare(object[] X, object[] Y)
        {
            if (ReferenceEquals(X, Y))
                return 0;

            if (X == null)
                return -1;

            if (Y == null)
                return 1;

            int Length = Math.Min(X.Length, Y.Length);

            for (int i = 0; i < Length; i++)
            {
                int c = CompareObject(X[i], Y[i]);
                if (c != 0)
                    return c;
            }

            return X.Length.CompareTo(Y.Length);
        }
        static public int CompareObject(object A, object B)
        {
            if (A == null && B == null)
                return 0;

            if (A == null)
                return -1;

            if (B == null)
                return 1;

            if (A is IComparable ComparableA && A.GetType() == B.GetType())
                return ComparableA.CompareTo(B);

            return StringComparer.CurrentCultureIgnoreCase.Compare(A.ToString(), B.ToString());
        }
    }

    static private void ValidateDef(PivotViewDef ViewDef)
    {
        /*
        if (Def.Fields == null || Def.Fields.Count == 0)
            throw new ApplicationException("PivotDef contains no fields.");

        if (!Def.GetValueFields().Any())
            throw new ApplicationException("PivotDef contains no value fields.");

        HashSet<string> FieldNames = new(StringComparer.OrdinalIgnoreCase);

        foreach (PivotFieldDef Field in Def.Fields)
        {
            if (Field == null)
                throw new ApplicationException("PivotDef contains null field.");

            if (string.IsNullOrWhiteSpace(Field.FieldName))
                throw new ApplicationException("PivotDef contains field with empty FieldName.");

            if (!FieldNames.Add(Field.FieldName))
                throw new ApplicationException($"Duplicate pivot field '{Field.FieldName}'.");
        }
        */
        ViewDef.Check();
    }
    static private CollectResult Collect<T>(
        IEnumerable<T> Source,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ColumnFields,
        List<PivotFieldDef> ValueFields)
    {
        CollectResult Result = new();

        Result.RowEntries.AddRange(Source.Select(CreateRowSourceEntry));

        foreach (RowSourceEntry Entry in Result.RowEntries)
            CollectEntry(Result, Entry, ViewDef, RowFields, ColumnFields, ValueFields);

        return Result;
    }
    static private void CollectEntry(
        CollectResult Result,
        RowSourceEntry Entry,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ColumnFields,
        List<PivotFieldDef> ValueFields)
    {
        object[] RowValues = RowFields.Select(x => GetValue(Entry, x.FieldName)).ToArray();
        object[] ColumnValues = ColumnFields.Select(x => GetValue(Entry, x.FieldName)).ToArray();

        string DetailRowKey = ComposeKey(RowValues);
        string DetailColumnKey = ComposeKey(ColumnValues);

        RegisterDetailKeys(Result, ViewDef, RowFields, ColumnFields, RowValues, ColumnValues, DetailRowKey, DetailColumnKey);

        foreach (PivotFieldDef ValueField in ValueFields)
        {
            object Value = GetValue(Entry, ValueField.FieldName);
            CollectValue(Result, ViewDef, RowFields, ColumnFields, ValueField, Value, RowValues, ColumnValues, DetailRowKey, DetailColumnKey);
        }
    }
    static private void RegisterDetailKeys(
        CollectResult Result,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ColumnFields,
        object[] RowValues,
        object[] ColumnValues,
        string DetailRowKey,
        string DetailColumnKey)
    {
        if (!Result.DetailRowKeyMap.ContainsKey(DetailRowKey))
            Result.DetailRowKeyMap[DetailRowKey] = RowValues;

        if (!Result.DetailColumnKeyMap.ContainsKey(DetailColumnKey))
            Result.DetailColumnKeyMap[DetailColumnKey] = ColumnValues;

        if (!ViewDef.ShowSubtotals)
            return;

        RegisterSubtotalRowKeys(Result, RowFields, RowValues);
        RegisterSubtotalColumnKeys(Result, ColumnFields, ColumnValues);
    }
    static private void RegisterSubtotalRowKeys(
        CollectResult Result,
        List<PivotFieldDef> RowFields,
        object[] RowValues)
    {
        for (int RowLevel = 0; RowLevel < RowFields.Count; RowLevel++)
        {
            Dictionary<string, object[]> RowKeyMap = GetOrCreateSubtotalObjectMap(Result.SubtotalRowKeyMaps, RowLevel);
            object[] SubtotalGroupValues = GetSubtotalGroupValues(RowValues, RowLevel);
            string SubtotalGroupKey = ComposeKey(SubtotalGroupValues);

            if (!RowKeyMap.ContainsKey(SubtotalGroupKey))
                RowKeyMap[SubtotalGroupKey] = GetSubtotalDisplayValues(RowValues, RowLevel);
        }
    }
    static private void RegisterSubtotalColumnKeys(
        CollectResult Result,
        List<PivotFieldDef> ColumnFields,
        object[] ColumnValues)
    {
        for (int ColumnLevel = 0; ColumnLevel < ColumnFields.Count; ColumnLevel++)
        {
            Dictionary<string, object[]> ColumnKeyMap = GetOrCreateSubtotalObjectMap(Result.SubtotalColumnKeyMaps, ColumnLevel);
            object[] SubtotalColumnGroupValues = GetSubtotalGroupValues(ColumnValues, ColumnLevel);
            string SubtotalColumnKey = ComposeKey(SubtotalColumnGroupValues);

            if (!ColumnKeyMap.ContainsKey(SubtotalColumnKey))
                ColumnKeyMap[SubtotalColumnKey] = GetSubtotalDisplayValues(ColumnValues, ColumnLevel);
        }
    }
    static private void CollectValue(
        CollectResult Result,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ColumnFields,
        PivotFieldDef ValueField,
        object Value,
        object[] RowValues,
        object[] ColumnValues,
        string DetailRowKey,
        string DetailColumnKey)
    {
        UpdateBucket(Result.DetailBuckets, DetailRowKey, DetailColumnKey, ValueField, Value);
        CollectRowSortBuckets(Result, RowFields, ValueField, Value, RowValues);
        CollectColumnSortBuckets(Result, ColumnFields, ValueField, Value, ColumnValues);
        CollectDetailGrandTotal(Result, ViewDef, ValueField, Value, DetailRowKey);
        CollectRowSubtotals(Result, ViewDef, RowFields, ValueField, Value, RowValues, DetailColumnKey);
        CollectColumnSubtotals(Result, ViewDef, RowFields, ColumnFields, ValueField, Value, RowValues, ColumnValues, DetailRowKey);
        CollectGrandTotals(Result, ViewDef, ValueField, Value, DetailColumnKey);
    }
    static private void CollectRowSortBuckets(
        CollectResult Result,
        List<PivotFieldDef> RowFields,
        PivotFieldDef ValueField,
        object Value,
        object[] RowValues)
    {
        if (RowFields.Count == 0)
            return;

        for (int RowLevel = 0; RowLevel < RowFields.Count; RowLevel++)
        {
            Dictionary<string, AggregateBucket> SortBucketMap = GetOrCreateSubtotalBucketMap(Result.RowSortBucketsByLevel, RowLevel);
            object[] SubtotalGroupValues = GetSubtotalGroupValues(RowValues, RowLevel);
            string SubtotalGroupKey = ComposeKey(SubtotalGroupValues);

            UpdateBucket(SortBucketMap, SubtotalGroupKey, string.Empty, ValueField, Value);
        }
    }
    static private void CollectColumnSortBuckets(
        CollectResult Result,
        List<PivotFieldDef> ColumnFields,
        PivotFieldDef ValueField,
        object Value,
        object[] ColumnValues)
    {
        if (ColumnFields.Count == 0)
            return;

        for (int ColumnLevel = 0; ColumnLevel < ColumnFields.Count; ColumnLevel++)
        {
            Dictionary<string, AggregateBucket> SortBucketMap = GetOrCreateSubtotalBucketMap(Result.ColumnSortBucketsByLevel, ColumnLevel);
            object[] SubtotalColumnGroupValues = GetSubtotalGroupValues(ColumnValues, ColumnLevel);
            string SubtotalColumnKey = ComposeKey(SubtotalColumnGroupValues);

            UpdateBucket(SortBucketMap, GrandRowKey, SubtotalColumnKey, ValueField, Value);
        }
    }
    static private void CollectDetailGrandTotal(
        CollectResult Result,
        PivotViewDef ViewDef,
        PivotFieldDef ValueField,
        object Value,
        string DetailRowKey)
    {
        if (!ViewDef.ShowGrandTotals)
            return;

        UpdateBucket(Result.DetailBuckets, DetailRowKey, string.Empty, ValueField, Value);
    }
    static private void CollectRowSubtotals(
        CollectResult Result,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        PivotFieldDef ValueField,
        object Value,
        object[] RowValues,
        string DetailColumnKey)
    {
        if (!ViewDef.ShowSubtotals || RowFields.Count == 0)
            return;

        for (int RowLevel = 0; RowLevel < RowFields.Count; RowLevel++)
        {
            Dictionary<string, AggregateBucket> BucketMap = GetOrCreateSubtotalBucketMap(Result.SubtotalBucketsByRowLevel, RowLevel);
            object[] SubtotalGroupValues = GetSubtotalGroupValues(RowValues, RowLevel);
            string SubtotalGroupKey = ComposeKey(SubtotalGroupValues);

            UpdateBucket(BucketMap, SubtotalGroupKey, DetailColumnKey, ValueField, Value);

            if (ViewDef.ShowGrandTotals)
                UpdateBucket(BucketMap, SubtotalGroupKey, string.Empty, ValueField, Value);
        }
    }
    static private void CollectColumnSubtotals(
        CollectResult Result,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ColumnFields,
        PivotFieldDef ValueField,
        object Value,
        object[] RowValues,
        object[] ColumnValues,
        string DetailRowKey)
    {
        if (!ViewDef.ShowSubtotals || ColumnFields.Count == 0)
            return;

        for (int ColumnLevel = 0; ColumnLevel < ColumnFields.Count; ColumnLevel++)
        {
            object[] SubtotalColumnGroupValues = GetSubtotalGroupValues(ColumnValues, ColumnLevel);
            string SubtotalColumnKey = ComposeKey(SubtotalColumnGroupValues);

            UpdateBucket(Result.DetailBuckets, DetailRowKey, SubtotalColumnKey, ValueField, Value);
            CollectRowColumnSubtotalCross(Result, RowFields, ValueField, Value, RowValues, SubtotalColumnKey);

            if (ViewDef.ShowGrandTotals)
                UpdateBucket(Result.GrandBuckets, GrandRowKey, SubtotalColumnKey, ValueField, Value);
        }
    }
    static private void CollectRowColumnSubtotalCross(
        CollectResult Result,
        List<PivotFieldDef> RowFields,
        PivotFieldDef ValueField,
        object Value,
        object[] RowValues,
        string SubtotalColumnKey)
    {
        if (RowFields.Count == 0)
            return;

        for (int RowLevel = 0; RowLevel < RowFields.Count; RowLevel++)
        {
            Dictionary<string, AggregateBucket> BucketMap = GetOrCreateSubtotalBucketMap(Result.SubtotalBucketsByRowLevel, RowLevel);
            object[] SubtotalGroupValues = GetSubtotalGroupValues(RowValues, RowLevel);
            string SubtotalGroupKey = ComposeKey(SubtotalGroupValues);

            UpdateBucket(BucketMap, SubtotalGroupKey, SubtotalColumnKey, ValueField, Value);
        }
    }
    static private void CollectGrandTotals(
        CollectResult Result,
        PivotViewDef ViewDef,
        PivotFieldDef ValueField,
        object Value,
        string DetailColumnKey)
    {
        if (!ViewDef.ShowGrandTotals)
            return;

        UpdateBucket(Result.GrandBuckets, GrandRowKey, DetailColumnKey, ValueField, Value);
        UpdateBucket(Result.GrandBuckets, GrandRowKey, string.Empty, ValueField, Value);
    }
    static private PivotData BuildPivotData(
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ColumnFields,
        List<PivotFieldDef> ValueFields,
        Dictionary<string, object[]> DetailRowKeyMap,
        Dictionary<int, Dictionary<string, object[]>> SubtotalRowKeyMaps,
        Dictionary<string, object[]> DetailColumnKeyMap,
        Dictionary<int, Dictionary<string, object[]>> SubtotalColumnKeyMaps,
        Dictionary<string, AggregateBucket> DetailBuckets,
        Dictionary<int, Dictionary<string, AggregateBucket>> SubtotalBucketsByRowLevel,
        Dictionary<int, Dictionary<string, AggregateBucket>> RowSortBucketsByLevel,
        Dictionary<int, Dictionary<string, AggregateBucket>> ColumnSortBucketsByLevel,
        Dictionary<string, AggregateBucket> GrandBuckets)
    {
        PivotData Result = new();

        AddRowHeaderColumns(Result, RowFields);

        BuildColumnsResult ColumnsResult = BuildValueColumns(
            Result,
            ViewDef,
            ColumnFields,
            ValueFields,
            DetailColumnKeyMap,
            SubtotalColumnKeyMaps,
            ColumnSortBucketsByLevel);

        List<RowOutputEntry> OutputRows = BuildOutputRows(
            ViewDef,
            RowFields,
            ValueFields,
            DetailRowKeyMap,
            SubtotalRowKeyMaps,
            RowSortBucketsByLevel);

        AddDataRows(
            Result,
            OutputRows,
            RowFields,
            ColumnsResult.ValueColumnEntries,
            DetailBuckets,
            SubtotalBucketsByRowLevel,
            GrandBuckets);

        return Result;
    }
    static private void AddRowHeaderColumns(PivotData Result, List<PivotFieldDef> RowFields)
    {
        for (int i = 0; i < RowFields.Count; i++)
        {
            PivotFieldDef RowField = RowFields[i];

            Result.Columns.Add(new PivotDataColumn
            {
                Key = RowField.FieldName,
                Caption = RowField.Caption,
                DataType = typeof(string),
                Format = RowField.Format,
                Kind = PivotDataColumnKind.RowHeader,
                RowLevel = i,
                SourceField = RowField
            });
        }
    }
    static private BuildColumnsResult BuildValueColumns(
        PivotData Result,
        PivotViewDef ViewDef,
        List<PivotFieldDef> ColumnFields,
        List<PivotFieldDef> ValueFields,
        Dictionary<string, object[]> DetailColumnKeyMap,
        Dictionary<int, Dictionary<string, object[]>> SubtotalColumnKeyMaps,
        Dictionary<int, Dictionary<string, AggregateBucket>> ColumnSortBucketsByLevel)
    {
        BuildColumnsResult BuildResult = new();

        BuildResult.OutputColumns.AddRange(BuildOutputColumns(
            ViewDef,
            ColumnFields,
            ValueFields,
            DetailColumnKeyMap,
            SubtotalColumnKeyMaps,
            ColumnSortBucketsByLevel));

        int ValueColumnIndex = 0;

        foreach (ColumnOutputEntry OutputColumn in BuildResult.OutputColumns)
        {
            foreach (PivotFieldDef ValueField in ValueFields)
            {
                string ResultFieldKey = $"C{ValueColumnIndex}";
                ValueColumnIndex++;

                string Caption = ComposeColumnCaption(OutputColumn, ValueField, ColumnFields);

                Result.Columns.Add(new PivotDataColumn
                {
                    Key = ResultFieldKey,
                    Caption = Caption,
                    DataType = GetAggregateResultType(ValueField.ValueAggregateType),
                    Format = ValueField.Format,
                    Kind = PivotDataColumnKind.Value,
                    SourceField = ValueField,
                    Tag = OutputColumn
                });

                BuildResult.ValueColumnEntries.Add(new ValueColumnEntry
                {
                    ColumnKey = OutputColumn.ColumnKey,
                    SourceFieldName = ValueField.FieldName,
                    ResultFieldKey = ResultFieldKey,
                    AggregateType = ValueField.ValueAggregateType
                });
            }
        }

        return BuildResult;
    }
    static private void AddDataRows(
        PivotData Result,
        List<RowOutputEntry> OutputRows,
        List<PivotFieldDef> RowFields,
        List<ValueColumnEntry> ValueColumnEntries,
        Dictionary<string, AggregateBucket> DetailBuckets,
        Dictionary<int, Dictionary<string, AggregateBucket>> SubtotalBucketsByRowLevel,
        Dictionary<string, AggregateBucket> GrandBuckets)
    {
        foreach (RowOutputEntry OutputRow in OutputRows)
        {
            Dictionary<string, AggregateBucket> BucketSource = OutputRow.RowType switch
            {
                PivotDataRowType.Normal => DetailBuckets,
                PivotDataRowType.Subtotal => SubtotalBucketsByRowLevel[OutputRow.Level],
                PivotDataRowType.GrandTotal => GrandBuckets,
                _ => DetailBuckets
            };

            object[] Values = new object[Result.Columns.Count];

            for (int i = 0; i < RowFields.Count; i++)
                Values[i] = OutputRow.RowValues[i];

            for (int i = 0; i < ValueColumnEntries.Count; i++)
            {
                ValueColumnEntry ValueEntry = ValueColumnEntries[i];
                string BucketKey = ComposeBucketKey(OutputRow.RowKey, ValueEntry.ColumnKey, ValueEntry.SourceFieldName);
                int ResultIndex = RowFields.Count + i;

                if (BucketSource.TryGetValue(BucketKey, out AggregateBucket Bucket))
                    Values[ResultIndex] = Bucket.GetResult();
                else
                    Values[ResultIndex] = null;
            }

            Result.Rows.Add(new PivotDataRow
            {
                Values = Values,
                RowType = OutputRow.RowType,
                Level = OutputRow.Level
            });
        }
    }
    static private List<RowOutputEntry> BuildOutputRows(
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ValueFields,
        Dictionary<string, object[]> DetailRowKeyMap,
        Dictionary<int, Dictionary<string, object[]>> SubtotalRowKeyMaps,
        Dictionary<int, Dictionary<string, AggregateBucket>> RowSortBucketsByLevel)
    {
        List<RowOutputEntry> Result = new();

        if (RowFields.Count == 0)
        {
            AddNoRowFieldsEntries(Result, ViewDef);
            return Result;
        }

        List<KeyValuePair<string, object[]>> OrderedDetailRows = GetOrderedDetailRows(
            DetailRowKeyMap,
            RowFields,
            ValueFields,
            RowSortBucketsByLevel);

        if (OrderedDetailRows.Count == 0)
            return Result;

        AppendRowLevel(Result, ViewDef, RowFields, SubtotalRowKeyMaps, OrderedDetailRows, 0);
        AddGrandTotalRow(Result, ViewDef, RowFields);

        return Result;
    }
    static private void AddNoRowFieldsEntries(List<RowOutputEntry> Result, PivotViewDef ViewDef)
    {
        Result.Add(new RowOutputEntry
        {
            RowKey = string.Empty,
            RowValues = Array.Empty<object>(),
            RowType = PivotDataRowType.Normal,
            Level = -1
        });

        if (ViewDef.ShowGrandTotals)
        {
            Result.Add(new RowOutputEntry
            {
                RowKey = GrandRowKey,
                RowValues = Array.Empty<object>(),
                RowType = PivotDataRowType.GrandTotal,
                Level = -1
            });
        }
    }
    static private List<KeyValuePair<string, object[]>> GetOrderedDetailRows(
        Dictionary<string, object[]> DetailRowKeyMap,
        List<PivotFieldDef> RowFields,
        List<PivotFieldDef> ValueFields,
        Dictionary<int, Dictionary<string, AggregateBucket>> RowSortBucketsByLevel)
    {
        return SortAxisEntries(
            DetailRowKeyMap,
            RowFields,
            ValueFields,
            true,
            RowSortBucketsByLevel);
    }
    static private void AddGrandTotalRow(
        List<RowOutputEntry> Result,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields)
    {
        if (!ViewDef.ShowGrandTotals)
            return;

        object[] GrandValues = new object[RowFields.Count];
        GrandValues[0] = "Grand Total";

        Result.Add(new RowOutputEntry
        {
            RowKey = GrandRowKey,
            RowValues = GrandValues,
            RowType = PivotDataRowType.GrandTotal,
            Level = -1
        });
    }
    static private void AppendRowLevel(
        List<RowOutputEntry> Output,
        PivotViewDef ViewDef,
        List<PivotFieldDef> RowFields,
        Dictionary<int, Dictionary<string, object[]>> SubtotalRowKeyMaps,
        List<KeyValuePair<string, object[]>> Rows,
        int Level)
    {
        int Index = 0;

        while (Index < Rows.Count)
        {
            object ValueAtLevel = Rows[Index].Value[Level];
            int GroupStart = Index;
            Index++;

            while (Index < Rows.Count && AreEqual(Rows[Index].Value[Level], ValueAtLevel))
                Index++;

            List<KeyValuePair<string, object[]>> GroupRows = Rows.GetRange(GroupStart, Index - GroupStart);

            if (Level < RowFields.Count - 1)
                AppendRowLevel(Output, ViewDef, RowFields, SubtotalRowKeyMaps, GroupRows, Level + 1);
            else
                AppendDetailRows(Output, GroupRows);

            if (ViewDef.ShowSubtotals && GroupRows.Count > 1)
            {
                object[] SubtotalGroupValues = GetSubtotalGroupValues(GroupRows[0].Value, Level);
                string SubtotalGroupKey = ComposeKey(SubtotalGroupValues);

                Output.Add(new RowOutputEntry
                {
                    RowKey = SubtotalGroupKey,
                    RowValues = SubtotalRowKeyMaps[Level][SubtotalGroupKey],
                    RowType = PivotDataRowType.Subtotal,
                    Level = Level
                });
            }
        }
    }
    static private List<ColumnOutputEntry> BuildOutputColumns(
        PivotViewDef ViewDef,
        List<PivotFieldDef> ColumnFields,
        List<PivotFieldDef> ValueFields,
        Dictionary<string, object[]> DetailColumnKeyMap,
        Dictionary<int, Dictionary<string, object[]>> SubtotalColumnKeyMaps,
        Dictionary<int, Dictionary<string, AggregateBucket>> ColumnSortBucketsByLevel)
    {
        List<ColumnOutputEntry> Result = new();

        if (ColumnFields.Count == 0)
        {
            AddNoColumnFieldsEntry(Result);
            return Result;
        }

        List<KeyValuePair<string, object[]>> OrderedDetailColumns = GetOrderedDetailColumns(
            DetailColumnKeyMap,
            ColumnFields,
            ValueFields,
            ColumnSortBucketsByLevel);

        if (OrderedDetailColumns.Count == 0)
        {
            AddGrandTotalColumnIfNeeded(Result, ViewDef);
            return Result;
        }

        AppendColumnLevel(Result, ViewDef, ColumnFields, SubtotalColumnKeyMaps, OrderedDetailColumns, 0);
        AddGrandTotalColumnIfNeeded(Result, ViewDef);

        return Result;
    }
    static private void AddNoColumnFieldsEntry(List<ColumnOutputEntry> Result)
    {
        Result.Add(new ColumnOutputEntry
        {
            ColumnKey = string.Empty,
            ColumnValues = Array.Empty<object>(),
            IsGrandTotal = true,
            Level = -1
        });
    }
    static private List<KeyValuePair<string, object[]>> GetOrderedDetailColumns(
        Dictionary<string, object[]> DetailColumnKeyMap,
        List<PivotFieldDef> ColumnFields,
        List<PivotFieldDef> ValueFields,
        Dictionary<int, Dictionary<string, AggregateBucket>> ColumnSortBucketsByLevel)
    {
        return SortAxisEntries(
            DetailColumnKeyMap,
            ColumnFields,
            ValueFields,
            false,
            ColumnSortBucketsByLevel);
    }
    static private void AddGrandTotalColumnIfNeeded(List<ColumnOutputEntry> Result, PivotViewDef ViewDef)
    {
        if (!ViewDef.ShowGrandTotals)
            return;

        Result.Add(new ColumnOutputEntry
        {
            ColumnKey = string.Empty,
            ColumnValues = Array.Empty<object>(),
            IsGrandTotal = true,
            Level = -1
        });
    }
    static private void AppendColumnLevel(
        List<ColumnOutputEntry> Output,
        PivotViewDef ViewDef,
        List<PivotFieldDef> ColumnFields,
        Dictionary<int, Dictionary<string, object[]>> SubtotalColumnKeyMaps,
        List<KeyValuePair<string, object[]>> Columns,
        int Level)
    {
        int Index = 0;

        while (Index < Columns.Count)
        {
            object ValueAtLevel = Columns[Index].Value[Level];
            int GroupStart = Index;
            Index++;

            while (Index < Columns.Count && AreEqual(Columns[Index].Value[Level], ValueAtLevel))
                Index++;

            List<KeyValuePair<string, object[]>> GroupColumns = Columns.GetRange(GroupStart, Index - GroupStart);

            if (Level < ColumnFields.Count - 1)
                AppendColumnLevel(Output, ViewDef, ColumnFields, SubtotalColumnKeyMaps, GroupColumns, Level + 1);
            else
                AppendDetailColumns(Output, GroupColumns);

            if (ViewDef.ShowSubtotals && GroupColumns.Count > 1)
            {
                object[] SubtotalGroupValues = GetSubtotalGroupValues(GroupColumns[0].Value, Level);
                string SubtotalGroupKey = ComposeKey(SubtotalGroupValues);

                Output.Add(new ColumnOutputEntry
                {
                    ColumnKey = SubtotalGroupKey,
                    ColumnValues = SubtotalColumnKeyMaps[Level][SubtotalGroupKey],
                    IsSubtotal = true,
                    Level = Level
                });
            }
        }
    }
    static private List<KeyValuePair<string, object[]>> SortAxisEntries(
        Dictionary<string, object[]> SourceMap,
        List<PivotFieldDef> AxisFields,
        List<PivotFieldDef> ValueFields,
        bool IsRowAxis,
        Dictionary<int, Dictionary<string, AggregateBucket>> SortBucketsByLevel)
    {
        List<KeyValuePair<string, object[]>> Items = SourceMap
            .OrderBy(x => x.Value, new ObjectArrayComparer())
            .ToList();

        if (Items.Count <= 1 || AxisFields.Count == 0)
            return Items;

        PivotFieldDef FirstValueField = ValueFields.FirstOrDefault();

        return SortAxisEntriesLevel(Items, AxisFields, FirstValueField, IsRowAxis, SortBucketsByLevel, 0);
    }
    static private List<KeyValuePair<string, object[]>> SortAxisEntriesLevel(
        List<KeyValuePair<string, object[]>> Items,
        List<PivotFieldDef> AxisFields,
        PivotFieldDef ValueField,
        bool IsRowAxis,
        Dictionary<int, Dictionary<string, AggregateBucket>> SortBucketsByLevel,
        int Level)
    {
        if (Items.Count <= 1 || Level >= AxisFields.Count)
            return Items;

        PivotFieldDef AxisField = AxisFields[Level];
        List<GroupInfo> Groups = GroupByLevel(Items, Level);

        foreach (GroupInfo Group in Groups)
        {
            if (Group.Items.Count > 1)
            {
                List<KeyValuePair<string, object[]>> SortedChildren = SortAxisEntriesLevel(
                    Group.Items,
                    AxisFields,
                    ValueField,
                    IsRowAxis,
                    SortBucketsByLevel,
                    Level + 1);

                Group.Items.Clear();
                Group.Items.AddRange(SortedChildren);
            }

            if (AxisField.SortByValue)
                Group.SortValue = GetGroupSortValue(Group.Items[0].Value, Level, ValueField, IsRowAxis, SortBucketsByLevel);
        }

        Groups.Sort((A, B) => CompareGroups(A, B, AxisField.SortByValue, AxisField.SortDirection == ListSortDirection.Descending));

        List<KeyValuePair<string, object[]>> Result = new();

        foreach (GroupInfo Group in Groups)
            Result.AddRange(Group.Items);

        return Result;
    }
    static private List<GroupInfo> GroupByLevel(List<KeyValuePair<string, object[]>> Items, int Level)
    {
        List<GroupInfo> Result = new();

        foreach (KeyValuePair<string, object[]> Item in Items)
        {
            object Value = Item.Value[Level];
            GroupInfo Group = Result.FirstOrDefault(x => AreEqual(x.Value, Value));

            if (Group == null)
            {
                Group = new GroupInfo
                {
                    Value = Value
                };
                Result.Add(Group);
            }

            Group.Items.Add(Item);
        }

        return Result;
    }
    static private decimal? GetGroupSortValue(
        object[] AxisValues,
        int Level,
        PivotFieldDef ValueField,
        bool IsRowAxis,
        Dictionary<int, Dictionary<string, AggregateBucket>> SortBucketsByLevel)
    {
        if (ValueField == null || SortBucketsByLevel == null)
            return null;

        if (!SortBucketsByLevel.TryGetValue(Level, out Dictionary<string, AggregateBucket> BucketMap))
            return null;

        string GroupKey = ComposeKey(GetSubtotalGroupValues(AxisValues, Level));

        decimal Sum = 0m;
        bool Found = false;
        string Prefix = IsRowAxis ? GroupKey + KeySeparator : GrandRowKey + KeySeparator;
        string Suffix = KeySeparator + ValueField.FieldName;

        foreach (KeyValuePair<string, AggregateBucket> Pair in BucketMap)
        {
            if (!Pair.Key.StartsWith(Prefix, StringComparison.Ordinal))
                continue;

            if (!Pair.Key.EndsWith(Suffix, StringComparison.Ordinal))
                continue;

            object Result = Pair.Value.GetResult();

            if (Result == null || Result == DBNull.Value)
                continue;

            try
            {
                Sum += Convert.ToDecimal(Result, CultureInfo.InvariantCulture);
                Found = true;
            }
            catch
            {
            }
        }

        return Found ? Sum : null;
    }
    static private int CompareGroups(GroupInfo A, GroupInfo B, bool SortByValue, bool SortDescending)
    {
        int Result;

        if (SortByValue)
        {
            Result = CompareNullableDecimal(A.SortValue, B.SortValue);

            if (Result == 0)
                Result = ObjectArrayComparer.CompareObject(A.Value, B.Value);
        }
        else
        {
            Result = ObjectArrayComparer.CompareObject(A.Value, B.Value);
        }

        return SortDescending ? -Result : Result;
    }
    static private int CompareNullableDecimal(decimal? A, decimal? B)
    {
        if (A.HasValue && B.HasValue)
            return A.Value.CompareTo(B.Value);

        if (A.HasValue)
            return 1;

        if (B.HasValue)
            return -1;

        return 0;
    }
    static private Dictionary<string, object[]> GetOrCreateSubtotalObjectMap(
        Dictionary<int, Dictionary<string, object[]>> Maps,
        int Level)
    {
        if (!Maps.TryGetValue(Level, out Dictionary<string, object[]> Map))
        {
            Map = new Dictionary<string, object[]>();
            Maps[Level] = Map;
        }

        return Map;
    }
    static private Dictionary<string, AggregateBucket> GetOrCreateSubtotalBucketMap(
        Dictionary<int, Dictionary<string, AggregateBucket>> Maps,
        int Level)
    {
        if (!Maps.TryGetValue(Level, out Dictionary<string, AggregateBucket> Map))
        {
            Map = new Dictionary<string, AggregateBucket>();
            Maps[Level] = Map;
        }

        return Map;
    }
    static private object[] GetSubtotalGroupValues(object[] DetailValues, int Level)
    {
        object[] Result = new object[DetailValues.Length];

        for (int i = 0; i <= Level; i++)
            Result[i] = DetailValues[i];

        return Result;
    }
    static private object[] GetSubtotalDisplayValues(object[] DetailValues, int Level)
    {
        object[] Result = new object[DetailValues.Length];

        for (int i = 0; i < Level; i++)
            Result[i] = DetailValues[i];

        Result[Level] = $"{DetailValues[Level]} Total";
        return Result;
    }
    static private void AppendDetailRows(List<RowOutputEntry> Output, List<KeyValuePair<string, object[]>> Rows)
    {
        foreach (KeyValuePair<string, object[]> Pair in Rows)
        {
            Output.Add(new RowOutputEntry
            {
                RowKey = Pair.Key,
                RowValues = Pair.Value,
                RowType = PivotDataRowType.Normal,
                Level = -1
            });
        }
    }
    static private void AppendDetailColumns(List<ColumnOutputEntry> Output, List<KeyValuePair<string, object[]>> Columns)
    {
        foreach (KeyValuePair<string, object[]> Pair in Columns)
        {
            Output.Add(new ColumnOutputEntry
            {
                ColumnKey = Pair.Key,
                ColumnValues = Pair.Value,
                IsSubtotal = false,
                IsGrandTotal = false,
                Level = -1
            });
        }
    }
    static private string ComposeColumnCaption(ColumnOutputEntry OutputColumn, PivotFieldDef ValueField, List<PivotFieldDef> ColumnFields)
    {
        if (OutputColumn.IsGrandTotal)
            return $"Total{Environment.NewLine}{ValueField.Caption}";

        if (!OutputColumn.IsSubtotal)
        {
            string DetailCaption = ComposeCaption(OutputColumn.ColumnValues);

            return string.IsNullOrWhiteSpace(DetailCaption)
                ? ValueField.Caption
                : $"{DetailCaption}{Environment.NewLine}{ValueField.Caption}";
        }

        int Level = OutputColumn.Level;
        List<string> Parts = new();

        for (int i = 0; i < Level; i++)
        {
            object Value = OutputColumn.ColumnValues[i];
            if (Value != null)
                Parts.Add(Value.ToString());
        }

        object SubtotalValue = OutputColumn.ColumnValues[Level];
        if (SubtotalValue != null)
            Parts.Add(SubtotalValue.ToString());

        Parts.Add(ValueField.Caption);

        return string.Join(Environment.NewLine, Parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
    static private void UpdateBucket(
        Dictionary<string, AggregateBucket> BucketMap,
        string RowKey,
        string ColumnKey,
        PivotFieldDef ValueField,
        object Value)
    {
        string BucketKey = ComposeBucketKey(RowKey, ColumnKey, ValueField.FieldName);

        if (!BucketMap.TryGetValue(BucketKey, out AggregateBucket Bucket))
        {
            Bucket = new AggregateBucket(ValueField.ValueAggregateType);
            BucketMap[BucketKey] = Bucket;
        }

        Bucket.Add(Value);
    }
    static private string ComposeBucketKey(string RowKey, string ColumnKey, string ValueFieldName)
    {
        return $"{RowKey}{KeySeparator}{ColumnKey}{KeySeparator}{ValueFieldName}";
    }
    static private string ComposeKey(object[] Values)
    {
        if (Values == null || Values.Length == 0)
            return string.Empty;

        return string.Join(KeySeparator, Values.Select(x => x == null ? NullToken : x.ToString()));
    }
    static private string ComposeCaption(object[] Values)
    {
        if (Values == null || Values.Length == 0)
            return string.Empty;

        return string.Join(Environment.NewLine, Values.Select(x => x?.ToString() ?? string.Empty));
    }
    static private RowSourceEntry CreateRowSourceEntry<T>(T Item)
    {
        RowSourceEntry Entry = new();

        if (Item is DataRowView RowView)
        {
            foreach (DataColumn Column in RowView.DataView.Table.Columns)
            {
                object Value = RowView[Column.ColumnName];
                Entry.Values[Column.ColumnName] = Value == DBNull.Value ? null : Value;
            }
        }
        else
        {
            PropertyInfo[] Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo Property in Properties)
                Entry.Values[Property.Name] = Property.GetValue(Item);
        }

        return Entry;
    }
    static private object GetValue(RowSourceEntry Entry, string FieldName)
    {
        if (Entry.Values.TryGetValue(FieldName, out object Value))
            return Value;

        throw new ApplicationException($"Field or property '{FieldName}' was not found.");
    }
    static private bool AreEqual(object A, object B)
    {
        if (A == null && B == null)
            return true;

        if (A == null || B == null)
            return false;

        return Equals(A, B);
    }
    static private Type GetAggregateResultType(PivotValueAggregateType AggregateType)
    {
        return AggregateType switch
        {
            PivotValueAggregateType.Count => typeof(int),
            PivotValueAggregateType.CountDistinct => typeof(int),
            PivotValueAggregateType.StdDev => typeof(decimal),
            PivotValueAggregateType.StdDevP => typeof(decimal),
            PivotValueAggregateType.Variance => typeof(decimal),
            PivotValueAggregateType.VarianceP => typeof(decimal),
            PivotValueAggregateType.Sum => typeof(decimal),
            PivotValueAggregateType.Avg => typeof(decimal),
            PivotValueAggregateType.Min => typeof(decimal),
            PivotValueAggregateType.Max => typeof(decimal),
            PivotValueAggregateType.Product => typeof(decimal),
            PivotValueAggregateType.None => typeof(decimal),
            _ => typeof(object)
        };
    }
    static private decimal SqrtDecimal(decimal Value)
    {
        if (Value < 0m)
            throw new ArgumentOutOfRangeException(nameof(Value));

        if (Value == 0m)
            return 0m;

        decimal Current = (decimal)Math.Sqrt((double)Value);

        for (int i = 0; i < 10; i++)
            Current = (Current + Value / Current) / 2m;

        return Current;
    }

    static public PivotData Execute(DataView Source, PivotViewDef ViewDef)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        return Execute(Source.Cast<DataRowView>(), ViewDef);
    }
    static public PivotData Execute<T>(IEnumerable<T> Source, PivotViewDef ViewDef)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));
        if (ViewDef == null)
            throw new ArgumentNullException(nameof(ViewDef));

        //Def.Normalize();
        ValidateDef(ViewDef);

        List<PivotFieldDef> RowFields = ViewDef.GetRowFields().ToList();
        List<PivotFieldDef> ColumnFields = ViewDef.GetColumnFields().ToList();
        List<PivotFieldDef> ValueFields = ViewDef.GetValueFields().ToList();

        CollectResult Collected = Collect(Source, ViewDef, RowFields, ColumnFields, ValueFields);

        return BuildPivotData(
            ViewDef,
            RowFields,
            ColumnFields,
            ValueFields,
            Collected.DetailRowKeyMap,
            Collected.SubtotalRowKeyMaps,
            Collected.DetailColumnKeyMap,
            Collected.SubtotalColumnKeyMaps,
            Collected.DetailBuckets,
            Collected.SubtotalBucketsByRowLevel,
            Collected.RowSortBucketsByLevel,
            Collected.ColumnSortBucketsByLevel,
            Collected.GrandBuckets);
    }
}

 