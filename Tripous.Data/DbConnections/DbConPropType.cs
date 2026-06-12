/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Defines the logical connection string property types used by connection adapters.
/// </summary>
public enum DbConPropType
{
    /// <summary>
    /// The database server name, host or address.
    /// </summary>
    Server,
    /// <summary>
    /// The database server port.
    /// </summary>
    Port,
    /// <summary>
    /// The database name, service name or file path.
    /// </summary>
    Database,
    /// <summary>
    /// The database user name.
    /// </summary>
    UserId,
    /// <summary>
    /// The database user password.
    /// </summary>
    Password,
    /// <summary>
    /// Indicates whether integrated security is used.
    /// </summary>
    IntegratedSecurity,
    /// <summary>
    /// Indicates whether the server certificate is trusted.
    /// </summary>
    TrustServerCertificate,
    /// <summary>
    /// The SSL mode used by the connection.
    /// </summary>
    SslMode,
    /// <summary>
    /// The character set used by the connection.
    /// </summary>
    Charset
}