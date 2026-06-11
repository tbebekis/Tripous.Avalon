/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

public class AppDataForm: DataForm
{
    // ● construction
    public AppDataForm()
    {
    }
}

public class DocumentDataForm : AppDataForm
{
    // ● protected fields
    protected Button btnPost;

    // ● protected
    protected virtual bool IsDocumentLocked() => CurrentRow != null && CurrentRow.AsBoolean("IsLocked");
    protected virtual bool CanPost()
    {
        if (!IsEditableForm || FormState != DataFormState.Edit || CurrentRow == null || HasChanges())
            return false;
        if (Module is not DocumentDataModule)
            return false;
        if ((TradeStatus)CurrentRow.AsInteger("TradeStatusId") != TradeStatus.Draft)
            return false;
        if (CurrentRow.AsBoolean("IsCancelled") || CurrentRow.AsBoolean("IsLocked"))
            return false;

        return true;
    }
    protected virtual async Task ExecutePost()
    {
        if (!CanPost())
            return;

        string Code = CurrentRow.AsString("Code");
        string DocumentText = string.IsNullOrWhiteSpace(Code) ? "document" : $"document: {Code}";
        string Message = $"Post {DocumentText}? After posting, the document can no longer be edited.";
        if (!await MessageBox.YesNo(Message, this))
            return;

        Dictionary<DataGrid, Tuple<int, DataGridColumn>> DetailGridSelection = ItemPage?.CaptureDetailGridSelection();
        Saving = true;
        try
        {
            DocumentDataModule DocumentModule = (DocumentDataModule)Module;
            object Id = DocumentModule.Post();
            fListTargetId = Id;
            ItemPage?.Refresh();
            ItemPage?.RestoreDetailGridSelection(DetailGridSelection);
            UiLog($"Posted {GetItemLogText(Id)}");
        }
        finally
        {
            Saving = false;
        }
    }
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.Post)
            await ExecutePost();

        await base.ExecuteCustom(Value);
    }
    protected override void EnableCommands()
    {
        base.EnableCommands();
        
        // ● visible ===============================================================
        btnPost.IsVisible = true;
        
        // ● enable ================================================================
        btnSave.IsEnabled = btnSave.IsEnabled && !IsDocumentLocked();
        btnPost.IsEnabled = CanPost();
    }
    protected override void EnableControls()
    {
        base.EnableControls();
        ItemPage?.SetReadOnly(IsDocumentLocked());
    }
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;
        
        btnPost = ToolBar.AddButton("document_mark_as_final.png", "Post Document", async () => await ExecuteCustom(DocumentAction.Post));
        ToolBar.PlaceControlAfter(btnSave, btnPost);
        return true;
    }

    // ● construction
    public DocumentDataForm()
    {
    }   
}

public class SalesOrderForm : DocumentDataForm
{
    // ● construction
    public SalesOrderForm()
    {
    } 
}
