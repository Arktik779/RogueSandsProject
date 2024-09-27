using System;
using UnityEngine;
using UnityEngine.AI;

namespace EK
{
    public class RangedEnemy : MonoBehaviour
    {
        public event Action OnDeath; // Event for when the enemy dies

        public NavMeshAgent agent;
        public Transform player;
        public Rigidbody RigidBody;
        public EnemyMagicAttackAction[] enemyAttacks;
        public EnemyMagicAttackAction currentAttack;

        RangedEnemyAnimatorManager rangedAnimatorManager;
        EnemyStats enemyStats;

        public LayerMask whatIsGround, whatIsPlayer;

        // Patrolling
        public Vector3 walkPoint;
        bool walkPointSet;
        public float walkPointRange;

        // Attacking
        public float timeBetweenAttacks;
        public GameObject projectile;
        public GameObject bulletParent;

        private bool alreadyAttacked = false;
        private bool canShoot = true;

        // States
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;

        // Ragdoll Components
        private Rigidbody[] ragdollRigidbodies;
        private Collider[] ragdollColliders;

        private void Awake()
        {
            player = GameObject.Find("Player2").transform;
            agent = GetComponent<NavMeshAgent>();
            enemyStats = GetComponent<EnemyStats>();
            rangedAnimatorManager = GetComponentInChildren<RangedEnemyAnimatorManager>();

            // Initialize Ragdoll Components
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollColliders = GetComponentsInChildren<Collider>();

            // Disable ragdoll components initially
            SetRagdollState(false);
        }

        private void Update()
        {
            // Check if the enemy's health is 0 or less and update canShoot flag
            if (enemyStats.currentHealth <= 0 && canShoot)
            {
                canShoot = false;
                HandleDeath();
            }

            // Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (canShoot)
            {
                if (playerInSightRange && !playerInAttackRange) ChasePlayer();
                if (playerInAttackRange && playerInSightRange) AttackPlayer();
            }
            else
            {
                // Optionally handle enemy death or disable other behaviors
                rangedAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                agent.isStopped = true;
            }
        }

        private void SearchWalkPoint()
        {
            // Calculate random point in range
            float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
            float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;
        }

        private void ChasePlayer()
        {
            agent.SetDestination(player.position);
            rangedAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        }

        private void AttackPlayer()
        {
            // Enemy stops while attacking 
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            rangedAnimatorManager.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);

            if (!alreadyAttacked)
            {
                // Attack script
                Rigidbody rb = Instantiate(projectile, bulletParent.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rangedAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                rb.AddForce(transform.forward * 18f, ForceMode.Impulse);
                rb.AddForce(transform.up * 4f, ForceMode.Impulse);

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
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

        private void HandleDeath()
        {
            // Call the OnDeath event if needed
            if (OnDeath != null)
            {
                OnDeath.Invoke();
            }

            // Play death animation
            rangedAnimatorManager.PlayTargetAnimation("Death_01", true);

            // Stop the agent and disable AI control
            agent.isStopped = true;

            // Activate ragdoll physics
            SetRagdollState(true);
        }

        // Enables or disables ragdoll physics
        private void SetRagdollState(bool state)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.isKinematic = !state;
            }

            foreach (Collider col in ragdollColliders)
            {
                col.enabled = state;
            }

            // Disable the main collider
            GetComponent<Collider>().enabled = !state;
        }

        public bool IsDead()
        {
            return enemyStats.currentHealth <= 0;
        }
    }
}
