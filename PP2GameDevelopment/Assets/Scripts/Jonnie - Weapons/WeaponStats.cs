using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public int shootDist;
    public float shootRate;
    public int shootDamage;
    public bool auto;

    public int ammoCurr;
    public int ammoRemaining;
    public int ammoMax;

    public GameObject model;
    public ParticleSystem hitEffect;
}
