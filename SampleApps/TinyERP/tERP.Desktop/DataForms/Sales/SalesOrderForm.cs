/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Data form for sales order documents.
/// </summary>
public class SalesOrderForm : DocumentDataForm
{
    // ● protected fields
    /// <summary>
    /// Button that creates a sales delivery note from the current sales order.
    /// </summary>
    protected Button btnCreateDeliveryNote;

    // ● protected
    /// <summary>
    /// Returns true when a sales delivery note can be created.
    /// </summary>
    protected virtual bool CanCreateDeliveryNote()
    {
        return Module is SalesOrderDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("TradeStatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled");
    }
    /// <summary>
    /// Creates a sales delivery note from the current sales order.
    /// </summary>
    protected virtual async Task ExecuteCreateDeliveryNote()
    {
        if (!CanCreateDeliveryNote())
            return;

        string Code = CurrentRow.AsString("Code");
        string OrderText = string.IsNullOrWhiteSpace(Code) ? Texts.L("SalesOrder", "Sales Order") : $"{Texts.L("SalesOrder", "Sales Order")}: {Code}";
        if (!await MessageBox.YesNo($"{Texts.L("CreateSalesDeliveryNoteFrom", "Create a Sales Delivery Note from")} {OrderText}?", this))
            return;

        SalesOrderDataModule SalesOrderModule = (SalesOrderDataModule)Module;
        SalesDeliveryNoteDataModule DeliveryNoteModule = SalesOrderModule.CreateDeliveryNote();
        DataFormContext Context = DataFormContext.Create("SalesDeliveryNote", DeliveryNoteModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    /// <summary>
    /// Executes a custom sales order command.
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

        btnCreateDeliveryNote = ToolBar.AddButton("document_export.png", Texts.L("CreateSalesDeliveryNote", "Create Sales Delivery Note"), async () => await ExecuteCustom(DocumentAction.CreateDeliveryNote));
        ToolBar.PlaceControlAfter(btnPost, btnCreateDeliveryNote);
        return true;
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public SalesOrderForm()
    {
    } 
}
