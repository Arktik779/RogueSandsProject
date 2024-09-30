using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK { 
    public class CamelInteraction : MonoBehaviour
    {
        private Animator animator;
        public Animator playerAnimator;
        private bool canPet = false;
        public GameObject EbuttonSign;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) ;
            {
                EbuttonSign.SetActive(true);
                canPet = true ;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EbuttonSign.SetActive(false);
                canPet = false;
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (canPet && Input.GetKeyDown(KeyCode.E)) 
            {
                PetCamel();
            }
        }
        void PetCamel()
        {
            animator.SetTrigger("Pet");
            playerAnimator.SetTrigger("PlayerPet");
        }
    }

}
