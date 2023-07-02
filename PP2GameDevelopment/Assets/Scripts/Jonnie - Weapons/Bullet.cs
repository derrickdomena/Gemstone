using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    [Header("----- Bullet Stats -----")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    void Start()
    {
        // Destroy object after so much time
        Destroy(gameObject, destroyTime);

        // Set Velocity
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Get Object to Damage
        IDamage damagable = other.GetComponent<IDamage>();


        // Damage target if Damagable
        if (damagable != null)
        {
            damagable.TakeDamage(damage);
            // Destroy bullet
            Destroy(gameObject);
        }

    }
}

