using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoIdleState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.transform.localScale = new Vector3(-1, 1, -1);
        animator.transform.localPosition = new Vector3(0, 2.5f, 0);
    }
}
