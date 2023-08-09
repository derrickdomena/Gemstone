using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Command", menuName = "Utilities/DeveloperConsole/Commands/Item Command")]
public class PlayerItemConsoleCommand : ConsoleCommand
{
    [SerializeField] Collect[] items;
    public override bool Process(string[] args)
    {
        if (args.Length != 1) { return false; }

        if (!int.TryParse(args[0], out int value))
        {
            return false;
        }
        Instantiate(items[value], new Vector3(gameManager.instance.player.transform.position.x, gameManager.instance.player.transform.position.y + 1, gameManager.instance.player.transform.position.z), Quaternion.identity);
        return true;
    }
}
