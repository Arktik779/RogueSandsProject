using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class SpikeTrapDemo : MonoBehaviour
    {
        public Animator spikeTrapAnim;
        public BoxCollider spearTrapCollider; 
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
            
            if (spikeTrapAnim.GetCurrentAnimatorStateInfo(0).IsName("Open") ||
                spikeTrapAnim.GetCurrentAnimatorStateInfo(0).IsName("Close"))
            {
                spearTrapCollider.transform.localPosition = GetUpdatedColliderPosition();
            }
        }

        private Vector3 GetUpdatedColliderPosition()
        {
            
            float offsetY = spikeTrapAnim.GetFloat("SpearHeight");
            return new Vector3(initialColliderPosition.x, initialColliderPosition.y + offsetY, initialColliderPosition.z);
        }

        private IEnumerator OpenCloseTrap()
        {
            while (true)
            {
                
                spikeTrapAnim.SetTrigger("open");
                
                yield return new WaitForSeconds(2);
                
                spikeTrapAnim.SetTrigger("close");
                
                yield return new WaitForSeconds(2);
            }
        }

        public void ResetTrigger()
        {
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