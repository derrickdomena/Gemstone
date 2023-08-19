using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

//[CreateAssetMenu(menuName = "ShopEffects")]
public class ShopEffects : ShopCollectibles
{
    public string itemName;
    public int cost;
    [SerializeField] public float amount1;
    [SerializeField] public float amount2;
    public Collect.CollectibleTypes collectibleTypes;
    CollectibleDrops collectibleDrops;

    public override void Apply()
    {
        
    }
}
