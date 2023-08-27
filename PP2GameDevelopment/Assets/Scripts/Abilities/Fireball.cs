using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("----- Fireball Components -----")]
    [SerializeField] GameObject explosionEffect;
    
    [Header("----- Fireball Stats -----")]
    [SerializeField] float blastRadius;
    [SerializeField] float blastForce;
    [SerializeField] int blastDamage;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    void Explode()
    {
        // Particle Effect
        Instantiate(explosionEffect, transform.position, transform.rotation);

        audioManager.PlaySFXAbilities(audioManager.explosionSound);

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
            if (damageable != null && !colliders[i].CompareTag("Player"))
            {
                damageable.TakeDamage(blastDamage);
                gameManager.instance.playerStats.TotalDamage += blastDamage;
            }
        }

        // Remove object
        Destroy(gameObject);
    }
}
