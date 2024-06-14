using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class ChestReward : MonoBehaviour
    {
        public int goldAwardedOnCompletion = 350;
        public GameObject EbuttonSign;
        public GameObject GoldParticle;
        public GameObject Chest;
        Animator anim;
        bool playerInRange = false;
        bool isChestOpened = false; 

        private void Awake()
        {
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isChestOpened)
            {
                OpenChest();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                EbuttonSign.SetActive(true);
                playerInRange = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                EbuttonSign.SetActive(false);
                playerInRange = false;
            }
        }

        private void OpenChest()
        {
            isChestOpened = true; // Mark the chest as opened
            anim.SetTrigger("OpenChest");
            RewardPlayer();
            GoldParticle.SetActive(true);
            StartCoroutine(CloseChestAfterDelay(3.0f)); // Adjust the delay as needed
            EbuttonSign.SetActive(false);
        }

        private void RewardPlayer()
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            GoldCountBar goldCountBar = FindObjectOfType<GoldCountBar>();

            if (playerStats != null)
            {
                playerStats.AddGold(goldAwardedOnCompletion);

                if (goldCountBar != null)
                {
                    goldCountBar.SetGoldCountText(playerStats.goldCount);
                }
            }
        }

        private IEnumerator CloseChestAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            anim.SetTrigger("CloseChest");
            GoldParticle.SetActive(false);
            Chest.SetActive(false);
        }
    }
}