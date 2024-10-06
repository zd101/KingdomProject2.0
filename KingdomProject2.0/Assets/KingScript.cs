using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingScript : MonoBehaviour
{
     public GameObject knightGameObject;
    private Animator KnightAnimator;
    private Rigidbody2D Knightrb;
    public GameObject enemyGameObject; 
    public float moveSpeed = 5f;       
    public float attackRange = 2f;     
    private bool isMoving = false;    
    private bool isAttacking = false;  
    private bool isFacingRight = true;
    private float moveInput;
    public GameObject AttackPoint;
    public float radius;
    public LayerMask enemies;

    void Start()
    {
        KnightAnimator = knightGameObject.GetComponent<Animator>();
        Knightrb = knightGameObject.GetComponent<Rigidbody2D>();
    }

    void Update() 
    {
        moveInput = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(0))
        {
            // Start moving towards the enemy
            isMoving = true;
            isAttacking = false;
            KnightAnimator.SetBool("isAttacking", false); // Ensure the attack animation is reset
        }

        if (Input.GetMouseButtonDown(1))
        {
            isMoving = false;
            isAttacking = false;
            KnightAnimator.SetBool("isAttacking", false); // Ensure the attack animation is reset
        }

        if (!isMoving && Mathf.Abs(moveInput) > 0)
        {
            // User is controlling the movement
            MoveKnight(moveInput);
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Move the knight towards the enemy
            MoveKnightTowardsEnemy();
        }
    }

    void MoveKnight(float moveInput)
    {
        Knightrb.velocity = new Vector2(moveInput * moveSpeed, Knightrb.velocity.y);
        //FlipSprite(moveInput);
        KnightAnimator.SetFloat("xVelocity", Mathf.Abs(Knightrb.velocity.x));
    }

    void MoveKnightTowardsEnemy()
    {
        // Calculate the direction to the enemy
        Vector2 direction = (enemyGameObject.transform.position - knightGameObject.transform.position).normalized;

        // Move the knight towards the enemy using Rigidbody2D velocity
        Knightrb.velocity = new Vector2(direction.x * moveSpeed, Knightrb.velocity.y);
        FlipSprite(direction.x);
        KnightAnimator.SetFloat("xVelocity", Mathf.Abs(Knightrb.velocity.x));

        // Calculate the distance to the enemy
        float distanceToEnemy = Vector2.Distance(knightGameObject.transform.position, enemyGameObject.transform.position);

        // Check if the knight is within attack range
        if (distanceToEnemy <= attackRange)
        {
            // Stop moving and start attacking
            isMoving = false;
            isAttacking = true;
            Knightrb.velocity = Vector2.zero; // Stop the knight's movement
            KnightAnimator.SetBool("isAttacking", true);
        }
    }

    void FlipSprite(float moveInput)
    {
        if (moveInput > 0 && !isFacingRight || moveInput < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = knightGameObject.transform.localScale;
            ls.x *= -1f;
            knightGameObject.transform.localScale = ls;
        }
    }
    

    public void Attack() {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(AttackPoint.transform.position, radius, enemies);

        foreach (Collider2D enemyGameObject in enemy) {
            Debug.Log("Hit enemy");
            //enemyGameObject.GetComponent<EnemyHealth>().health -= 10;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(AttackPoint.transform.position, radius);
    }
}
