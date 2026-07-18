/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Represents an object that can receive messages from the <see cref="Broadcaster"/>.
/// </summary>
public interface IBroadcasterListener
{
    // ● public
    /// <summary>
    /// Processes a broadcaster message.
    /// </summary>
    /// <param name="Args">The broadcaster event arguments.</param>
    void ProcessBroadcasterMessage(BroadcasterArgs Args);
}
