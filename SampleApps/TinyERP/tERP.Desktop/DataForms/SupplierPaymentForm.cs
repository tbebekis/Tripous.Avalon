/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Data form for supplier payment documents.
/// </summary>
public class SupplierPaymentForm : DocumentDataForm
{
    // ● protected fields
    /// <summary>
    /// Creates a Supplier Payment Cancellation from the current payment.
    /// </summary>
    protected Button BtnCreateCancellation;

    // ● protected
    /// <summary>
    /// Returns true when the current Supplier Payment can create a Cancellation document.
    /// </summary>
    protected virtual bool CanCreateCancellation()
    {
        return Module is PaymentDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("StatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled")
               && string.IsNullOrWhiteSpace(CurrentRow.AsString("CancelledPaymentId"))
               && string.IsNullOrWhiteSpace(CurrentRow.AsString("CancellationPaymentId"));
    }
    /// <summary>
    /// Creates and displays a Supplier Payment Cancellation.
    /// </summary>
    protected virtual async Task ExecuteCreateCancellation()
    {
        if (!CanCreateCancellation())
            return;
        string Code = CurrentRow.AsString("Code");
        string PaymentText = string.IsNullOrWhiteSpace(Code) ? "Supplier Payment" : $"Supplier Payment: {Code}";
        if (!await MessageBox.YesNo($"Create a Supplier Payment Cancellation from {PaymentText}?", this))
            return;
        PaymentDataModule CancellationModule = ((PaymentDataModule)Module).CreateCancellation();
        DataFormContext Context = DataFormContext.Create("SupplierPaymentCancellation", CancellationModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateCancellation)
            await ExecuteCreateCancellation();
        await base.ExecuteCustom(Value);
    }
    protected override async Task ExecuteSave()
    {
        await base.ExecuteSave();
        if (Module is PaymentDataModule PaymentModule && !string.IsNullOrWhiteSpace(PaymentModule.AmountAdjustmentMessage))
            await MessageBox.Info(PaymentModule.AmountAdjustmentMessage, this);
    }
    protected override void EnableCommands()
    {
        base.EnableCommands();
        BtnCreateCancellation.IsVisible = true;
        BtnCreateCancellation.IsEnabled = CanCreateCancellation();
    }
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;
        BtnCreateCancellation = ToolBar.AddButton("document_torn.png", "Create Supplier Payment Cancellation", async () => await ExecuteCustom(DocumentAction.CreateCancellation));
        ToolBar.PlaceControlAfter(btnPost, BtnCreateCancellation);
        return true;
    }

    // ● construction
    public SupplierPaymentForm()
    {
    }
}
