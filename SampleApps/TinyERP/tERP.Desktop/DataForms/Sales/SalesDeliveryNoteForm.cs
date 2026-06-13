/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;


public class SalesDeliveryNoteForm : DocumentDataForm
{
    // ● protected fields
    protected Button btnCreateReturn;
    protected Button btnCreateInvoice;

    // ● protected
    protected virtual bool CanCreateInvoice()
    {
        return Module is SalesDeliveryNoteDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    protected virtual bool CanCreateReturn()
    {
        return Module is SalesDeliveryNoteDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    protected virtual async Task ExecuteCreateReturn()
    {
        if (!CanCreateReturn())
            return;

        SalesDeliveryNoteDataModule DeliveryNoteModule = (SalesDeliveryNoteDataModule)Module;
        if (!DeliveryNoteModule.HasRemainingTransformQuantity())
        {
            await MessageBox.Info("The source document has no remaining quantity to transform.", this);
            return;
        }

        string Code = CurrentRow.AsString("Code");
        string DeliveryText = string.IsNullOrWhiteSpace(Code) ? "Sales Delivery Note" : $"Sales Delivery Note: {Code}";
        if (!await MessageBox.YesNo($"Create a Sales Return from {DeliveryText}?", this))
            return;

        SalesReturnDataModule ReturnModule = DeliveryNoteModule.CreateReturn();
        DataFormContext Context = DataFormContext.Create("SalesReturn", ReturnModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    protected virtual async Task ExecuteCreateInvoice()
    {
        if (!CanCreateInvoice())
            return;

        SalesDeliveryNoteDataModule DeliveryNoteModule = (SalesDeliveryNoteDataModule)Module;
        if (!DeliveryNoteModule.HasRemainingInvoiceQuantity())
        {
            await MessageBox.Info("The source document has no remaining quantity to invoice.", this);
            return;
        }

        string Code = CurrentRow.AsString("Code");
        string DeliveryText = string.IsNullOrWhiteSpace(Code) ? "Sales Delivery Note" : $"Sales Delivery Note: {Code}";
        if (!await MessageBox.YesNo($"Create a Sales Invoice from {DeliveryText}?", this))
            return;

        SalesInvoiceDataModule InvoiceModule = DeliveryNoteModule.CreateInvoice();
        DataFormContext Context = DataFormContext.Create("SalesInvoice", InvoiceModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateReturn)
            await ExecuteCreateReturn();
        if (Value is DocumentAction InvoiceAction && InvoiceAction == DocumentAction.CreateInvoice)
            await ExecuteCreateInvoice();

        await base.ExecuteCustom(Value);
    }
    protected override void EnableCommands()
    {
        base.EnableCommands();

        btnCreateReturn.IsVisible = true;
        btnCreateReturn.IsEnabled = CanCreateReturn();
        btnCreateInvoice.IsVisible = true;
        btnCreateInvoice.IsEnabled = CanCreateInvoice();
    }
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;

        btnCreateReturn = ToolBar.AddButton("document_redirect.png", "Create Sales Return", async () => await ExecuteCustom(DocumentAction.CreateReturn));
        ToolBar.PlaceControlAfter(btnPost, btnCreateReturn);
        btnCreateInvoice = ToolBar.AddButton("document_export.png", "Create Sales Invoice", async () => await ExecuteCustom(DocumentAction.CreateInvoice));
        ToolBar.PlaceControlAfter(btnCreateReturn, btnCreateInvoice);
        return true;
    }

    // ● construction
    public SalesDeliveryNoteForm()
    {
    }
}