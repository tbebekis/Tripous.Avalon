/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Displays a Stock Transaction and provides cancellation creation.
/// </summary>
public class StockTradeForm: DocumentDataForm
{
    // ● protected fields
    /// <summary>
    /// Creates a reversing Stock Transaction.
    /// </summary>
    protected Button BtnCreateCancellation;

    // ● protected
    /// <summary>
    /// Returns true when the current Stock Transaction can be cancelled.
    /// </summary>
    protected virtual bool CanCreateCancellation()
    {
        return Module is StockTradeDataModule
               && FormState == DataFormState.Edit
               && CurrentRow != null
               && !HasChanges()
               && (TradeStatus)CurrentRow.AsInteger("StatusId") == TradeStatus.Posted
               && !CurrentRow.AsBoolean("IsCancelled")
               && string.IsNullOrWhiteSpace(CurrentRow.AsString("CancelsStockTradeId"))
               && string.IsNullOrWhiteSpace(CurrentRow.AsString("CancelledByStockTradeId"));
    }
    /// <summary>
    /// Creates and displays a reversing Stock Transaction.
    /// </summary>
    protected virtual async Task ExecuteCreateCancellation()
    {
        if (!CanCreateCancellation())
            return;

        string Code = CurrentRow.AsString("Code");
        string DocumentText = string.IsNullOrWhiteSpace(Code) ? Texts.L("StockTransaction", "Stock Transaction") : $"{Texts.L("StockTransaction", "Stock Transaction")}: {Code}";
        if (!await MessageBox.YesNo($"{Texts.L("CreateCancellationFor", "Create a cancellation for")} {DocumentText}?", this))
            return;

        StockTradeDataModule CancellationModule = ((StockTradeDataModule)Module).CreateCancellation();
        DataFormContext Context = DataFormContext.Create("StockTrade", CancellationModule, this);
        Context.StartAction = DataFormAction.Insert;
        await AppFormDialog.ShowModalDataForm(Context);
        ItemPage?.Refresh();
    }
    /// <summary>
    /// Executes custom Stock Transaction actions.
    /// </summary>
    protected override async Task ExecuteCustom(object Value)
    {
        if (Value is DocumentAction Action && Action == DocumentAction.CreateCancellation)
            await ExecuteCreateCancellation();

        await base.ExecuteCustom(Value);
    }
    /// <summary>
    /// Updates Stock Transaction toolbar command states.
    /// </summary>
    protected override void EnableCommands()
    {
        base.EnableCommands();

        BtnCreateCancellation.IsVisible = true;
        BtnCreateCancellation.IsEnabled = CanCreateCancellation();
    }
    /// <summary>
    /// Creates the Stock Transaction toolbar.
    /// </summary>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;

        BtnCreateCancellation = ToolBar.AddButton("document_torn.png", Texts.L("CreateStockCancellation", "Create Stock Cancellation"), async () => await ExecuteCustom(DocumentAction.CreateCancellation));
        ToolBar.PlaceControlAfter(btnPost, BtnCreateCancellation);
        return true;
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public StockTradeForm()
    {
    }
}
