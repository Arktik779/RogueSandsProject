using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class EnemyBossManager : MonoBehaviour
    {
        UIBossHealthbar bossHealthbar;
        EnemyStats enemyStats;
        public Animator animator;

        // State machine and states
        private BossState currentState;
        public BossState activateState;
        public BossState chaseState;

        [Header("Detection Settings")]
        public float detectionRadius = 20f;
        public float maximumDetectionAngle = 80f;
        public float minimumDetectionAngle = -80f;
        public LayerMask detectionLayer;

        [Header("Raycast Settings")]
        public Transform eyeTransform;

        private void Awake()
        {
            bossHealthbar = FindObjectOfType<UIBossHealthbar>();
            enemyStats = GetComponent<EnemyStats>();
            animator = GetComponentInChildren<Animator>();

            // Initialize states
            activateState = new ActivateState();
            chaseState = new ChaseState();
        }

        private void Start()
        {
            bossHealthbar.SetBossMaxHealth(enemyStats.maxHealth);
            // Optionally, you can set currentState to null
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.UpdateState(this);
            }

            DetectPlayer();
        }

        public void SwitchState(BossState newState)
        {
            if (currentState != null)
            {
                currentState.ExitState(this);
            }

            currentState = newState;
            currentState.EnterState(this);
        }

        private void DetectPlayer()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

            foreach (Collider target in colliders)
            {
                Vector3 targetDirection = (target.transform.position - transform.position).normalized;
                float viewAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewAngle > minimumDetectionAngle && viewAngle < maximumDetectionAngle)
                {
                    if (Physics.Raycast(eyeTransform.position, targetDirection, out RaycastHit hit, detectionRadius))
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            SwitchState(activateState);
                            break;
                        }
                    }
                }
            }
        }

        // Draw Gizmos to visualize the boss's sight range and field of view
        private void OnDrawGizmosSelected()
        {
            if (eyeTransform == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            Vector3 leftBoundary = DirectionFromAngle(transform.eulerAngles.y, minimumDetectionAngle);
            Vector3 rightBoundary = DirectionFromAngle(transform.eulerAngles.y, maximumDetectionAngle);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(eyeTransform.position, eyeTransform.position + leftBoundary * detectionRadius);
            Gizmos.DrawLine(eyeTransform.position, eyeTransform.position + rightBoundary * detectionRadius);
        }

        // Helper method to get a direction vector from an angle (used for vision cone)
        private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
        {
            angleInDegrees += eulerY;
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }

    // Abstract base class for boss states
    public abstract class BossState
    {
        public abstract void EnterState(EnemyBossManager bossManager);
        public abstract void UpdateState(EnemyBossManager bossManager);
        public abstract void ExitState(EnemyBossManager bossManager);
    }

    // Activate State: Boss plays an activation animation when the player is detected
    public class ActivateState : BossState
    {
        private bool hasPlayedActivationAnimation = false;

        public override void EnterState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss entered Activate State.");
            bossManager.animator.SetTrigger("Activate"); // Trigger the activation animation
            hasPlayedActivationAnimation = true;
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            if (hasPlayedActivationAnimation)
            {
                if (bossManager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    bossManager.SwitchState(bossManager.chaseState);
                }
            }
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss exited Activate State.");
            hasPlayedActivationAnimation = false; // Reset for next activation
        }
    }

    // Chase State: Boss chases the player after activation
    public class ChaseState : BossState
    {
        public override void EnterState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss entered Chase State.");
            bossManager.animator.SetTrigger("Run"); // Trigger the chase animation
        }

        public override void UpdateState(EnemyBossManager bossManager)
        {
            // Implement chasing logic here (e.g., move towards the player)
        }

        public override void ExitState(EnemyBossManager bossManager)
        {
            Debug.Log("Boss exited Chase State.");
        }
    }
}
