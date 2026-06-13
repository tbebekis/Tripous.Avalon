/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

public class PurchaseOrderForm : DocumentDataForm
{
    // ● protected fields
    protected Button btnCreateDeliveryNote;

    // ● protected
    protected virtual bool CanCreateDeliveryNote()
    {
        return Module is PurchaseOrderDataModule
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
        string OrderText = string.IsNullOrWhiteSpace(Code) ? "Purchase Order" : $"Purchase Order: {Code}";
        if (!await MessageBox.YesNo($"Create a Purchase Delivery Note from {OrderText}?", this))
            return;

        PurchaseOrderDataModule PurchaseOrderModule = (PurchaseOrderDataModule)Module;
        PurchaseDeliveryNoteDataModule DeliveryNoteModule = PurchaseOrderModule.CreateDeliveryNote();
        DataFormContext Context = DataFormContext.Create("PurchaseDeliveryNote", DeliveryNoteModule, this);
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

        btnCreateDeliveryNote = ToolBar.AddButton("document_export.png", "Create Purchase Delivery Note", async () => await ExecuteCustom(DocumentAction.CreateDeliveryNote));
        ToolBar.PlaceControlAfter(btnPost, btnCreateDeliveryNote);
        return true;
    }

    // ● construction
    public PurchaseOrderForm()
    {
    }
}