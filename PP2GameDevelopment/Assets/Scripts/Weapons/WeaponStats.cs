using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObject/WeaponStats", order = 2)]
public class WeaponStats : ScriptableObject
{
    [Header("Gun")]
    public int shootDist;
    public float shootRate;
    public int shootDamage;
    public int ammoCurr;
    public int ammoMax;
    public int ammoReserve;
    public int ammoReserveMax;
    public int shootDamageOrig;
    public bool auto;
    public float projectileSpeed;
    

    [Header("Melee")]
    public int attackDamage;
    public int attackDamageOrig;
    public float attackSpeed;
    public int attackDistance;

    [Header("General")]
    public GameObject model;
    public ParticleSystem hitEffect;
    public string weaponType;

}
