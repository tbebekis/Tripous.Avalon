 #pragma warning disable CS1591

 using System.ComponentModel;
 using System.Dynamic;
 using System.Text.Json.Serialization;

namespace Tripous 
{
    public class DynamicClass: DynamicObject, INotifyPropertyChanged, ICustomTypeDescriptor
    {
        /* private */
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }
        void NotifyToRefreshAllProperties()
        {
            OnPropertyChanged(string.Empty);
        }

        /* construction */
        public DynamicClass()
        {
        }
        public DynamicClass(string JsonText)
        {
            FromJson(JsonText);
        }

        /* overrides */
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return Properties.TryGetValue(binder.Name, out result);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Properties[binder.Name] = value;
            NotifyToRefreshAllProperties();
            return true;
        }

        /* ICustomTypeDescriptor implementation */
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        public string GetClassName()
        {
            return GetType().Name;
        }
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[0]);
        }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            DynamicPropertyDescriptor[] PropList = Properties
                .Select(Entry => new DynamicPropertyDescriptor(this, Entry.Key, Entry.Value?.GetType() ?? typeof(object), attributes))
                .ToArray();
 
            return new PropertyDescriptorCollection(PropList);
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this; // return Properties;
        }
 
        public string ToJson()
        {
            return Json.Serialize(this);
        }
        public void FromJson(string JsonText)
        {
            dynamic Dyn = Json.Deserialize<DynamicClass>(JsonText);
            DynamicClass Instance = Dyn as DynamicClass;
            this.Properties = Instance.Properties;
        }

        public void RemoveAllProperties()
        {
            this.Properties.Clear();
        }

        /* properties */
        [JsonIgnore]
        public object this[string PropName]
        {
            get
            {
                return Properties[PropName];
            }
            set
            {
                object OldValue = null;

                if (Properties.ContainsKey(PropName))
                    OldValue = Properties[PropName];

                Properties[PropName] = value;

                if (OldValue != value)
                    OnPropertyChanged(PropName);
            }
        }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        [JsonIgnore]
        public int PropertyCount { get { return Properties.Keys.Count; } }

        /* events - INotifyPropertyChanged implementation */
        public event PropertyChangedEventHandler PropertyChanged;
    }




    public class DynamicPropertyDescriptor : PropertyDescriptor
    {
        DynamicClass Instance;
        Type PropType;

        /* constructor */
        public DynamicPropertyDescriptor(DynamicClass Instance, string PropName, Type PropType, Attribute[] PropAttributes)
            : base(PropName, PropAttributes)
        {
            this.Instance = Instance;
            this.PropType = PropType;
        }

        /* overrides */
        public override object GetValue(object component)
        {
            return Instance[Name];
        }
        public override void SetValue(object component, object value)
        {
            Instance[Name] = value;
        }
        public override bool CanResetValue(object component)
        {
            return true;
        }
        public override void ResetValue(object component)
        {
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        /* properties */
        public override Type ComponentType  { get { return Instance.GetType(); } }
        public override bool IsReadOnly  { get { return false; } }
        public override Type PropertyType { get { return PropType; } }
    }



}







 
