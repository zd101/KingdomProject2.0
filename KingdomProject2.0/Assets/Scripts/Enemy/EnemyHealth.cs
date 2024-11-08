using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //public float health;
    //private float currentHealth;
    private Animator anim;

    public GameObject attackPoint; // Reference to the attack point GameObject
    public float radius;
    public LayerMask enemies;
    public float damage;
    public AudioSource woodWall;

    void Start()
    {
        anim = GetComponent<Animator>();
       // currentHealth = health;
    }

    void Update()
    {
        /*
        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
        }

        if (health <= 0)
        {
            //Debug.Log("Enemy is dead");
        }
        */
    }

    public void Attacking()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach (Collider2D enemyCollider in enemiesHit)
        {
            Debug.Log("Hit enemy");

            // Call StartFlashing on all descendants of the enemy
            CallStartFlashingRecursively(enemyCollider.transform);
            enemyCollider.GetComponent<EnemyAttack>().EnemyHealth -=damage;
            enemyCollider.GetComponent<EnemyAttack>().Hit.Play();
        }
    }

    private void CallStartFlashingRecursively(Transform parent)
    {
        // Check if the current transform has a DamageScript and call StartFlashing
        DamageScript damageScript = parent.GetComponent<DamageScript>();
        if (damageScript != null)
        {
            damageScript.StartFlashing();
        }

        // Recursively call this method on all children
        foreach (Transform child in parent)
        {
            CallStartFlashingRecursively(child);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
}