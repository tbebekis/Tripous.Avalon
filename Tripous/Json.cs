namespace Tripous;
    
/// <summary>
/// Helper json static class
/// <para>SEE: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-overview </para>
/// </summary>
static public class Json
{

    static JsonSerializerOptions fSerializerOptions;
    static JsonSerializerOptions fDefaultSerializerOptions;

    static void JsonLoaded(object Instance)
    {
        if (Instance is IJsonLoadable)
        {
            IJsonLoadable Loadable = Instance as IJsonLoadable;
            Loadable.JsonLoaded();
        }
    }

    // ● serialization options
    /// <summary>
    /// Creates and returns json serialization options
    /// </summary>
    static public JsonSerializerOptions CreateJsonOptions(
        bool CameCase = false,
        bool Formatted = true,
        bool CaseInsensitiveProperties = true,
        int Decimals = 0,
        string[] ExcludeProperties = null
        )
    {
        if (CreateOptionsFunc != null)
            return CreateOptionsFunc(CameCase, Formatted, CaseInsensitiveProperties, Decimals, ExcludeProperties);

        JsonSerializerOptions Result = new();
        SetupJsonOptions(Result, CameCase, Formatted, CaseInsensitiveProperties, Decimals, ExcludeProperties);
        return Result;
    }
    static public void SetupJsonOptions(
        JsonSerializerOptions JsonOptions,
        bool CameCase = false,
        bool Formatted = true,
        bool CaseInsensitiveProperties = true,
        int Decimals = 0,
        string[] ExcludeProperties = null
        )
    {
        JsonOptions.PropertyNamingPolicy = CameCase ? JsonNamingPolicy.CamelCase : new JsonNamingPolicyAsIs();
        JsonOptions.DictionaryKeyPolicy = JsonOptions.PropertyNamingPolicy;
        JsonOptions.PropertyNameCaseInsensitive = CaseInsensitiveProperties;
        JsonOptions.WriteIndented = Formatted;
        JsonOptions.IgnoreReadOnlyProperties = false;
        JsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // JsonIgnoreCondition.Always;
        JsonOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        JsonOptions.AllowTrailingCommas = true;
        JsonOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        // if it is ReferenceHandler.Preserve then it adds "$id" and "$values" entries to the resulting json
        JsonOptions.ReferenceHandler = null; // ReferenceHandler.Preserve; // or ReferenceHandler.IgnoreCycles

        JsonOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            
        if (Decimals > 0)
        {
            JsonOptions.Converters.Insert(0, new DecimalConverter(Decimals));
            JsonOptions.Converters.Insert(0, new DoubleConverter(Decimals));
        }

        JsonOptions.Converters.Insert(0, new JsonStringEnumConverter(JsonOptions.PropertyNamingPolicy));

        if (ExcludeProperties != null)
            JsonOptions.TypeInfoResolver = new ExcludePropertiesTypeInfoResolver(ExcludeProperties);
    }

    // ● serialize
    /// <summary>
    /// Serializes a specified instance.
    /// </summary>
    static public string Serialize(object Instance, JsonSerializerOptions JsonOptions)
    {
        JsonOptions = JsonOptions ?? SerializerOptions;
        string JsonText = JsonSerializer.Serialize(Instance, JsonOptions);
        return JsonText;
    }
    /// <summary>
    /// Serializes a specified instance.
    /// </summary>
    static public string Serialize(object Instance, bool Formatted, bool CameCase = false)
    {
        JsonSerializerOptions Options = CreateJsonOptions(CameCase: CameCase, Formatted: Formatted, Decimals: DefaultDecimals);
        return Serialize(Instance, Options);
    }
    /// <summary>
    /// Serializes a specified instance.
    /// </summary>
    static public string Serialize(object Instance)
    {
        return Serialize(Instance, SerializerOptions);
    }
    /// <summary>
    /// Serializes a specified instance.
    /// </summary>
    static public string Serialize(object Instance, string[] ExcludeProperties)
    {
        JsonSerializerOptions Options = CreateJsonOptions(ExcludeProperties: ExcludeProperties, Decimals: DefaultDecimals);
        return Serialize(Instance, Options);
    }

