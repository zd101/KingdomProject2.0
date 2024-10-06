using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
  public float speed = 5f; // Speed at which the GameObject will move towards the target
    public float stopRange = 1f; // Adjustable range within which the soldier will stop when following the target
    public float enemyStopRange = 0f; // Adjustable range within which the soldier will stop when following the enemy (set to 0 to go all the way)
    public float moveTowardsRange = 10f; // Adjustable range within which the soldier will start moving towards the enemy
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = false; // Indicates whether the soldier is facing right
    private float moveInput; // Stores the horizontal movement input
    public float EnemyHealth;
    public ParticleSystem deathBurst;
    private List<Transform> playerTroopsTargets = new List<Transform>(); // List of player troops' targets
    public AudioSource Hit;
    public AudioSource Death;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = 0f; // Start with speed = 0
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyHealth <= 0)
        {
            Debug.Log("Enemy is dead");
            //Death.Play();
            gameObject.SetActive(false);
            deathBurst.transform.position = transform.position;
            //deathBurst.Play();
            return; // Exit update if enemy is dead
        }

        // Find all GameObjects with the tag "PlayerTroops"
        playerTroopsTargets.Clear();
        GameObject[] playerTroops = GameObject.FindGameObjectsWithTag("PlayerTroops");
        foreach (GameObject troop in playerTroops)
        {
            playerTroopsTargets.Add(troop.transform);
        }

        // Move towards the closest player troop if any are found
        Transform closestTarget = FindClosestTarget();
        if (closestTarget != null && gameObject.layer != LayerMask.NameToLayer("Obstacles"))
        {
            float distanceToTarget = Vector2.Distance(transform.position, closestTarget.position);
            if (distanceToTarget <= moveTowardsRange)
            {
                speed = 1f; // Set speed to move towards the target
                MoveTowardsTarget(closestTarget, enemyStopRange);
            }
            else
            {
                // Set speed to 0 when the target is out of range
                speed = 0f;
                rb.velocity = Vector2.zero;
                animator.SetFloat("xVelocity", 0);
            }
        }
        else
        {
            // Stop the character if there is no target
            rb.velocity = Vector2.zero;
            animator.SetFloat("xVelocity", 0);
        }

        CheckPlayerSoldiersStatus();
    }

    Transform FindClosestTarget()
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in playerTroopsTargets)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    void MoveTowardsTarget(Transform moveTarget, float stopDistance)
    {
        // Calculate distance to the target
        float distanceToTarget = Vector2.Distance(transform.position, moveTarget.position);

        if (distanceToTarget > stopDistance)
        {
            // Calculate direction to the target
            Vector2 direction = (moveTarget.position - transform.position).normalized;

            // Set Rigidbody2D velocity
            rb.velocity = direction * speed;

            // Determine move input for horizontal direction
            moveInput = direction.x;

            // Update animator with the absolute value of the x velocity
            animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

            // Optionally: Call the Flipsprite function to flip the sprite if needed
            //Flipsprite();
        }
        else
        {
            // Stop the character if within stop range
            rb.velocity = Vector2.zero;
            animator.SetFloat("xVelocity", 0);
            animator.SetBool("isAttacking", true);
        }
    }

     void CheckPlayerSoldiersStatus()
    {
        bool allEnemiesInactive = true;

        foreach (Transform enemy in playerTroopsTargets)
        {
            if (enemy.gameObject.activeSelf)
            {
                allEnemiesInactive = false;
                break;
            }
        }

        if (allEnemiesInactive)
        {
            animator.SetBool("isAttacking", false);
        }
    }
}