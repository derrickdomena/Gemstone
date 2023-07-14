using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible 
{
    //all of these are interactions with the player in the world space
    //so we can just use 1 interface class
    //collectibles
    void GiveGem(int amount);
    //ammo
  //  void GiveAmmo(int amount1);
    //hp
    //void GiveHP(int amount2);
}
