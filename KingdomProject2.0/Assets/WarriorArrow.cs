using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorArrow : MonoBehaviour
{
    public float lifetime = 0.1f; // Time before the arrow disappears if it doesn't hit anything
    public Transform attackPoint; // The point from which the attack happens
    public float radius = 1f; // Radius around the attackPoint to detect enemies
    public LayerMask enemies; // Layer to detect enemies
    public int damage = 2; // Amount of damage to deal

    void Start()
    {
        // Destroy the arrow after a set time if it doesn't hit anything
        StartCoroutine(DestroyAfterDelay(1f));
    }

    // Detect when the arrow collides with an object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the object has the "Enemy" tag
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Call the attacking function when colliding with an enemy
            Attacking();
            Destroy(gameObject); // Destroy the arrow after hitting an enemy
        }
        
        // If the object has the "Obstacles" tag
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            // Destroy the arrow on impact with obstacles
            Destroy(gameObject);
        }
    }

    // Attacking function that handles enemy damage and effects
    public void Attacking()
    {
        // Find all enemies within the attack radius
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, radius, enemies);

        foreach (Collider2D enemyCollider in enemiesHit)
        {
            Debug.Log("Hit enemy");

            // Apply damage to the enemy
            EnemyAttack enemyAttack = enemyCollider.GetComponent<EnemyAttack>();
            if (enemyAttack != null)
            {
                enemyAttack.EnemyHealth -= damage;
                enemyAttack.Hit.Play(); // Play hit sound/effect
            }

            // Optionally, trigger any visual feedback, such as flashing
            CallStartFlashingRecursively(enemyCollider.transform);
        }
    }

    // Recursively call StartFlashing on all descendants of the enemy
    private void CallStartFlashingRecursively(Transform parent)
    {
        DamageScript damageScript = parent.GetComponent<DamageScript>();
        if (damageScript != null)
        {
            damageScript.StartFlashing();
        }

        foreach (Transform child in parent)
        {
            CallStartFlashingRecursively(child); // Recursively call for all children
        }
    }

    // Coroutine to destroy the arrow after a delay
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // Optional: Draw the attack range in the editor for visualizing the attack area
    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
}
