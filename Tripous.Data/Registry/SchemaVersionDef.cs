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
/// Represents the baseline schema version definition and component registration entry.
/// </summary>
public class SchemaVersionDef : BaseDef
{
    // ● protected methods
    /// <summary>
    /// Executes internal structural schema scripts and entity updates configuration adjustments during assembly registration.
    /// </summary>
    protected virtual void RegisterInternal()
    {
 
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance of the SchemaVersionDef class.
    /// </summary>
    public SchemaVersionDef()
    {
    }

    // ● public methods
    /// <summary>
    /// Validates system preconditions and commits structural registration descriptors directly to target tracking schema version layers.
    /// </summary>
    public void Register()
    {
        if (!IsRegistered)
        {
            if (string.IsNullOrWhiteSpace(Domain))
                throw new TripousDataException($"A {nameof(SchemaVersionDef)} must have a domain name such as {Sys.APPLICATION}");
            
            if (string.IsNullOrWhiteSpace(ConnectionName))
                throw new TripousDataException($"A {nameof(SchemaVersionDef)} must have a valid connection name");
            
            if (!Db.Connections.Contains(ConnectionName))
                throw new TripousDataException($"Connection Name {ConnectionName} not found for a {nameof(SchemaVersionDef)} schema");
            
            if (VersionNumber <= 0)
                throw new TripousDataException($"Version Number {VersionNumber} is not valid for a {nameof(SchemaVersionDef)} schema");
            
            Schema = Schemas.FindOrAdd(Domain, ConnectionName);
            Version = Schema.FindOrAdd(VersionNumber);

            RegisterInternal();
            
            IsRegistered = true;
        }
    }

    // ● properties
    /// <summary>
    /// Gets a value indicating whether this version configuration has been processed and integrated.
    /// </summary>
    public bool IsRegistered { get; private set; }
    /// <summary>
    /// Gets the structural schema descriptor instance mapping the target domain environment.
    /// </summary>
    public Schema Schema { get; private set; }
    /// <summary>
    /// Gets the specific version release context container belonging to this database definition tracking line.
    /// </summary>
    public SchemaVersion Version { get; private set; }
    /// <summary>
    /// Gets the global subsystem category or application group context identifier.
    /// </summary>
    public virtual string Domain { get; } = Sys.APPLICATION;
    /// <summary>
    /// Gets the unique targeting backend key moniker identifier layout used for data source connection definitions lookup.
    /// </summary>
    public virtual string ConnectionName { get; } = DbConfig.DefaultConnectionName;
    /// <summary>
    /// Gets the sequence tracker index key numbering layout signifying database state migrations levels.
    /// </summary>
    public virtual int VersionNumber { get; } = -1;
    /// <summary>
    /// Gets a value indicating whether the definition schema layout can be automatically serialized into files.
    /// </summary>
    [JsonIgnore] public override bool IsSerializable => false;
}