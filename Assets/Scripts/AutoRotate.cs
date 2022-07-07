using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : StateMachineBehaviour
{
    public float Speed;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var autoRotateFlag = animator.GetComponent<ModelData>();
        if (autoRotateFlag.StopAutoRotate)
        {
            return;
        }

        animator.transform.Rotate(Vector3.up * Speed);
    }
}