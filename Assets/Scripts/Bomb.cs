using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionRadius = 5f; // Radius of the explosion
    public int damage = 50; // Damage dealt by the bomb
    public float countdown = 2f; // Time until explosion
    public GameObject explosionEffect;

    void Start()
    {
        // Start the countdown to explosion
        Invoke("Explode", countdown);
        explosionEffect = Resources.Load<GameObject>("Prefabs/Explosion");
    }

    void Explode()
    {
        // Find all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);
        foreach (Collider hit in colliders)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Call the TakeDamage method
                damageable.takeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the player's Rigidbody component
            Debug.Log("Killing momentum");
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Kill the player's momentum by setting velocity to zero
                Debug.Log("Found RB");
                playerRigidbody.velocity = Vector3.zero;
                playerRigidbody.angularVelocity = Vector3.zero; // Optional: Stop rotational momentum
            }
        }
    }

    // Optional: Draw the explosion radius in the editor for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}


