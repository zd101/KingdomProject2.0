using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTactics : MonoBehaviour
{
    public GameObject wall; // The wall GameObject that will be destroyed
    public float soldierAdvanceDistance = 2f; // Distance soldiers move forward
    public float archerRetreatDistance = 1.5f; // Distance archers move backward
    public float moveSpeed = 5f; // Speed for soldiers and archers
    public GameObject[] enemySoldiers; // Array of enemy soldier GameObjects
    public GameObject[] enemyArchers; // Array of enemy archer GameObjects
    public float attackInterval = 10f; // Time between hit-and-run attacks
    public float attackRange = 2f; // Range for attacking player troops
    public float soldierAttackSpeed = 3f; // Speed of soldier movement when attacking
    public GameObject[] playerTroops; // Array of player's troop GameObjects (can be auto-assigned)

    private bool wallBroken = false; // Tracks whether the wall is broken

    void Start()
    {
        StartCoroutine(TriggerAttacks()); // Start periodic hit-and-run attacks
    }

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

    // Coroutine to handle periodic hit-and-run attacks
    IEnumerator TriggerAttacks()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval); // Wait for the attack interval

            // Select three random enemy soldiers
            PickThreeUnitsToAttack();
        }
    }

    // Method to pick three random enemy soldiers to attack the nearest player troops
    void PickThreeUnitsToAttack()
    {
        if (enemySoldiers.Length < 3)
        {
            Debug.LogWarning("Not enough enemy soldiers to choose from.");
            return;
        }

        // Shuffle and pick 3 random soldiers
        List<GameObject> selectedSoldiers = new List<GameObject>(enemySoldiers);
        Shuffle(selectedSoldiers); // Shuffle the soldiers to choose randomly

        for (int i = 0; i < 3; i++)
        {
            GameObject soldier = selectedSoldiers[i];
            // Start moving the selected soldier toward the nearest player troop
            StartCoroutine(MoveAndAttack(soldier));
        }
    }

    // Shuffle method (Fisher-Yates) to randomize the soldier selection
    void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Coroutine to handle movement and attacking of selected soldiers
    IEnumerator MoveAndAttack(GameObject soldier)
    {
        Transform target = FindNearestPlayerTroop(soldier.transform.position);

        if (target != null)
        {
            Rigidbody2D soldierRb = soldier.GetComponent<Rigidbody2D>();
            Animator soldierAnimator = soldier.GetComponent<Animator>();

            // Move towards the target
            while (Vector3.Distance(soldier.transform.position, target.position) > attackRange)
            {
                Vector2 direction = (target.position - soldier.transform.position).normalized;
                soldierRb.velocity = direction * soldierAttackSpeed;

                // Update animator's xVelocity parameter
                soldierAnimator.SetFloat("xVelocity", Mathf.Abs(soldierRb.velocity.x));

                yield return null; // Wait for the next frame
            }

            // Perform the attack (you can add more logic here)
            Attack(target.gameObject, soldier);

            // Stop the soldier's movement after the attack
            soldierRb.velocity = Vector2.zero;
        }
    }

    // Method to find the nearest player troop for an enemy soldier
    Transform FindNearestPlayerTroop(Vector2 soldierPosition)
    {
        if (playerTroops.Length == 0)
        {
            Debug.LogWarning("No player troops found!");
            return null;
        }

        GameObject nearestTroop = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject troop in playerTroops)
        {
            float distance = Vector2.Distance(soldierPosition, troop.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTroop = troop;
            }
        }

        return nearestTroop != null ? nearestTroop.transform : null;
    }

    // Placeholder attack method
    void Attack(GameObject playerTroop, GameObject soldier)
    {
        Debug.Log(soldier.name + " attacks " + playerTroop.name);
        // Add attack logic here (reduce health, play animation, etc.)
    }

    // Original MoveSoldiersForward logic (advance soldiers after wall breaks)
    IEnumerator MoveSoldiersForward()
    {
        foreach (GameObject soldier in enemySoldiers)
        {
            Animator soldierAnimator = soldier.GetComponent<Animator>();
            Rigidbody2D soldierRb = soldier.GetComponent<Rigidbody2D>();

            Vector3 targetPosition = soldier.transform.position + new Vector3(-soldierAdvanceDistance, 0, 0); // Move left
            while (Vector3.Distance(soldier.transform.position, targetPosition) > 0.1f)
            {
                Vector2 direction = (targetPosition - soldier.transform.position).normalized;
                soldierRb.velocity = direction * moveSpeed;

                soldierAnimator.SetFloat("xVelocity", Mathf.Abs(soldierRb.velocity.x));
                yield return null;
            }

            soldierRb.velocity = Vector2.zero;
        }
    }

    // Original MoveArchersBackward logic (retreat archers after wall breaks)
    IEnumerator MoveArchersBackward()
    {
        List<Vector3> retreatPositions = new List<Vector3>();
        List<Rigidbody2D> archerRigidbodies = new List<Rigidbody2D>();
        List<Animator> archerAnimators = new List<Animator>();

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
                retreatPositions.Add(archer.transform.position); // If no soldier found, stay in place
            }
        }

        bool allArchersReached = false;
        while (!allArchersReached)
        {
            allArchersReached = true;

            for (int i = 0; i < enemyArchers.Length; i++)
            {
                Rigidbody2D archerRb = archerRigidbodies[i];
                Animator archerAnimator = archerAnimators[i];
                Vector3 retreatPosition = retreatPositions[i];

                if (Vector3.Distance(enemyArchers[i].transform.position, retreatPosition) > 0.1f)
                {
                    Vector2 direction = (retreatPosition - enemyArchers[i].transform.position).normalized;
                    archerRb.velocity = direction * moveSpeed;

                    archerAnimator.SetFloat("xVelocity", Mathf.Abs(archerRb.velocity.x));

                    allArchersReached = false;
                }
                else
                {
                    archerRb.velocity = Vector2.zero;
                    archerAnimator.SetFloat("xVelocity", 0f);
                }
            }

            yield return null;
        }
    }

    // Find the closest soldier to a given archer
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
