using System;
using System.Diagnostics;
using UnityEngine;

public class HealthSystem 
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;
    public int health;
    private int healthMax;
    public HealthSystem(int healthMax)
    {
        this.healthMax = healthMax;
        health = healthMax;
    }

    public int Gethealth()
    {
        return health;
    }
    public float GetHealthPercent()
    {
        return (float)health/ healthMax;
    }
    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        gameManager.instance.BossHPBar.fillAmount = GetHealthPercent();
        if (health < 0)
        {
            health = 0;
        }
        if (OnDamaged != null)
        {
            OnDamaged(this, EventArgs.Empty);
        }
        if(health <= 0)
        {
            Die(); 
        }
    }
    public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > healthMax) health = healthMax;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }
    public void Die()
    {
        if(OnDead != null) OnDead(this, EventArgs.Empty);
    }
}
