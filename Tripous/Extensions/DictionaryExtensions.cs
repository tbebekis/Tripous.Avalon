namespace Tripous
{
    using System.Xml;
    using System.Collections;
    using System.Text;
    using System.Data;
    
    /// <summary>
    /// A helper class for returning typed values from an IDictionary in a safe manner.
    /// It assumes that the FlagName is always a string
    /// </summary>
    static public class DictionaryExtensions
    {
        /// <summary>
        /// Returns the Value of FlagName if exists, else Default
        /// </summary>
        static public object AsObject(this IDictionary Dictionary, string Key, object Default)
        {
            if (Default == null)
                throw new TripousArgumentNullException("Default", "Default parameter can not be null");

            if ((Dictionary.Contains(Key)) && (Dictionary[Key] != null))
            {
                if (Dictionary[Key].GetType().Equals(Default.GetType()))
                    return Dictionary[Key];
            }

            return Default;
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else Default
        /// </summary>
        static public int AsInteger(this IDictionary Dictionary, string Key, int Default)
        {
            if ((Dictionary.Contains(Key)) && (Dictionary[Key] != null))
            {
                if (Dictionary[Key].GetType().Equals(typeof(int)))
                    return (int)Dictionary[Key];
                else
                    return Convert.ToInt32(Dictionary[Key]);
            }

            return Default;
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else Default
        /// </summary>
        static public int AsInteger(this IDictionary Dictionary, string Key)
        {
            return AsInteger(Dictionary, Key, 0);
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else 0
        /// </summary>
        static public string AsString(this IDictionary Dictionary, string Key, string Default)
        {

            if ((Dictionary.Contains(Key)) && (Dictionary[Key] != null))
            {
                if (Dictionary[Key].GetType().Equals(typeof(string)))
                    return (string)Dictionary[Key];
                else
                    return Dictionary[Key].ToString();
            }

            return Default;
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else the empty string
        /// </summary>
        static public string AsString(this IDictionary Dictionary, string Key)
        {
            return AsString(Dictionary, Key, "");
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else Default
        /// </summary>
        static public double AsFloat(this IDictionary Dictionary, string Key, double Default)
        {
            if ((Dictionary.Contains(Key)) && (Dictionary[Key] != null))
            {
                if (Dictionary[Key].GetType().Equals(typeof(double)))
                    return (double)Dictionary[Key];
                else
                    return Convert.ToInt32(Dictionary[Key]);
            }

            return Default;
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else 0
        /// </summary>
        static public double AsFloat(this IDictionary Dictionary, string Key)
        {
            return AsFloat(Dictionary, Key, 0);
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else Default
        /// </summary>
        static public bool AsBoolean(this IDictionary Dictionary, string Key, bool Default)
        {

            if ((Dictionary.Contains(Key)) && (Dictionary[Key] != null))
            {
                if (Dictionary[Key].GetType().Equals(typeof(bool)))
                    return (bool)Dictionary[Key];
                else
                    return Convert.ToBoolean(Dictionary[Key]);
            }

            return Default;
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else false
        /// </summary>
        static public bool AsBoolean(this IDictionary Dictionary, string Key)
        {
            return AsBoolean(Dictionary, Key, false);
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else Default
        /// </summary>
        static public DateTime AsDateTime(this IDictionary Dictionary, string Key, DateTime Default)
        {
            if ((Dictionary.Contains(Key)) && (Dictionary[Key] != null))
            {
                if (Dictionary[Key].GetType().Equals(typeof(DateTime)))
                    return (DateTime)Dictionary[Key];
                else
                    return Convert.ToDateTime(Dictionary[Key]);
            }

            return Default;
        }
        /// <summary>
        /// Returns the Value of FlagName if exists, else DateTime.Now
        /// </summary>
        static public DateTime AsDateTime(this IDictionary Dictionary, string Key)
        {
            return AsDateTime(Dictionary, Key, DateTime.Now);
        }

        /* string, string extensions */
        /// <summary>
        /// Converts Dictionary to a text lines, where each line has the format FlagName=Value
        /// </summary>
        static public string DicToText(this IDictionary<string, string> Dictionary)
        {

            StringBuilder SB = new StringBuilder("");

            foreach (string Key in Dictionary.Keys)
            {
                SB.Append(Key);
                SB.Append("=");
                SB.Append(Dictionary[Key]);
                SB.Append(Environment.NewLine);
            }

            return SB.ToString();
        }
        /// <summary>
        /// Loads Dictionary using Text. Text must be zero or more lines, where each line has the format FlagName=Value 
        /// </summary>
        static public void TextToDic(this IDictionary<string, string> Dictionary, string Text)
        {

            Dictionary.Clear();

            string[] Lines = Text.Split(Environment.NewLine);
            string[] KeyValue;
            string Key;
            string Value;
            for (int i = 0; i < Lines.Length; i++)
            {
                KeyValue = Lines[i].Split('=');
                if (KeyValue.Length > 0)
                    Key = KeyValue[0];
                else
                    Key = "";

                if (KeyValue.Length > 1)
                    Value = KeyValue[1];
                else
                    Value = "";

                Dictionary.Add(Key, Value);
            }


        }
        /// <summary>
        /// Saves Dictionary to ParentNode. A child node with ListNodeName is created to that ParentNode. Then each Dictionary entry
        /// becomes a child Node to that ListNodeName Node, with ItemNodeName. FlagName and Value of each Dictionary entry are saved as
        /// attributes to that item Node.
        /// </summary>
        static public void SaveToXml(this IDictionary<string, string> Dictionary, XmlNode ParentNode, string ListNodeName, string ItemNodeName)
        {
            ICollection<string> Keys = Dictionary.Keys;

            ParentNode = ParentNode.AppendChild(ParentNode.OwnerDocument.CreateElement(ListNodeName));
            XmlElement El;

            foreach (string Key in Keys)
            {
                El = ParentNode.AppendChild(ParentNode.OwnerDocument.CreateElement(ListNodeName)) as XmlElement;
                El.SetAttribute("FlagName", Key);
                El.SetAttribute("Value", Dictionary[Key]); 
            }
        }
        /// <summary>
        /// Saves Dictionary to ParentNode. A child node with ListNodeName is created to that ParentNode. Then each Dictionary entry
        /// becomes a child Node to that ListNodeName Node, with "item" as node name. FlagName and Value of each Dictionary entry are saved as
        /// attributes to that item Node.
        /// </summary>
        static public void SaveToXml(this IDictionary<string, string> Dictionary, XmlNode ParentNode, string ListNodeName)
        {
            SaveToXml(Dictionary, ParentNode, ListNodeName, "item");
        }
        /// <summary>
        /// Loads Dictionary from ParentNode. See SaveToXml() for a description of the node tree
        /// </summary>
        static public void LoadFromXml(this IDictionary<string, string> Dictionary, XmlNode ParentNode, string ListNodeName)
        {
            Dictionary.Clear();

            ParentNode = ParentNode.SelectSingleNode(ListNodeName);  
            if ((ParentNode != null) && ParentNode.HasChildNodes)
            {
                XmlElement El;
                string Key;
                string Value;
                foreach (XmlNode Node in ParentNode.ChildNodes)
                {
                    if (Node is XmlElement)
                    {
                        El = Node as XmlElement;
                        Key = El.GetAttribute("FlagName");
                        Value = El.GetAttribute("Value");
                        if (!string.IsNullOrWhiteSpace(Key))
                            Dictionary[Key] = Value;
                    }
                }
            }
        }
       
        
        /// <summary>
        /// Assigns Dest to Source
        /// </summary>
        static public void Assign<TKey, TValue>(this IDictionary<TKey, TValue> Source, IDictionary<TKey, TValue> Dest, bool ClearSource = true)
        {
            if (ClearSource)
                Source.Clear();

            if (Dest != null)
                foreach (TKey Key in Dest.Keys)
                {
                    Source.Add(Key, Dest[Key]);
                }
        }

        /* string, object extensions */
        /// <summary>
        /// Returns the value of Key if exists and not null, else returns Default.
        /// </summary>
        static public T ValueOf<T>(this IDictionary<string, object> Data, string Key, T Default)
        {
            if ((Data != null) && Data.ContainsKey(Key))
            {
                if (Data[Key] != null)
                    return (T)Data[Key];
            }

            return Default;
        }

        /// <summary>
        /// Returns the Value of Key
        /// </summary>
        static public object AsObject(this IDictionary<string, object> Data, string Key)
        {
            return ValueOf(Data, Key, default(object));
        }
        /// <summary>
        /// Returns the Value of Key
        /// </summary>
        static public string AsString(this IDictionary<string, object> Data, string Key)
        {
            return ValueOf(Data, Key, default(string));
        }
        /// <summary>
        /// Returns the Value of Key
        /// </summary>
        static public int AsInteger(this IDictionary<string, object> Data, string Key)
        {
            return ValueOf(Data, Key, default(int));
        }
        /// <summary>
        /// Returns the Value of Key
        /// </summary>
        static public bool AsBoolean(this IDictionary<string, object> Data, string Key)
        {
            return ValueOf(Data, Key, default(bool));
        }
        /// <summary>
        /// Returns the Value of Key
        /// </summary>
        static public double AsFloat(this IDictionary<string, object> Data, string Key)
        {
            return ValueOf(Data, Key, default(double));
        }
        /// <summary>
        /// Returns the Value of Key
        /// </summary>
        static public DateTime AsDateTime(this IDictionary<string, object> Data, string Key)
        {
            return ValueOf(Data, Key, default(DateTime));
        }


        /// <summary>
        /// Returns the value of Key if exists and not null, else returns Default.
        /// </summary>
        static public T ValueOf<T>(this Hashtable Data, string Key, T Default)
        {
            if ((Data != null) && Data.ContainsKey(Key))
            {
                if (Data[Key] != null)
                    return (T)Data[Key];
            }

            return Default;
        }
        /// <summary>
        /// Passes values from Values to Row.        
        /// </summary>
        static public void ValuesToRow(this IDictionary Values, DataRow Row)
        {
            string FieldName;
            DataColumn Column;
            IDictionaryEnumerator enumerator = Values.GetEnumerator();
            enumerator.Reset();
            while (enumerator.MoveNext())
            {
                FieldName = enumerator.Key.ToString();
                if (Row.Table.Columns.Contains(FieldName))
                {
                    Column = Row.Table.Columns[FieldName];
                    Row[Column] = enumerator.Value;
                }

            }
        }

    }
}
