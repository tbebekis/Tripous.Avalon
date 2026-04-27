namespace Tripous.Desktop;

/// <summary>
/// The action of a data-form, as a result of a button or mouse item click, or any other cause.
/// </summary>
[Flags]
public enum DataFormAction
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// Home. Displays the drop-down of the home button
    /// </summary>
    Home = 1,
    /// <summary>
    /// Find. Hides the item part. Displays the list part and makes the pnlFind, the lef sidebar, visible.
    /// <para>Can be called from both parts.</para>
    /// </summary>
    Find = 2,

    /// <summary>
    /// List. Hides the item part. Displays the list part.
    /// <para>Can be called from item part only.</para>
    /// </summary>
    List = 4,
    /// <summary>
    /// Insert. Hides the list part. Displays the item part in insert mode, for a new row to be added.
    /// <para>Can be called from both parts.</para>
    /// </summary>
    Insert = 8,
    /// <summary>
    /// Edit. Hides the list part. Displays the item part in edit mode, having selected the current row for editing.
    /// <para>Can be called from list part only.</para>
    /// </summary>
    Edit = 0x10,
    /// <summary>
    /// Delete. Deletes the selected row.
    /// <para>Can be called from both parts.</para>
    /// </summary>
    Delete = 0x20,
    /// <summary>
    /// Save. Saves the changes made in the current row.
    /// <para>Can be called from item part only.</para>
    /// </summary>
    Save = 0x40,

    /// <summary>
    /// Cancel.
    /// <para>In a IsList form: Sets the modal result to Cancel.</para>
    /// <para>In a IsMaster form: When the form is in FormState.List then if the form is modal then it sets the modal result to Cancel. Else, if non-modal, then it closes the form. </para>
    /// <para>In a IsMaster form: When the form is in FormState.Insert | FormState.Edit then it cancels the edit operation.</para>
    /// </summary>
    Cancel = 0x80,
    /// <summary>
    /// OK.
    /// <para>In a IsList form: It saves any changes.  </para>
    /// <para>In any form mode: If it is modal then it sets the modal result to OK, else just closes the form.</para>
    /// </summary>
    Ok = 0x100,
    /// <summary>
    /// Close.
    /// <para>Closes the form.</para>
    /// </summary>
    Close = 0x200,
}