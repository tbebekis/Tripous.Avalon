

namespace Tripous
{
    using System.Collections;
    using System.Data;
    
    /// <summary>
    /// Extensions
    /// </summary>
    static public class DataColumnExtensions
    {
        /// <summary>
        /// Creates and returns a new DataColumn based on Source.
        /// </summary>
        static public DataColumn CloneColumn(this DataColumn Source)
        {
            DataColumn Result = new DataColumn(Source.ColumnName, Source.DataType);


            Result.AllowDBNull = Source.AllowDBNull;
            Result.AutoIncrement = Source.AutoIncrement;
            Result.AutoIncrementSeed = Source.AutoIncrementSeed;
            Result.AutoIncrementStep = Source.AutoIncrementStep;
            Result.Caption = Source.Caption;
            Result.ColumnMapping = Source.ColumnMapping;
            Result.DateTimeMode = Source.DateTimeMode;
            Result.DefaultValue = Source.DefaultValue;
            Result.Expression = Source.Expression;
            Result.MaxLength = Source.MaxLength;
            Result.Namespace = Source.Namespace;
            Result.Prefix = Source.Prefix;
            Result.ReadOnly = Source.ReadOnly;
            Result.Unique = Source.Unique;

            foreach (DictionaryEntry Entry in Source.ExtendedProperties)
                Result.ExtendedProperties.Add(Entry.Key, Entry.Value);

            return Result;
        }
        /// <summary>
        /// Copies SourceColumn to Target DataTable.
        /// </summary>
        static public void CopyStructureTo(this DataColumn SourceColumn, DataTable Target, bool AllowDBNull)
        {
            DataColumn Column = CloneColumn(SourceColumn);
            Column.AllowDBNull = AllowDBNull;
            Target.Columns.Add(Column);
        }
        /// <summary>
        /// Copies SourceColumn to Target DataTable.
        /// </summary>
        static public void CopyStructureTo(this DataColumn SourceColumn, DataTable Dest)
        {
            CopyStructureTo(SourceColumn, Dest, true);
        }


        /* Column pseudo-properties stored in Column.ExtendedProperties */
        /// <summary>
        /// Returns true if Column is not null and has a Visible pseudo-property defined in its ExtendedProperties.
        /// </summary>
        static public bool IsVisibleSet(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.ContainsKey("Visible");
        }
        /// <summary>
        /// Returns true if the Column satisfies the condition (pseudo-property).
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public bool IsVisible(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.AsBoolean("Visible", true);
        }
        /// <summary>
        /// Sets a condition (pseudo-property) of the Column to the Value.
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public void IsVisible(this DataColumn Column, bool Value)
        {
            if (Column != null)
                Column.ExtendedProperties["Visible"] = Value;
        }
        /// <summary>
        /// Returns true if the Column satisfies the condition (pseudo-property).
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public bool IsDateTime(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.AsBoolean("IsDateTime", false);
        }
        /// <summary>
        /// Sets a condition (pseudo-property) of the Column to the Value.
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public void IsDateTime(this DataColumn Column, bool Value)
        {
            if (Column != null)
                Column.ExtendedProperties["IsDateTime"] = Value;
        }
        /// <summary>
        /// Returns true if the Column satisfies the condition (pseudo-property).
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public bool IsDate(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.AsBoolean("IsDate", false);
        }
        /// <summary>
        /// Sets a condition (pseudo-property) of the Column to the Value.
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public void IsDate(this DataColumn Column, bool Value)
        {
            if (Column != null)
                Column.ExtendedProperties["IsDate"] = Value;
        }
        /// <summary>
        /// Returns true if the Column satisfies the condition (pseudo-property).
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public bool IsTime(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.AsBoolean("IsTime", false);
        }
        /// <summary>
        /// Sets a condition (pseudo-property) of the Column to the Value.
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public void IsTime(this DataColumn Column, bool Value)
        {
            if (Column != null)
                Column.ExtendedProperties["IsTime"] = Value;
        }
        /// <summary>
        /// Returns true if the Column satisfies the condition (pseudo-property).
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public bool IsCheckBox(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.AsBoolean("IsCheckBox", false);
        }
        /// <summary>
        /// Sets a condition (pseudo-property) of the Column to the Value.
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public void IsCheckBox(this DataColumn Column, bool Value)
        {
            if (Column != null)
                Column.ExtendedProperties["IsCheckBox"] = Value;
        }
        /// <summary>
        /// Returns true if the Column satisfies the condition (pseudo-property).
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public bool IsMemo(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.AsBoolean("IsMemo", false);
        }
        /// <summary>
        /// Sets a condition (pseudo-property) of the Column to the Value.
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public void IsMemo(this DataColumn Column, bool Value)
        {
            if (Column != null)
                Column.ExtendedProperties["IsMemo"] = Value;
        }
        /// <summary>
        /// Returns true if the Column satisfies the condition (pseudo-property).
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public bool IsImage(this DataColumn Column)
        {
            return Column == null ? false : Column.ExtendedProperties.AsBoolean("IsImage", false);
        }
        /// <summary>
        /// Sets a condition (pseudo-property) of the Column to the Value.
        /// <para>NOTE: The condition is a pseudo-property defined in its ExtendedProperties.</para>
        /// </summary>
        static public void IsImage(this DataColumn Column, bool Value)
        {
            if (Column != null)
                Column.ExtendedProperties["IsImage"] = Value;
        }
        /// <summary>
        /// Returns the value of the TitleKey key from the Column.ExtendedProperties
        /// </summary>
        static public string TitleKey(this DataColumn Column)
        {
            return Column == null ? string.Empty : Column.ExtendedProperties.AsString("TitleKey", string.Empty);
        }
        /// <summary>
        /// Sets the Value of the TitleKey key to the Column.ExtendedProperties
        /// </summary>
        static public void SetTitleKey(this DataColumn Column, string Value)
        {
            if (Column != null)
                Column.ExtendedProperties["TitleKey"] = Value;
        }

