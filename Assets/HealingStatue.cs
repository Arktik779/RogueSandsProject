using System.Collections;
using UnityEngine;

namespace EK
{
    public class HealingStatue : MonoBehaviour
    {
        public GameObject EbuttonSign;
        public ParticleSystem healEffect;
        private bool playerInRange = false;
        private bool used = false;  // Ensure it happens only once

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !used)
            {
                EbuttonSign.SetActive(true);
                playerInRange = true;
                Debug.Log("Player entered range, can heal: " + !used);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EbuttonSign.SetActive(false);
                playerInRange = false;
                Debug.Log("Player exited range");
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
                        used = true;  // Set used to true to ensure it's only used once
                        EbuttonSign.SetActive(false);  // Hide "Press E" UI
                        Debug.Log("Healing activated");
                    }
                    else
                    {
                        Debug.Log("PlayerStats not found");
                    }
                }
                else
                {
                    Debug.Log("Healing already used");
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
            Debug.Log("Health restored to " + playerStats.currentHealth);
        }

        private IEnumerator ActivateHealEffect()
        {
            if (healEffect != null)
            {
                healEffect.gameObject.SetActive(true); // Make sure the particle system game object is active
                healEffect.Play();  // Activate particle effect
                Debug.Log("Particle effect activated");
                yield return new WaitForSeconds(3);  // Duration of the particle effect
                healEffect.Stop();  // Deactivate particle effect
                healEffect.gameObject.SetActive(false); // Optional: deactivate the particle system game object
                Debug.Log("Particle effect deactivated");
            }
            else
            {
                Debug.Log("Heal effect not assigned");
            }
        }
    }
}