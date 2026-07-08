/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Provides a Tripous Locator2 in-place editor for GroupGrid cells.
/// </summary>
public class GroupGridLocatorInplaceEditor: GroupGridDropDownInplaceEditorBase
{
    // ● private fields
    readonly LocatorDef fLocatorDef;
    readonly string fSearchField;
    readonly DataRowView fTargetRowView;
    DataTable fDropDownTable;
    GroupGrid fDropDownGrid;
    DataRow fSelectedRow;
    bool fIsSettingText;

    // ● private methods
    bool ContainsSearchTrigger(string Term) => !string.IsNullOrWhiteSpace(Term) && Term.TrimEnd().EndsWith("?");
    string GetSearchTerm(string Term) => !string.IsNullOrWhiteSpace(Term) ? Term.Trim().TrimEnd('?').Trim() : string.Empty;
    string GetDisplayText(DataRow Row)
    {
        if (Row == null || fLocatorDef == null)
            return string.Empty;

        List<string> Parts = [];
        foreach (string FieldName in fLocatorDef.GetResultFields())
        {
            if (FieldName.IsSameText(fLocatorDef.KeyField))
                continue;

            DataColumn Column = Row.Table.FindColumn(FieldName);
            if (Column != null && !Sys.IsNull(Row[Column]))
                Parts.Add(Row[Column].ToString());
        }

        return string.Join(" - ", Parts);
    }
    LocatorRequest CreateRequest(string SearchTerm)
    {
        LocatorRequest Result = new()
        {
            Context = new LocatorContext(fLocatorDef?.Name),
            SearchField = fSearchField,
            SearchTerm = SearchTerm,
            IsMultiRow = true,
        };

        DataRow Row = fTargetRowView?.Row;
        if (Row != null)
        {
            Result.Context.Params["Row"] = Row;
            Result.Context.Params["DataRow"] = Row;
        }

        return Result;
    }
    void SetText(string Text)
    {
        fIsSettingText = true;
        try
        {
            TextBox.Text = Text ?? string.Empty;
        }
        finally
        {
            fIsSettingText = false;
        }
    }
    void SetSelectedRow(DataRow Row)
    {
        fSelectedRow = Row;
        SetText(GetDisplayText(Row));
        RaiseValueChanged();
    }
    void LoadDropDownTable(DataTable Table)
    {
        fDropDownTable = Table;
    }
    void ClearDropDownGrid()
    {
        if (fDropDownGrid == null)
            return;

        fDropDownGrid.DoubleTapped -= DropDownGrid_DoubleTapped;
        fDropDownGrid.RemoveHandler(InputElement.KeyDownEvent, DropDownGrid_KeyDown);
        fDropDownGrid.ItemsSource = null;
        fDropDownGrid = null;
    }
    DataRow GetDropDownCurrentRow() => fDropDownGrid?.CurrentRow is DataRowView RowView ? RowView.Row : null;
    int FindCurrentDropDownRowIndex()
    {
        if (fDropDownTable == null || fDropDownTable.DefaultView.Count == 0)
            return -1;

        string Text = TextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(Text))
            return -1;

        List<string> CandidateFields = [];
        if (!string.IsNullOrWhiteSpace(fSearchField))
            CandidateFields.Add(fSearchField);
        CandidateFields.AddRange(fLocatorDef.GetResultFields().Where(FieldName => !CandidateFields.Any(x => x.IsSameText(FieldName))));

        for (int Index = 0; Index < fDropDownTable.DefaultView.Count; Index++)
        {
            DataRow Row = fDropDownTable.DefaultView[Index].Row;
            foreach (string FieldName in CandidateFields)
            {
                DataColumn Column = Row.Table.FindColumn(FieldName);
                if (Column != null && string.Equals(Convert.ToString(Row[Column], CultureInfo.CurrentCulture)?.Trim(), Text, StringComparison.OrdinalIgnoreCase))
                    return Index;
            }
        }

