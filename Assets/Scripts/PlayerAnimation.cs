using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    public Player player;

    public SpriteRenderer spriteRenderer;

    enum AnimState { none, idle, run};

    AnimState state;

    private void Start() {
        state = AnimState.none;
    }


    private void Update() {
        if (player.GetVelocity().magnitude < 1f){
            anim.SetBool("IsMoving", false);
        }
        else {
            anim.SetBool("IsMoving", true);
        }


        if (player.GetPlayerDirection() == -1)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }
}
