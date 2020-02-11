using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueAttack : BaseEnemyAction {

    public float dmg;

    public override void DoAction() {
        anim.Play("Start" + actionAnimationName);
        Debug.Log("Tongue Attack");
        //actionFrameChecker
    }
}
