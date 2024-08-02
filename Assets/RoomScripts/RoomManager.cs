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
            Debug.Log("RoomManager Start called.");
            // Ensure all portals start inactive
            foreach (GameObject portal in portals)
            {
                if (portal != null)
                {
                    portal.SetActive(false);
                    Debug.Log($"Portal {portal.name} set to inactive.");
                }
                else
                {
                    Debug.LogWarning("Found a null portal in the portals list.");
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
                    Debug.LogWarning("Found a null EnemyManager in the enemies list.");
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
                    Debug.LogWarning("Found a null RangedEnemy in the enemies list.");
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
            Debug.Log("Activating portal");
            if (portals.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, portals.Count);
                if (portals[randomIndex] != null)
                {
                    portals[randomIndex].SetActive(true);
                    Debug.Log($"Portal {portals[randomIndex].name} is now active.");
                }
                else
                {
                    Debug.LogError("Randomly selected portal is null.");
                }
            }
            else
            {
                Debug.LogError("No portals available to activate.");
            }
        }
    }
}