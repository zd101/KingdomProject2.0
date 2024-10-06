using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorArrow : MonoBehaviour
{
    public float lifetime = 5f; // Time before the arrow disappears if it doesn't hit anything

    void Start()
    {
        // Destroy the arrow after 'lifetime' seconds if it doesn't hit anything
        StartCoroutine(DestroyAfterDelay(lifetime));
    }

    // Detect when the arrow collides with an object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object collided with has the "Enemy" or "Obstacles" tag
        if (collision.gameObject.CompareTag("Enemy") || 
            collision.gameObject.CompareTag("Obstacles"))
        {
            // Destroy the arrow on impact
            Destroy(gameObject);
            
            // Optionally, you can also reduce the target's health here, if applicable
            // Example: collision.gameObject.GetComponent<TroopHealth>().TakeDamage(damage);
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}