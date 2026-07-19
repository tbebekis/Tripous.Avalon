/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Provides an admin editor for system resource translations.
/// </summary>
public partial class ResourceTranslationsForm : AppForm
{
    // ● private fields
    ResourceTranslationTable fTranslationTable;
    Dictionary<string, ResourceTranslationLanguage> fLanguageMap = [];
    bool fLoading;
    bool fReloadPending;

    // ● private methods
    void LoadTranslations()
    {
        if (fLoading)
        {
            fReloadPending = true;
            return;
        }

        fLoading = true;
        try
        {
            GroupGridBinder.UnBindGrid(GridTranslations);
            fTranslationTable = ResourceTranslationService.Load(AppHost.Store);
            fLanguageMap = fTranslationTable.Languages.ToDictionary(item => item.ColumnName, StringComparer.OrdinalIgnoreCase);
            GroupGridBinder.BindGrid(GridTranslations, fTranslationTable.Table.DefaultView, GoToFirst: true);
            GridTranslations.BestFitColumns();
        }
        finally
        {
            fLoading = false;
            if (fReloadPending)
            {
                fReloadPending = false;
                Ui.Post(LoadTranslations);
            }
        }
    }
    void GridTranslations_CellValueCommitted(object Sender, GroupGridCellEditEventArgs Args)
    {
        if (fLoading || Args == null || Args.Cell.Column == null)
            return;

        string ColumnName = Args.Cell.Column.Name;
        if (!fLanguageMap.TryGetValue(ColumnName, out ResourceTranslationLanguage Language) || Language.IsEnglish)
            return;

        if (Args.Cell.RowIndex < 0 || Args.Cell.RowIndex >= fTranslationTable.Table.DefaultView.Count)
            return;

        DataRowView RowView = fTranslationTable.Table.DefaultView[Args.Cell.RowIndex];
        if (RowView?.Row == null)
            return;

        string ResKey = RowView.Row.AsString("ResKey");
        string ResValue = Args.Value == null ? "" : Convert.ToString(Args.Value, CultureInfo.CurrentCulture) ?? "";
        ResourceTranslationService.Save(AppHost.Store, Language.Id, ResKey, ResValue);
        RowView.Row.AcceptChanges();
        AppHost.Log($"Resource translation saved: {ResKey} [{Language.Code}]");
    }
    string GetDeletedResourceKey(DataRowView RowView)
    {
        if (RowView?.Row == null || !RowView.Row.Table.Columns.Contains("ResKey"))
            return string.Empty;

        DataRow Row = RowView.Row;
        return Row.RowState == DataRowState.Deleted
            ? Convert.ToString(Row["ResKey", DataRowVersion.Original], CultureInfo.CurrentCulture) ?? string.Empty
            : Row.AsString("ResKey", string.Empty);
    }
    void GridTranslations_RowDeleted(object Sender, GroupGridRowOperationEventArgs Args)
    {
        if (fLoading || Args?.Row is not DataRowView RowView)
            return;

        string ResKey = GetDeletedResourceKey(RowView);
        if (string.IsNullOrWhiteSpace(ResKey))
            return;

        ResourceTranslationService.DeleteResourceKey(AppHost.Store, ResKey);
        AppHost.Log($"Resource translations deleted: {ResKey}");
    }

    // ● protected methods
    /// <summary>
    /// Initializes the form.
    /// </summary>
    protected override void FormInitialize()
    {
        GridTranslations.IsReadOnly = false;
        GridTranslations.IsSettingsMenuItemsVisible = false;
        GridTranslations.CellValueCommitted += GridTranslations_CellValueCommitted;
        GridTranslations.RowDeleted += GridTranslations_RowDeleted;
        LoadTranslations();
    }
    /// <summary>
    /// Handles broadcaster events.
    /// </summary>
    /// <param name="EventName">The broadcaster event name.</param>
    /// <param name="Args">The broadcaster event arguments.</param>
    protected override void HandleBroadcasterEvent(string EventName, IDictionary<string, object> Args)
    {
        base.HandleBroadcasterEvent(EventName, Args);
        if (EventName.IsSameText("SysStrRes.Changed"))
        {
            LoadTranslations();
            return;
        }

        if (!EventName.IsSameText("DataModule.Saved") || Args == null || !Args.TryGetValue("ModuleName", out object ModuleNameValue))
            return;

        string ModuleName = Convert.ToString(ModuleNameValue, CultureInfo.CurrentCulture) ?? "";
        if (ModuleName.IsSameText("SYS_LANG") || ModuleName.IsSameText("SysLang"))
            LoadTranslations();
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public ResourceTranslationsForm()
    {
        InitializeComponent();
    }
}
