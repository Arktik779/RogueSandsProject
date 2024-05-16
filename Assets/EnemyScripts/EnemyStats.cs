using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EK { 
    public class EnemyStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        Animator animator;
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }
        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }
        private int SetMaxHealthFromHealthLevel()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth = currentHealth - damage;

            animator.Play("TakeDamage_02");

            //HANDLE PLAYER DEATH
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.Play("Death_01");
            }
        }
    }

}