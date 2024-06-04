using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.AI;

namespace EK { 
    public class RangedEnemy : MonoBehaviour
    {
        public NavMeshAgent agent;

        public Transform player;

        public Rigidbody RigidBody;

        RangedEnemyAnimatorManager rangedAnimatorManager;

        public EnemyMagicAttackAction[] enemyAttacks;

        public EnemyMagicAttackAction currentAttack;

        public LayerMask whatIsGround, whatIsPlayer;

        //Patrolling
        public Vector3 walkPoint;
        bool walkPointSet;
        public float walkPointRange;

        //Attacking
        public float timeBetweenAttacks;
        bool alreadyAttacked;
        public GameObject projectile;
        public GameObject bulletParent;

        //States
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;

        private void Awake()
        {
            player = GameObject.Find("Player2").transform;
            agent = GetComponent<NavMeshAgent>();
            rangedAnimatorManager = GetComponentInChildren<RangedEnemyAnimatorManager>();
        }
        private void Update()
        {
            //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        private void Patrolling()
        {
            if (!walkPointSet) SearchWalkPoint();

            if(walkPointSet)
            agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }

        private void SearchWalkPoint()
        {
           // Calculate random point in range
           float randomZ = Random.Range(-walkPointRange, walkPointRange);
           float randomX = Random.Range(-walkPointRange, walkPointRange);

           walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
           if (Physics.Raycast(walkPoint,-transform.up, 2f, whatIsGround))
            walkPointSet = true;
        }

        private void ChasePlayer()
        {
            agent.SetDestination(player.position);
            rangedAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            

        }

        private void AttackPlayer()
        {
            rangedAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            //Enemy stops while attacking 
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            rangedAnimatorManager.anim.SetFloat("Vertical",0, 0.1f, Time.deltaTime);

            if (!alreadyAttacked)
            {
                
                //Attack script
                Rigidbody rb = Instantiate(projectile,bulletParent.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rangedAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                rb.AddForce(transform.forward * 20f,ForceMode.Impulse);
                rb.AddForce(transform.up * 4f, ForceMode.Impulse);
                




                alreadyAttacked = true;
                Invoke(nameof(ResetAttack),timeBetweenAttacks);
                rangedAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            }
        }
        private void ResetAttack()
        {
            alreadyAttacked = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sightRange);

        }

    }



}