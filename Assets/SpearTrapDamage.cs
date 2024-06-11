using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class SpearTrapDamage : MonoBehaviour
    {
        public int damageAmount = 100;
        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !hasTriggered)
            {
                PlayerStats playerStats = other.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damageAmount);
                    hasTriggered = true;
                }
            }
        }
        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
}