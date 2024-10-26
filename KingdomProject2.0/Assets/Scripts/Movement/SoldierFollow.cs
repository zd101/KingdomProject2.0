using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierFollow : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float stopRange = 1f;
    public float enemyStopRange = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = false;
    private float moveInput;
    private KingScript KingScript;
    public GameObject targetGameObject;
    private List<Transform> enemyTargets = new List<Transform>();
    private bool moveToEnemy = false;
    private bool isSpeedZero = false;
    public float SoldierHealth;
    private bool targetingEnemies = false;
    private bool noTarget = false;
    float inputHorizontal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        KingScript = targetGameObject.GetComponent<KingScript>();
    }

    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        if (SoldierHealth <= 0)
        {
            Debug.Log("Soldier is dead");
            gameObject.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSpeed();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetNoTarget(true);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            targetingEnemies = !targetingEnemies;
            moveToEnemy = targetingEnemies;

            if (targetingEnemies)
            {
                enemyTargets.Clear();
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    enemyTargets.Add(enemy.transform);
                }

                SetNoTarget(false);
            }
            else
            {
                target = targetGameObject.transform;
                Debug.Log("Returning to default target (Player/King).");
            }
        }

        if (!noTarget && !isSpeedZero)
        {
            if (targetingEnemies && enemyTargets.Count > 0)
            {
                Transform closestEnemy = FindClosestEnemy();
                if (closestEnemy != null)
                {
                    MoveTowardsTarget(closestEnemy, enemyStopRange);
                }
            }
            else if (target != null && !targetingEnemies)
            {
                MoveTowardsTarget(target, stopRange);
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("xVelocity", 0);
            }
        }
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
            animator.SetBool("isAttacking", false);

            Vector2 direction = (moveTarget.position - transform.position).normalized;
            moveInput = direction.x;

            //Debug.Log("Move Input: " + moveInput);

            float targetVelocityX = moveInput * speed;
            float smoothedVelocityX = Mathf.Lerp(rb.velocity.x, targetVelocityX, Time.deltaTime * 10f);

            rb.velocity = new Vector2(smoothedVelocityX, rb.velocity.y);

            if (Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
            }

            Flipsprite(); // Flip the GameObject based on movement direction
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("xVelocity", 0);

            if (targetingEnemies || moveTarget.CompareTag("Enemy"))
            {
                animator.SetBool("isAttacking", true);
            }
        }
    }

    // Flipping the entire GameObject based on direction by modifying the local scale
    void Flipsprite()
    {
        //Debug.Log("Flipsprite() called - moveInput: " + moveInput + ", isFacingRight: " + isFacingRight);

        // Check if the direction has significantly changed
        if (isFacingRight && inputHorizontal < -0.01)
        {
            isFacingRight = false;
            FlipGameObject(); // Flip the GameObject to face left   //only flips when soldier stops, if moveInput < - 0.1 (SOMETHING TO DO WITH SCALE OF CHILD AND PARENT MAYBE BEING OPPOSITE SOMETHING, rapid flipping)
            Debug.Log("Flipped to face left");
        }
        else if (!isFacingRight && inputHorizontal > 0.01)
        {
            isFacingRight = true;
            FlipGameObject(); // Flip the GameObject to face right
            Debug.Log("Flipped to face right");
        }
    }

    // Flip the entire GameObject by changing its local scale
    void FlipGameObject()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;  // Flip the x-axis scale to reverse the orientation
        transform.localScale = localScale;  // Apply the new scale
    }

    void ToggleSpeed()
    {
        if (isSpeedZero)
        {
            speed = 5f;
        }
        else
        {
            speed = 0f;
            rb.velocity = Vector2.zero;
            animator.SetFloat("xVelocity", 0);
        }
        isSpeedZero = !isSpeedZero;
    }

    public void SetNoTarget(bool value)
    {
        noTarget = value;

        if (noTarget)
        {
            target = null;
        }
    }
}
