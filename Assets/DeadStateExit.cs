﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("DeathComplete", true);
    }
}
