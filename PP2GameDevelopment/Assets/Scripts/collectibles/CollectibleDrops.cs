using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drops")] 
public class CollectibleDrops : ScriptableObject
{
    public Collect.CollectibleTypes drops;
    public int value;
    public string prefabName;
    public int numberOfPrefabsToCreate;
    
    public Collect.CollectibleTypes Drop(int select)
    {
        switch (select)
        {
            default:
            case 0:
                drops = Collect.CollectibleTypes.Ammo;
                break;
            case 1:
                drops = Collect.CollectibleTypes.Gem;
                break;
            case 2:
                drops = Collect.CollectibleTypes.HealthPack;
                break;
        }
        return drops;
    }
}
