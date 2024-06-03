using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK {
    public class ProjectileDamage : MonoBehaviour
    {
        public int currentWeaponDamage = 25;
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();



                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                    Destroy(this.gameObject);
                }
            }

        }
    }
}