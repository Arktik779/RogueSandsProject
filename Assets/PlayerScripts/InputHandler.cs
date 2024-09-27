using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool b_Input;
        public bool rb_Input;
        public bool rt_Input;

        public bool rollFlag;
        public bool comboFlag;

        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        AnimatorHandler animatorHandler;

        Vector2 movementInput;
        Vector2 cameraInput;

        public bool isAttacking = false; // Track whether the player is currently attacking

        private void Awake()
        {
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            }
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleAttackInput(delta);
        }

        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleRollInput(float delta)
        {
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
            if (b_Input)
            {
                rollFlag = true;
            }
        }

        private void HandleAttackInput(float delta)
        {
            rb_Input = inputActions.PlayerActions.RB.WasPerformedThisFrame();
            rt_Input = inputActions.PlayerActions.RT.WasPerformedThisFrame();

            // Handle light attack with RB Input
            if (rb_Input)
            {
                if (playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                }
                else
                {
                    if (playerManager.isInteracting) return;
                    if (playerManager.canDoCombo) return;

                    animatorHandler.anim.SetBool("isUsingRightHand", true);
                    playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
                }
            }

            // Handle heavy attack with RT Input
            if (rt_Input)
            {
                // Prevent initiating a new attack if currently attacking
                if (playerManager.isInteracting || isAttacking) return;

                isAttacking = true; // Set to true when starting an attack
                playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);

                // Coroutine to reset the attacking state
                StartCoroutine(ResetAttackStateAfterDelay());
            }
        }

        // Coroutine to reset the attacking state
        private IEnumerator ResetAttackStateAfterDelay()
        {
            float attackAnimationLength = animatorHandler.GetAnimationLength(playerInventory.rightWeapon.OH_Heavy_Attack_1);
            yield return new WaitForSeconds(attackAnimationLength);
            isAttacking = false; // Reset attacking state
        }
    }
}
