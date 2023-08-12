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
            case 3:
                drops = Collect.CollectibleTypes.CooldownE;
                break;
            case 4:
                drops = Collect.CollectibleTypes.CooldownQ;
                break;
            case 5:
                drops = Collect.CollectibleTypes.MaxHPUp;
                break;
            case 6:
                drops = Collect.CollectibleTypes.MaxSpeedUp;
                break;
            case 7:
                drops = Collect.CollectibleTypes.CritUp;
                break;
            case 8:
                drops = Collect.CollectibleTypes.DashUp;
                break;
        }
        return drops;
    }
}
