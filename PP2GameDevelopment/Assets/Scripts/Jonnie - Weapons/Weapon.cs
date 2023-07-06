using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour

{
    public static Weapon instance;

    [Header("----- Weapon Component -----")]

    [SerializeField] GameObject weapon;
    [SerializeField] public Transform shootingPOS;

    [Header("----- Weapon Stats -----")]

    [SerializeField] public int ammo;
    [SerializeField] public int magazines;

    [SerializeField] public float shootRate;
    [SerializeField] public int shootDamage;
    [SerializeField] public int shootDistance;


    private int ammoOrig;
    public bool automatic;

    private void Start()
    {
        ammoOrig = ammo;
    }

    void Awake()
    {
        instance = this;
    }

    // // Weapon Script Work
    // Handles Weapon Reload and Magazine Size
    public void ReloadWeapon()
    {
        // Only Reaload's Weapon if ammo is less than init
        // ial ammoCount and not less than zero.
        // Reloads when the reloadKey is press
        if (ammo < ammoOrig)
        {

            if (magazines > 0)
            {
                ammo = ammoOrig;
                magazines--;
            }
        }
    }
    //decrement ammo;
    public void ammoUpdate()
    {
        ammo--;
    }

}
