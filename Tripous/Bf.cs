namespace Tripous
{
    using System.Reflection;
    
    /// <summary>
    /// Bf stands for Bitfield and is a helper class for sets, that
    /// is enum types marked with the FlagsAttribute attribute.
    /// <para>Here is the possible operations</para>
    /// <para><c>C = A | B                           // Union</c> </para>
    /// <para><c>C = A &amp; B                       // Intersection</c> </para>
    /// <para><c>C = A ^ B                           // Difference</c> </para>
    /// <para><c>C = A ^ (A &amp; B)                 // Subtraction</c> </para>
    /// <para><c>bool Result = (A &amp; B) == A      // Membership</c> </para>
    /// </summary>
    static public class Bf
    {

        static private string EnumTypesToJsonResult = string.Empty;

        static private void Check(object A, object B)
        {
            if ((A == null) || (B == null))
                throw (new ArgumentNullException());

            if (A.GetType() != B.GetType())
                throw (new ApplicationException("Different set types"));
        }

        /// <summary>
        /// Union (or). Returns the union of A and B. 
        /// <para>The result is a new set containing ALL the elements of A and B.</para>
        /// <para><c>Result = A | B</c></para>
        /// </summary>
        public static object Union(object A, object B)
        {
            Check(A, B);
            return (int)A | (int)B;
        }
        /// <summary>
        /// Intersection (and). Returns the intersection of A and B. 
        /// <para>The result is a new set containing just the COMMON elements of A and B.</para>
        /// <para><c>Result = A &amp; B</c></para>
        /// </summary>
        public static object Junction(object A, object B)
        {
            Check(A, B);
            return (int)A & (int)B;
        }
        /// <summary>
        /// Difference (xor). Returns the difference of A and B. 
        /// <para>The result is a new set containing the NON COMMON the elements of A and B.</para>
        /// <para><c>Result = A ^ B</c></para>
        /// </summary>
        public static object Dif(object A, object B)
        {
            Check(A, B);
            return (int)A ^ (int)B;
        }
        /// <summary>
        /// Subtraction (-). Returns the subtraction of B from A. 
        /// <para>The result is a new set containing the the elements of A MINUS the elements of B.</para>
        /// <para><c>Result = A ^ (A &amp; B)</c></para>
        /// </summary>
        public static object Subtract(object A, object B)
        {
            Check(A, B);
            return (int)A ^ ((int)A & (int)B);
        }
        /// <summary>
        /// Membership (in). Returns true if A in B. A can be a single value or a set. 
        /// <para>Returns true if ALL elements of A are in B.</para>
        /// <para><c>Result = (A &amp; B) == A</c></para>
        /// </summary>
        public static bool Member(object A, object B)
        {
            Check(A, B);

            if (0 == (int)A)
                return false;

            return (((int)A & (int)B) == (int)A);
        }
        /// <summary>
        /// Membership (in). Returns true if A in B. A can be a single value or a set. 
        /// <para>Returns true if ALL elements of A are in B.</para>
        /// <para><c>Result = (A &amp; B) == A</c></para>
        /// </summary>
        public static bool In(object A, object B)
        {
            return Member(A, B);
        }
        /// <summary>
        /// Returns true if A and B have at least one element in common.
        /// </summary>
        public static bool AnyIn(object A, object B)
        {
            object J = Junction(A, B);
            return !IsEmpty(J);
        }
        /// <summary>
        /// Returns a bit mask with all values in the EnumType
        /// </summary>
        public static int All(Type EnumType)
        {
            int Result = 0;
            foreach (int i in EnumGetValues(EnumType))
            {
                Result |= i;
            }
            return Result;
        }
        /// <summary>
        /// Returns true if A is null or 0.
        /// </summary>
        public static bool IsEmpty(object A)
        {
            return (A == null) || ((int)A == 0);
        }
 
        // ● Compact Framework compatibility  
        /// <summary>
        /// Gets an array of T, where T is the enum type, which contains all the values of the EnumType
        /// starting from the FirstIndex enum value
        /// </summary>
        static public Array EnumGetValues(Type EnumType, int FirstIndex = 0)
        {
            if (EnumType.BaseType != typeof(System.Enum))
                throw new ArgumentException("EnumType is not a System.Enum");

            FieldInfo[] FieldInfo = EnumType.GetFields(BindingFlags.Static | BindingFlags.Public);
 
            List<object> List = new List<object>();
            for (int i = 0; i < FieldInfo.Length; i++)
            {
                if (i >= FirstIndex)
                    List.Add(FieldInfo[i].GetValue(null));
            }

            Array Result = List.ToArray();
            //Array Result = Array.CreateInstance(EnumType, FieldInfo.Length);
            //for (int i = FirstIndex; i < FieldInfo.Length; i++)
            //    Result.SetValue(FieldInfo[i].GetValue(null), i);

            return Result;

        }
        /// <summary>
        /// Gets a string array containing the string literals of the values of the EnumType
        /// starting from the FirstIndex enum value
        /// </summary>
        static public string[] EnumGetNames(Type EnumType, int FirstIndex = 0)
        {
            Array Values = EnumGetValues(EnumType, FirstIndex);

            if (Values.Length > 0)
            {
                string[] Result = new string[Values.Length];
                for (int i = 0; i < Values.Length; i++)
                    Result[i] = Values.GetValue(i).ToString();

                return Result;
            }

            return new string[0];

        }
        /// <summary>
        /// Gets the values of an enum type as a List starting from the FirstIndex enum value
        /// <para><c> List = Sys.EnumGetValueList&lt;CriterionType&gt;() </c></para>
        /// </summary>
        static public List<T> EnumGetValueList<T>(int FirstIndex = 0)
        {
            Array A = Bf.EnumGetValues(typeof(T), FirstIndex);
            List<T> Result = new List<T>();

            for (int i = 0; i < A.Length; i++)
                Result.Add((T)A.GetValue(i));

            return Result;
        }
 
        /// <summary>
        /// Converts an Enum Type to Json string
        /// </summary>
        public static string EnumToJson(Type EnumType, bool OmitEnumTypeName = false)
        {
            if (!EnumType.IsEnum)
                throw new InvalidOperationException("enum expected");

            Array A;
            A = Enum.GetValues(EnumType);

            var Values = A.Cast<object>()
                        .ToDictionary(enumValue => enumValue.ToString(), enumValue => (int)enumValue);

            if (OmitEnumTypeName)
                return string.Format("{{ {0} }}", Json.Serialize(Values));
            else
                return string.Format("{{ \"{0}\" : {1} }}", EnumType.Name, Json.Serialize(Values));

        }
 
    }
}

