using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStationaryMovement : MonoBehaviour
{
   public Transform baseObject; // Reference to the Base GameObject
    public float radius = 10f; // Radius within which NPCs will move
    public float moveSpeed = 3f; // Speed of the NPC movement
    public float changeDirectionInterval = 2f; // Interval in seconds to change direction

    private Vector2 targetPosition; // Target position to move towards
    private float timer;
    private float originalY; // Original y-position of the NPC
    private Rigidbody2D rb;
    private Animator animator;
    private bool isFacingRight = false; // Assume the NPC starts facing right

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (baseObject == null)
        {
            Debug.LogError("Base object not assigned.");
            enabled = false;
            return;
        }

        originalY = transform.position.y; // Store the original y-position
        SetNewTargetPosition();
        timer = changeDirectionInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SetNewTargetPosition();
            timer = changeDirectionInterval;
        }

        MoveTowardsTarget();
        FlipSprite();
    }

    private void SetNewTargetPosition()
    {
        // Generate a random x position within the radius around the baseObject
        float randomX = Random.Range(-radius, radius);
        Vector2 basePosition = baseObject.position;
        targetPosition = new Vector2(basePosition.x + randomX, originalY);
    }

    private void MoveTowardsTarget()
    {
        Vector2 currentPosition = rb.position; // Use rb.position instead of transform.position

        if (Mathf.Abs(currentPosition.x - targetPosition.x) > 0.1f)
        {
            Vector2 direction = new Vector2((targetPosition.x - currentPosition.x), 0).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, 0); // Update velocity
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop movement if close to target
        }

        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x)); // Update animation based on velocity
    }

    private void FlipSprite()
    {
        // Flip the sprite based on the direction of movement
        if ((rb.velocity.x > 0 && !isFacingRight) || (rb.velocity.x < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
}
