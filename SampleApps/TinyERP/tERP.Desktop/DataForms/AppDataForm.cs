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
    protected virtual bool IsDocumentCancelled() => CurrentRow != null
                                                     && CurrentRow.Table.Columns.Contains("IsCancelled")
                                                     && CurrentRow.AsBoolean("IsCancelled");
    protected virtual bool IsDocumentLocked()
    {
        if (CurrentRow == null)
            return false;
        if (CurrentRow.Table.Columns.Contains("IsLocked"))
            return CurrentRow.AsBoolean("IsLocked");
        return GetDocumentStatus() != TradeStatus.Draft;
    }
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
    protected virtual async Task ExecutePost()
    {
        if (!CanPost())
            return;

        string Code = CurrentRow.AsString("Code");
        string DocumentText = string.IsNullOrWhiteSpace(Code) ? "document" : $"document: {Code}";
        string Message = $@"Post {DocumentText}? 

After posting, the document can no longer be edited.
";
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
    // ● protected fields
    protected Button btnCreateDeliveryNote;

    // ● protected
    protected virtual bool CanCreateDeliveryNote()
    {
        return Module is SalesOrderDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    protected virtual async Task ExecuteCreateDeliveryNote()
    {
        if (!CanCreateDeliveryNote())
            return;

        string Code = CurrentRow.AsString("Code");
        string OrderText = string.IsNullOrWhiteSpace(Code) ? "Sales Order" : $"Sales Order: {Code}";
        if (!await MessageBox.YesNo($"Create a Sales Delivery Note from {OrderText}?", this))
            return;

        SalesOrderDataModule SalesOrderModule = (SalesOrderDataModule)Module;
        SalesDeliveryNoteDataModule DeliveryNoteModule = SalesOrderModule.CreateDeliveryNote();
        DataFormContext Context = DataFormContext.Create("SalesDeliveryNote", DeliveryNoteModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateDeliveryNote)
            await ExecuteCreateDeliveryNote();

        await base.ExecuteCustom(Value);
    }
    protected override void EnableCommands()
    {
        base.EnableCommands();

        btnCreateDeliveryNote.IsVisible = true;
        btnCreateDeliveryNote.IsEnabled = CanCreateDeliveryNote();
    }
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;

        btnCreateDeliveryNote = ToolBar.AddButton("document_export.png", "Create Sales Delivery Note", async () => await ExecuteCustom(DocumentAction.CreateDeliveryNote));
        ToolBar.PlaceControlAfter(btnPost, btnCreateDeliveryNote);
        return true;
    }

    // ● construction
    public SalesOrderForm()
    {
    } 
}

public class SalesDeliveryNoteForm : DocumentDataForm
{
    // ● construction
    public SalesDeliveryNoteForm()
    {
    }
}

public class SalesInvoiceForm : DocumentDataForm
{
    // ● construction
    public SalesInvoiceForm()
    {
    }
}

public class SalesCreditNoteForm : DocumentDataForm
{
    // ● construction
    public SalesCreditNoteForm()
    {
    }
}

public class SalesReturnForm : DocumentDataForm
{
    // ● construction
    public SalesReturnForm()
    {
    }
}

public class SalesCancellationForm : DocumentDataForm
{
    // ● construction
    public SalesCancellationForm()
    {
    }
}

public class PurchaseOrderForm : DocumentDataForm
{
    // ● construction
    public PurchaseOrderForm()
    {
    }
}

public class PurchaseDeliveryNoteForm : DocumentDataForm
{
    // ● construction
    public PurchaseDeliveryNoteForm()
    {
    }
}

public class PurchaseInvoiceForm : DocumentDataForm
{
    // ● construction
    public PurchaseInvoiceForm()
    {
    }
}

public class PurchaseCreditNoteForm : DocumentDataForm
{
    // ● construction
    public PurchaseCreditNoteForm()
    {
    }
}

public class PurchaseReturnForm : DocumentDataForm
{
    // ● construction
    public PurchaseReturnForm()
    {
    }
}

public class PurchaseCancellationForm : DocumentDataForm
{
    // ● construction
    public PurchaseCancellationForm()
    {
    }
}

public class StockCountForm : DocumentDataForm
{
    // ● construction
    public StockCountForm()
    {
    }
}
