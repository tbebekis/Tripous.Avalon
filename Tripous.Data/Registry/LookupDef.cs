/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a lookup list.
/// <para>Lookup list items may come from</para>
/// <list type="bullet">
/// <item>a SELECT statement, given in the <see cref="SqlText"/> property</item>
/// <item>a SELECT statement constructed using the <see cref="TableName"/></item>
/// <item>a <see cref="DataTable"/> passed to <see cref="LookupSource.LoadForm"/>() method</item>
/// <item>an enum type, given in the <see cref="EnumTypeName"/> property</item>
/// <item>a <see cref="LookupSource"/> derived class, given in the <see cref="ClassName"/> property</item>
/// <item>as a last resort using the <see cref="BaseDef.Name"/> as a <see cref="TableName"/></item>
/// </list>
/// </summary>
public class LookupDef : BaseDef
{
    bool fUseNullItem;
    string fValueField = "Id";
    string fDisplayField = "Name";
    string fSqlText;
    string fTableName;
    string fConnectionName;
    string fEnumTypeName;
    string fClassName;
    string fForm;
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public LookupDef()
    {
    }
    
    /// <summary>
    /// Creates and returns a <see cref="LookupSource"/> associated with this definition.
    /// </summary>
    public LookupSource Create()
    {
        LookupSource Result = TypeStore.CreateInstance<LookupSource>(ClassName);
        Result.Initialize(this);
        return Result;
    }
    
    // ● properties
    /// <summary>
    /// When true then the first item in the list is a null item.
    /// </summary>
    public bool UseNullItem
    {
        get => fUseNullItem;
        set
        {
            if (fUseNullItem != value)
            {
                fUseNullItem = value;
                NotifyPropertyChanged(nameof(UseNullItem));
            }
        }
    }
    /// <summary>
    /// The field used in getting the value.
    /// <para>Used only when <see cref="TableName"/> or <see cref="SqlText"/> is defined or the list is loaded
    /// using <see cref="LookupSource.Select"/> or <see cref="LookupSource.LoadForm"/> a <see cref="DataTable"/></para>
    /// </summary>
    public string ValueField
    {
        get => fValueField;
        set
        {
            if (fValueField != value)
            {
                fValueField = value;
                NotifyPropertyChanged(nameof(ValueField));
            }
        }
    }  
    /// <summary>
    /// The field used in getting the display value.
    /// <para>Used only when <see cref="TableName"/> or <see cref="SqlText"/> is defined or the list is loaded
    /// using <see cref="LookupSource.Select"/> or <see cref="LookupSource.LoadForm"/> a <see cref="DataTable"/></para>
    /// </summary>
    public string DisplayField
    {
        get => fDisplayField;
        set
        {
            if (fDisplayField != value)
            {
                fDisplayField = value;
                NotifyPropertyChanged(nameof(DisplayField));
            }
        }
    }
    /// <summary>
    /// The connection name used in getting an <see cref="SqlStore"/> in order to execute the <see cref="SqlText"/> SELECT statement.
    /// </summary>
    public string ConnectionName  
    {
        get => !string.IsNullOrWhiteSpace(fConnectionName)? fConnectionName: DbConfig.DefaultConnectionName;
        set { if (fConnectionName != value) { fConnectionName = value; NotifyPropertyChanged(nameof(ConnectionName)); } }
    }
    /// <summary>
    /// The SELECT statement
    /// </summary>
    public string SqlText
    {
        get => fSqlText;
        set
        {
            if (fSqlText != value)
            {
                fSqlText = value;
                NotifyPropertyChanged(nameof(SqlText));
            }
        }
    }
    /// <summary>
    /// When not empty results in a SELECT statement like <c>select * from TableName</c>
    /// </summary>
    public string TableName
    {
        get => fTableName;
        set
        {
            if (fTableName != value)
            {
                fTableName = value;
                NotifyPropertyChanged(nameof(TableName));
            }
        }
    }
    /// <summary>
    /// An enum type used in filling the list
    /// </summary>
    public string EnumTypeName
    {
        get => fEnumTypeName;
        set
        {
            if (fEnumTypeName != value)
            {
                fEnumTypeName = value;
                NotifyPropertyChanged(nameof(EnumTypeName));
            }
        }
    }
    /// <summary>
    /// The class name of a <see cref="LookupSource"/> derived class.
    /// </summary>
    public string ClassName
    {
        get => !string.IsNullOrWhiteSpace(fClassName)? fClassName: typeof(LookupSource).FullName;
        set
        {
            if (fClassName != value)
            {
                fClassName = value;
                NotifyPropertyChanged(nameof(ClassName));
            }
        }
    }
    /// <summary>
    /// The name of a form that displays the table.
    /// </summary>
    public string Form
    {
        get => !string.IsNullOrWhiteSpace(fForm)? fForm: Name;
        set { if (fForm != value) { fForm = value; NotifyPropertyChanged(nameof(Form)); } }
    }
}