    // ● deserialize
    /// <summary>
    /// Deserializes (creates) an object of a specified type by deserializing a specified json text.
    /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
    /// </summary>
    static public object Deserialize(string JsonText, Type ReturnType, JsonSerializerOptions JsonOptions = null)
    {
        JsonOptions = JsonOptions ?? SerializerOptions;
        object Result = JsonSerializer.Deserialize(JsonText, ReturnType, JsonOptions);
        JsonLoaded(Result);
        return Result;
    }
    /// <summary>
    /// Deserializes (creates) an object of a specified type by deserializing a specified json text.
    /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
    /// </summary>
    static public T Deserialize<T>(string JsonText, JsonSerializerOptions JsonOptions = null)
    {
        JsonOptions = JsonOptions ?? SerializerOptions;
        T Result = JsonSerializer.Deserialize<T>(JsonText, JsonOptions);
        JsonLoaded(Result);
        return Result;
    }
    /// <summary>
    /// Loads an object's properties from a specified json text.
    /// <para>If no options specified then it uses the <see cref="SerializerOptions"/> options</para> 
    /// </summary>
    static public void PopulateObject(object Instance, string JsonText, JsonSerializerOptions JsonOptions = null)
    {
        if (Instance != null && !string.IsNullOrWhiteSpace(JsonText))
        {
            JsonOptions ??= SerializerOptions;  
            JsonPopulator.PopulateObject(Instance, JsonText, JsonOptions);
            JsonLoaded(Instance);
        }
    }

    // ● miscs
    /// <summary>
    /// Returns a specified json text as formatted for readability.
    /// </summary>
    static public string Format(string JsonText)
    {
        if (!string.IsNullOrWhiteSpace(JsonText))
        {
            var JsonOptions = CreateJsonOptions(CameCase: false, Formatted: true, Decimals: DefaultDecimals);
#nullable enable
            JsonNode? Node = JsonSerializer.Deserialize<JsonNode>(JsonText);
#nullable disable
            JsonText = JsonSerializer.Serialize(Node, JsonOptions);
        }

        return JsonText;
    }
    /// <summary>
    /// Converts an object to JsonNode
    /// </summary>
    static public JsonNode ObjectToJsonNode(object Instance)
    {
        string JsonText = Serialize(Instance);
#nullable enable
        JsonNode? Node = JsonNode.Parse(JsonText);
#nullable disable

        return Node;
    }
    /// <summary>
    /// Converts a json text to a Dictionary instance.
    /// </summary>
    static public Dictionary<string, string> ToDictionary(string JsonText)
    {
        return Deserialize<Dictionary<string, string>>(JsonText);
    }
    /// <summary>
    /// Converts json text to a dynamic object which actually is a <see cref="JsonObject"/>
    /// </summary>
    static public dynamic ToDynamic(string JsonText)
    {
#nullable enable
        JsonNode? Node = JsonSerializer.Deserialize<JsonNode>(JsonText);
#nullable disable
        return Node as dynamic;
    }

