using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class RoomManager : MonoBehaviour
    {
        public List<EnemyManager> enemies; // Directly assigned enemies in the Room prefab

        private List<GameObject> portals = new List<GameObject>();
        private bool isPortalActivated = false;


        private void Update()
        {
            // Continuously check if all enemies are defeated
            CheckEnemiesDefeated();
        }

        public void AssignPortals(List<GameObject> newPortals)
        {
            portals = newPortals;
            foreach (GameObject portal in portals)
            {
                portal.SetActive(false); // Ensure all portals start inactive
            }
        }

        private void CheckEnemiesDefeated()
        {
            foreach (EnemyManager enemy in enemies)
            {
                if (!enemy.IsDead())
                {
                    return; // If any enemy is not dead, do nothing
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
                int randomIndex = Random.Range(0, portals.Count);
                portals[randomIndex].SetActive(true);
                
            }
           
        }
    }
}