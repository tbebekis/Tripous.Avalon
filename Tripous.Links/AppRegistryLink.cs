/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// A local or remote registry
/// </summary>
public abstract class AppRegistryLink
{
    // ●  construction 
    public AppRegistryLink()
    {
    }
    
    // ● public
    public abstract Task Initialize();
    
 
    // ● properties
    public virtual bool IsInitialized { get; protected set; }
    
    public abstract ReadOnlyDefList<ICommandExecutor> CommandExecutors { get; }
    public abstract ReadOnlyDefList<CommandDef> MenuCommands { get; }  
    public abstract ReadOnlyDefList<CommandDef> ToolBarCommands { get; }  
}