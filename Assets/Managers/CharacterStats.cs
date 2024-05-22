using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK { 
    public class CharacterStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;

        public bool isDead;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 pos = transform.position + transform.up;
            Gizmos.DrawLine(pos, pos + transform.forward * 5);
        }

    }
   


}