using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour

{
    public static Weapon instance;

    [Header("----- Weapon Component -----")]

    [SerializeField] GameObject weapon;
    [SerializeField] public Transform shootingPOS;
    [SerializeField] public GameObject bullet;

    [Header("----- Weapon Stats -----")]

    [SerializeField] public int ammo;
    [SerializeField] public int magazines;

    [SerializeField] public float shootRate;
    [SerializeField] public int shootDamage;
    [SerializeField] public int shootDistance;


    public bool automatic;

    void Awake()
    {
        instance = this;
    }
    

}