    // ● to-from file
    /// <summary>
    /// Saves an instance as json text in a specified file.
    /// </summary>
    static public void SaveToFile(object Instance, string FilePath, string Encoding = "utf-8")
    {
        string Folder = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(Folder))
            Directory.CreateDirectory(Folder);
        string JsonText = Serialize(Instance);
        File.WriteAllText(FilePath, JsonText, System.Text.Encoding.GetEncoding(Encoding));
    }

    /// <summary>
    /// Loads the properties of an instance by reading the json text of a specified file.
    /// </summary>
    static public void LoadFromFile(object Instance, string FilePath, string Encoding = "utf-8")
    {
        if (File.Exists(FilePath))
        {
            string JsonText = File.ReadAllText(FilePath, System.Text.Encoding.GetEncoding(Encoding));
            PopulateObject(Instance, JsonText);
        }
    }
    /// <summary>
    ///  Creates and returns an object of ClassType using the json text of a specified file
    /// </summary>
    static public object LoadFromFile(Type ClassType, string FilePath, string Encoding = "utf-8")
    {
        if (File.Exists(FilePath))
        {
            string JsonText = File.ReadAllText(FilePath, System.Text.Encoding.GetEncoding(Encoding));
            return Deserialize(JsonText, ClassType);
        }

        return null;
    }

    // ● streams
    /// <summary>
    /// Converts Instance to a json string using the NewtonSoft json serializer and then to stream.
    /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
    /// <para>NOTE: UTF8 encoding is used.</para>
    /// </summary>
    static public MemoryStream SerializeToStream(object Instance, JsonSerializerOptions JsonOptions = null)
    {
        MemoryStream MS = new MemoryStream();
        SerializeToStream(Instance, MS, JsonOptions);
        return MS;
    }
    /// <summary>
    /// Converts Instance to a json string using the NewtonSoft json serializer and then to stream.
    /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
    /// <para>NOTE: UTF8 encoding is used.</para>
    /// </summary>
    static public void SerializeToStream(object Instance, Stream Stream, JsonSerializerOptions JsonOptions = null)
    {
        string JsonText = Serialize(Instance, JsonOptions);
        JsonTextToStream(JsonText, Stream);
    }
    /// <summary>
    /// Converts a specified json text to a stream.
    /// <para>NOTE: UTF8 encoding is used.</para>
    /// </summary>
    static public void JsonTextToStream(string JsonText, Stream Stream)
    {
        byte[] Buffer = Encoding.UTF8.GetBytes(JsonText);
        Stream.Write(Buffer, 0, Buffer.Length);
    }

    /// <summary>
    /// Reads the json text from a stream and then deserializes (creates) an object of a specified type.
    /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
    /// <para>NOTE: UTF8 encoding is used.</para>
    /// </summary>
    static public object DeserializeFromStream(Type ClassType, Stream Stream, JsonSerializerOptions JsonOptions = null)
    {
        string JsonText = StreamToJsonText(Stream);
        return Deserialize(JsonText, ClassType, JsonOptions);
    }
    /// <summary>
    /// Loads an object's properties from a specified stream, after reading the json text from the stream.
    /// <para>If no settings specified then it uses the default JsonSerializerSettings</para> 
    /// <para>NOTE: UTF8 encoding is used.</para>
    /// </summary>
    static public void DeserializeFromStream(object Instance, Stream Stream, JsonSerializerOptions JsonOptions = null)
    {
        string JsonText = StreamToJsonText(Stream);
        PopulateObject(Instance, JsonText, JsonOptions);
    }
    /// <summary>
    /// Reads a stream as json text.
    /// <para>NOTE: UTF8 encoding is used.</para>
    /// </summary>
    static public string StreamToJsonText(Stream Stream)
    {
        string JsonText = string.Empty;
        if (Stream != null && Stream.Length > 0)
        {
            using (StreamReader reader = new StreamReader(Stream, Encoding.UTF8))
            {
                JsonText = reader.ReadToEnd();
            }
        }

        return JsonText;
    }


    /// <summary>
    /// Returns the text of the input stream of a request (HttpContext.Request.Body) as a Dictionary. To be used when POST-ing json data.
    /// </summary>
    static public Dictionary<string, dynamic> GetRequestDic(Stream RequestBodyStream)
    {
        if (RequestBodyStream != null && RequestBodyStream.CanSeek)
        {
            string Text = StreamToJsonText(RequestBodyStream);
            if (!string.IsNullOrWhiteSpace(Text))
            {
                return Deserialize<Dictionary<string, dynamic>>(Text);
            }
        }

        return new Dictionary<string, dynamic>();
    }
    
    /// <summary>
    /// Clones an object.
    /// <para>The specified instance should be marked with the SerializableAttribute and provide a default constructor.</para>
    /// </summary>
    static public T CloneObject<T>(T Source) where T: new()
    { 
        if (ReferenceEquals(Source, null))
            return default(T);

        string JsonText = Json.Serialize(Source);
        T Dest = Json.Deserialize<T>(JsonText);
        return Dest; 
    }
    /// <summary>
    /// Assigns a source object properties to a dest object
    /// </summary>
    static public void AssignObject<T, T2>(T Source, T2 Dest)
    {
        string JsonText = Json.Serialize(Source);
        Json.PopulateObject(Dest, JsonText);
    }

    // ● properties
    /// <summary>
    /// Returns the default serialization options
    /// </summary>
    static public JsonSerializerOptions DefaultSerializerOptions
    {
        get
        {
            if (fDefaultSerializerOptions == null)
                fDefaultSerializerOptions = CreateJsonOptions(CameCase: false, Formatted: true, CaseInsensitiveProperties: true, Decimals: DefaultDecimals);

            return fDefaultSerializerOptions;
        }
    }
    /// <summary>
    /// Used when no options or null options are specified.
    /// </summary>
    static public JsonSerializerOptions SerializerOptions
    {
        get => fSerializerOptions != null ? fSerializerOptions : DefaultSerializerOptions;
        set => fSerializerOptions = value;
    }
    /// <summary>
    /// This property is used whenever this class is about to create <see cref="JsonSerializerOptions"/>.
    /// <para>If this property is null then the <see cref="CreateJsonOptions"/>() method is used. </para>
    /// <para>The following is the signature the callback should have.</para>
    /// <code>JsonSerializerOptions CreateOptionsFunc(bool CameCase, bool Formatted, bool CaseInsensitiveProperties = true, int Decimals, string[] ExcludeProperties)</code>
    /// </summary>
    static public Func<bool, bool, bool, int, string[], JsonSerializerOptions> CreateOptionsFunc { get; set; }

    /// <summary>
    /// Controls the decimal places when serializing decimal and double values.
    /// </summary>
    static public int DefaultDecimals { get; set; } = 2;
}

