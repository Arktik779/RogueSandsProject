using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EK
{
    public class QuestManager : MonoBehaviour
    {
        public Text questText;
        public int currentQuestIndex = 0;

        private List<string> quests = new List<string>
        {
            "Build a camp tent 0/1",
            "Go inside the pyramid",
            "Build a laboratory 0/1"
        };

        private void Start()
        {
            UpdateQuestText();
        }

        public void CompleteQuest()
        {
            if (currentQuestIndex < quests.Count - 1)
            {
                currentQuestIndex++;
                UpdateQuestText();
            }
            else
            {
                // All quests are completed
                questText.text = "All quests completed!";
            }
        }

        private void UpdateQuestText()
        {
            if (questText != null && currentQuestIndex < quests.Count)
            {
                questText.text = quests[currentQuestIndex];
            }
        }
    }
}