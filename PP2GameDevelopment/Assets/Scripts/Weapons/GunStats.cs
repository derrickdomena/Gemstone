using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunStats", menuName = "ScriptableObject/GunStats", order = 2)]
public class GunStats : ScriptableObject
{
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
    public GameObject model;
    public ParticleSystem hitEffect;

    AudioSource shoot;
}
