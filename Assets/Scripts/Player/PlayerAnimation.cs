using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anim;

    PlayerAttack playerAttack;

    public Player player;

    public SpriteRenderer spriteRenderer;

    enum AnimState { none, idle, run, startJump, apexJump, dash, falling, attacking, groundStun};

    AnimState state;

    private void Start() {
        state = AnimState.idle;
        playerAttack = GetComponent<PlayerAttack>();
    }


    public void StartDash() {
        state = AnimState.dash;
        anim.Play("Dash");
    }

    public void StartJump() {
        Debug.Log("Start Jump");
        EndAttack();
        state = AnimState.startJump;
        anim.SetBool("OnGround", false);
        anim.SetBool("IsJumping", true);
        anim.Play("StartJump");
    }

    public void JumpApex() {
        Debug.Log("Apex");
        state = AnimState.apexJump;
        anim.Play("JumpApex");
    }

    public void Falling() {
        EndAttack();
        state = AnimState.falling;
        anim.SetBool("OnGround", false);
        anim.Play("Falling");
    }

    public void OnGround() {
        anim.SetBool("OnGround", true);
        anim.SetBool("IsJumping", false);
        anim.SetBool("HighFall", false);
        state = AnimState.idle;
    }

    public void Attack(string animationName) {
        anim.Play(animationName);
        state = AnimState.attacking;
    }

    void EndAttack() {
        playerAttack.onAttack = false;
    }

    public void EndAttackAnimation() {
        EndAttack();
        UpdateState();
    }

    void FlipSprite() {
        if (player.GetPlayerDirection() == -1)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }


    public void FallStun() {
        Debug.Log("Fall Stun");
        anim.SetBool("OnGround", true);
        anim.SetBool("IsJumping", false);
        anim.SetBool("HighFall", true);
        state = AnimState.groundStun;
    }

    public void UpdateState() {
        state = AnimState.none;
        Debug.Log("Reset State");
    }    

    private void Update() {
        if (state != AnimState.startJump && state != AnimState.apexJump && state != AnimState.falling && state != AnimState.attacking){
            bool isMoving = player.GetVelocity().magnitude >= .5f;
            anim.SetBool("IsMoving", isMoving);
            if (!isMoving)
                state = AnimState.idle;
        }

        FlipSprite();


        if (player.GetVelocity().y > 0) {
            if (state != AnimState.startJump) {
                StartJump();
            }
        }

        if (state == AnimState.startJump && player.GetVelocity().y < .1f) {
            JumpApex();
        }

        if (state != AnimState.falling && player.GetVelocity().y < -.1f)
            Falling();

    }
}
