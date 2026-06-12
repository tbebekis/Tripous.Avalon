/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Provides global database configuration settings used by the data layer.
/// </summary>
static public class DbConfig
{
        
    /// <summary>
    /// When is set indicates that the Oids are Guid strings.  
    /// <para>Defaults to true.</para>
    /// </summary>
    static public bool GuidOids { get; set; } = true;
    /// <summary>
    /// Gets the variables prefix used in SQL statements.
    /// <para>Defaults to :@, e.g. :@Today</para>
    /// </summary>
    static public string VariablesPrefix { get; set; } = ":@";
    /// <summary>
    /// The field name of the company field, used in various tables. 
    /// <para>Defaults to CompanyId</para>
    /// </summary>
    static public string CompanyFieldName { get; set; } = "CompanyId";
    /// <summary>
    /// The Id of the current company, if any, else null.
    /// </summary>
    static public object CompanyId { get; set; } = Sys.StandardCompanyGuid;
    /// <summary>
    /// ReadOnly. Returns the value of the CompanyId as a string for constructing SQL statements.
    /// </summary>
    static public string CompanyIdSql
    {
        get
        {

            if (CompanyId == null)
            {
                if (GuidOids)
                    return Sys.StandardCompanyGuid.QS();
                return "-1";
            }

            Type T = CompanyId.GetType();

            if ((T == typeof(System.String)) || (T == typeof(System.Guid)))
                return CompanyId.ToString().QS();
            else
                return CompanyId.ToString();
        }

    }
    /// <summary>
    /// ReadOnly. Returns the effective value of the CompanyId.
    /// </summary>
    static public object CompanyIdValue
    {
        get
        {
            if (CompanyId == null)
            { 
                return GuidOids ? (object)Sys.StandardCompanyGuid : -1;
            }
            else
            {
                return CompanyId;
            }
        }
    }
    /// <summary>
    /// Gets the default data type used for object identifiers.
    /// </summary>
    static public DataFieldType OidDataType => GuidOids ? DataFieldType.String : DataFieldType.Integer;
    /// <summary>
    /// Gets the default field size used for object identifiers.
    /// </summary>
    static public int OidSize => OidDataType == DataFieldType.String ? 40 : 0; 
    /// <summary>
    /// The name of the default database connection.
    /// </summary>
    static public string DefaultConnectionName { get; set; } = Sys.DEFAULT;
    /// <summary>
    /// The module name used by code providers.
    /// </summary>
    static public string CodeProviderModuleName { get; set; } = "NumberSeries";
    /// <summary>
    /// Gets or sets the system database initialization table name.
    /// </summary>
    static public string SysDbIniTableName { get; set; } = "SYS_INI";
    /// <summary>
    /// Gets or sets the system log table name.
    /// </summary>
    static public string SysLogTableName { get; set; } = "SYS_LOG";
    /// <summary>
    /// Gets or sets the system number series table name.
    /// </summary>
    static public string SysNumberSeriesTableName { get; set; } = "SYS_NUMBER_SERIES";
    /// <summary>
    /// Gets or sets the system configuration table name.
    /// </summary>
    static public string SysConfigTableName { get; set; } = "SYS_CONFIG";
    /// <summary>
    /// Gets or sets the application users table name.
    /// </summary>
    static public string SysAppUserTableName { get; set; } = "SYS_APP_USER";
    /// <summary>
    /// Gets or sets the key field name of the SYS_INI table.
    /// </summary>
    static public string SysDbIniEntryField { get; set; }  = "EntryKey";
    /// <summary>
    /// Gets or sets the value field name of the SYS_INI table.
    /// </summary>
    static public string SysDbIniValueField { get; set; }  = "EntryValue";
    /// <summary>
    /// Gets or sets the blob value field name of the SYS_INI table.
    /// </summary>
    static public string SysDbIniBlobField { get; set; }  = "EntryData";
}