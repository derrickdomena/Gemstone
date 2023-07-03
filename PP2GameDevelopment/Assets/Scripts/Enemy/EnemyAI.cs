using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;

    [Header("----- Stats -----")]
    [SerializeField] int hp;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        StartCoroutine(FlashDamage());

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator FlashDamage()
    {
        Color orig = model.material.color;
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = orig;
    }
}