public interface IJsonLoadable
{
    void JsonLoaded();
}

/// <summary>
/// Internal black-box JSON populator that mirrors JSON into an existing instance:
/// - Clears/zeros EVERYTHING inside the instance first (lists/dicts/arrays/objects/scalars),
///   keeping only the top-level reference.
/// - Then fills from JSON so the final state reflects exactly the provided JSON.
/// Honors per-property STJ attributes: [JsonIgnore(Always)], [JsonPropertyName], [JsonNumberHandling], [JsonConverter].
/// </summary>
internal static class JsonPopulator
{
    /// <summary>Populates an existing instance from JSON text with "mirror" semantics.</summary>
    internal static void PopulateObject(object instance, string jsonText, JsonSerializerOptions options = null)
    {
        if (instance == null || string.IsNullOrWhiteSpace(jsonText))
            return;

        options ??= new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var bytes = Encoding.UTF8.GetBytes(jsonText);
        var reader = new Utf8JsonReader(bytes);
        using var doc = JsonDocument.ParseValue(ref reader);
        PopulateFromElement(instance, doc.RootElement, options);
    }

    // ---------- Core dispatch (root) ----------

    private static void PopulateFromElement(object instance, JsonElement element, JsonSerializerOptions options)
    {
        if (instance is null) return;

        var t = instance.GetType();

        if (typeof(IDictionary).IsAssignableFrom(t))
        {
            // Clear then fill
            if (instance is IDictionary d) d.Clear();
            PopulateDictionary((IDictionary)instance, element, options);
            return;
        }

        if (IsListLike(t))
        {
            // Lists: clear; Arrays: zero all
            ClearListOrArray(instance);
            PopulateListLike(instance, element, options);
            return;
        }

        if (element.ValueKind == JsonValueKind.Object)
        {
            // Full object: clear everything inside first, then fill
            ClearObjectShallow(instance);
            PopulateObjectProperties(instance, element, options);
        }
    }

    // ---------- Object properties: clear-all then fill ----------

