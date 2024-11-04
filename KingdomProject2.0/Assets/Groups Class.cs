using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Group
{

    //testing github branches -- Theeze be eaze

    [SerializeField] // Expose the capsules list to the Inspector
    public List<GameObject> capsules = new List<GameObject>();

    public Vector3 targetPosition;
    public bool isTargetPositionSet = false;
    public float formationSpacing =0.5f;

    public Group(float groundYPosition)
    {
        targetPosition = new Vector3(0, groundYPosition, 0);
    }

    // Function to set the target position for the entire group
    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
        isTargetPositionSet = true;

        // All soldiers are now set to move to the target
        foreach (GameObject capsule in capsules)
        {
            SoldierFollow soldierFollow = capsule.GetComponent<SoldierFollow>();
            if (soldierFollow != null)
            {
                soldierFollow.SetNoTarget(false);  // Allow soldiers to move towards the target
            }
        }
    }

    // Function to move the entire group towards the target at once (horizontal movement only)
    public void MoveUnits(float speed)
    {
        if (capsules.Count == 0 || !isTargetPositionSet)
            return;

        // Calculate the width of the entire formation
        float totalFormationWidth = (capsules.Count - 1) * formationSpacing;
        Vector3 formationStart = targetPosition - new Vector3(totalFormationWidth / 2, 0, 0);

        // Move each soldier in the group towards their calculated formation position (only left or right)
        foreach (GameObject capsule in capsules)
        {
            Vector3 formationPosition = formationStart + new Vector3(capsules.IndexOf(capsule) * formationSpacing, 0, 0);

            // Calculate distance to the target position
            float distanceToTarget = Vector2.Distance(capsule.transform.position, formationPosition);

            // Get Animator and Rigidbody to update the movement and animation
            Transform childTransform = capsule.transform.GetChild(0);  // Get the first (and only) child
            Rigidbody2D rb = capsule.GetComponent<Rigidbody2D>();
            Animator animator = childTransform.GetComponent<Animator>();  // Get Animator from child

            // Move the soldier horizontally towards the formation position (X-axis only)
            Vector3 horizontalTargetPosition = new Vector3(formationPosition.x, capsule.transform.position.y, capsule.transform.position.z);
            capsule.transform.position = Vector3.MoveTowards(capsule.transform.position, horizontalTargetPosition, speed * Time.deltaTime);

            // Calculate direction for sprite flipping
            float direction = formationPosition.x - capsule.transform.position.x;

            // Check if soldier has reached the target (within 0.1 units on the X-axis)
            if (Mathf.Abs(direction) < 0.1f)
            {
                // Stop movement and set velocity to 0 if they are at the target
                if (animator != null)
                {
                    animator.SetFloat("xVelocity", 0f);  // Stop animation once they reach the target
                }
            }
            else
            {
                // Update the animator with the current movement speed
                if (animator != null)
                {
                    float movementSpeed = Mathf.Abs(direction);
                    animator.SetFloat("xVelocity", movementSpeed);  // Update animation based on movement
                }

                /*/ **Reversed Sprite Flipping**: Flip the sprite based on the opposite direction
                if ((direction > 0 && capsule.transform.localScale.x > 0) || (direction < 0 && capsule.transform.localScale.x < 0))
                {
                    capsule.transform.localScale = new Vector3(-capsule.transform.localScale.x, capsule.transform.localScale.y, capsule.transform.localScale.z);
                }
                */
            }
        }

        // Check if all soldiers have reached their formation positions
        if (capsules.TrueForAll(c => Mathf.Abs(c.transform.position.x - (formationStart.x + capsules.IndexOf(c) * formationSpacing)) < 0.1f))
        {
            isTargetPositionSet = false;  // Reset flag if all soldiers are in position
        }
    }
}
