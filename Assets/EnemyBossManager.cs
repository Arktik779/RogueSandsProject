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

        // Attack management
        public float attackRange = 5f;      // Boss attack range
        public float attackCooldown = 1f;   // Cooldown between attacks
        public float lastAttackTime;         // To track when the boss can attack next
        public float stoppingDistance = 3f;  // Distance at which the boss stops before attacking
        public float attackWidth = 4f;       // Width of the attack range

        // Attack index
        public int attackIndex; // Added to store the current attack index

        // Camera shake effect
        public CameraShake cameraShake;  // CameraShake reference

        // State management
        private BossState currentState;
        public BossState activateState;
        public BossState chaseState;
        public BossState idleState;
        public BossState attackState;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            // Initialize states
            activateState = new ActivateState();
            chaseState = new ChaseState();
            idleState = new BossIdleState();
            attackState = new BossAttackState(); // Added attack state

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

        private void Start()
        {
            // Ensure NavMeshAgent is disabled at start
            navMeshAgent.enabled = false;
            SwitchState(idleState); // Start in the idle state to wait for player detection
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
            if (collision.gameObject.CompareTag("Player"))
            {
                // No physical interaction should be applied here since the Rigidbody is kinematic
            }
        }

        public bool IsPlayerInAttackRange()
        {
            if (currentTarget == null) return false;

            Vector3 targetPosition = currentTarget.position;
            Vector3 bossPosition = transform.position;

            // Calculate the direction to the target
            Vector3 directionToTarget = (targetPosition - bossPosition).normalized;

            // Calculate the attack range position in front of the boss
            Vector3 attackPosition = bossPosition + transform.forward * attackRange;

            // Check if the player is within the square range
            bool inHorizontalRange = Mathf.Abs(targetPosition.x - attackPosition.x) <= attackWidth / 2;
            bool inVerticalRange = targetPosition.z > bossPosition.z && Vector3.Distance(bossPosition, targetPosition) <= attackRange;

            return inHorizontalRange && inVerticalRange;
        }

        // Gizmo visualization for attack range
        private void OnDrawGizmos()
        {
            // Draw the attack range as a rectangle in front of the boss
            Gizmos.color = Color.red; // Color for the attack range visualization
            Vector3 frontLeft = transform.position + transform.forward * attackRange - transform.right * (attackWidth / 2);
            Vector3 frontRight = transform.position + transform.forward * attackRange + transform.right * (attackWidth / 2);
            Vector3 backLeft = transform.position + transform.forward * (attackRange - stoppingDistance) - transform.right * (attackWidth / 2);
            Vector3 backRight = transform.position + transform.forward * (attackRange - stoppingDistance) + transform.right * (attackWidth / 2);

            // Draw the rectangle
            Gizmos.DrawLine(frontLeft, frontRight);
            Gizmos.DrawLine(frontRight, backRight);
            Gizmos.DrawLine(backRight, backLeft);
            Gizmos.DrawLine(backLeft, frontLeft);
        }
    }

    // Abstract base class for all boss states
    public abstract class BossState
    {
        public abstract void EnterState(EnemyBossManager bossManager);
        public abstract void UpdateState(EnemyBossManager bossManager);
        public abstract void ExitState(EnemyBossManager bossManager); // Correct signature for exit state
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

        public override void ExitState(EnemyBossManager bossManager) { }
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

            // Trigger camera shake only during activation
            if (bossManager.cameraShake != null)
            {
                bossManager.cameraShake.TriggerShake(shakeDuration, shakeMagnitude);
            }

            // Continue waiting for the rest of the animation
            yield return new WaitForSeconds(animationDuration - shakeStartTime);

            // After the animation completes, enable NavMeshAgent and switch to ChaseState
            bossManager.navMeshAgent.enabled = true;
            bossManager.SwitchState(bossManager.chaseState);
        }

        public override void UpdateState(EnemyBossManager bossManager) { }

        public override void ExitState(EnemyBossManager bossManager) { }
    }

    // Chase State: Boss chases the player after activation
    public class ChaseState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            bossManager.animator.SetTrigger("StartChase");
            bossManager.navMeshAgent.isStopped = false;  // Allow movement
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            if (bossManager.currentTarget != null)
            {
                float distance = Vector3.Distance(bossManager.transform.position, bossManager.currentTarget.position);

                // Switch to attack if within range and cooldown allows
                if (bossManager.IsPlayerInAttackRange() && Time.time > bossManager.lastAttackTime + bossManager.attackCooldown)
                {
                    // Stay in attack state instead of switching
                    bossManager.SwitchState(bossManager.attackState); // Stay in attack state
                }
                else if (distance > bossManager.attackRange + bossManager.stoppingDistance)
                {
                    // Move towards the player
                    bossManager.navMeshAgent.SetDestination(bossManager.currentTarget.position); // Chase player
                }
            }
            else
            {
                bossManager.currentTarget = null; // Clear current target if lost
            }
        }

        public override void ExitState(EnemyBossManager bossManager) { } // Correct signature
    }

    // Boss Attack State: Boss attacks the player when in range
    public class BossAttackState : BossState
    {
        private System.Random random = new System.Random();  // To generate random attack index

        public override void EnterState(EnemyBossManager bossManager)
        {
            bossManager.navMeshAgent.isStopped = true;  // Stop the boss from moving

            // Select random attack index
            bossManager.attackIndex = random.Next(0, 3); // Assuming you have three attack types: 0, 1, 2

            // Set animator parameter for the attack
            bossManager.animator.SetInteger("AttackIndex", bossManager.attackIndex);
            bossManager.animator.SetTrigger("AttackTrigger");

            bossManager.lastAttackTime = Time.time;  // Record the time of attack

            // Use a coroutine to wait for attack animation to finish
            bossManager.StartCoroutine(WaitForAttackAnimation(bossManager));
        }

        private System.Collections.IEnumerator WaitForAttackAnimation(EnemyBossManager bossManager)
        {
            float attackDuration = 2f;  // Adjust based on the attack animation length
            yield return new WaitForSeconds(attackDuration);

            // After attack, check if the player is still in range
            if (bossManager.IsPlayerInAttackRange())
            {
                // Continue attacking
                bossManager.SwitchState(bossManager.attackState);
            }
            else
            {
                // If the player is out of range, switch back to chase state
                bossManager.navMeshAgent.isStopped = false;  // Resume movement after attack
                bossManager.SwitchState(bossManager.chaseState);  // Switch back to chasing the player
            }
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            // Continuous attack if in range
            if (bossManager.IsPlayerInAttackRange() && Time.time > bossManager.lastAttackTime + bossManager.attackCooldown)
            {
                EnterState(bossManager); // Re-enter attack state to continue attacking
            }
        }

        public override void ExitState(EnemyBossManager bossManager) { }
    }
}
