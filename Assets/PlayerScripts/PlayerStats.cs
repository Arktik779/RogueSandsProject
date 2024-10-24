using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EK { 
    public class PlayerStats : CharacterStats
    {
        public GameOverScreen GameOverScreen;
        public HealthBar healthbar;
        AnimatorHandler animatorHandler;
        GameObject deathScreenBackground;
        public GameObject deathScreen;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }


        //Max Health on Start
        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            healthbar.SetMaxHealth(maxHealth);
        }

        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (isDead)
                return;
            currentHealth = currentHealth - damage;

            healthbar.SetCurrentHealth(currentHealth);

            animatorHandler.PlayTargetAnimation("TakeDamage_02", true);

            //HANDLE PLAYER DEATH
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animatorHandler.PlayTargetAnimation("Death_01", true);
                isDead = true;
                deathScreen.SetActive(true);
            }
        }

        public void AddGold(int gold)
        {
            goldCount = goldCount + gold;
        }

    }
}

