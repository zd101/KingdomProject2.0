using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Group
{
    [SerializeField] // Expose the capsules list to the Inspector
    public List<GameObject> capsules = new List<GameObject>();

    public Vector3 targetPosition;
    public bool isTargetPositionSet = false;
    public float formationSpacing = 1.0f;

    public Group(float groundYPosition)
    {
        targetPosition = new Vector3(0, groundYPosition, 0);
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isTargetPositionSet = true;

        // Disable the "noTarget" mode for each soldier in the group
        foreach (GameObject capsule in capsules)
        {
            SoldierFollow soldierFollow = capsule.GetComponent<SoldierFollow>();
            if (soldierFollow != null)
            {
                soldierFollow.SetNoTarget(false);  // Disable noTarget mode so soldiers can move to the flag
            }
        }
    }

    public void MoveUnits(float speed)
    {
        if (capsules.Count == 0 || !isTargetPositionSet)
            return;

        int capsuleCount = capsules.Count;
        float totalFormationWidth = (capsuleCount - 1) * formationSpacing;
        Vector3 formationStart = targetPosition - new Vector3(totalFormationWidth / 2, 0, 0);

        for (int i = 0; i < capsuleCount; i++)
        {
            GameObject capsule = capsules[i];

            // If Rigidbody2D and Animator are on the child (first child, index 0)
            Transform childTransform = capsule.transform.GetChild(0);  // Get the first (and only) child
            Rigidbody2D rb = childTransform.GetComponent<Rigidbody2D>();  // Get Rigidbody2D from child
            Animator animator = childTransform.GetComponent<Animator>();  // Get Animator from child

            // Debugging: Ensure Rigidbody and Animator exist
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D not found on child of capsule: " + capsule.name);
                continue;
            }
            if (animator == null)
            {
                Debug.LogError("Animator not found on child of capsule: " + capsule.name);
                continue;
            }

            Vector3 formationPosition = formationStart + new Vector3(i * formationSpacing, 0, 0);
            float distanceToTarget = Vector2.Distance(capsule.transform.position, formationPosition);

            if (distanceToTarget > 0.1f)
            {
                // Move capsule towards the formation position
                Vector2 direction = (formationPosition - capsule.transform.position).normalized;
                float targetVelocityX = direction.x * speed;
                rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);

                // Debugging: Check velocity changes
                Debug.Log("Capsule " + capsule.name + " moving with velocity X: " + rb.velocity.x);
            }
            else
            {
                // Stop the capsule if it's close enough to the target
                rb.velocity = new Vector2(0, rb.velocity.y);
                Debug.Log("Capsule " + capsule.name + " stopped.");
            }

            // Update the animator with the current velocity
            animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x)); // Set the velocity to the animator

            // Debugging: Ensure Animator xVelocity is being updated
            Debug.Log("Animator xVelocity for capsule " + capsule.name + ": " + Mathf.Abs(rb.velocity.x));
        }

        // Check if all capsules have reached their formation positions
        if (capsules.TrueForAll(c => Vector3.Distance(c.transform.position, formationStart + new Vector3(capsules.IndexOf(c) * formationSpacing, 0, 0)) < 0.1f))
        {
            isTargetPositionSet = false;
        }
    }

    public void SpawnNewUnit(GameObject capsulePrefab, float groundYPosition)
    {
        Vector3 spawnPosition = new Vector3(capsules.Count * formationSpacing, groundYPosition, 0);
        GameObject newCapsule = GameObject.Instantiate(capsulePrefab, spawnPosition, Quaternion.identity);
        capsules.Add(newCapsule);
    }
}
