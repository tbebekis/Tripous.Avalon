/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Represents the comprehensive database schema container, managing structural metadata collection.
/// </summary>
public class DbSchema
{
    // ● constructors
    /// <summary>
    /// Initializes a new instance of the DbSchema class with specified connection info parameters.
    /// </summary>
    public DbSchema(DbConnectionInfo ConnectionInfo)
    {
        this.ConnectionInfo = ConnectionInfo;
    }

    // ● public methods
    /// <summary>
    /// Loads all metadata information from the data provider source if not already loaded.
    /// </summary>
    public void Load()
    {
        if (IsLoaded)
            return;

        DbSchemaLoader.Load(this);

        IsLoaded = true;
    }
    /// <summary>
    /// Clears and unloads all cached metadata definitions from the schema collections.
    /// </summary>
    public void UnLoad()
    {
        DbSchemaLoader.UnLoad(this);
        IsLoaded = false;
    }
    /// <summary>
    /// Refreshes and reloads the structural database metadata from the server source.
    /// </summary>
    public void ReLoad()
    {
        DbSchemaLoader.ReLoad(this);
    }

    // ● properties
    /// <summary>
    /// Gets a value indicating whether the metadata structures have been successfully loaded.
    /// </summary>
    public bool IsLoaded { get; private set; }
    /// <summary>
    /// Gets the unique identifier name of the connection configuration.
    /// </summary>
    public string Name => ConnectionInfo.Name;
    /// <summary>
    /// Gets the database management system platform type classification.
    /// </summary>
    public DbServerType DbServerType => ConnectionInfo.DbServerType;
    /// <summary>
    /// Gets the collection of schema table structures defined within the database.
    /// </summary>
    public List<DbMetaTable> Tables { get; } = new();
    /// <summary>
    /// Gets the collection of relational projection view definitions within the database.
    /// </summary>
    public List<DbMetaView> Views { get; } = new();
    /// <summary>
    /// Gets the collection of programmable stored modules or routines within the database.
    /// </summary>
    public List<DbMetaProcedure> Procedures { get; } = new();
    /// <summary>
    /// Gets the collection of sequence values or unique identifier generator definitions within the database.
    /// </summary>
    public List<DbMetaSequence> Sequences { get; } = new();
    /// <summary>
    /// Gets the underlying connection configuration attributes used for data schema discovery.
    /// </summary>
    public DbConnectionInfo ConnectionInfo { get; }
}