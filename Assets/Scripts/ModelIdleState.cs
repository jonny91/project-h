using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelIdleState : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.rotation = Quaternion.identity;
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }
}
