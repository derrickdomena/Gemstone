using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible 
{
    //all of these are interactions with the player in the world space
    //so we can just use 1 interface class
    //collectibles
    void GiveGem(int amount);
    //Ammo
    void GiveAmmo(int amount1);
    //HP
    void GiveHP(int amount2);
    //E Cooldown
    public void ReduceECooldown(float amount);
    //Q Cooldown
    public void ReduceQCooldown(float amount);
    //Addition to Max HP
    public void MaxHPUp(float increase, float heal);
    //Adding & Increasing crit chance
    public void CritChanceUp(int chance, float dam);
    //Adding dash uses and distance
    public void DashUp(int uses, float time);



}
