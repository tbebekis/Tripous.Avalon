// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides host services for a group grid drop-down in-place editor.
/// </summary>
public interface IGroupGridDropDownEditorHost
{
    // ● methods
    /// <summary>
    /// Closes the active drop-down without committing a value.
    /// </summary>
    void CloseDropDown();
    /// <summary>
    /// Commits a value selected by the active drop-down editor.
    /// </summary>
    /// <param name="Value">The selected value.</param>
    /// <returns>True if the value was committed; otherwise, false.</returns>
    bool CommitDropDownValue(object Value);
    /// <summary>
    /// Cancels the active drop-down without committing a value.
    /// </summary>
    /// <returns>True if the drop-down cancel request was handled; otherwise, false.</returns>
    bool CancelDropDown();
    /// <summary>
    /// Restores focus to the active in-place editor.
    /// </summary>
    void RestoreEditorFocus();
}
