using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Immunity Command", menuName = "Utilities/DeveloperConsole/Commands/Immunity Command")]
public class PlayerImmunityConsoleCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (args.Length != 1) { return false; }

        if (!bool.TryParse(args[0], out bool value))
        {
            return false;
        }
        gameManager.instance.playerScript.Immune(value);
        return true;
    }
}
