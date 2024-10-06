using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this value to set the movement speed
    float moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    bool isFacingRight = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find the Animator component on the child GameObject
        animator = GetComponentInChildren<Animator>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on the child GameObject.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator not found on the child GameObject.");
        }
    }

    void Update()
    {
        // Get input from the horizontal axis (left and right arrow keys)
        moveInput = Input.GetAxis("Horizontal");

        if (moveInput != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        FlipSprite();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("isAttacking", true);
        }
    }

    void FlipSprite()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
             isFacingRight = !isFacingRight;

            // Flip the sprite of the parent GameObject
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

    public void EndAttack()
    {
        animator.SetBool("isAttacking", false);
    }
}
