using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("----- Compnents -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Stats ------")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
