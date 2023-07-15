using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("----- Grenade Components -----")]
    [SerializeField] GameObject explosionEffect;
    //[SerializeField] AudioSource audioSource;

    [Header("----- Grenade Stats -----")]
    [SerializeField] float blastDelay;
    [SerializeField] float blastRadius;
    [SerializeField] float blastForce;
    [SerializeField] int blastDamage;

    float countdown;
    bool hasExploded = false;
    

    // Start is called before the first frame update
    void Start()
    {
        countdown = blastDelay;
    }

    // Update is called once per frame
    void Update()
    {
        Countdown();
    }

    void Countdown()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            // Grenade Audio
            //audioSource.PlayDelayed(blastDelay);
            hasExploded = true;
        }
    }

    void Explode()
    {
        // Particle Effect
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Add force to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            // Add force to rigidbody objects
            Rigidbody rb = colliders[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(blastForce, transform.position, blastRadius);
            }

            // Damage objects that inherit IDamage
            IDamage damageable = colliders[i].GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.TakeDamage(blastDamage);
            }
        }

        // Remove object
        Destroy(gameObject);
    }
}
