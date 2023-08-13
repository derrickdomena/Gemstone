using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeStats", menuName = "ScriptableObject/MeleeStats", order = 2)]
public class MeleeStats : ScriptableObject
{
    public float attackDistance;
    public float attackDistanceOrig;
    public float attackDelay;
    public float attackDelayOrig;
    public float attackSpeed;
    public float attackSpeedOrig;
    public int attackDamage;
    public int attackDamageOrig;

    public GameObject model;
    public GameObject hitEffect;
}
