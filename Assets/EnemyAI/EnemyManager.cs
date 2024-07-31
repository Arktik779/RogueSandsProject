using System;
using UnityEngine;
using UnityEngine.AI;

namespace EK
{
    public class EnemyManager : CharacterManager
    {
        public event Action OnDeath; // Event for when the enemy dies
        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorManager enemyAnimationManager;
        EnemyStats enemyStats;

        public State currentState;
        public CharacterStats currentTarget;
        public NavMeshAgent navmeshAgent;
        public Rigidbody enemyRigidBody;

        public bool isPerformingAction;
        public bool isInteracting;
        public float rotationSpeed = 15;
        public float maximumAttackRange = 1.5f;

        [Header("A.I Settings")]
        public float detectionRadius = 20;
        public float maximumDetectionAngle = 80;
        public float minimumDetectionAngle = -80;

        public float currentRecoveryTime = 0;

        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimationManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStats = GetComponent<EnemyStats>();
            navmeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyRigidBody = GetComponent<Rigidbody>();
            navmeshAgent.enabled = false;
        }

        private void Start()
        {
            enemyRigidBody.isKinematic = false;
        }

        private void Update()
        {
            HandleRecoveryTimer();
            HandleStateMachine();

            isInteracting = enemyAnimationManager.anim.GetBool("isInteracting");

            if (enemyStats.isDead && OnDeath != null)
            {
                Debug.Log($"{gameObject.name} is dead, invoking OnDeath event.");
                OnDeath.Invoke(); // Trigger the OnDeath event
                OnDeath = null; // Unsubscribe all listeners to prevent multiple invocations
            }
        }

        private void LateUpdate()
        {
            navmeshAgent.transform.localPosition = Vector3.zero;
            navmeshAgent.transform.localRotation = Quaternion.identity;
        }

        private void HandleStateMachine()
        {
            if (enemyStats.isDead)
                return;

            if (currentState != null)
            {
                State nextState = currentState.Tick(this, enemyStats, enemyAnimationManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }

        private void SwitchToNextState(State state)
        {
            currentState = state;
        }

        private void HandleRecoveryTimer()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }
            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
            }
        }

        public bool IsDead()
        {
            return enemyStats.isDead;
        }
    }
}