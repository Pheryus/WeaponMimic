using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEnemyAction : MonoBehaviour {
    public int frames;

    public string actionAnimationName;

    public int staminaSpent;

    public string actionName;

    public HeuristicFunction heuristic;
    
    public FrameChecker actionFrameChecker;

    public AnimationClipExtended animationClip;

    protected Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    public virtual void DoAction() {

    }

}