        /// <summary>
        /// Sets the Width extended property
        /// </summary>
        static public void SetWidth(this DataColumn Column, int Value)
        {
            if (Column != null)
                Column.ExtendedProperties["Width"] = Value;
        }
        /// <summary>
        /// Gets the Width extended property
        /// </summary>
        static public int GetWidth(this DataColumn Column, int Default)
        {
            if (Column != null)
                return Column.ExtendedProperties.AsInteger("Width", Default); //Sys.AsInteger(Column.ExtendedProperties["Width"], Default);
            else
                return 0;
        }
        /// <summary>
        /// Gets the Width extended property
        /// </summary>
        static public int GetWidth(this DataColumn Column)
        {
            return GetWidth(Column, 50);
        }
        /// <summary>
        /// Returns true if Column is not null and has a Width pseudo-property defined in its ExtendedProperties.
        /// </summary>
        static public bool IsWidthSet(this DataColumn Column)
        {
            if (Column != null)
                return Column.ExtendedProperties.ContainsKey("Width");
            return false;
        }


        /// <summary>
        /// Sets the Column as a negative autoincrement one, if it is of an integer type.
        /// </summary>
        static public void SetAsAutoInc(this DataColumn Column)
        {
            if ((Column != null) && (Column.DataType == typeof(int) || Column.DataType == typeof(System.Int64)))  
            {
                Column.AutoIncrement = true;
                Column.AutoIncrementSeed = -1;
                Column.AutoIncrementStep = -1;
            }
        }

        /// <summary>
        /// Returns a character that denotes the datatype of the column
        /// </summary>
        static public string DataTypeToJson(this DataColumn Column)
        {
            if (Column.IsMemo())
                return "Memo";

            if (Column.IsImage())
                return "Blob";

            if (Column.DataType == typeof(string))
                return "String";
 
            if (Column.DataType == typeof(int) || Column.DataType == typeof(System.Int64))
                return Column.IsCheckBox()? "Boolean": "Integer";

            if (Column.DataType == typeof(float) || Column.DataType == typeof(double))
                return "Float";

            if (Column.DataType == typeof(decimal))
                return "Decimal";

            if (Column.DataType == typeof(DateTime))
                return Column.IsDate() ? "Date" : "DateTime";

            if (Column.DataType == typeof(byte[]) || Column.DataType == typeof(object))
                return "Blob";

            return "Unknown";
        }
        /// <summary>
        /// Converts a json datatype to column datatype
        /// </summary>
        static public void JsonToDataType(this DataColumn Column, string Json)
        {
            switch (Json)
            {
                case "String":
                    Column.DataType = typeof(string);
                    break;
                case "Integer":
                case "Boolean":
                    Column.DataType = typeof(int);
                    break;
                case "Float":
                    Column.DataType = typeof(double);
                    break;
                case "Decimal":
                    Column.DataType = typeof(decimal);
                    break;
                case "Date":
                case "DateTime":
                    Column.DataType = typeof(DateTime);
                    break;
                case "Blob":
                    Column.DataType = typeof(byte[]);
                    break;
                case "Memo":
                    Column.DataType = typeof(string);
                    break;
            }
 
        }
    }
}
