using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierFollow : MonoBehaviour
{
   public Transform target; // The target that this GameObject will follow
    public float speed = 5f; // Speed at which the GameObject will move towards the target
    public float stopRange = 1f; // Adjustable range within which the soldier will stop
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = false; // Indicates whether the soldier is facing right
    private float moveInput; // Stores the horizontal movement input
    private KingScript KingScript;
    public GameObject targetGameObject; // Reference to the GameObject with the target script


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        KingScript = targetGameObject.GetComponent<KingScript>();
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate distance to target
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget > stopRange)
            {
                // Calculate direction to target
                Vector2 direction = (target.position - transform.position).normalized;

                // Set Rigidbody2D velocity
                rb.velocity = direction * speed;

                // Determine move input for horizontal direction
                moveInput = direction.x;

                // Update animator with the absolute value of the x velocity
                animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

                // Call the Flipsprite function to flip the sprite if needed
                Flipsprite();
            }
            else
            {
                // Stop the character if within stop range
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
    }

    void Flipsprite()
    {
        if ((isFacingRight && moveInput < 0f) || (!isFacingRight && moveInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

     public void CallAttackFunction()
    {
        if (KingScript != null)
        {
            KingScript.Attack();
        }
    }

}
