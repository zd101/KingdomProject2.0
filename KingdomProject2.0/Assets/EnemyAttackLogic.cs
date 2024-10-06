using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackLogic : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator anim;

    public GameObject attackPoint; // Reference to the attack point GameObject
    public float radius;
    public LayerMask goodNPCS;
    public float damage;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    void Update()
    {
        if (health < currentHealth)
        {
            currentHealth = health;
            anim.SetTrigger("Attacked");
        }

        if (health <= 0)
        {
            //Debug.Log("Enemy is dead");
        }
    }

    public void EnemyAttack()
    {
        Collider2D[] goodNPCSHit = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, goodNPCS);

        foreach (Collider2D goodNPCSCollider in goodNPCSHit)
        {
            Debug.Log("Hit goodNPCS");

            // Call StartFlashing on all descendants of the enemy
            CallStartFlashingRecursively(goodNPCSCollider.transform);
            goodNPCSCollider.GetComponent<SoldierFollow>().SoldierHealth -=damage;
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
