using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour, IDamageable
{
    public int health = 100; // Health of the enemy

    // Method to handle taking damage
    public virtual void takeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Health remaining: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    // Method to handle death
    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject); // Destroy the enemy object
    }
}
