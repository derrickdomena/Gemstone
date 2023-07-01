using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("----- Weapon Component -----")]


    [Header("----- Weapon Stats -----")]

    [SerializeField] int ammo;
    [SerializeField] int magazines;

    [SerializeField] float shootRate;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;

    [SerializeField] Transform shootingPOS;
    [SerializeField] GameObject bullet;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
