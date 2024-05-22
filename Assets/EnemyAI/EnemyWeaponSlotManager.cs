using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK { 
    public class EnemyWeaponSlotManager : MonoBehaviour
    {
        public WeaponItem rightHandWeapon;
        WeaponHolderSlot rightHandSlot;

        DamageCollider rightHandDamageCollider;

        private void Awake()
        {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }

        private void Start()
        {
            LoadWeaponInHand(); 
        }

        public void LoadWeaponOnSlot(WeaponItem weapon,bool isRight)
        {
           if (isRight) 
            {
                rightHandSlot.currentWeapon = weapon;
                rightHandSlot.LoadWeaponModel(weapon);
                LoadWeaponsDamageCollider(false);
            } 
        }
        public void LoadWeaponsDamageCollider(bool isRight)
        {
            if(isRight)
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            }
        }

        public void LoadWeaponInHand()
        {
            if (rightHandWeapon != null)
            {
                LoadWeaponOnSlot(rightHandWeapon, true);
            }
        }
        public void OpenDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void EnableCombo()
        {
            //anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            //anim.SetBool("canDoCombo", false);
        }
    }

 }