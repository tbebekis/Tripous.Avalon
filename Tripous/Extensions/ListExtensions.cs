namespace Tripous
{
    using System.ComponentModel;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Data;
    
    /// <summary>
    /// Extensions
    /// </summary>
    static public class ListExtensions
    {

        /// <summary>
        /// Returns true if Index is in the range 0..List.Count - 1
        /// </summary>
        static public bool IsValidIndex(this IList List, int Index)
        {
            return (List != null) && (Index >= 0) && (Index < List.Count);
        }


        /// <summary>
        /// Returns the index of Value in List, case insensitively, if exists, else -1.
        /// </summary>
        static public int IndexOfText(this IList<string> List, string Value)
        {
            if (List != null)
            {
                for (int i = 0; i < List.Count; i++)
                    if (string.Compare(List[i], Value, StringComparison.InvariantCultureIgnoreCase) == 0)  
                        return i;
            }
            return -1;
        }
        /// <summary>
        /// Returns trur if Value exists in List, case insensitively.
        /// </summary>
        static public bool ContainsText(this IList<string> List, string Value)
        {
            return IndexOfText(List, Value) != -1;
        }
        /// <summary>
        /// Returns the content of List as an object array.
        /// </summary>
        static public object[] AsArray(this IList List)
        {
            if (List != null)
            {
                object[] Result = new object[List.Count];
                for (int i = 0; i < List.Count; i++)
                    Result[i] = List[i];
                return Result;
            }

            return new object[0];
        }
        /// <summary>
        /// Returns the content of a generic string list as a string
        /// </summary>
        static public string AsTextLines(this IList<string> List)
        {

            if ((List == null) || (List.Count == 0))
                return string.Empty;

            StringBuilder SB = new StringBuilder();
            foreach (string S in List)
            {
                SB.AppendLine(S);
            }

            return SB.ToString();
        }
        /// <summary>
        /// Gets the whole content of the List as a single line of text,
        /// where each line ends with a comma (,)
        /// </summary>
        static public string CommaText_OLD(this IList<string> List)
        {
            if (List == null)
                return string.Empty;

            StringBuilder SB = new StringBuilder();
            foreach (string S in List)
            {
                if (!string.IsNullOrWhiteSpace(S))
                {
                    SB.Append(S);
                    SB.Append(',');
                }
            }

            if (SB.Length > 0)
            {
                if (SB[SB.Length - 1] == ',')
                    SB.Remove(SB.Length - 1, 1);
            }

            return SB.ToString();
        }
        /// <summary>
        /// Gets the whole content of the List as a single line of text,
        /// where each line ends with a comma (,)
        /// </summary>
        static public string CommaText(this IList<string> List, bool WithLineBreaks = false)
        {
            if ((List == null) || (List.Count == 0))
                return string.Empty;

            List<string> Temp = new List<string>();
            for (int i = 0; i < List.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(List[i]))
                    Temp.Add(List[i]);
            }

            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < Temp.Count; i++)
            {
                SB.Append(i == Temp.Count - 1 ? Temp[i] : Temp[i] + ", ");
                if (WithLineBreaks)
                {
                    SB.AppendLine();
                }
            }

            return SB.ToString();
        }


        /// <summary>
        /// Exchanges source to source positions.
        /// </summary>
        static public void Exchange(this IList List, int SourceIndex, int DestIndex)
        {

            if ((SourceIndex >= 0) && (SourceIndex <= List.Count - 1) && (DestIndex >= 0) && (DestIndex <= List.Count - 1))
            {
                object Source = List[SourceIndex];
                object Dest = List[DestIndex];

                List[SourceIndex] = Dest;
                List[DestIndex] = Source;
            }


        }
        /// <summary>
        /// Exchanges source to source positions.
        /// </summary>
        static public void Exchange(this IList List, object Source, object Dest)
        {
            Exchange(List, List.IndexOf(Source), List.IndexOf(Dest));
        }
        /// <summary>
        /// Returns true if item can change position.
        /// <para>NOTE: Down = true means towards to 0, where false means towards to List.Count    </para>
        /// </summary>
        static public bool CanMove(this IList List, int Index, bool Down)
        {
            if (Down)   // towards to 0
                return (Index > 0);
            else        // towards to List.Count   
                return ((Index >= 0) && (Index <= List.Count - 1));
        }
        /// <summary>
        /// Returns true if item can change position.
        /// </summary>
        static public bool CanMove(this IList List, object Obj, bool Down)
        {
            return CanMove(List, List.IndexOf(Obj), Down);
        }
        /// <summary>
        /// Moves item a position up or down.
        /// Returns true if item can change position.
        /// </summary>
        static public bool Move(this IList List, int Index, bool Down)
        {
            bool Res = CanMove(List, Index, Down);

            int NewIndex;

            if (Down)
                NewIndex = Index - 1;
            else
                NewIndex = Index + 1;

            Exchange(List, Index, NewIndex);

            return Res;

        }
        /// <summary>
        /// Moves item a position up or down.
        /// Returns true if item can change position.
        /// </summary>
        static public bool Move(this IList List, object Obj, bool Down)
        {
            return Move(List, List.IndexOf(Obj), Down);
        }
        /// <summary>
        /// Saves List to FileName
        /// </summary>
        static public void SaveToFile(this IList List, string FileName)
        {
            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i] != null)
                {
                    SB.Append(List[i].ToString());
                    SB.Append(Environment.NewLine);
                }
            }

            File.WriteAllText(FileName, SB.ToString());
        }
        /// <summary>
        /// Loads List from FileName
        /// </summary>
        static public void LoadFromFile(this IList List, string FileName)
        {
            if (!List.IsReadOnly)
            {
                List.Clear();

                string Text = File.ReadAllText(FileName); // Streams.LoadTextFromFile(FileName, Encoding.Default);
                string[] Lines = Text.Split(Environment.NewLine);
                for (int i = 0; i < Lines.Length; i++)
                    List.Add(Lines[i]);
            }
        }

        /// <summary>
        /// Converts a generic list to DataTable.
        /// <para>Property names of the list element type become column names in the DataTable.</para>
        /// </summary>
        static public DataTable ToDataTable<T>(this IList<T> SourceList)
        {
            PropertyDescriptorCollection PropList = TypeDescriptor.GetProperties(typeof(T));

            DataTable Result = new DataTable();
            foreach (PropertyDescriptor PropDes in PropList)
                Result.Columns.Add(PropDes.Name, Nullable.GetUnderlyingType(PropDes.PropertyType) ?? PropDes.PropertyType);

            DataRow Row;
            foreach (T Item in SourceList)
            {
                Row = Result.NewRow();
                foreach (PropertyDescriptor PropDes in PropList)
                    Row[PropDes.Name] = PropDes.GetValue(Item) ?? DBNull.Value;
                Result.Rows.Add(Row);
            }

            return Result;
        }
        /// <summary>
        /// Returns a specified string list as a dictionary.
        /// <para>Each string in the list must contain an equal sign character, e.g. Key=Value, for this to succeed.</para>
        /// </summary>
        static public Dictionary<string, string> ToDictionary(this IList<string> SourceList)
        {
            Dictionary<string, string> Dictionary = new Dictionary<string, string>();

            if (SourceList != null && SourceList.Count > 0)
            {
                string[] Parts;
                foreach (string TitleKey in SourceList)
                {
                    if (!string.IsNullOrWhiteSpace(TitleKey))
                    {
                        Parts = TitleKey.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if (Parts.Length >= 2)
                        {
                            Dictionary[Parts[0]] = Parts[1];
                        }
                    }
                }
            }

            return Dictionary;
        }


        /// <summary>
        /// Splits a sequence into chunks of a defined size.
        /// <example>
        /// <code>
        /// int[] numbers = { 1, 2, 3, 4, 5, 6, 7 };
        /// var NumberLists = Split(numbers, 3);        // results in 3 lists as {1, 2, 3}, {4, 5, 6}, {7}
        /// </code>
        /// </example>
        /// </summary>
        static public List<List<T>> Split<T>(this IEnumerable<T> Source, int ChunkSize)
        {
            int i = 0;
            IEnumerable<IGrouping<int, T>> Groups = Source.GroupBy(item => i++ / ChunkSize);
            IEnumerable<List<T>> Lists = Groups.Select(group => group.ToList());
            List<List<T>> Result = Lists.ToList();
            return Result;
        }


        /// <summary>
        ///  Used in constructing SQL statements that contain a WHERE clause of the type
        ///  <para><c>  where FIELD_NAME in (...)</c></para>
        ///  This method limits the number of elements inside the in (...) according to the passed in ModValue, in order
        ///  to avoid problems with database servers that have such a limit.
        ///  <para>It returns a string array where each element contains no more than ModValue of the FieldName values from Table.</para>
        /// </summary>
        static public string[] GetKeyValuesList(this IList<object> SourceList, string FieldName, int ModValue, bool DiscardBelowZeroes)
        {
            List<string> List = new List<string>();

            if ((SourceList != null) && (SourceList.Count > 0))
            {
                bool IsString = false;

                foreach (var v in SourceList)
                {
                    if (v != null)
                    {
                        IsString = v.GetType() == typeof(string);
                        break;
                    }
                }


                int Counter = 0;
                string S = "";
                object Value;

                for (int i = 0; i < SourceList.Count; i++)
                {
                    if (SourceList[i] != null)
                    {
                        Value = SourceList[i];
                        if (IsString)
                        {
                            S = S + string.Format("'{0}', ", Value.ToString());
                            Counter++;
                        }
                        else if ((!DiscardBelowZeroes) || (DiscardBelowZeroes && (Sys.AsInteger(Value, -1) > 0)))
                        {
                            S = S + string.Format("{0}, ", Value);
                            Counter++;
                        }
                    }

                    if (Counter % ModValue == 0)
                    {
                        List.Add(S);
                        S = "";
                    }

                }


                if (S != "")
                    List.Add(S);


                // remove last comma
                for (int i = 0; i < List.Count; i++)
                {
                    S = List[i].Trim();
                    if (S.Length > 1)
                        S = S.Remove(S.Length - 1, 1);

                    List[i] = string.Format(" ({0}) ", S);
                }

            }

            return List.ToArray();
        }
    }
}
