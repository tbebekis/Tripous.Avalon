namespace Tripous
{
    using System.Reflection;
    
    /// <summary>
    /// Type related extension methods
    /// </summary>
    static public class TypeExtensions
    {

        /// <summary>
        /// Returns true if a type defines a default public constructor
        /// </summary>
        static public bool HasDefaultConstructor(this Type ClassType)
        {
            if (ClassType != null)
            {
                BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public;
                ConstructorInfo Constructor = ClassType.GetConstructor(BindingFlags, null, new Type[0], null);
                return Constructor != null;
            }

            return false;
        }
        /// <summary>
        /// A kind of "virtual constructor" of any type of object. 
        /// Example call for a constructor with no params/arguments
        ///    Create(typeof(object), new Type[]{ },  new object[]{ });
        /// </summary>
        static public object Create(this Type ClassType, Type[] Params, object[] Args)
        {
            if (ClassType != null)
            {
                BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                ConstructorInfo Constructor = ClassType.GetConstructor(BindingFlags, null, Params, null);
                if (Constructor == null)
                    throw new ApplicationException("Constructor not found for class: " + ClassType.Name);
                return Constructor.Invoke(Args);
            }

            return null;
        }
        /// <summary>
        /// Tries to infer Type-s of Args. If an Arg is null, it infers the typeof(object)
        /// </summary>
        static public Type[] GetArgTypes(object[] Args)
        {
            if ((Args == null) || (Args.Length == 0))
                return new Type[0];

            Type[] Result = new Type[Args.Length];

            for (int i = 0; i < Args.Length; i++)
            {
                if (Args[i] == null)
                    Result[i] = typeof(object);
                else
                    Result[i] = Args[i].GetType();
            }

            return Result;
        }
        /// <summary>
        /// A kind of "virtual constructor" of any type of object. 
        /// Example call for a constructor
        ///    Create(typeof(object), new object[]{ });
        /// </summary>
        static public object Create(this Type ClassType, object[] Args)
        {
            return Create(ClassType, GetArgTypes(Args), Args);
        }
        /// <summary>
        /// A kind of "virtual constructor" of any type of object. 
        /// Example call for a constructor with no params/arguments
        ///    Create(typeof(object));
        /// </summary>
        static public object Create(this Type ClassType)
        {
            if (ClassType.HasDefaultConstructor())
                return Create(ClassType, new Type[] { }, new object[] { });
            return null;
        }

        /// <summary>
        /// Returns true if ClassType implements all InterfaceTypes.  
        /// </summary>
        static public bool ImplementsInterfaces(this Type ClassType, Type[] InterfaceTypes)
        {
            if (ClassType != null)
            {
                Type[] typeInterfaces = ClassType.GetInterfaces();

                return Array.TrueForAll(InterfaceTypes, x => Array.IndexOf(typeInterfaces, x) != -1);
            }

            return false;
        }
        /// <summary>
        /// Returns true if ClassType implements InterfaceType.  
        /// </summary>
        static public bool ImplementsInterface(this Type ClassType, Type InterfaceType)
        {
            return (ClassType != null) && (Array.IndexOf(ClassType.GetInterfaces(), InterfaceType) != -1);
        }

        /// <summary>
        /// Returns true if ClassType is a or inherits from Value.
        /// </summary>
        static public bool InheritsFrom(this Type ClassType, Type Value)
        {
            return (ClassType != null) && (Value != null) && ((ClassType == Value || ClassType.IsSubclassOf(Value)));
        }
        /// <summary>
        /// Returns true if Instance is a or inherits from Value.
        /// </summary>
        static public bool InheritsFrom(this object Instance, Type Value)
        {
            return (Instance != null) && InheritsFrom(Instance.GetType(), Value);
        }



        /// <summary>
        /// Returns true if the ClassType has a public instance property with PropertyName
        /// </summary>
        static public bool HasPublicProperty(this Type ClassType, string PropertyName)
        {
            return FindPublicProperty(ClassType, PropertyName) != null;
        }
        /// <summary>
        /// Finds the public instance property of ClassType by PropertyName
        /// </summary>
        static public PropertyInfo FindPublicProperty(this Type ClassType, string PropertyName)
        {
            if (ClassType != null)
            {
                try
                {
                    return ClassType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.Public);
                }
                catch
                {
                }
            }

            return null;
        }
        /// <summary>
        /// Returns the value of the public prorperty PropertyName of Instance. Instance must be of type ClassType.
        /// <para>If Instance is null or Instance has not a PropertyName public property, null is returned.</para>
        /// </summary>
        static public object GetPublicPropertyValue(this Type ClassType, object Instance, string PropertyName)
        {
            if (Instance != null)
            {
                PropertyInfo PropInfo = FindPublicProperty(ClassType, PropertyName);
                if (PropInfo != null)
                    return PropInfo.GetValue(Instance, null);
            }

            return null;
        }
        /// <summary>
        /// Returns the value of the public property PropertyName, if any, else null.
        /// </summary>
        static public object GetPublicPropertyValue(object Instance, string PropertyName)
        {
            if (Instance != null)
            {
                PropertyInfo PropInfo = FindPublicProperty(Instance.GetType(), PropertyName);
                if (PropInfo != null)
                    return PropInfo.GetValue(Instance, null);
            }

            return null;
        }

        /// <summary>
        /// Returns true if the PropInfo is an one-dimensional integer indexer property 
        /// </summary>
        static public bool IsIntegerIndexer(PropertyInfo PropInfo)
        {
            return (string.Compare("Item", PropInfo.Name, true) == 0)
                    && (PropInfo.GetIndexParameters().GetLength(0) == 1)
                    && (PropInfo.GetIndexParameters()[0].ParameterType == typeof(int));
        }

        /// <summary>
        /// Returns the one-dimensional integer indexer property of the ClassType, if it has one, otherwise null.
        /// </summary>
        static public PropertyInfo FindIntegerIndexer(this Type ClassType)
        {
            if (ClassType != null)
            {
                PropertyInfo[] Props = ClassType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (PropertyInfo PropInfo in Props)
                    if (IsIntegerIndexer(PropInfo))
                        return PropInfo;
            }

            return null;
        }
        /// <summary>
        /// Helper for the public GetFields() method.
        /// </summary>
        static private void GetFields(Type CurrentType, Type LastType, IList<FieldInfo> List)
        {
            if ((CurrentType != null) && (LastType != null) && (List != null))
            {
                BindingFlags BF = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                FieldInfo[] Fields = CurrentType.GetFields(BF);

                foreach (FieldInfo Field in Fields)
                    List.Add(Field);

                if ((CurrentType != LastType) && (CurrentType != typeof(System.Object)))
                {
                    GetFields(CurrentType.BaseType, LastType, List);
                }
            }
        }
        /// <summary>
        /// Returns a list of public and non-public fields of the Type, including
        /// inherited fields. It includes fields up to LastType.
        /// </summary>
        static public IList<FieldInfo> GetFields(this Type Type, Type LastType)
        {
            List<FieldInfo> List = new List<FieldInfo>();
            GetFields(Type, LastType, List);
            return List;
        }
        /// <summary>
        /// Finds and returns a public or non-public FieldInfo with FieldName, if any in the Type
        /// or any base type, else null.
        /// </summary>
        static public FieldInfo FindField(this Type Type, string FieldName)
        {
            IList<FieldInfo> Fields = GetFields(Type, typeof(System.Object));

            foreach (FieldInfo Field in Fields)
            {
                if (string.Compare(FieldName, Field.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return Field;
            }

            return null;
        }
        /// <summary>
        /// Returns the value of a public or non-public field with FieldName which is
        /// declared in the type of the Instance or any of its ancestors. Returns null
        /// if the field is not found.
        /// </summary>
        static public object FindField(object Instance, string FieldName)
        {
            if (Instance != null)
            {
                FieldInfo Field = FindField(Instance.GetType(), FieldName);
                if (Field != null)
                    return Field.GetValue(Instance);
            }

            return null;
        }
        /// <summary>
        /// Returns true if T is of a Nullable type
        /// </summary>
        static public bool IsNullable(this Type T)
        {
            return (T != null) && T.IsGenericType && (T.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
        /// <summary>
        /// Returns true if T is decimal or nullable decimal
        /// </summary>
        static public bool IsDecimal(this Type T)
        {
            return (T != null) && (T == typeof(decimal)) || (T == typeof(Nullable<decimal>));
        }
        /// <summary>
        /// Returns true if T is double or nullable double
        /// </summary>
        static public bool IsDouble(this Type T)
        {
            return (T != null) && (T == typeof(double)) || (T == typeof(Nullable<double>));
        }


        static HashSet<Type> IntegerTypeSet = new HashSet<Type>
        {
            typeof(Byte),
            typeof(SByte),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64),
            typeof(UInt16),
            typeof(UInt32),
            typeof(UInt64)
        };


        /// <summary>
        /// Returns true if a specified type is an integer type, i.e. Byte, SByte, Int16, Int32, Int64, UInt16, UInt32, UInt64
        /// </summary>
        static public bool IsIntegerType(this Type T)
        {
            return IntegerTypeSet.Contains(T) || IntegerTypeSet.Contains(Nullable.GetUnderlyingType(T));
        }
        
        static public bool IsNumericType(this Type DataType)
        {
            return
                DataType == typeof(byte) ||
                DataType == typeof(sbyte) ||
                DataType == typeof(short) ||
                DataType == typeof(ushort) ||
                DataType == typeof(int) ||
                DataType == typeof(uint) ||
                DataType == typeof(long) ||
                DataType == typeof(ulong) ||
                DataType == typeof(float) ||
                DataType == typeof(double) ||
                DataType == typeof(decimal);
        }

        static public bool IsDateType(this Type DataType)
        {
            return DataType == typeof(DateTime);
        }
    }
}
