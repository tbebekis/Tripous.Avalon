/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Displays a Supplier Payment Cancellation document.
/// </summary>
public class SupplierPaymentCancellationForm : DocumentDataForm
{
    // ● construction
    public SupplierPaymentCancellationForm()
    {
    }

    // ● public
    /// <summary>
    /// Returns true when a detail grid command can execute.
    /// </summary>
    public override bool CanExecuteGridCommand(GridCommandContext Context)
    {
        if (Context?.Command?.ActionType == GridActionType.Add && Context.Table?.TableName.IsSameText("PaymentSettlement") == true)
            return false;
        return base.CanExecuteGridCommand(Context);
    }
}
