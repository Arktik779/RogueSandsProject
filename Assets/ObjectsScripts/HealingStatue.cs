using System.Collections;
using UnityEngine;

namespace EK
{
    public class HealingStatue : MonoBehaviour
    {
        public GameObject EbuttonSign;
        public ParticleSystem healEffect;
        public GameObject healthIcon;
        private bool playerInRange = false;
        private bool used = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !used)
            {
                EbuttonSign.SetActive(true);
                playerInRange = true;
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EbuttonSign.SetActive(false);
                playerInRange = false;
                
            }
        }

        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                if (!used)
                {
                    PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                    if (playerStats != null)
                    {
                        RestoreHealth(playerStats);
                        StartCoroutine(ActivateHealEffect());
                        used = true;  
                        EbuttonSign.SetActive(false);  
                        healthIcon.SetActive(false);
                        
                    }
                    
                }
              
            }
        }

        private void RestoreHealth(PlayerStats playerStats)
        {
            if (playerStats.isDead)
                return;

            int healthToRestore = playerStats.maxHealth / 2;
            playerStats.currentHealth += healthToRestore;
            if (playerStats.currentHealth > playerStats.maxHealth)
            {
                playerStats.currentHealth = playerStats.maxHealth;
            }

            playerStats.healthbar.SetCurrentHealth(playerStats.currentHealth);
           
        }

        private IEnumerator ActivateHealEffect()
        {
            if (healEffect != null)
            {
                healEffect.gameObject.SetActive(true);
                healEffect.Play();
                yield return new WaitForSeconds(3);
                healEffect.Stop();
                healEffect.gameObject.SetActive(false); 
                
            }
            
        }
    }
}