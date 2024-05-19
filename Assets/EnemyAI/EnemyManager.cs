using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EK 
{
    public class EnemyManager : CharacterManager
    {
        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorManager enemyAnimationManager;
        public bool isPerformingAction;

        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;

        [Header("A.I Settings")]
        public float detectionRadius = 20;
        // the higher or lower those angles are, the greater detection field of view is 
        public float maximumDetectionAngle = 80;
        public float minimumDetectionAngle = -80;

        public float currentRecoveryTime = 0;
        private void Awake() 
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimationManager = GetComponentInChildren<EnemyAnimatorManager>();
        }

        private void Update()
        {
            HandleRecoveryTimer();
        }

        private void FixedUpdate()
        {
            HandleCurrentAction();
            
        }

        private void HandleCurrentAction()
        {
            if(enemyLocomotionManager.currentTarget != null)
            {
                enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);
            }
            
            if (enemyLocomotionManager.currentTarget == null) 
            {
                enemyLocomotionManager.HandleDetection();
            }
            else if (enemyLocomotionManager.distanceFromTarget > enemyLocomotionManager.stoppingDistance) 
            {
                enemyLocomotionManager.HandleMoveToTarget();
            }
            else if (enemyLocomotionManager.distanceFromTarget <= enemyLocomotionManager.stoppingDistance)
            {
                AttackTarget();
            }
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

        #region Attacks
        private void GetNewAttack()
        {
            Vector3 targetsDirection = enemyLocomotionManager.currentTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetsDirection, transform.forward);
            enemyLocomotionManager.distanceFromTarget = Vector3.Distance(enemyLocomotionManager.currentTarget.transform.position, transform.position);

            int maxScore = 0;

            for(int i = 0;i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if(enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                    && enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle 
                        && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
                
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            for (int i = 0; i < enemyAttacks.Length;i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (enemyLocomotionManager.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                    && enemyLocomotionManager.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                        && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                    {
                        if (currentAttack != null)
                        return;

                        temporaryScore += enemyAttackAction.attackScore;

                        if(temporaryScore > randomValue)
                        {
                            currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }

        #endregion

        private void AttackTarget()
        {

            if (isPerformingAction)
            return;


            if (currentAttack == null)
            {
                GetNewAttack();

            }
            else
            {
                isPerformingAction = true;
                currentRecoveryTime = currentAttack.recoveryTime;
                enemyAnimationManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                currentAttack = null;
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }


}