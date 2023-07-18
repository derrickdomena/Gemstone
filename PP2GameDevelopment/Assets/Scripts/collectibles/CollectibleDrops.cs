using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drops")] 
public class CollectibleDrops : ScriptableObject
{
    public Collect.CollectibleTypes drops;

    public void Drops()
    {
        switch(drops)
        {
            default:
            case Collect.CollectibleTypes.HealthPack:
                //Do something
                break;

        }
    }
}
