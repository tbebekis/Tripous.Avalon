/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Data form for purchase delivery note documents.
/// </summary>
public class PurchaseDeliveryNoteForm : DocumentDataForm
{
    // ● protected fields
    /// <summary>
    /// Button that creates a purchase return from the current delivery note.
    /// </summary>
    protected Button btnCreateReturn;
    /// <summary>
    /// Button that creates a purchase invoice from the current delivery note.
    /// </summary>
    protected Button btnCreateInvoice;

    // ● protected
    /// <summary>
    /// Returns true when a purchase invoice can be created.
    /// </summary>
    protected virtual bool CanCreateInvoice()
    {
        return Module is PurchaseDeliveryNoteDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    /// <summary>
    /// Returns true when a purchase return can be created.
    /// </summary>
    protected virtual bool CanCreateReturn()
    {
        return Module is PurchaseDeliveryNoteDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    /// <summary>
    /// Creates a purchase return from the current delivery note.
    /// </summary>
    protected virtual async Task ExecuteCreateReturn()
    {
        if (!CanCreateReturn())
            return;

        PurchaseDeliveryNoteDataModule DeliveryNoteModule = (PurchaseDeliveryNoteDataModule)Module;
        if (!DeliveryNoteModule.HasRemainingTransformQuantity())
        {
            await MessageBox.Info("The source document has no remaining quantity to transform.", this);
            return;
        }

        string Code = CurrentRow.AsString("Code");
        string DeliveryText = string.IsNullOrWhiteSpace(Code) ? "Purchase Delivery Note" : $"Purchase Delivery Note: {Code}";
        if (!await MessageBox.YesNo($"Create a Purchase Return from {DeliveryText}?", this))
            return;

        PurchaseReturnDataModule ReturnModule = DeliveryNoteModule.CreateReturn();
        DataFormContext Context = DataFormContext.Create("PurchaseReturn", ReturnModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    /// <summary>
    /// Creates a purchase invoice from the current delivery note.
    /// </summary>
    protected virtual async Task ExecuteCreateInvoice()
    {
        if (!CanCreateInvoice())
            return;

        PurchaseDeliveryNoteDataModule DeliveryNoteModule = (PurchaseDeliveryNoteDataModule)Module;
        if (!DeliveryNoteModule.HasRemainingInvoiceQuantity())
        {
            await MessageBox.Info("The source document has no remaining quantity to invoice.", this);
            return;
        }

        string Code = CurrentRow.AsString("Code");
        string DeliveryText = string.IsNullOrWhiteSpace(Code) ? "Purchase Delivery Note" : $"Purchase Delivery Note: {Code}";
        if (!await MessageBox.YesNo($"Create a Purchase Invoice from {DeliveryText}?", this))
            return;

        PurchaseInvoiceDataModule InvoiceModule = DeliveryNoteModule.CreateInvoice();
        DataFormContext Context = DataFormContext.Create("PurchaseInvoice", InvoiceModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    /// <summary>
    /// Executes a custom purchase delivery note command.
    /// </summary>
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateReturn)
            await ExecuteCreateReturn();
        if (Value is DocumentAction InvoiceAction && InvoiceAction == DocumentAction.CreateInvoice)
            await ExecuteCreateInvoice();

        await base.ExecuteCustom(Value);
    }
    /// <summary>
    /// Updates command state.
    /// </summary>
    protected override void EnableCommands()
    {
        base.EnableCommands();

        btnCreateReturn.IsVisible = true;
        btnCreateReturn.IsEnabled = CanCreateReturn();
        btnCreateInvoice.IsVisible = true;
        btnCreateInvoice.IsEnabled = CanCreateInvoice();
    }
    /// <summary>
    /// Creates the form toolbar.
    /// </summary>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;

        btnCreateReturn = ToolBar.AddButton("document_redirect.png", "Create Purchase Return", async () => await ExecuteCustom(DocumentAction.CreateReturn));
        ToolBar.PlaceControlAfter(btnPost, btnCreateReturn);
        btnCreateInvoice = ToolBar.AddButton("document_export.png", "Create Purchase Invoice", async () => await ExecuteCustom(DocumentAction.CreateInvoice));
        ToolBar.PlaceControlAfter(btnCreateReturn, btnCreateInvoice);
        return true;
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public PurchaseDeliveryNoteForm()
    {
    }
}
