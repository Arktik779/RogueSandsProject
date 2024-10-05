using System;
using UnityEngine;
using UnityEngine.AI;

namespace EK
{
    public class EnemyBossManager : MonoBehaviour
    {
        public event Action OnDeath; // Event for when the enemy dies

        public NavMeshAgent navMeshAgent;  // Reference to NavMeshAgent
        public Transform currentTarget;     // Reference to the player target

        // Animator manager
        public Animator animator;

        // Detection settings
        public float detectionRadius = 20f; // Radius within which the boss detects the player
        public LayerMask whatIsPlayer;      // Layer to detect player

        // State management
        private BossState currentState;
        public BossState activateState;
        public BossState chaseState;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            // Initialize states
            activateState = new ActivateState();
            chaseState = new ChaseState();
        }

        private void Start()
        {
            SwitchState(activateState); // Start in the activation state
        }

        private void Update()
        {
            currentState?.UpdateState(this);
            DetectPlayer(); // Detect player continuously
        }

        public void SwitchState(BossState newState)
        {
            currentState?.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);
            Debug.Log($"Switched to state: {currentState.GetType().Name}");
        }

        private void DetectPlayer()
        {
            Debug.Log("Detecting Player...");

            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, whatIsPlayer);
            Debug.Log("Number of colliders found: " + colliders.Length);

            if (colliders.Length > 0) // If any players are detected
            {
                foreach (Collider target in colliders)
                {
                    Debug.Log("Detected collider: " + target.name);
                    currentTarget = target.transform; // Set the current target to the detected player

                    if (currentTarget != null)
                    {
                        Debug.Log("Current target set to: " + currentTarget.name);
                        // If we are in ActivateState and a target is detected, we can stay in this state
                        if (currentState is ActivateState)
                        {
                            return; // Stay in the current state, wait for the animation
                        }

                        // Switch to Chase State only if currently not in Activate State
                        if (!(currentState is ChaseState))
                        {
                            SwitchState(chaseState); // Switch to chase state when the player is detected
                        }
                        return; // Exit after detecting the player
                    }
                }
            }
            else
            {
                // If no player is detected and currently in chase state, stop chasing
                if (currentState is ChaseState)
                {
                    Debug.Log("No players detected. Stopping chase.");
                    currentTarget = null; // Clear current target
                    navMeshAgent.isStopped = true; // Stop the NavMeshAgent
                }
            }
        }
    }

    // Abstract base class for all boss states
    public abstract class BossState
    {
        public abstract void EnterState(EnemyBossManager bossManager);
        public abstract void UpdateState(EnemyBossManager bossManager);
        public abstract void ExitState(EnemyBossManager bossManager);
    }

    // Activate State: Plays activation animation when the player is detected
    public class ActivateState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss entered Activate State.");
            bossManager.animator.Play("BossActivate");
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            // Check if the activation animation is finished and target is not null
            AnimatorStateInfo stateInfo = bossManager.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 1f && bossManager.currentTarget != null)
            {
                Debug.Log("Boss completed activation animation, starting chase.");
                bossManager.SwitchState(bossManager.chaseState); // Switch to chase state
            }
            else if (bossManager.currentTarget == null)
            {
                Debug.Log("Current target is null, staying in Activate State.");
            }
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss exited Activate State.");
        }
    }

    // Chase State: Boss chases the player after activation
    public class ChaseState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss entered Chase State.");
            bossManager.animator.Play("BossRun");
            bossManager.navMeshAgent.isStopped = false; // Ensure the nav mesh agent is moving
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            if (bossManager.currentTarget != null)
            {
                Debug.Log("Chasing target: " + bossManager.currentTarget.name);
                bossManager.navMeshAgent.SetDestination(bossManager.currentTarget.position); // Chase player
            }
            else
            {
                Debug.Log("No current target to chase.");
                bossManager.navMeshAgent.isStopped = true; // Stop moving if there's no target
            }
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss exited Chase State.");
            bossManager.navMeshAgent.isStopped = true; // Stop moving when exiting the chase state
        }
    }
}
