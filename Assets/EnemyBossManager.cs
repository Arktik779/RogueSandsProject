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
        public BossState idleState; // Use BossIdleState for idle state

        // Reference to camera shake
        public CameraShake cameraShake;  // CameraShake reference

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            // Initialize states
            activateState = new ActivateState();
            chaseState = new ChaseState();
            idleState = new BossIdleState(); // Use BossIdleState
        }

        private void Start()
        {
            // Ensure NavMeshAgent is disabled at start
            navMeshAgent.enabled = false;
            SwitchState(idleState); // Start in the idle state to wait for player detection

            // Attempt to find CameraShake if not already assigned
            if (cameraShake == null)
            {
                cameraShake = FindObjectOfType<CameraShake>();
                if (cameraShake == null)
                {
                    Debug.LogError("CameraShake component not found in the scene!");
                }
            }
        }

        private void Update()
        {
            currentState?.UpdateState(this); // Update the current state
            DetectPlayer(); // Continuously detect players
        }

        public void SwitchState(BossState newState)
        {
            currentState?.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);
        }

        private void DetectPlayer()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, whatIsPlayer);

            if (colliders.Length > 0) // If any players are detected
            {
                currentTarget = colliders[0].transform; // Set the current target to the first detected player

                // Only switch to activate state if currently idle and a target is detected
                if (currentState is BossIdleState)
                {
                    SwitchState(activateState); // Switch to activation state
                }
            }
            else
            {
                // If no player is detected and currently in ChaseState, clear target
                if (currentState is ChaseState)
                {
                    currentTarget = null; // Clear current target
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Prevent the boss from bouncing back by making sure we don't apply any physical reactions
            if (collision.gameObject.CompareTag("Player"))
            {
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
            bossManager.navMeshAgent.enabled = false; // Ensure the NavMeshAgent is disabled
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            // No specific logic for idle state; just wait for detection
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            // This state does not need to handle re-enabling the agent
        }
    }

    // Activate State: Plays activation animation when the player is detected
    public class ActivateState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            bossManager.animator.SetTrigger("Activate");

            // Start coroutine to wait for animation completion and camera shake
            bossManager.StartCoroutine(WaitForActivationAnimation(bossManager));
        }

        private System.Collections.IEnumerator WaitForActivationAnimation(EnemyBossManager bossManager)
        {
            // Total animation duration
            float animationDuration = 7f;

            // Shake parameters
            float shakeStartTime = animationDuration / 3f; // Shake starts in the middle (3.5 seconds)
            float shakeDuration = 3f;
            float shakeMagnitude = 0.2f;

            // Wait for half the duration before triggering the camera shake
            yield return new WaitForSeconds(shakeStartTime);

            // Trigger camera shake
            if (bossManager.cameraShake != null)
            {
                bossManager.cameraShake.TriggerShake(shakeDuration, shakeMagnitude);
                Debug.Log("Camera Shake triggered.");
            }

            // Continue waiting for the rest of the animation
            yield return new WaitForSeconds(animationDuration - shakeStartTime);

            // After the animation completes, enable NavMeshAgent and switch to ChaseState
            bossManager.navMeshAgent.enabled = true;
            bossManager.SwitchState(bossManager.chaseState);
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            // No update logic needed; coroutine handles state transition
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            // No exit logic needed for activation state
        }
    }

    // Chase State: Boss chases the player after activation
    public class ChaseState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            bossManager.animator.SetTrigger("StartChase");
            // NavMeshAgent is enabled and will chase the player
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            if (bossManager.currentTarget != null)
            {
                bossManager.navMeshAgent.SetDestination(bossManager.currentTarget.position); // Chase player
            }
            else
            {
                bossManager.currentTarget = null; // Clear current target if lost
            }
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            // No exit logic needed for chase state
        }
    }
}
