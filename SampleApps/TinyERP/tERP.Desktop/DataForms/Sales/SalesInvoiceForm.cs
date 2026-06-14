/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Displays a Sales Invoice and provides Credit Note and Cancellation creation.
/// </summary>
public class SalesInvoiceForm: DocumentDataForm
{
    // ● protected fields
    /// <summary>
    /// Creates a Sales Credit Note from the current Invoice.
    /// </summary>
    protected Button BtnCreateCreditNote;
    /// <summary>
    /// Creates a Sales Cancellation from the current Invoice.
    /// </summary>
    protected Button BtnCreateCancellation;

    // ● protected
    /// <summary>
    /// Returns true when the current Sales Invoice can create a Credit Note.
    /// </summary>
    protected virtual bool CanCreateCreditNote()
    {
        return Module is SalesInvoiceDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    /// <summary>
    /// Returns true when the current Sales Invoice can create a Cancellation document.
    /// </summary>
    protected virtual bool CanCreateCancellation()
    {
        return Module is SalesInvoiceDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    /// <summary>
    /// Creates and displays a Sales Credit Note after validating the current database remainder.
    /// </summary>
    protected virtual async Task ExecuteCreateCreditNote()
    {
        if (!CanCreateCreditNote())
            return;

        SalesInvoiceDataModule InvoiceModule = (SalesInvoiceDataModule)Module;
        if (!InvoiceModule.HasRemainingCreditQuantity())
        {
            await MessageBox.Info("The source document has no remaining quantity to credit.", this);
            return;
        }

        string Code = CurrentRow.AsString("Code");
        string InvoiceText = string.IsNullOrWhiteSpace(Code) ? "Sales Invoice" : $"Sales Invoice: {Code}";
        if (!await MessageBox.YesNo($"Create a Sales Credit Note from {InvoiceText}?", this))
            return;

        SalesCreditNoteDataModule CreditNoteModule = InvoiceModule.CreateCreditNote();
        DataFormContext Context = DataFormContext.Create("SalesCreditNote", CreditNoteModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    /// <summary>
    /// Creates and displays a Sales Cancellation after validating the current database state.
    /// </summary>
    protected virtual async Task ExecuteCreateCancellation()
    {
        if (!CanCreateCancellation())
            return;

        SalesInvoiceDataModule InvoiceModule = (SalesInvoiceDataModule)Module;
        if (InvoiceModule.HasCreditedQuantity())
        {
            await MessageBox.Info("A Sales Invoice with posted Credit Notes cannot be cancelled.", this);
            return;
        }

        string Code = CurrentRow.AsString("Code");
        string InvoiceText = string.IsNullOrWhiteSpace(Code) ? "Sales Invoice" : $"Sales Invoice: {Code}";
        if (!await MessageBox.YesNo($"Create a Sales Cancellation from {InvoiceText}?", this))
            return;

        SalesCancellationDataModule CancellationModule = InvoiceModule.CreateCancellation();
        DataFormContext Context = DataFormContext.Create("SalesCancellation", CancellationModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateCreditNote)
            await ExecuteCreateCreditNote();
        if (Value is DocumentAction CancellationAction && CancellationAction == DocumentAction.CreateCancellation)
            await ExecuteCreateCancellation();

        await base.ExecuteCustom(Value);
    }
    protected override void EnableCommands()
    {
        base.EnableCommands();

        BtnCreateCreditNote.IsVisible = true;
        BtnCreateCreditNote.IsEnabled = CanCreateCreditNote();
        BtnCreateCancellation.IsVisible = true;
        BtnCreateCancellation.IsEnabled = CanCreateCancellation();
    }
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;

        BtnCreateCreditNote = ToolBar.AddButton("document_redirect.png", "Create Sales Credit Note", async () => await ExecuteCustom(DocumentAction.CreateCreditNote));
        ToolBar.PlaceControlAfter(btnPost, BtnCreateCreditNote);
        BtnCreateCancellation = ToolBar.AddButton("document_torn.png", "Create Sales Cancellation", async () => await ExecuteCustom(DocumentAction.CreateCancellation));
        ToolBar.PlaceControlAfter(BtnCreateCreditNote, BtnCreateCancellation);
        return true;
    }

    // ● construction
    public SalesInvoiceForm()
    {
    }
}
