using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace EK
{
    public class LaboratoryScript : MonoBehaviour
    {
        public GameObject EbuttonSign;
        public GameObject Laboratory;
        bool playerInRange = false;
        public GameObject playerWithScript;
        public GoldCountBar goldCountBar;
        public GameObject LaboratoryConstructionArea;
        public QuestManager questManager;

        PlayerStats playerStats;

        private void Awake()
        {
            playerStats = playerWithScript.GetComponent<PlayerStats>();
            Laboratory.SetActive(false);

            if (goldCountBar == null)
            {
                goldCountBar = FindObjectOfType<GoldCountBar>();
            }
            if (questManager == null)
            {
                questManager = FindObjectOfType<QuestManager>();
            }


        }

        private void Update()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {

                ActivateTent();

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

        private void ActivateTent()
        {

            if (playerStats.goldCount >= 1000)
            {
                playerStats.AddGold(-1000);
                Laboratory.SetActive(true);
                EbuttonSign?.SetActive(false);
                LaboratoryConstructionArea.SetActive(false);
                

                if (goldCountBar != null)
                {
                    goldCountBar.SetGoldCountText(playerStats.goldCount);
                }

                if (questManager != null)
                {
                    questManager.CompleteQuest();
                }

            }

        }

    }

}
