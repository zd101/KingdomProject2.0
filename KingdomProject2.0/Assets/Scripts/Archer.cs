using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    public GameObject arrowPrefab; // The arrow projectile prefab
    public Transform shootingPoint; // The point from which the arrow will be shot
    public float arrowSpeed = 10f; // The base speed of the arrow projectile
    public float strengthMultiplier = 1.0f; // Adjustable multiplier to increase the strength of the arrow
    public float timeBetweenShots = 1f; // Delay between shots
    private float lastShotTime;
    private Transform currentTarget; // To store the current target for shooting
    private Animator animator; // Reference to the Animator component
    public AudioSource ArrowSound;

    void Start()
    {
        // Get the Animator component attached to the Archer GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Find all GameObjects with the 'PlayerTroops' tag
        GameObject[] playerTroops = GameObject.FindGameObjectsWithTag("PlayerTroops");

        // Check each player troop and look for the closest one within shooting range
        foreach (GameObject troop in playerTroops)
        {
            // Check the distance between the archer and the PlayerTroops
            float distance = Vector2.Distance(transform.position, troop.transform.position);

            if (distance <= 10f && Time.time >= lastShotTime + timeBetweenShots) // Example range of 10f
            {
                animator.SetBool("isAttacking", true); // Trigger attack animation
                currentTarget = troop.transform; // Store the current target
                lastShotTime = Time.time; // Update the time of the last shot
                break; // Stop checking other troops once a valid target is found
            }
        }
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

            // Adjust the launch angle for projectile arc
            float launchAngle = 35f; // A slightly lower angle than 45 to ensure faster flight for distance

            // Convert angle to radians for calculation
            float launchAngleRad = launchAngle * Mathf.Deg2Rad;

            // Calculate velocity with gravity and the distance
            float gravity = Mathf.Abs(Physics2D.gravity.y);
            float velocity = Mathf.Sqrt((gravity * distance * distance) / (2 * (distance * Mathf.Tan(launchAngleRad) - heightDifference)));

            // If velocity is zero or invalid, set a minimum velocity to avoid weak shots
            if (float.IsNaN(velocity) || velocity <= 0) velocity = arrowSpeed;

            // Apply the strength multiplier to adjust the arrow's speed and strength
            velocity *= strengthMultiplier;

            // Calculate the velocity components based on the angle
            float velocityX = velocity * Mathf.Cos(launchAngleRad);
            float velocityY = velocity * Mathf.Sin(launchAngleRad);

            // Apply the calculated velocity to the arrow's Rigidbody2D
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Apply the velocity with the direction towards the target
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
}