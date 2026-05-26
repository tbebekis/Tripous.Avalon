/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

static public class AppRegistry
{
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
    static public bool CommandExists(string CommandName) => FindCommand(CommandName) != null;
    static public Command FindCommand(string CommandName) => GetCommandsAll().Find(c => c.Name == CommandName);
    static public Command GetCommand(string CommandName)
    {
        Command Result = GetCommandsAll().Find(c => c.Name == CommandName);
        if (Result == null)
            throw new TripousException($"Command {CommandName} not found");
        return Result;
    }
     
    
    // ●  properties 
    static public DefList<Command> MenuCommands { get; } = new();
    static public DefList<Command> ToolBarCommands { get; } = new();
}