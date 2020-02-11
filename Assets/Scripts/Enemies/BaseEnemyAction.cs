using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEnemyAction : MonoBehaviour {

	[Header("Heuristic")]

	[SerializeField]
	public List<Condition> conditions;

	public float value;

	public float GetHeuristicValue() {
		return value;
	}

	[Header ("Action")]
	public int frames;

    public string actionAnimationName;

    public int staminaSpent;

    public string actionName;


    public FrameChecker actionFrameChecker;

    public AnimationClipExtended animationClip;

    protected Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    public virtual void DoAction() {

    }

}
