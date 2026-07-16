/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Returns schema metadata for a database connection.
/// </summary>
[AjaxOperation("App.DatabaseWorkbench.GetSchema")]
public class DatabaseWorkbenchGetSchema: AppAjaxOperation
{
    // ● private
    static object ColumnPacket(DbMetaColumn Column) => new
    {
        Name = Column.Name,
        DisplayText = Column.DisplayText
    };
    static object IndexPacket(DbMetaIndex Index) => new
    {
        Name = Index.Name,
        DisplayText = Index.DisplayText
    };
    static object ConstraintPacket(DbMetaConstraint Constraint) => new
    {
        Name = Constraint.Name,
        DisplayText = Constraint.DisplayText
    };
    static object TriggerPacket(DbMetaTrigger Trigger) => new
    {
        Name = Trigger.Name,
        DisplayText = Trigger.DisplayText
    };
    static object TablePacket(DbMetaTable Table) => new
    {
        Table.Name,
        Columns = Table.Columns.Select(ColumnPacket).ToArray(),
        Indexes = Table.Indexes.Select(IndexPacket).ToArray(),
        Constraints = Table.Constraints.Select(ConstraintPacket).ToArray(),
        Triggers = Table.Triggers.Select(TriggerPacket).ToArray(),
        SourceCode = Table.GetCreateTable(),
        FieldList = Table.GetFieldNameList(),
        SelectSql = $"select * from {Table.Name}"
    };
    static object ViewPacket(DbMetaView View) => new
    {
        View.Name,
        Columns = View.Columns.Select(ColumnPacket).ToArray(),
        SourceCode = View.SourceCode,
        FieldList = View.GetFieldNameList(),
        SelectSql = $"select * from {View.Name}"
    };

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            Sys.Throw("Access denied.");

        string ConnectionName = GetStringParam(Request, "ConnectionName");
        if (string.IsNullOrWhiteSpace(ConnectionName))
            Sys.Throw("No connection specified.");

        DbConnectionInfo ConnectionInfo = Db.Connections.Get(ConnectionName);
        DbSchema Schema = ConnectionInfo.Schema;
        if (!Schema.IsLoaded)
            Schema.Load();

        AjaxResponse Result = new(Request.OperationName);
        Result["Schema"] = new
        {
            Schema.Name,
            ConnectionName = ConnectionInfo.Name,
            Tables = Schema.Tables.Select(TablePacket).ToArray(),
            Views = Schema.Views.Select(ViewPacket).ToArray()
        };
        return Result;
    }
}
