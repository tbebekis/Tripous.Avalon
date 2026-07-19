/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Data form for purchase order documents.
/// </summary>
public class PurchaseOrderForm : DocumentDataForm
{
    // ● protected fields
    /// <summary>
    /// Button that creates a purchase delivery note from the current purchase order.
    /// </summary>
    protected Button btnCreateDeliveryNote;

    // ● protected
    /// <summary>
    /// Returns true when a purchase delivery note can be created.
    /// </summary>
    protected virtual bool CanCreateDeliveryNote()
    {
        return Module is PurchaseOrderDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    /// <summary>
    /// Creates a purchase delivery note from the current purchase order.
    /// </summary>
    protected virtual async Task ExecuteCreateDeliveryNote()
    {
        if (!CanCreateDeliveryNote())
            return;

        string Code = CurrentRow.AsString("Code");
        string OrderText = string.IsNullOrWhiteSpace(Code) ? Texts.L("PurchaseOrder", "Purchase Order") : $"{Texts.L("PurchaseOrder", "Purchase Order")}: {Code}";
        if (!await MessageBox.YesNo($"{Texts.L("CreatePurchaseDeliveryNoteFrom", "Create a Purchase Delivery Note from")} {OrderText}?", this))
            return;

        PurchaseOrderDataModule PurchaseOrderModule = (PurchaseOrderDataModule)Module;
        PurchaseDeliveryNoteDataModule DeliveryNoteModule = PurchaseOrderModule.CreateDeliveryNote();
        DataFormContext Context = DataFormContext.Create("PurchaseDeliveryNote", DeliveryNoteModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    /// <summary>
    /// Executes a custom purchase order command.
    /// </summary>
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateDeliveryNote)
            await ExecuteCreateDeliveryNote();

        await base.ExecuteCustom(Value);
    }
    /// <summary>
    /// Updates command state.
    /// </summary>
    protected override void EnableCommands()
    {
        base.EnableCommands();

        btnCreateDeliveryNote.IsVisible = true;
        btnCreateDeliveryNote.IsEnabled = CanCreateDeliveryNote();
    }
    /// <summary>
    /// Creates the form toolbar.
    /// </summary>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;

        btnCreateDeliveryNote = ToolBar.AddButton("document_export.png", Texts.L("CreatePurchaseDeliveryNote", "Create Purchase Delivery Note"), async () => await ExecuteCustom(DocumentAction.CreateDeliveryNote));
        ToolBar.PlaceControlAfter(btnPost, btnCreateDeliveryNote);
        return true;
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public PurchaseOrderForm()
    {
    }
}
