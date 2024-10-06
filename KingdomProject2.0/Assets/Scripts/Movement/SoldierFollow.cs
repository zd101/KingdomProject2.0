using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierFollow : MonoBehaviour
{
    public Transform target; // The target that this GameObject will follow
    public float speed = 5f; // Speed at which the GameObject will move towards the target
    public float stopRange = 1f; // Adjustable range within which the soldier will stop when following the target
    public float enemyStopRange = 0f; // Adjustable range within which the soldier will stop when following the enemy (set to 0 to go all the way)
    public float separationDistance = 1f; // Distance between soldiers to avoid stacking
    public float separationStrength = 1f; // Strength of the separation force applied
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = false; // Indicates whether the soldier is facing right
    private float moveInput; // Stores the horizontal movement input
    private KingScript KingScript;
    public GameObject targetGameObject; // Reference to the GameObject with the target script
    private List<Transform> enemyTargets = new List<Transform>(); // List of enemy targets
    private bool moveToEnemy = false; // Tracks whether the soldier should move towards the enemy
    private bool isSpeedZero = false; // Tracks whether the speed is set to 0
    public float SoldierHealth;
    private bool targetingEnemies = false; // Tracks if currently targeting enemies or default target
    private bool noTarget = false; // Tracks whether the soldier should follow no target

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        KingScript = targetGameObject.GetComponent<KingScript>();
    }

    void Update()
    {
        if (SoldierHealth <= 0)
        {
            Debug.Log("Soldier is dead");
            gameObject.SetActive(false);
            return;
        }

        // Toggle speed between 0 and 1 when 'Q' is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSpeed();
        }

        // Check if 'G' is pressed to toggle between enemy and default target
        if (Input.GetKeyDown(KeyCode.G))
        {
            targetingEnemies = !targetingEnemies; // Toggle targeting between enemies and default target
            moveToEnemy = targetingEnemies;
            noTarget = false; // Disable noTarget mode when switching to enemy or default target

            if (targetingEnemies)
            {
                // Find all enemies with the "Enemy" tag and store them in the list
                enemyTargets.Clear();
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    enemyTargets.Add(enemy.transform);
                }
            }
        }

        // Check if 'N' is pressed to enter No Target mode (stationary)
        if (Input.GetKeyDown(KeyCode.N))
        {
            noTarget = true; // Enable no target mode
            targetingEnemies = false; // Disable targeting enemies
        }

        // Only move or follow targets when speed is not zero
        if (!isSpeedZero)
        {
            if (noTarget)
            {
                // **No target mode**: The soldier won't follow any target, but can still move on its own
                // No target-following logic here, just allow other movement systems
                // You can still move them through other means, such as manual controls
                animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
            }
            else if (targetingEnemies && enemyTargets.Count > 0)
            {
                // Move towards the closest enemy target
                Transform closestEnemy = FindClosestEnemy();
                if (closestEnemy != null)
                {
                    MoveTowardsTarget(closestEnemy, enemyStopRange);
                    if (Mathf.Abs(rb.velocity.x) == 0)
                    {
                        animator.SetBool("isAttacking", true);
                    }
                }
            }
            else if (!targetingEnemies && target != null)
            {
                // Move towards the default target (e.g., the king or a base)
                MoveTowardsTarget(target, stopRange);
            }
            else
            {
                // Stop the character if there is no target
                rb.velocity = Vector2.zero;
                animator.SetFloat("xVelocity", 0);
            }
        }
        else
        {
            // If speed is zero, ensure the soldier stops
            rb.velocity = Vector2.zero;
            animator.SetFloat("xVelocity", 0);
        }

        // Apply separation logic to avoid stacking soldiers
        //ApplySeparation();
    }

    Transform FindClosestEnemy()
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform enemy in enemyTargets)
        {
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    void MoveTowardsTarget(Transform moveTarget, float stopDistance)
    {
        float distanceToTarget = Vector2.Distance(transform.position, moveTarget.position);

        if (distanceToTarget > stopDistance)
        {
            Vector2 direction = (moveTarget.position - transform.position).normalized;

            moveInput = direction.x;

            float targetVelocityX = moveInput * speed;
            float smoothedVelocityX = Mathf.Lerp(rb.velocity.x, targetVelocityX, Time.deltaTime * 10f);

            rb.velocity = new Vector2(smoothedVelocityX, rb.velocity.y);

            animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
            animator.SetBool("isAttacking", false);

            Flipsprite();
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("xVelocity", 0);
            //animator.SetBool("isAttacking", true);
        }
    }

    void Flipsprite()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;

            Transform parentTransform = transform.parent;
            if (parentTransform != null)
            {
                Vector3 ls = parentTransform.localScale;
                ls.x *= -1f;
                parentTransform.localScale = ls;
                Debug.Log("Flipping sprite, new parent localScale: " + parentTransform.localScale);
            }
            else
            {
                Debug.LogError("Parent GameObject not found.");
            }
        }
    }

    void ToggleSpeed()
    {
        if (isSpeedZero)
        {
            speed = 1f; // Restore speed to the original value
        }
        else
        {
            speed = 0f; // Set speed to 0 and stop movement
            rb.velocity = Vector2.zero; // Ensure the velocity is zero when speed is toggled off
            animator.SetFloat("xVelocity", 0); // Ensure the animator reflects the stopped movement
        }
        isSpeedZero = !isSpeedZero;
    }

    public void SetNoTarget(bool value)
    {
        noTarget = value;  // Set the noTarget mode to the given value
        Debug.Log("NoTarget enabled: " + noTarget);
    }
}