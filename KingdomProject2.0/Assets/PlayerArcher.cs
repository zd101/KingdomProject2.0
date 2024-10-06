using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArcher : MonoBehaviour
{
    public GameObject arrowPrefab; // The arrow projectile prefab
    public Transform shootingPoint; // The point from which the arrow will be shot
    public float shootingRange = 10f; // The range within which the archer will shoot at PlayerTroops
    public float arrowSpeed = 10f; // The initial speed of the arrow projectile
    public float timeBetweenShots = 1f; // Delay between shots
    private float lastShotTime;
    private Transform currentTarget; // To store the current target for shooting
    private Animator animator; // Reference to the Animator component
    public AudioSource ArrowSound;
    private bool isFacingRight = true; // To track the direction the archer is facing

    void Start()
    {
        // Get the Animator component attached to the Archer GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Find all GameObjects with the 'Enemy' tag
        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Check each enemy and look for the closest one within shooting range
        foreach (GameObject enemy in Enemies)
        {
            // Check the distance between the archer and the Enemy
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance <= shootingRange && Time.time >= lastShotTime + timeBetweenShots)
            {
                animator.SetBool("isAttacking", true); // Trigger attack animation
                currentTarget = enemy.transform; // Store the current target
                lastShotTime = Time.time; // Update the time of the last shot

                // Face towards the target
                FaceTarget(enemy.transform);
                break; // Stop checking other enemies once a valid target is found
            }
        }
    }

    // Function to make the archer face the target
    private void FaceTarget(Transform target)
    {
        // Check the direction of the target
        if (target.position.x < transform.position.x && isFacingRight)
        {
            // If the enemy is to the left and the archer is facing right, flip the archer
            Flip();
        }
        else if (target.position.x > transform.position.x && !isFacingRight)
        {
            // If the enemy is to the right and the archer is facing left, flip the archer
            Flip();
        }
    }

    // Function to flip the archer
    private void Flip()
    {
        isFacingRight = !isFacingRight;

        // Multiply the archer's localScale by -1 to flip it
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    // Public function to shoot the arrow, this can be called from an animation event
    public void ShootArrow()
    {
        // Only shoot if there's a valid target
        if (currentTarget != null)
        {
            // Instantiate the arrow at the shooting point
            GameObject arrow = Instantiate(arrowPrefab, shootingPoint.position, Quaternion.identity);

            // Calculate the distance and direction to the target
            Vector2 direction = (currentTarget.position - shootingPoint.position);
            float distance = direction.magnitude;
            float heightDifference = direction.y;

            // Set an angle for the projectile launch (in degrees)
            float launchAngle = 45f; // You can tweak this angle for different arcs

            // Convert angle to radians for calculation
            float launchAngleRad = launchAngle * Mathf.Deg2Rad;

            // Increase the arrow's initial velocity to ensure it has enough power
            float gravity = Mathf.Abs(Physics2D.gravity.y);
            float velocity = Mathf.Sqrt((gravity * distance * distance) / (2 * (distance * Mathf.Tan(launchAngleRad) - heightDifference)));

            // If velocity is zero or invalid, set a minimum velocity to avoid weak shots
            if (float.IsNaN(velocity) || velocity <= 0) velocity = 20f; // A fallback to ensure the arrow still shoots

            // Calculate the velocity components
            float velocityX = velocity * Mathf.Cos(launchAngleRad);
            float velocityY = velocity * Mathf.Sin(launchAngleRad);

            // Apply the calculated velocity to the arrow's Rigidbody2D
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(velocityX * Mathf.Sign(direction.x), velocityY);

                // Rotate the arrow to point in the direction of its velocity
                StartCoroutine(RotateArrowTowardsVelocity(rb, arrow));
            }

            // Play arrow sound
            ArrowSound.Play();
        }
    }

    // Coroutine to rotate the arrow towards its velocity direction during flight
    private IEnumerator RotateArrowTowardsVelocity(Rigidbody2D arrowRb, GameObject arrow)
    {
        while (arrowRb != null)
        {
            Vector2 velocity = arrowRb.velocity;
            if (velocity.sqrMagnitude > 0.1f) // Only update rotation if the arrow is moving
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            yield return null; // Wait for the next frame before continuing
        }
    }

    // Optional: Draw a gizmo to visualize the shooting range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}