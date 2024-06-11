using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class SpikeTrapDemo : MonoBehaviour
    {
        public Animator spikeTrapAnim; // Animator for the SpikeTrap
        public BoxCollider spearTrapCollider; // Collider for the spear trap
        private Vector3 initialColliderPosition;

        private void Awake()
        {
            spikeTrapAnim = GetComponent<Animator>();
            if (spearTrapCollider == null)
            {
                spearTrapCollider = GetComponentInChildren<BoxCollider>();
            }
            initialColliderPosition = spearTrapCollider.transform.localPosition;
            StartCoroutine(OpenCloseTrap());
        }

        private void Update()
        {
            // Update collider position based on spear animation
            if (spikeTrapAnim.GetCurrentAnimatorStateInfo(0).IsName("Open") ||
                spikeTrapAnim.GetCurrentAnimatorStateInfo(0).IsName("Close"))
            {
                spearTrapCollider.transform.localPosition = GetUpdatedColliderPosition();
            }
        }

        private Vector3 GetUpdatedColliderPosition()
        {
            // Calculate the new position for the collider based on the animation
            // This is a placeholder calculation; adjust it based on your specific animation
            float offsetY = spikeTrapAnim.GetFloat("SpearHeight");
            return new Vector3(initialColliderPosition.x, initialColliderPosition.y + offsetY, initialColliderPosition.z);
        }

        private IEnumerator OpenCloseTrap()
        {
            while (true)
            {
                // Play open animation
                spikeTrapAnim.SetTrigger("open");
                // Wait 2 seconds
                yield return new WaitForSeconds(2);
                // Play close animation
                spikeTrapAnim.SetTrigger("close");
                // Wait 2 seconds
                yield return new WaitForSeconds(2);
            }
        }

        public void ResetTrigger()
        {
            Debug.Log("ResetTrigger called"); // Debug log to confirm the method is called

            // Access the child object and reset the trigger
            Transform spearTrapCollider = transform.Find("SpearTrapCollider");
            if (spearTrapCollider != null)
            {
                SpearTrapDamage spearTrapDamage = spearTrapCollider.GetComponent<SpearTrapDamage>();
                if (spearTrapDamage != null)
                {
                    spearTrapDamage.ResetTrigger();
                }
            }
        }

        public void ActivateTrap()
        {
            if (spikeTrapAnim != null)
            {
                spikeTrapAnim.SetTrigger("ActivateTrap");
            }
        }
    }
}