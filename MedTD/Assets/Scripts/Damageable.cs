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

    protected virtual void Die()
    {
        //Debug.Log("Damageable.Die");
        Destroy(gameObject);
    }
}
