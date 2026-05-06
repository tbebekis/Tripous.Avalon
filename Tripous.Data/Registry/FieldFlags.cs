namespace Tripous.Data;

/// <summary>
/// A list of possible field flags.
/// </summary>
[Flags]
public enum FieldFlags
{
    /// <summary>
    /// Container of the flags is set
    /// </summary>
    None = 0,
    /// <summary>
    /// Must be visible
    /// </summary>
    Visible = 1,
    /// <summary>
    /// Determines whether the field can be modified
    /// </summary>
    ReadOnly = 2,
    /// <summary>
    /// Concerns controls that display the field    
    /// </summary>
    ReadOnlyUI = 4,
    /// <summary>
    /// The field is editable when inserting only
    /// </summary>
    ReadOnlyEdit = 8,
    /// <summary>
    /// Can not be null
    /// </summary>
    Required = 0x10,
    /// <summary>
    /// It is an integer field that must be displayed in a check box control. 0 = false, 1 = true.
    /// </summary>
    Boolean = 0x20,
    /// <summary>
    /// Field is a multiline text
    /// </summary>
    Memo = 0x40,
    /// <summary>
    /// Field is an image, i.e. png, jpg, etc.
    /// </summary>
    Image = 0x80,
    /// <summary>
    /// Field is a path to an image.
    /// </summary>
    ImagePath = 0x100,
    /// <summary>
    /// The field is not used with INSERT or UPDATE statements. 
    /// <para>It maybe something like the ExtraField or an identity/autoinc field,
    /// in a position other than that of a primary key</para>
    /// </summary>
    NoInsertUpdate  = 0x200,
    /// <summary>
    /// A foreign key field
    /// </summary>
    ForeignKey = 0x400,
    /// <summary>
    /// The field does NOT exist in the database. It just added to the DataTable schema for some reason.
    /// </summary>
    Extra = 0x800,
    /// <summary>
    /// Field is searchable and can be a part of a filter.
    /// </summary>
    Searchable = 0x1000,
}