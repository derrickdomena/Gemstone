using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public int shootDist;
    public float shootRate;
    public int shootDamage;
    public int ammoCurr;
    public int ammoMax;
    public int ammoReserve;
    public int ammoReserveMax;


    public bool auto;

    public GameObject model;
    public ParticleSystem hitEffect;

    AudioSource shoot;

}
