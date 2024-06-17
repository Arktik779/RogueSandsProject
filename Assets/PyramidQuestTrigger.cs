using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class PyramidQuestTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                QuestManager questManager = FindObjectOfType<QuestManager>();
                if (questManager != null && questManager.currentQuestIndex == 1) 
                {
                    questManager.CompleteQuest();
                }
            }
        }
    }
}