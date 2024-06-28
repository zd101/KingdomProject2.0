using UnityEngine;

public class SoldierAttack : MonoBehaviour
{
  public float attackRange = 1.0f; // The range within which the soldier will attack
    public float attackCooldown = 1.0f; // Cooldown time between attacks
    public GameObject enemy; // The enemy GameObject

    private float nextAttackTime = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enemy != null)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= attackRange && Time.time >= nextAttackTime)
            {
                // Attack the enemy
                Attack();
                nextAttackTime = Time.time + attackCooldown; // Set the next attack time
            }
            else if (distanceToEnemy > attackRange)
            {
                // Stop the attack animation if the enemy is out of range
                animator.SetBool("isAttacking", false);
            }
        }
    }

    void Attack()
    {
        // Trigger the attack animation
        animator.SetBool("isAttacking", true);

        // Optionally, you can add code here to deal damage to the enemy
        // enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
    }
}