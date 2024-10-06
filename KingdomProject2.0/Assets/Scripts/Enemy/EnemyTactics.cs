using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTactics : MonoBehaviour
{
    public GameObject wall; // The wall GameObject that will be destroyed
    public float soldierAdvanceDistance = 2f; // The adjustable distance that soldiers will move forward (left)
    public float archerRetreatDistance = 1.5f; // The distance that archers will move backward (right)
    public float moveSpeed = 5f; // Speed at which soldiers and archers move
    public GameObject[] enemySoldiers; // Array of enemy soldier GameObjects
    public GameObject[] enemyArchers; // Array of enemy archer GameObjects

    private bool wallBroken = false; // Tracks whether the wall is broken

    void Update()
    {
        // Check if the wall has been destroyed and the wallBroken flag is false
        if (!wall.activeSelf && !wallBroken)
        {
            wallBroken = true; // Set the wallBroken flag to true

            // Start the retreat and advance actions
            StartCoroutine(MoveSoldiersForward());
            StartCoroutine(MoveArchersBackward());
        }
    }

    IEnumerator MoveSoldiersForward()
    {
        foreach (GameObject soldier in enemySoldiers)
        {
            Animator soldierAnimator = soldier.GetComponent<Animator>(); 
            Rigidbody2D soldierRb = soldier.GetComponent<Rigidbody2D>(); 

            Vector3 targetPosition = soldier.transform.position + new Vector3(-soldierAdvanceDistance, 0, 0); // Move left
            while (Vector3.Distance(soldier.transform.position, targetPosition) > 0.1f)
            {
                // Calculate direction to the target
                Vector2 direction = (targetPosition - soldier.transform.position).normalized;

                // Set Rigidbody2D velocity to move the soldier
                soldierRb.velocity = direction * moveSpeed;

                // Update animator's xVelocity parameter
                soldierAnimator.SetFloat("xVelocity", Mathf.Abs(soldierRb.velocity.x));
                yield return null;
            }

            // Stop the soldier's movement
            soldierRb.velocity = Vector2.zero;
        }
    }

    IEnumerator MoveArchersBackward()
    {
        List<Vector3> retreatPositions = new List<Vector3>();
        List<Rigidbody2D> archerRigidbodies = new List<Rigidbody2D>();
        List<Animator> archerAnimators = new List<Animator>();

        // Calculate retreat positions for all archers and get their Rigidbody2D components
        foreach (GameObject archer in enemyArchers)
        {
            Rigidbody2D archerRb = archer.GetComponentInChildren<Rigidbody2D>();
            Animator archerAnimator = archer.GetComponentInChildren<Animator>();
            archerRigidbodies.Add(archerRb);
            archerAnimators.Add(archerAnimator);

            GameObject closestSoldier = FindClosestSoldier(archer);
            if (closestSoldier != null)
            {
                Vector3 retreatPosition = closestSoldier.transform.position + new Vector3(archerRetreatDistance, 0, 0);
                retreatPositions.Add(retreatPosition);
            }
            else
            {
                retreatPositions.Add(archer.transform.position); // If no soldier found, set current position as target
            }
        }

        bool allArchersReached = false;
        while (!allArchersReached)
        {
            allArchersReached = true; // Assume all archers have reached their targets initially

            for (int i = 0; i < enemyArchers.Length; i++)
            {
                Rigidbody2D archerRb = archerRigidbodies[i];
                Animator archerAnimator = archerAnimators[i];
                Vector3 retreatPosition = retreatPositions[i];

                if (Vector3.Distance(enemyArchers[i].transform.position, retreatPosition) > 0.1f)
                {
                    // Calculate direction to the retreat position
                    Vector2 direction = (retreatPosition - enemyArchers[i].transform.position).normalized;

                    // Set Rigidbody2D velocity to move the archer
                    archerRb.velocity = direction * moveSpeed;

                    // Update animator's xVelocity parameter
                    archerAnimator.SetFloat("xVelocity", Mathf.Abs(archerRb.velocity.x));

                    allArchersReached = false; // If any archer hasn't reached the target, set this to false
                }
                else
                {
                    // Stop the archer's movement
                    archerRb.velocity = Vector2.zero;
                    archerAnimator.SetFloat("xVelocity", 0f);
                }
            }

            yield return null; // Wait for the next frame before continuing the loop
        }
    }

    GameObject FindClosestSoldier(GameObject archer)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject soldier in enemySoldiers)
        {
            float distance = Vector3.Distance(archer.transform.position, soldier.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = soldier;
            }
        }

        return closest;
    }
}