using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    

    private enum state
    {
        Patrol,
        Halt
    }

    private state currentState;
    private float distance;
    private GameObject currentPatrolPoint;

    public GameObject PatrolPointPrefab;
    public float speed;

    void Start()
    {
        currentPatrolPoint = PatrolPointPrefab;
    }

    void Update()
    {
        EnterPatrolFunction();
    }

    //--------------------------------------------------------------------------------------------ENTER/UPDATE/EXIT STATE FUNCTIONS-------------------------------------------------------------------------------------------------------------------------------
    void EnterPatrolFunction()
    {
        UpdatePatrolFunction();
    }

    void UpdatePatrolFunction()
    {
        distance = Vector2.Distance(transform.position, currentPatrolPoint.transform.position);
        Vector2 direction = currentPatrolPoint.transform.position - transform.position;

        transform.position = Vector2.MoveTowards(this.transform.position, currentPatrolPoint.transform.position, speed * Time.deltaTime);
    }

    void ExitPatrolFunction()
    {
        //exit patrol
    }

    void EnterHaltFunction()
    {
        //enter patrol
    }

    void UpdateHaltFunction()
    {
        //update patrol
    }

    void ExitHaltFunction()
    {
        //exit patrol
    }

    //-------------------------------------------------------------------------------------------- OTHER FUNCTIONS-------------------------------------------------------------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {


        if (other.gameObject)
        {
            Debug.Log("this worked");

            InstantiatePatrolPoint();

            Destroy(other.gameObject);
        }
    }

    void InstantiatePatrolPoint()
    {
        float randomX = Random.Range(-10, 10);
        Vector2 spawnPosition = new Vector2(randomX, currentPatrolPoint.transform.position.y);

        GameObject newPatrolPoint = Instantiate(PatrolPointPrefab, spawnPosition, Quaternion.identity);

        currentPatrolPoint = newPatrolPoint;
    }
}

