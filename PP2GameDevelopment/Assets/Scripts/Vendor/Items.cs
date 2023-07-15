using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    public enum ItemType
    {
        Damage,
        Speed, 
        Health
    }

    public static int GetCost(ItemType type)
    {
        switch (type)
        {
            default:
            case ItemType.Damage:       return 50;
            case ItemType.Speed:        return 30;
            case ItemType.Health:       return 50;
        }
    }

    public static int GetUpgradeValues(ItemType type)
    {
        switch (type)
        {
            default:
            case ItemType.Speed:       return 10;
            case ItemType.Health:      return 10;
            case ItemType.Damage:      return 2;
        }
    }
}