        return -1;
    }
    void SelectDropDownCurrentRow()
    {
        DataRow Row = GetDropDownCurrentRow();
        if (Row == null)
            return;

        DropDownHost?.CommitDropDownValue(Row);
    }
    void Search(bool CommitSingleResult)
    {
        if (fLocatorDef == null)
            return;

        Search(CommitSingleResult, GetSearchTerm(TextBox.Text));
    }
    void Search(bool CommitSingleResult, string SearchTerm)
    {
        if (fLocatorDef == null)
            return;

        LogBox.AppendLine($"Grid Locator2: Searching for term: {SearchTerm}");
        try
        {
            LocatorResult Result = Locators.Execute(CreateRequest(SearchTerm));
            if (Result.HasTooManyResults)
            {
                fDropDownTable = null;
                LogBox.AppendLine($"Grid Locator2: Too many rows for term: {SearchTerm}");
                Ui.Post(async () => await MessageBox.Info(Result.Message, this));
            }
            else if (Result.Status == LocatorResultStatus.NoResult)
            {
                fDropDownTable = null;
                LogBox.AppendLine($"Grid Locator2: No rows found for term: {SearchTerm}");
                Ui.Post(async () => await MessageBox.Info("No rows found.", this));
            }
            else if (Result.HasSingleResult)
            {
                fDropDownTable = null;
                LogBox.AppendLine($"Grid Locator2: Found 1 row for term: {SearchTerm}");
                if (CommitSingleResult && DropDownHost != null)
                    DropDownHost.CommitDropDownValue(Result.Table.Rows[0]);
                else
                {
                    LoadDropDownTable(Result.Table);
                    OpenDropDown();
                }
            }
            else
            {
                LogBox.AppendLine($"Grid Locator2: Found {Result.Count} rows for term: {SearchTerm}");
                LoadDropDownTable(Result.Table);
                OpenDropDown();
            }
        }
        catch (Exception e)
        {
            fDropDownTable = null;
            LogBox.AppendLine($"Grid Locator2: {e.Message}");
            Ui.Post(async () => await MessageBox.Error(e, this));
        }
    }
    void DropDownGrid_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        SelectDropDownCurrentRow();
        Args.Handled = true;
    }
    void DropDownGrid_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter)
        {
            SelectDropDownCurrentRow();
            Args.Handled = true;
        }
        else if (Args.Key == Key.Escape)
        {
            CancelDropDown();
            Args.Handled = true;
        }
    }
    GroupGrid CreateDropDownGrid()
    {
        GroupGrid Result = new()
        {
            AutoGenerateColumns = false,
            IsToolBarVisible = false,
            IsGroupPanelVisible = false,
            IsFilterPanelVisible = Ui.Settings.ShowLocatorGridFilterPanel,
            IsTotalsSummaryVisible = false,
            IsSettingsMenuItemsVisible = false,
            Width = DropDownWidth,
            Height = DropDownHeight,
            MinWidth = DropDownWidth,
            MinHeight = DropDownHeight,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            AreIdColumnsVisible = false,
        };

        if (fDropDownTable != null)
        {
            foreach (DataColumn Column in fDropDownTable.Columns)
            {
                if (!fLocatorDef.GetResultFields().Any(FieldName => FieldName.IsSameText(Column.ColumnName)))
                    continue;
                if (Column.ColumnName.IsSameText(fLocatorDef.KeyField))
                    continue;

                Result.Columns.Add(GroupGridBinder.CreateGridColumn(Column, Format: Column.DataType.GetDefaultFormat(), Alignment: Column.DataType.GetTextAlignment(), IsReadOnly: true));
            }

            Result.ItemsSource = fDropDownTable.DefaultView;
        }

        Result.DoubleTapped += DropDownGrid_DoubleTapped;
        Result.AddHandler(InputElement.KeyDownEvent, DropDownGrid_KeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, handledEventsToo: true);
        return Result;
    }
    Control CreateDropDownHost()
    {
        Grid Panel = new()
        {
            Width = DropDownWidth,
            Height = DropDownHeight,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        fDropDownGrid = CreateDropDownGrid();
        Panel.Children.Add(fDropDownGrid);
        return Panel;
    }

    // ● protected methods
    /// <inheritdoc />
    protected override void ToggleDropDown()
    {
        Search(CommitSingleResult: false, SearchTerm: string.Empty);
    }
    /// <inheritdoc />
    protected override object GetDropDownSelectedValue()
    {
        return GetDropDownCurrentRow();
    }

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public GroupGridLocatorInplaceEditor(LocatorDef LocatorDef, string SearchField, DataRowView TargetRowView)
    {
        fLocatorDef = LocatorDef;
        fSearchField = SearchField;
        fTargetRowView = TargetRowView;
        TextBox.TextChanged += (Sender, Args) =>
        {
            if (!fIsSettingText && ContainsSearchTrigger(TextBox.Text))
                Search(CommitSingleResult: true);
        };
    }

    // ● public methods
    /// <inheritdoc />
    public override void Cleanup()
    {
        base.Cleanup();
        ClearDropDownGrid();
    }
    /// <inheritdoc />
    public override void SelectDropDownItem(object Item)
    {
        if (Item is DataRow Row)
            SetSelectedRow(Row);
    }
    /// <inheritdoc />
    public override Control CreateDropDownControl()
    {
        ClearDropDownGrid();
        return CreateDropDownHost();
    }
    /// <inheritdoc />
    public override void DropDownOpened(Control DropDownControl)
    {
        if (fDropDownGrid == null)
            return;

        Ui.Post(() =>
        {
            fDropDownGrid.BestFitColumns();
            if (fDropDownTable != null && fDropDownTable.DefaultView.Count > 0)
            {
                int RowIndex = FindCurrentDropDownRowIndex();
                GroupGridBinder.SelectRow(fDropDownGrid, RowIndex >= 0 ? RowIndex : 0);
            }
            fDropDownGrid.Focus(NavigationMethod.Pointer, KeyModifiers.None);
        });
    }

    // ● properties
    /// <inheritdoc />
    public override object Value
    {
        get => fSelectedRow;
        set
        {
            if (value is DataRow Row)
                SetSelectedRow(Row);
            else
            {
                fSelectedRow = null;
                SetText(Sys.IsNull(value) ? string.Empty : value.ToString());
            }
        }
    }
    /// <inheritdoc />
    public override double DropDownHeight => Ui.Settings.ShowLocatorGridFilterPanel ? 320 : 260;
    /// <inheritdoc />
    public override double DropDownWidth => 560;
}
