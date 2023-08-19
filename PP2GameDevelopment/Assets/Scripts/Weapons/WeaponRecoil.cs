using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] float recoilIntensity;
    // Gives the visual effect of the weapon having recoil.
    public void Recoil()
    {
        transform.localPosition -= Vector3.forward * recoilIntensity;
    }
}
