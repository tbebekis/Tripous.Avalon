/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

static public class DbConfig
{
        
    /// <summary>
    /// When is set indicates that the Oids are Guid strings.  
    /// <para>Defaults to true.</para>
    /// </summary>
    static public bool GuidOids { get; set; } = true;
    /// <summary>
    /// Gets the variables prefix in Sql statements. Defaults to :@, e.g. :@Today
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
    /// ReadOnly. Returns the value of the CompanyId as a string for constructing Sql statements.
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
    /// ReadOnly. Returns the value of the CompanyId
    /// </summary>
    static public object CompanyIdValue
    {
        get
        {
            if (CompanyId == null)
            { 
                return GuidOids? (object)Sys.StandardCompanyGuid: -1;
            }
            else
            {
                return CompanyId;
            }
        }
    }
    
    /// <summary>
    /// Gets the default SimpleType data type for Id fields, based on the GuidOids setting in the Variables
    /// </summary>
    static public DataFieldType OidDataType => GuidOids ? DataFieldType.String : DataFieldType.Integer;
    /// <summary>
    /// Gets the size of a field for  the default SimpleType data type for Id fields
    /// </summary>
    static public int OidSize => OidDataType == DataFieldType.String ? 40 : 0; 
    
    /// <summary>
    /// The name of the default database connection
    /// </summary>
    static public string DefaultConnectionName { get; set; } = Sys.DEFAULT;

    static public string CodeProviderModuleName { get; set; } = "NumberSeries";
    
    static public string SysDbIniTableName { get; set; } = "SYS_INI";
    static public string SysLogTableName { get; set; } = "SYS_LOG";
    static public string SysNumberSeriesTableName { get; set; } = "SYS_NUMBER_SERIES";
    static public string SysConfigTableName { get; set; } = "SYS_CONFIG";
    static public string SysAppUserTableName { get; set; } = "SYS_APP_USER";

    static public string SysDbIniEntryField { get; set; }  = "EntryKey";
    static public string SysDbIniValueField { get; set; }  = "EntryValue";
    static public string SysDbIniBlobField { get; set; }  = "EntryData";
}