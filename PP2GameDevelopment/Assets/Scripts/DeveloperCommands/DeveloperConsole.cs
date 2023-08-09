using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeveloperConsole
{
    private readonly string prefix;
    private readonly IEnumerable<IConsoleCommand> commands;
    public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
    {
        this.prefix = prefix;
        this.commands = commands;
    }

    //takes in the entire input value  "/givePlayer blah blah"
    public void ProcessInput(string inputValue)
    {
        //if the inputValue doesnt start with the prefix dont process the command
        if(!inputValue.StartsWith(prefix)) { return; }
        
        //gets the rest of the value that isnt the prefix
        inputValue = inputValue.Remove(0, prefix.Length);

        //gets the command word and the arguments
        string[] inputSplit = inputValue.Split(' ');

        //index 0 should always be the command word
        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        ProcessCommands(commandInput, args); 
    }

    public void ProcessCommands(string commandInput, string[] args)
    {
        //goes through all commands that exist
        foreach (var command in commands)
        {
            //if the command doesnt match the one we are checking.. continue
            if(!commandInput.Equals(command.CommandWord, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            //if the command does match.. Process the command
            if(command.Process(args))
            {
                return;
            }
        }
    }
}
