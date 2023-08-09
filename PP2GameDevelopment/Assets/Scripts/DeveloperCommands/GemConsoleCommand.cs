using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gem Command", menuName = "Utilities/DeveloperConsole/Commands/Gem Command")]
public class GemConsoleCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if(args.Length != 1) { return false; }

        if (!float.TryParse(args[0], out float value))
        {
            return false;
        }
        gameManager.instance.updateGemCount((int)value); 
        return true;
    }
}
