using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float startHealth;

    protected float health;

    protected void Start()
    {
        //Debug.Log("Damageable.Start");

        health = startHealth;
    }

    internal void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
        else
        {
            HealthBar healthBar = GetComponent<HealthBar>();
            if (healthBar != null)
            {
                if (startHealth <= 0f) startHealth = health;
                if (health > startHealth) health = startHealth;
                healthBar.UpdateGreenPercentage(health, startHealth);
            }
        }
    }

    internal void RegenerateHealth(float addedHealth)
    {
        if (health >= startHealth) return;

        health += addedHealth;
        if (health > startHealth) health = startHealth;

        HealthBar healthBar = GetComponent<HealthBar>();
        if (healthBar != null)
        {
            //if (startHealth <= 0f) startHealth = health;
            //if (health > startHealth) health = startHealth;
            healthBar.UpdateGreenPercentage(health, startHealth);
        }
        //Debug.Log("health = " + health);
        //Debug.Log("\n");
    }

    protected virtual void Die()
    {
        //Debug.Log("Damageable.Die");
        Destroy(gameObject);
    }

    public void RemoveHealthBar()
    {
        //Debug.Log("Damageable.RemoveHealthBar");
        HealthBar healthBar = GetComponent<HealthBar>();
        if (healthBar != null)
        {
            healthBar.DestroyHealthBar();
        }
    }
}
