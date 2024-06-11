using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EK
{
    public class SwingTrapDamage : MonoBehaviour
    {
        Collider damageCollider;
        public int currentWeaponDamage = 100;

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();



                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }

        }
    }

}
