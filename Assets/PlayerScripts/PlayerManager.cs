using EK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EK { 
public class PlayerManager : MonoBehaviour
{
        InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;

        public bool isInteracting;
        public bool canDoCombo;

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }


        void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        
        }

    
        void Update()
        {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
       
            
        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }

        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
        }

    }

}
