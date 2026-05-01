namespace Tripous.Data;

/// <summary>
/// Represents a list of <see cref="MemTable"/> instances, detail to a master MemTable.
/// <para>Both the master and the detail MemTable instances must belong to the
/// same DataSet, otherwise an exception is thrown.</para>
/// </summary>
public class DetailList : Collection<MemTable>
{
    // ● private
    private MemTable OwnerTable = null; // the owner table, which becomes the master of any other table added
    private int ActiveCount = 0;
    
    private static void ClearMasterRecursive(MemTable table)
    {
        MemTable[] children = table.GetDetails().ToArray();

        foreach (MemTable child in children)
            ClearMasterRecursive(child);

        table.Master = null;
    }
    private void OwnerTable_CurrentRowChanged(object sender, EventArgs e)
    {
        if (!Active)
            return;
        
        foreach (MemTable DetailTable in this)
            DetailTable.MasterRowChanged();
    }
    private void ValidateRelationSchema(MemTable master, MemTable detail)
    {
        if (detail.MasterFields == null || detail.MasterFields.Length == 0)
            throw new TripousDataException(
                $"[DataLib] {master.TableName} -> {detail.TableName}: MasterFields not defined");

        if (detail.DetailFields == null || detail.DetailFields.Length == 0)
            throw new TripousDataException(
                $"[DataLib] {master.TableName} -> {detail.TableName}: DetailFields not defined");

        if (detail.MasterFields.Length != detail.DetailFields.Length)
            throw new TripousDataException(
                $"[DataLib] {master.TableName} -> {detail.TableName}: MasterFields and DetailFields count mismatch");

        detail.ValidateRelationSchema();
    }

    /// <summary>
    /// Throws an exception if the master and the detail MemTable instances
    /// in the list do not belong to the same DataSet.
    /// </summary>
    private void CheckDatasets()
    {
        foreach (MemTable DetailTable in this)
            CheckDatasets(DetailTable);
    }
    private void CheckDatasets(MemTable DetailTable)
    {
        if (DetailTable == null)
            throw new TripousArgumentNullException(nameof(DetailTable));

        if (OwnerTable == null)
            throw new TripousDataException("OwnerTable is null.");

        if (OwnerTable.DataSet == null)
            throw new TripousDataException("MasterTable Table has no DataSet");

        if (DetailTable.DataSet == null)
            throw new TripousDataException("A DetailTable Table has no DataSet");

        if (DetailTable.DataSet != OwnerTable.DataSet)
            throw new TripousDataException("MasterTable.DataSet != DetailTable.DataSet");
    }

    /// <summary>
    /// Activates the direct master-detail relationship between OwnerTable and DetailTable.
    /// </summary>
    private void ActivateDetail(MemTable DetailTable)
    {
        if (DetailTable == null)
            throw new TripousArgumentNullException(nameof(DetailTable));

        CheckDatasets(DetailTable);
        ValidateRelationSchema(OwnerTable, DetailTable);

        DetailTable.Locale = OwnerTable.Locale;
        DetailTable.CaseSensitive = OwnerTable.CaseSensitive;
        DetailTable.Details.Active = true;
        DetailTable.MasterRowChanged();
    }
    /// <summary>
    /// Deactivates the direct master-detail relationship between OwnerTable and DetailTable.
    /// </summary>
    private void DeactivateDetail(MemTable DetailTable)
    {
        if (DetailTable == null)
            return;

        while (DetailTable.Details.Active)
            DetailTable.Details.Active = false;

        DetailTable.DataView.RowFilter = string.Empty;
    }

    protected override void InsertItem(int index, MemTable DetailTable)
    {
        if (DetailTable == null)
            throw new TripousArgumentNullException(nameof(DetailTable));

        if (this.Contains(DetailTable))
            throw new TripousDataException("Cannot add a detail table twice");

        CheckDatasets(DetailTable);

        base.InsertItem(index, DetailTable);

        DetailTable.Master = OwnerTable;

        if (this.Active)
            ActivateDetail(DetailTable);
    }
    protected override void RemoveItem(int index)
    {
        MemTable DetailTable = this[index];

        if (this.Active)
            DeactivateDetail(DetailTable);
        else
            while (DetailTable.Details.Active)
                DetailTable.Details.Active = false;

        base.RemoveItem(index);

        ClearMasterRecursive(DetailTable);
    }
    protected override void ClearItems()
    {
        while (this.Count > 0)
            RemoveItem(this.Count - 1);
    }

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    internal DetailList(MemTable ownerTable)
    {
        OwnerTable = ownerTable ?? throw new TripousArgumentNullException(nameof(ownerTable));
        OwnerTable.CurrentRowChanged += OwnerTable_CurrentRowChanged;
    }

    // ● public
    public bool Contains(string TableName) => this.FirstOrDefault(x => Sys.IsSameText(TableName, x.TableName)) != null;
    public MemTable Find(string TableName) => this.FirstOrDefault(x => Sys.IsSameText(TableName, x.TableName));
    public MemTable Get(string TableName)
    {
        MemTable Result = this.FirstOrDefault(x => Sys.IsSameText(TableName, x.TableName));
        if (Result == null)
            throw new TripousDataException($"Table not found in {OwnerTable.TableName} details: {TableName}");
        return Result;
    }
    
    // ● properties
    /// <summary>
    /// Activates and de-activates the master-detail relation-ship between
    /// the master MemTable and the details.
    /// <para>WARNING: Tables MUST HAVE already columns created.</para>
    /// </summary>
    public bool Active
    {
        get { return ActiveCount >= 1; }
        set
        {
            if (value)
            {
                ActiveCount++;

                if (ActiveCount == 1)
                {
                    CheckDatasets();

                    foreach (MemTable DetailTable in this)
                        ActivateDetail(DetailTable);
                }
            }
            else
            {
                ActiveCount--;

                if (ActiveCount == 0)
                {
                    foreach (MemTable DetailTable in this)
                        DeactivateDetail(DetailTable);
                }

                if (ActiveCount < 0)
                    ActiveCount = 0;
            }
        }
    }
}