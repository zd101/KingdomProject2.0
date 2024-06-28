using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this value to set the movement speed
    float moveInput;
    private Rigidbody2D rb;
    Animator animator;
    bool isFacingRight = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from the horizontal axis (left and right arrow keys)
        moveInput = Input.GetAxis("Horizontal");
        Flipsprite();

        if (moveInput > .1f || moveInput < -.1f) {
            animator.SetBool("isWalking", true);
        } else {
            animator.SetBool("isWalking", false);
        }
    }
    private void FixedUpdate() {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

        if(Input.GetMouseButtonDown(0)) {
            animator.SetBool("isAttacking", true);
        }
         
    }
    void Flipsprite() {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f) {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public void EndAttack() {
        animator.SetBool("isAttacking", false);
    }
    }

