using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK { 
    public class WeaponSlotManager : MonoBehaviour
    {
        WeaponHolderSlot rightHandSlot;
        DamageCollider rightHandDamageCollider;

        private void Awake()
        {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if(weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }
        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isRight)
        {
            if(isRight)
            {
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
            }
        }
        #region Handle Weapon's Damage Collider
        

        private void LoadRightWeaponDamageCollider()
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        public void OpenRightDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

       public void CloseRightDamageCollider() 
        {
            rightHandDamageCollider.DisableDamageCollider();
        }
        #endregion
    }


}
