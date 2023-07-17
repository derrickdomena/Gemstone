using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour, IDamage
{
    public event EventHandler OnDead;

    [Header("Boss Stats")]
    [SerializeField] int moveSpeed;
    [SerializeField] int hpAmount;
    public int heathUpdate;
    public HealthSystem HealthSystem { get; private set; }
 
    [Header("Attack stuff")]
    [SerializeField] float range;


    [Header("Boss Phase stuff")]
    public Texture m_MainTexture, PhaseTwoTexture, PhaseThreeTexture;
    Renderer m_Renderer;
    public GameObject bossPrefab;
    private Material Material;



    private void Awake()
    {
        HealthSystem = new HealthSystem(hpAmount);
    }
    //private void Setup(HealthSystem healthSystem)
    //{
    //    this.HealthSystem = healthSystem;
    //    healthSystem.OnDead += HealthSystem_OnDead;
    //}
    private void Start()
    {
        m_Renderer = bossPrefab.GetComponent<Renderer>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PhaseOne()
    {

    }
    public void PhaseTwo()
    {
        m_Renderer.material.SetTexture("_MainTex", PhaseTwoTexture);
        
    }
    public void PhaseThree()
    {
        m_Renderer.material.SetTexture("_MainTex", PhaseThreeTexture);
        
    }
    public HealthSystem GetHealthSystem()
    {
        return HealthSystem;
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        //It died destroy it
        if(OnDead != null) OnDead(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        HealthSystem.Damage(amount);
    }
}
