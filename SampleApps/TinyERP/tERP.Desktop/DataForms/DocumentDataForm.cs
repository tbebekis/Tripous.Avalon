/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Base form for document data modules.
/// </summary>
public class DocumentDataForm : AppDataForm
{
    // ● protected fields
    /// <summary>
    /// Button that posts the current document.
    /// </summary>
    protected Button btnPost;

    // ● protected
    /// <summary>
    /// Returns the status of the current document.
    /// </summary>
    protected virtual TradeStatus GetDocumentStatus()
    {
        if (CurrentRow == null)
            return TradeStatus.None;
        if (CurrentRow.Table.Columns.Contains("TradeStatusId"))
            return (TradeStatus)CurrentRow.AsInteger("TradeStatusId");
        if (CurrentRow.Table.Columns.Contains("StatusId"))
            return (TradeStatus)CurrentRow.AsInteger("StatusId");
        return TradeStatus.None;
    }
    /// <summary>
    /// Returns true when the current document is cancelled.
    /// </summary>
    protected virtual bool IsDocumentCancelled() => CurrentRow != null
                                                     && CurrentRow.Table.Columns.Contains("IsCancelled")
                                                     && CurrentRow.AsBoolean("IsCancelled");
    /// <summary>
    /// Returns true when the current document is locked.
    /// </summary>
    protected virtual bool IsDocumentLocked()
    {
        if (CurrentRow == null)
            return false;
        if (CurrentRow.Table.Columns.Contains("IsLocked"))
            return CurrentRow.AsBoolean("IsLocked");
        return GetDocumentStatus() != TradeStatus.Draft;
    }
    /// <summary>
    /// Returns true when the current document can be posted immediately.
    /// </summary>
    protected virtual bool CanPost()
    {
        if (!IsEditableForm || FormState != DataFormState.Edit || CurrentRow == null || HasChanges())
            return false;
        if (Module is not DocumentDataModule)
            return false;
        if (GetDocumentStatus() != TradeStatus.Draft)
            return false;
        if (IsDocumentCancelled() || IsDocumentLocked())
            return false;

        return true;
    }
    /// <summary>
    /// Returns true when the current document can attempt posting.
    /// </summary>
    protected virtual bool CanAttemptPost()
    {
        if (!IsEditableForm || FormState != DataFormState.Edit || CurrentRow == null)
            return false;
        if (Module is not DocumentDataModule)
            return false;
        if (GetDocumentStatus() != TradeStatus.Draft)
            return false;
        if (IsDocumentCancelled() || IsDocumentLocked())
            return false;
        return true;
    }
    /// <summary>
    /// Posts the current document.
    /// </summary>
    protected virtual async Task ExecutePost()
    {
        if (!CanAttemptPost())
        {
            EnableCommands();
            return;
        }
        try
        {
            ((DocumentDataModule)Module).CheckCanCommit(false);
        }
        catch (Exception e)
        {
            EnableCommands();
            await MessageBox.Error(e, this);
            return;
        }

        string Code = CurrentRow.AsString("Code");
        string DocumentText = string.IsNullOrWhiteSpace(Code) ? "document" : $"document: {Code}";
        string Message = $@"Post {DocumentText}? 

After posting, the document can no longer be edited.
";
        if (!await MessageBox.YesNo(Message, this))
            return;

        Dictionary<GroupGrid, Tuple<int, GroupGridColumn>> DetailGridSelection = ItemPage?.CaptureDetailGridSelection();
        Saving = true;
        try
        {
            DocumentDataModule DocumentModule = (DocumentDataModule)Module;
            object Id = DocumentModule.Post();
            fListTargetId = Id;
            ItemPage?.Refresh();
            ItemPage?.RestoreDetailGridSelection(DetailGridSelection);
            Broadcaster.Send("Document.Posted", this, DocumentPostedInfo.FromModule(ModuleDef.Name, DocumentModule).ToDictionary());
            UiLog($"Posted {GetItemLogText(Id)}");
            Ui.SuccessNote($"Posted {GetItemLogText(Id)}");
            FormState = DataFormState.Edit;
            UpdateUi();
        }
        catch (Exception e)
        {
            EnableCommands();
            await MessageBox.Error(e, this);
        }
        finally
        {
            Saving = false;
        }
    }
    /// <summary>
    /// Executes a custom document command.
    /// </summary>
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.Post)
            await ExecutePost();

        await base.ExecuteCustom(Value);
    }
    /// <summary>
    /// Updates command state.
    /// </summary>
    protected override void EnableCommands()
    {
        base.EnableCommands();
        
        // ● visible ===============================================================
        btnPost.IsVisible = true;
        
        // ● enable ================================================================
        btnSave.IsEnabled = btnSave.IsEnabled && !IsDocumentLocked();
        btnPost.IsEnabled = CanPost();
    }
    /// <summary>
    /// Updates control state.
    /// </summary>
    protected override void EnableControls()
    {
        base.EnableControls();
        ItemPage?.SetReadOnly(IsDocumentLocked());
    }
    /// <summary>
    /// Creates the form toolbar.
    /// </summary>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;
        
        btnPost = ToolBar.AddButton("document_mark_as_final.png", "Post Document", async () => await ExecuteCustom(DocumentAction.Post));
        ToolBar.PlaceControlAfter(btnSave, btnPost);
        return true;
    }
    /// <summary>
    /// Handles broadcaster notifications.
    /// </summary>
    protected override void HandleBroadcasterEvent(string EventName, IDictionary<string, object> Args)
    {
        base.HandleBroadcasterEvent(EventName, Args);
        if (!EventName.IsSameText("Document.Posted") || CurrentRow == null || FormState != DataFormState.Edit)
            return;
        if (HasChanges())
        {
            UiLog("Document changed by another form; refresh is skipped because this form has unsaved changes.");
            return;
        }

        DocumentPostedInfo Info = DocumentPostedInfo.FromParams(Args);
        string DocumentId = CurrentRow.AsString(ModuleDef.Table.KeyField);
        if (!Info.AffectsDocument(DocumentId))
            return;

        Dispatcher.UIThread.Post(() =>
        {
            if (CurrentRow != null && FormState == DataFormState.Edit && !HasChanges())
            {
                Refresh();
                UiLog($"Refreshed {GetItemLogText(DocumentId)} after document posting.");
            }
        }, DispatcherPriority.Background);
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public DocumentDataForm()
    {
    }   
}
