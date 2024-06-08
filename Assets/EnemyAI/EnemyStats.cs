using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EK { 
    public class EnemyStats : CharacterStats
    {
        Animator animator;

        public UIEnemyHealthBar enemyHealthBar;
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }
        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
            enemyHealthBar.SetMaxHealth(maxHealth);
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
            enemyHealthBar.SetHealth(currentHealth);
            animator.Play("TakeDamage_02");

            //HANDLE PLAYER DEATH
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.Play("Death_01");
                isDead = true;
            }
        }
    }

}