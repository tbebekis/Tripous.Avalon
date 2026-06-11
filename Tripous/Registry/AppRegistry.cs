/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Provides global access to application commands.
/// </summary>
static public class AppRegistry
{
    // ● static public methods
    /// <summary>
    /// Returns all registered commands, including child commands.
    /// </summary>
    static public List<Command> GetCommandsAll()
    {
        List<Command> Result = new();

        void AddCommand(Command Cmd)
        {
            if (!Result.Contains(Cmd))
                Result.Add(Cmd);

            if (Cmd.HasChildren)
            {
                foreach (Command cmdChild in Cmd.Commands)
                    AddCommand(cmdChild);
            }
        }
        
        foreach (Command Cmd in ToolBarCommands)
            AddCommand(Cmd);
        
        foreach (Command Cmd in MenuCommands)
            AddCommand(Cmd);
        
        return Result;
    }
    /// <summary>
    /// Returns true when a command with the specified name exists.
    /// </summary>
    static public bool CommandExists(string CommandName) => FindCommand(CommandName) != null;
    /// <summary>
    /// Finds and returns a command by name, if any; otherwise returns null.
    /// </summary>
    static public Command FindCommand(string CommandName) => GetCommandsAll().Find(c => c.Name == CommandName);
    /// <summary>
    /// Returns a command by name.
    /// Throws an exception when the command is not found.
    /// </summary>
    static public Command GetCommand(string CommandName)
    {
        Command Result = GetCommandsAll().Find(c => c.Name == CommandName);
        if (Result == null)
            throw new TripousException($"Command {CommandName} not found");
        return Result;
    }
     
    // ● properties 
    /// <summary>
    /// Gets the registered menu commands.
    /// </summary>
    static public DefList<Command> MenuCommands { get; } = new();
    /// <summary>
    /// Gets the registered toolbar commands.
    /// </summary>
    static public DefList<Command> ToolBarCommands { get; } = new();
}