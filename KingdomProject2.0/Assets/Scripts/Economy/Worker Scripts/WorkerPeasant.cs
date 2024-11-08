using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerPeasant : MonoBehaviour
{
    public enum WorkerState { Idle, Gathering, Depositing }
    private WorkerState currentState;

    // Wander bounds
    public Transform leftBound;         // Left boundary
    public Transform rightBound;        // Right boundary
    public float paceSpeed = 1f;
    public float minPauseTime = 1f;
    public float maxPauseTime = 3f;

    private Vector2 targetPosition;
    private bool isPaused = false;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; // Disable physics influence for smoother movement
        }

        currentState = WorkerState.Idle;
        SetRandomTargetPosition();  // Set an initial random target within bounds
        StartCoroutine(StateManager());
    }

    private IEnumerator StateManager()
    {
        while (true)
        {
            switch (currentState)
            {
                case WorkerState.Idle:
                    yield return StartCoroutine(WanderWithinBounds());
                    break;
                case WorkerState.Gathering:
                    // Placeholder for gathering behavior
                    break;
                case WorkerState.Depositing:
                    // Placeholder for depositing behavior
                    break;
            }
            yield return null;
        }
    }

    private IEnumerator WanderWithinBounds()
    {
        while (currentState == WorkerState.Idle)
        {
            if (!isPaused)
            {
                MoveWorker();
            }
            yield return null;
        }
    }

    private void MoveWorker()
    {
        // Smooth movement using Rigidbody2D velocity
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * paceSpeed;

        // Check if we've reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
        {
            rb.velocity = Vector2.zero;          // Stop moving
            StartCoroutine(PauseAndSetNewTarget()); // Pause, then choose a new position
        }
    }

    // Sets a random target within the bounds
    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(leftBound.position.x, rightBound.position.x);
        targetPosition = new Vector2(randomX, transform.position.y);
    }

    private IEnumerator PauseAndSetNewTarget()
    {
        isPaused = true;
        float pauseTime = Random.Range(minPauseTime, maxPauseTime);
        yield return new WaitForSeconds(pauseTime);
        isPaused = false;
        SetRandomTargetPosition(); // Pick a new random point within the bounds
    }

    public void ChangeState(WorkerState newState)
    {
        currentState = newState;
        if (rb != null) rb.velocity = Vector2.zero; // Reset velocity on state change
    }
}
