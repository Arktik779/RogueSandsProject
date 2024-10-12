using System;
using UnityEngine;
using UnityEngine.AI;

namespace EK
{
    public class EnemyBossManager : MonoBehaviour
    {
        public event Action OnDeath; // Event for when the enemy dies

        public NavMeshAgent navMeshAgent;  // Reference to NavMeshAgent
        public Transform currentTarget;    // Reference to the player target

        // Animator manager
        public Animator animator;

        // Detection settings
        public float detectionRadius = 20f; // Radius within which the boss detects the player
        public LayerMask whatIsPlayer;      // Layer to detect player

        // State management
        private BossState currentState;
        public BossState activateState;
        public BossState chaseState;
        public BossState idleState; // Changed to BossIdleState

        // New flag to indicate if boss is activating
        public bool isActivating = false;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            // Initialize states
            activateState = new ActivateState();
            chaseState = new ChaseState();
            idleState = new BossIdleState(); // Use BossIdleState instead of IdleState
        }

        private void Start()
        {
            // Start in idle state to prevent any action until a player is detected
            SwitchState(idleState);
        }

        private void Update()
        {
            DetectPlayer(); // Continuously detect players
            currentState?.UpdateState(this); // Update the current state only if it's set
        }

        public void SwitchState(BossState newState)
        {
            currentState?.ExitState(this);
            currentState = newState;
            if (currentState != null)
            {
                currentState.EnterState(this);
                Debug.Log($"Switched to state: {currentState.GetType().Name}");
            }
        }

        private void DetectPlayer()
        {
            Debug.Log("Detecting Player...");

            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, whatIsPlayer);
            Debug.Log("Number of colliders found: " + colliders.Length);

            if (colliders.Length > 0 && !isActivating) // If any players are detected and not activating
            {
                currentTarget = colliders[0].transform; // Set the current target to the first detected player
                Debug.Log("Current target set to: " + currentTarget.name);

                // Only switch to activate state if currently idle and a target is detected
                if (currentState is BossIdleState)
                {
                    SwitchState(activateState); // Switch to activation state
                }
            }
            else
            {
                // If no player is detected and currently in ChaseState, stop chasing
                if (currentState is ChaseState)
                {
                    Debug.Log("No players detected. Stopping chase.");
                    currentTarget = null; // Clear current target
                    navMeshAgent.isStopped = true; // Stop the NavMeshAgent
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Prevent the boss from bouncing back by making sure we don't apply any physical reactions
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Boss collided with Player but no bouncing will occur.");
                // No physical interaction should be applied here since the Rigidbody is kinematic
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

    // Boss Idle State: Boss does nothing until a player is detected
    public class BossIdleState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss is now idle.");
            bossManager.navMeshAgent.isStopped = true; // Stop the NavMeshAgent in idle state
            bossManager.navMeshAgent.velocity = Vector3.zero; // Ensure no movement
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            // No specific logic for idle state; just wait for detection
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss exited BossIdle State.");
        }
    }

    // Activate State: Plays activation animation when the player is detected
    public class ActivateState : BossState
    {
        private CameraShake cameraShake;

        public override void EnterState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss entered Activate State.");
            bossManager.animator.SetTrigger("Activate"); // Use the Activate trigger
            bossManager.navMeshAgent.isStopped = true; // Stop the NavMeshAgent during activation
            bossManager.navMeshAgent.velocity = Vector3.zero; // Ensure no movement

            // Set activating flag to true
            bossManager.isActivating = true;

            // Find the CameraShake script and trigger screen shake
            cameraShake = GameObject.FindObjectOfType<CameraShake>();

            if (cameraShake != null)
            {
                cameraShake.TriggerShake(1f, 0.3f); // Trigger a shake with a duration of 1 second and magnitude of 0.3
            }
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
                Debug.Log("Current target is lost. Exiting activation state.");
                bossManager.SwitchState(bossManager.idleState); // Return to idle if no target
            }
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss exited Activate State.");
            bossManager.navMeshAgent.isStopped = false; // Allow the NavMeshAgent to move when exiting the activation state
            bossManager.isActivating = false; // Set activating flag to false
        }
    }

    // Chase State: Boss chases the player after activation
    public class ChaseState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss entered Chase State.");
            bossManager.animator.SetTrigger("StartChase"); // Use the StartChase trigger
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