    private static void PopulateObjectProperties(object instance, JsonElement obj, JsonSerializerOptions options)
    {
        var type = instance.GetType();

        // Case-insensitive map of JSON fields
        var jsonMap = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var p in obj.EnumerateObject())
            jsonMap[p.Name] = p.Value;

        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.GetIndexParameters().Length == 0 && p.CanRead);

        // 1) CLEAR EVERYTHING inside the instance (except [JsonIgnore(Always)] and no-setter cases we can't null)
        foreach (var p in props)
            ClearProperty(instance, p);

        // 2) FILL from JSON (mirror)
        foreach (var p in props)
        {
            // Skip ignored-always
            var ignoreAttr = p.GetCustomAttribute<JsonIgnoreAttribute>();
            if (ignoreAttr != null && ignoreAttr.Condition == JsonIgnoreCondition.Always)
                continue;

            var jpNameAttr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
            var effectiveName = jpNameAttr?.Name ?? p.Name;

            if (!jsonMap.TryGetValue(effectiveName, out var jsonValue))
                continue; // property remains in cleared/default state

            var pType = p.PropertyType;

            // If JSON explicitly null => set null/default (or keep cleared state)
            if (jsonValue.ValueKind == JsonValueKind.Null)
            {
                if (p.CanWrite)
                {
                    if (IsNonNullableValueType(pType))
                        p.SetValue(instance, Activator.CreateInstance(pType));
                    else
                        p.SetValue(instance, null);
                }
                else
                {
                    // Read-only member:
                    // - List/Dict: already cleared
                    // - Array: already zeroed
                    // - Complex: already nulled if had setter; otherwise cleared shallowly
                }
                continue;
            }

            // Simple (value-like) types
            if (IsSimple(pType))
            {
                var val = DeserializeElementForProperty(jsonValue, pType, p, options);
                if (p.CanWrite)
                {
                    p.SetValue(instance, val);
                }
                // else: cannot set a read-only scalar -> leave cleared/default
                continue;
            }

            // Arrays
            if (pType.IsArray)
            {
                if (jsonValue.ValueKind != JsonValueKind.Array) continue;

                if (p.CanWrite)
                {
                    var newArr = DeserializeElementForProperty(jsonValue, pType, p, options);
                    p.SetValue(instance, newArr);
                }
                else
                {
                    // read-only array property: fill in place (already zeroed)
                    var current = p.GetValue(instance);
                    if (current != null)
                        PopulateListLike(current, jsonValue, DerivePerPropertyOptions(p, options));
                }
                continue;
            }

            // Lists
            if (IsListLike(pType))
            {
                if (jsonValue.ValueKind != JsonValueKind.Array) continue;

                if (p.CanWrite)
                {
                    var newList = DeserializeElementForProperty(jsonValue, pType, p, options);
                    p.SetValue(instance, newList);
                }
                else
                {
                    var current = p.GetValue(instance);
                    if (current != null)
                        PopulateListLike(current, jsonValue, DerivePerPropertyOptions(p, options));
                }
                continue;
            }

            // Dictionaries
            if (typeof(IDictionary).IsAssignableFrom(pType))
            {
                if (jsonValue.ValueKind != JsonValueKind.Object) continue;

                if (p.CanWrite)
                {
                    var newDict = DeserializeElementForProperty(jsonValue, pType, p, options);
                    p.SetValue(instance, newDict);
                }
                else
                {
                    var current = p.GetValue(instance) as IDictionary;
                    if (current != null)
                    {
                        current.Clear();
                        PopulateDictionary(current, jsonValue, DerivePerPropertyOptions(p, options));
                    }
                }
                continue;
            }

            // Complex object
            if (p.CanWrite)
            {
                var newObj = DeserializeElementForProperty(jsonValue, pType, p, options);
                p.SetValue(instance, newObj);
            }
            else
            {
                var currentObj = p.GetValue(instance);
                if (currentObj != null)
                {
                    ClearObjectShallow(currentObj);
                    PopulateFromElement(currentObj, jsonValue, DerivePerPropertyOptions(p, options));
                }
            }
        }
    }

    // ---------- CLEAR helpers ----------

    /// <summary>Clears everything inside an object shallowly (sets to null/default, clears lists/dicts, zeros arrays).</summary>
    private static void ClearObjectShallow(object obj)
    {
        if (obj == null) return;

        var type = obj.GetType();
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.GetIndexParameters().Length == 0 && p.CanRead);

        foreach (var p in props)
            ClearProperty(obj, p);
    }

    /// <summary>Clears a single property value based on its shape.</summary>
    private static void ClearProperty(object owner, PropertyInfo p)
    {
        // Skip ignored-always
        var ignoreAttr = p.GetCustomAttribute<JsonIgnoreAttribute>();
        if (ignoreAttr != null && ignoreAttr.Condition == JsonIgnoreCondition.Always)
            return;

        var pType = p.PropertyType;
        var current = SafeGetValue(owner, p);

        // Arrays
        if (pType.IsArray)
        {
            if (p.CanWrite)
            {
                // Null it out; will be set from JSON if present
                p.SetValue(owner, null);
            }
            else if (current is IList arrList)
            {
                var elemType = GetListElementType(pType) ?? typeof(object);
                for (int i = 0; i < arrList.Count; i++)
                    arrList[i] = DefaultFor(elemType);
            }
            return;
        }

        // Lists
        if (IsListLike(pType))
        {
            if (current is IList list)
                list.Clear();

            if (p.CanWrite)
                p.SetValue(owner, null); // leave null until JSON fills (mirror)
            return;
        }

        // Dictionaries
        if (typeof(IDictionary).IsAssignableFrom(pType))
        {
            if (current is IDictionary dict)
                dict.Clear();

            if (p.CanWrite)
                p.SetValue(owner, null);
            return;
        }

        // Complex object
        if (!IsSimple(pType))
        {
            if (p.CanWrite)
            {
                p.SetValue(owner, null);
            }
            else if (current != null)
            {
                // Can't replace -> clear its inside
                ClearObjectShallow(current);
            }
            return;
        }

        // Simple (value-like)
        if (p.CanWrite)
        {
            p.SetValue(owner, pType.IsValueType ? Activator.CreateInstance(pType) : null);
        }
    }

    /// <summary>Clears a root list/array instance.</summary>
    private static void ClearListOrArray(object listInstance)
    {
        if (listInstance is IList ilist)
        {
            var t = listInstance.GetType();
            if (t.IsArray)
            {
                var elemType = GetListElementType(t) ?? typeof(object);
                for (int i = 0; i < ilist.Count; i++)
                    ilist[i] = DefaultFor(elemType);
            }
            else
            {
                ilist.Clear();
            }
        }
    }

    // ---------- FILL helpers for collections ----------

    /// <summary>Fills a list/array from a JSON array. Lists are assumed cleared; arrays are assumed zeroed.</summary>
    private static void PopulateListLike(object listInstance, JsonElement jsonArray, JsonSerializerOptions options)
    {
        if (jsonArray.ValueKind != JsonValueKind.Array) return;
        var listType = listInstance.GetType();
        var isArray = listType.IsArray;

        if (listInstance is not IList ilist) return;

        var elementType = GetListElementType(listType) ?? typeof(object);

        if (!isArray)
        {
            foreach (var itemEl in jsonArray.EnumerateArray())
            {
                object newItem = itemEl.ValueKind == JsonValueKind.Null
                    ? (IsNonNullableValueType(elementType) ? DefaultFor(elementType) : null)
                    : DeserializeElement(itemEl, elementType, options);

                ilist.Add(newItem);
            }
            return;
        }

        // Array: fill up to array length
        int idx = 0;
        foreach (var itemEl in jsonArray.EnumerateArray())
        {
            if (idx >= ilist.Count) break;

            ilist[idx] = itemEl.ValueKind == JsonValueKind.Null
                ? (IsNonNullableValueType(elementType) ? DefaultFor(elementType) : null)
                : (IsSimple(elementType)
                    ? DeserializeElement(itemEl, elementType, options)
                    : DeserializeElement(itemEl, elementType, options));

            idx++;
        }
    }

    private static void PopulateDictionary(IDictionary dict, JsonElement jsonObj, JsonSerializerOptions options)
    {
        if (jsonObj.ValueKind != JsonValueKind.Object) return;

        dict.Clear(); // mirror

        var dictType = dict.GetType();
        var valueType = GetDictionaryValueType(dictType) ?? typeof(object);

        foreach (var prop in jsonObj.EnumerateObject())
        {
            var key = prop.Name;

            if (prop.Value.ValueKind == JsonValueKind.Null)
            {
                dict[key] = null;
                continue;
            }

            dict[key] = DeserializeElement(prop.Value, valueType, options);
        }
    }

    // ---------- Attribute-aware deserialize ----------

    private static object DeserializeElementForProperty(JsonElement el, Type targetType, PropertyInfo p, JsonSerializerOptions baseOptions)
    {
        var opts = DerivePerPropertyOptions(p, baseOptions);
        try
        {
            return JsonSerializer.Deserialize(el.GetRawText(), targetType, opts);
        }
        catch
        {
            // If null into non-nullable value type → default(T)
            if (el.ValueKind == JsonValueKind.Null && IsNonNullableValueType(targetType))
                return Activator.CreateInstance(targetType);
            throw;
        }
    }

    private static JsonSerializerOptions DerivePerPropertyOptions(PropertyInfo p, JsonSerializerOptions baseOptions)
    {
        var opts = new JsonSerializerOptions(baseOptions);

        var nhAttr = p.GetCustomAttribute<JsonNumberHandlingAttribute>();
        if (nhAttr != null)
            opts.NumberHandling = nhAttr.Handling;

        var convAttr = p.GetCustomAttribute<JsonConverterAttribute>();
        if (convAttr != null)
        {
            var converter = convAttr.CreateConverter(p.PropertyType);
            if (converter != null)
                opts.Converters.Insert(0, converter);
        }

        return opts;
    }

    private static object DeserializeElement(JsonElement el, Type targetType, JsonSerializerOptions options)
        => JsonSerializer.Deserialize(el.GetRawText(), targetType, options);

    // ---------- Type-shape helpers ----------

    private static bool IsSimple(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;
        if (t.IsPrimitive || t.IsEnum) return true;
        return t == typeof(string)
            || t == typeof(decimal)
            || t == typeof(DateTime)
            || t == typeof(DateTimeOffset)
            || t == typeof(TimeSpan)
            || t == typeof(Guid)
            || t == typeof(Uri);
    }

    private static bool IsNonNullableValueType(Type t)
        => t.IsValueType && Nullable.GetUnderlyingType(t) == null;

    private static bool IsListLike(Type t)
    {
        if (t.IsArray) return true;
        if (typeof(IList).IsAssignableFrom(t)) return true;
        return t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
    }

    private static Type GetListElementType(Type t)
    {
        if (t.IsArray) return t.GetElementType();
        var ilistGeneric = t.GetInterfaces()
                            .Concat(new[] { t })
                            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
        return ilistGeneric?.GetGenericArguments()[0];
    }

    private static Type GetDictionaryValueType(Type t)
    {
        var idictGeneric = t.GetInterfaces()
                            .Concat(new[] { t })
                            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        return idictGeneric?.GetGenericArguments()[1];
    }

    private static object SafeGetValue(object owner, PropertyInfo p)
    {
        try { return p.GetValue(owner); } catch { return null; }
    }

    private static object DefaultFor(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
}


