/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Sql type tokens used as placeholders in constructing RDBMS-neutral CREATE TABLE statements.
/// </summary>
static public class SqlTypeTokens
{
    /// <summary>
    /// Constant
    /// </summary>
    public const string CPRIMARY_KEY = "@PRIMARY_KEY";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CAUTO_INC = "@AUTO_INC";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CVARCHAR = "@VARCHAR";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CNVARCHAR = "@NVARCHAR";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CFLOAT = "@FLOAT";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CDECIMAL = "@DECIMAL";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CDECIMAL_ = "@DECIMAL_";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CDATE = "@DATE";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CDATE_TIME = "@DATE_TIME";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CBOOL = "@BOOL";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CBLOB = "@BLOB";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CBLOB_TEXT = "@BLOB_TEXT";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CNBLOB_TEXT = "@NBLOB_TEXT";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CNOT_NULL = "@NOT_NULL";
    /// <summary>
    /// Constant
    /// </summary>
    public const string CNULL = "@NULL";
}