using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EK {
    public class UIBossHealthbar : MonoBehaviour
    {
        Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        private void Start()
        {
            SetUIHealthBarToInactive();
        }

        public void SetUIHealthBarToActive()
        {
            slider.gameObject.SetActive(true);
        }
        
        public void SetUIHealthBarToInactive()
        {
            slider.gameObject.SetActive(false);
        }
        public void SetBossMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        public void SetBossCurrentHealth(int currentHealth)
        {
            slider.value = currentHealth;
        }
    }
}
