using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

    namespace EK { 
    public class QuestManager : MonoBehaviour
    {
        public Text questText;
        private bool questCompleted = false;

        private void Start()
        {
            UpdateQuestText("Build a camp tent 0/1");
        }

        public void CompleteQuest()
        {
            questCompleted = true;
            UpdateQuestText("Build research laboratory 0/1");
        }
        private void UpdateQuestText(string newText) 
        {
        if (questText != null)
            {
                questText.text = newText;
            }
        
        }
    }

}
