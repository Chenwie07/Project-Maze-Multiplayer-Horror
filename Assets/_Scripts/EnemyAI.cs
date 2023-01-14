//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent _agent;
    public Transform _player;
    public LayerMask _whatIsGround, _whatIsPlayer;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking 
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public void Awake()
    {
        _player = GameObject.FindWithTag("Player").transform;

        _agent = GetComponent<NavMeshAgent>();

    }

    private void FixedUpdate()
    {
        //check for sight and attack range

        // you can also modify this such that the range won't go through walls, instead of using floats, use...
        // perhaps an object with collisions or something. 

        // return true if from this object's transform position, using the sight range as a radius (of the sphere), if 
        // it collides with an object that belongs to the layermask of _whatIsPlayer - in this case it'll return true if 
        // there the sphere collides with the collider of our player. 
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, _whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, _whatIsPlayer);

        // connditions to influence AI reaction. 
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            _agent.SetDestination(walkPoint); 

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached? 
        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false; // so get a new one from the update above. 

    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX , transform.position.y, transform.position.z + randomZ);
        // make sure the walk point is part of ground
        if(Physics.Raycast(walkPoint, -transform.up, 2f, _whatIsGround))
            walkPointSet = true;
    }

    void ChasePlayer()
    {
        // change sound, or play sfx effect, something like AHA, or there you are!
        // increase enemy speed, to indicate he started running (even play run animation if you have). 
        // post-processing or graphics detailed effect can be enabled here as well. 
        print("Chasing"); 
        _agent.SetDestination(_player.position); 
    }

    void AttackPlayer()
    {
        // the enemy shouldn't move when attacking. 
        _agent.SetDestination(transform.position);

        transform.LookAt(_player); // this will rotate the player to the z direction. 
        
        if (!alreadyAttacked)
        {
            // put attack code sequence here
            print("Attacking..."); 
            //

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks); 
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false; 
    }

    // to visualize the attack and sight range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
