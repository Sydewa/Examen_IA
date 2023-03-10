using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State { Patrolling, Chasing, Attack }
public class Enemy_StateManager : MonoBehaviour
{
    NavMeshAgent agent;
    State currentState;

    Transform Player;

    Vector3 destination;
    Vector3 spawnPoint;

    [Header ("Patrol")]
    public float patrolRadius;
    public Vector2 waitTimeInterval;
    float elapsedTimePatrolling;

    [Header ("Chasing")]
    public float visionRange;
    public float endVisionRange;

    [Header ("Attack")]
    public float attackRange;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        spawnPoint = transform.position;

        //Pillamos el transform del player
        Player = GameObject.Find("Player").transform;
        
        //Ponemos un state de default
        currentState = State.Patrolling;

        //Seteamos un punto de patrol
        destination = Random.insideUnitSphere * patrolRadius;
        agent.destination = destination + spawnPoint;
    }

    void Update()
    {
        StateMachine();
    }

    void StateMachine()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if(Vector3.Distance(transform.position, Player.position) < visionRange)
                {
                    currentState = State.Chasing;
                }
            break;

            case State.Chasing:
                Chasing();
                if(Vector3.Distance(transform.position, Player.position) < attackRange)
                {
                    currentState = State.Attack;
                }
                if(Vector3.Distance(transform.position, Player.position) > endVisionRange)
                {
                    currentState = State.Patrolling;
                }
            break;

            case State.Attack:
                Debug.Log("Attack");
                currentState = State.Patrolling;
            break;

            default:
            //Poner funciones en el default en un state machine cerrado suele dar problemas, por eso esta vacio
            break;
        }
    }

    void Patrol()
    {
        if(agent.remainingDistance < 0.5f)
        {
            elapsedTimePatrolling += Time.deltaTime;
            if(elapsedTimePatrolling >= Random.Range(waitTimeInterval.x, waitTimeInterval.y))
            {
                destination = Random.insideUnitSphere * patrolRadius;
                agent.destination = destination + spawnPoint;
                elapsedTimePatrolling = 0f;
            }
        }
    }

    void Chasing()
    {
        destination = Player.position;
        agent.destination = destination;
    }

    void OnDrawGizmosSelected()
    {
        //Patrol radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(spawnPoint, patrolRadius);

        //Vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        //Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //End vision range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, endVisionRange);
    }

}
