using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObject/PlayerStats", order = 3)]
public class PlayerStats : ScriptableObject
{
    [field: SerializeField]
    public int NumberOfKills;

    [field: SerializeField]
    public int TotalDamage;
}
