using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EK 
{
    public class EnemyManager : CharacterManager
    {
        EnemyLocomotionManager enemyLocomotionManager;
        bool isPerformingAction;

        [Header("A.I Settings")]
        public float detectionRadius = 20;
        // the higher or lower those angles are, the greater detection field of view is 
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;
        private void Awake() 
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        }

        private void Update()
        {
            HandleCurrentAction();
        }

        private void HandleCurrentAction()
        {
            if (enemyLocomotionManager.currentTarget == null) 
            {
                enemyLocomotionManager.HandleDetection();
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }


}
