using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class RoomManager : MonoBehaviour
    {
        public List<GameObject> portals; // Manually assigned portals in the Room prefab
        public List<EnemyManager> enemyManagers; // Directly assigned EnemyManager scripts in the Room prefab
        public List<RangedEnemy> rangedEnemies; // Directly assigned RangedEnemy scripts in the Room prefab

        private bool isPortalActivated = false;

        private void Start()
        {
            // Ensure all portals start inactive
            foreach (GameObject portal in portals)
            {
                if (portal != null)
                {
                    portal.SetActive(false);
                }
                else
                {
                }
            }
        }

        private void Update()
        {
            CheckEnemiesDefeated();
        }

        private void CheckEnemiesDefeated()
        {
            foreach (EnemyManager enemy in enemyManagers)
            {
                if (enemy == null)
                {
                    continue;
                }

                if (!enemy.IsDead())
                {
                    return; // If any EnemyManager is not dead, do nothing
                }
            }

            foreach (RangedEnemy enemy in rangedEnemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                if (!enemy.IsDead())
                {
                    return; // If any RangedEnemy is not dead, do nothing
                }
            }

            // If all enemies are dead, activate the portal
            if (!isPortalActivated)
            {
                ActivatePortal();
                isPortalActivated = true;
            }
        }

        public void ActivatePortal()
        {
            if (portals.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, portals.Count);
                if (portals[randomIndex] != null)
                {
                    portals[randomIndex].SetActive(true);
                }
            }

        }
    }
}