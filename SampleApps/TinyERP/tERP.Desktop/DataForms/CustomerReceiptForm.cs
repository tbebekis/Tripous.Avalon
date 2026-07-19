/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Data form for customer receipt documents.
/// </summary>
public class CustomerReceiptForm : DocumentDataForm
{
    // ● protected fields
    /// <summary>
    /// Creates a Customer Receipt Cancellation from the current receipt.
    /// </summary>
    protected Button BtnCreateCancellation;

    // ● protected
    /// <summary>
    /// Returns true when the current Customer Receipt can create a Cancellation document.
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
    /// Creates and displays a Customer Receipt Cancellation.
    /// </summary>
    protected virtual async Task ExecuteCreateCancellation()
    {
        if (!CanCreateCancellation())
            return;
        string Code = CurrentRow.AsString("Code");
        string PaymentText = string.IsNullOrWhiteSpace(Code) ? Texts.L("CustomerReceipt", "Customer Receipt") : $"{Texts.L("CustomerReceipt", "Customer Receipt")}: {Code}";
        if (!await MessageBox.YesNo($"{Texts.L("CreateCustomerReceiptCancellationFrom", "Create a Customer Receipt Cancellation from")} {PaymentText}?", this))
            return;
        PaymentDataModule CancellationModule = ((PaymentDataModule)Module).CreateCancellation();
        DataFormContext Context = DataFormContext.Create("CustomerReceiptCancellation", CancellationModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    /// <summary>
    /// Executes a custom customer receipt command.
    /// </summary>
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateCancellation)
            await ExecuteCreateCancellation();
        await base.ExecuteCustom(Value);
    }
    /// <summary>
    /// Saves the current receipt and displays any amount adjustment message.
    /// </summary>
    protected override async Task ExecuteSave()
    {
        await base.ExecuteSave();
        if (Module is PaymentDataModule PaymentModule && !string.IsNullOrWhiteSpace(PaymentModule.AmountAdjustmentMessage))
            await MessageBox.Info(PaymentModule.AmountAdjustmentMessage, this);
    }
    /// <summary>
    /// Updates command state.
    /// </summary>
    protected override void EnableCommands()
    {
        base.EnableCommands();
        BtnCreateCancellation.IsVisible = true;
        BtnCreateCancellation.IsEnabled = CanCreateCancellation();
    }
    /// <summary>
    /// Creates the form toolbar.
    /// </summary>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;
        BtnCreateCancellation = ToolBar.AddButton("document_torn.png", Texts.L("CreateCustomerReceiptCancellation", "Create Customer Receipt Cancellation"), async () => await ExecuteCustom(DocumentAction.CreateCancellation));
        ToolBar.PlaceControlAfter(btnPost, BtnCreateCancellation);
        return true;
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public CustomerReceiptForm()
    {
    }
}
