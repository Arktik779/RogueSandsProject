using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EK
{
    public class RangedEnemyAnimatorManager : AnimatorManager
    {
        RangedEnemy rangedEnemy;
        private void Awake()
        {
            anim = GetComponent<Animator>();
            rangedEnemy = GetComponentInParent<RangedEnemy>();
        }
        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            rangedEnemy.RigidBody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            rangedEnemy.RigidBody.velocity = velocity;
        }
    }

}