/// <summary> 
/// A <see cref="JsonNamingPolicy"/> that leaves property names as they are.
/// <para>For example: <c>CustomerName</c> is serialized as <c>CustomerName</c> and <c>Customer_Name</c> is serialized as <c>Customer_Name</c></para>
/// </summary>
public class JsonNamingPolicyAsIs : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name;
    }
}


/// <summary>
/// Used in excluding properties when serializing.
/// <para>E.g. JsonConvert.SerializeObject(Instance, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new JsonNetContractResolver(ExcludeProperties) }) </para>
/// </summary>
internal class ExcludePropertiesTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    string[] ExcludeProperties = new string[0];

    static void RemoveAll<T>(IList<T> list, Predicate<T> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i]))
            {
                list.RemoveAt(i--);
            }
        }
    }
    void ModifierFunc(JsonTypeInfo TypeInfo)
    {
        if (TypeInfo.Kind != JsonTypeInfoKind.Object)
            return;

        RemoveAll(TypeInfo.Properties, prop => ExcludeProperties.Contains(prop.Name));
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ExcludePropertiesTypeInfoResolver(string[] ExcludeProperties)
    {
        this.ExcludeProperties = ExcludeProperties;
        this.Modifiers.Insert(0, ModifierFunc);
    }
}


internal class DecimalConverter : JsonConverter<decimal>
{
    int Decimals;
    public DecimalConverter(int Decimals)
    {
        this.Decimals = Decimals >= 1 ? Decimals : 2;
    }

    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDecimal();
    }
    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        string Value = value.ToString($"N{Decimals}", CultureInfo.InvariantCulture);
        writer.WriteRawValue(Value);
    }
}


internal class DoubleConverter : JsonConverter<double>
{
    int Decimals;
    public DoubleConverter(int Decimals)
    {
        this.Decimals = Decimals >= 1 ? Decimals : 2;
    }

    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDouble();
    }
    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        string Value = value.ToString($"N{Decimals}", CultureInfo.InvariantCulture);
        writer.WriteRawValue(Value);
    }
}