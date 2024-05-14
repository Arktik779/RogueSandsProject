using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK { 
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;
        public WeaponItem rightWeapon;

        private void Awake()
        {
           weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, true);
        }
    }

}