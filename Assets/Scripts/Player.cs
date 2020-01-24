﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    [Header("Player Upgrades")]
    public bool learnDoubleJump;
    public bool learnDash;


    #region Jump
    [Header("Jump")]
    [Space(10)]
    public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;


    [Range(0,1)]
    public float doubleJumpStrength;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .05f;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    public float maxFallVelocity;

    public int framesToJumpAfterLeaveGround;
    int actualFrameLeaveGround;

    bool canDoubleJump;

    #endregion

    #region WallJump
    [Space(10)]
    [Header("Wall Jump")]
    
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    public Vector2 wallJumpClimb;
    public Vector2 wallLeap;
    public bool slideOnWall;
    bool wallColliding;
    float timeToWallUnstick;
    int wallDirX;
    #endregion

    #region Dash
    [Space(10)]
    [Header("Dash")]
    public int framesToAccelDash;
    public int framesToDeccelDash;
    public int framesToConstantDashDuration;
    public float maxDashSpeed;
    public float startDashSpeed;
    public int framesToRechargeDash;
    public bool decceleratesInDash;
    bool canDash = true;
    bool onDashCooldown;
    DashState dashState;
    int dashDirection;
    enum DashState { none, accel, continuous, deccel };
    float actualDashSpeed;
    Vector3 dashDistance = Vector3.zero;

    int dashFrame;

    #endregion

    #region Move
    [Space(10)]
    [Header("Speed")]

    public float moveSpeed = 6;
    public float moveSpeedAfterDash;

    public int framesToStartChangeDirection;
    int changeDirectionActualFrame = 0;
    Vector3 velocity;
    float velocityXSmoothing;
    #endregion

    Controller2D controller;

    [HideInInspector]
	public Vector2 directionalInputs;

    int playerDirection = 1;

    public GameObject playerGhost;


	void Start() {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}


    public void UpdatePhysics() {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = 0;//Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

	void Update() {
        
		CalculateVelocity ();
        CalculateDashVelocity();
	    HandleWallSliding ();
        ResetDash();
		controller.Move (velocity * Time.deltaTime, directionalInputs);

		if (controller.collisions.above || controller.collisions.below) velocity.y = 0;
		
        UpdateLeaveGroundFrameWindow();
    }

    void UpdateLeaveGroundFrameWindow() {
        if (controller.collisions.below) {
            actualFrameLeaveGround = framesToJumpAfterLeaveGround;
            canDoubleJump = true;
        }
        else if (actualFrameLeaveGround > 0) actualFrameLeaveGround--;

    }

    public Vector3 GetVelocity() {
        return velocity;
    }

    void ResetDash() {
        if (dashState != DashState.none)
            return;

        if (controller.collisions.below) {
            canDash = true;
        }

        if (onDashCooldown) {
            dashFrame++;
            if (dashFrame >= framesToRechargeDash) {
                dashFrame = 0;
                onDashCooldown = false;
            }
        }
    }

    public void OnDashInput() {
        if (dashState == DashState.none && canDash && !onDashCooldown && learnDash) {
            dashState = DashState.accel;
            dashDirection = playerDirection;
            velocity = Vector2.zero;
            onDashCooldown = true;
            dashFrame = 0;
            canDash = false;
            dashDistance = transform.position;
        }
    }

    public void CalculateDashVelocity() {
        if (dashState == DashState.accel) {
            if (dashFrame >= framesToAccelDash) {
                dashFrame = 0;
                dashState = DashState.continuous;
            }
            else {
                float diffPerFrame = (maxDashSpeed - startDashSpeed) / framesToAccelDash;
                velocity.x = (startDashSpeed + dashFrame * diffPerFrame ) * dashDirection;
                dashFrame++;
            }
        }
        if (dashState == DashState.continuous) {

            if (decceleratesInDash) {
                if (dashFrame >= framesToConstantDashDuration) {
                    dashState = DashState.deccel;
                    dashFrame = 0;
                }
                else {
                    velocity.x = maxDashSpeed * dashDirection;
                    dashFrame++;
                }
            }
            else {
                if (dashFrame >= framesToConstantDashDuration - framesToAccelDash) {
                    dashState = DashState.none;
                    dashFrame = 0;
                    if (directionalInputs.x == 0)
                        velocity.x = 0;
                    else {
                        velocity.x = moveSpeed * dashDirection;
                    }

                }
                else {
                    velocity.x = maxDashSpeed * dashDirection;
                    dashFrame++;
                }
            }
        }
        
        if (dashState == DashState.deccel) {
            if (dashFrame >= framesToDeccelDash) {
                dashFrame = 0;
                dashState = DashState.none;

                if (directionalInputs.x == 0)
                    velocity.x = 0;
                else {
                    velocity.x = moveSpeed * dashDirection;
                }
                Debug.Log(Vector3.Distance(transform.position, dashDistance));
            }
            else {
                float diffPerFrame = (maxDashSpeed - moveSpeed) / framesToDeccelDash;
                Debug.Log("diff per frame: " + diffPerFrame);
                if (dashDirection == 1) {
                    velocity.x = Mathf.Max((maxDashSpeed - dashFrame * diffPerFrame) * dashDirection, dashDirection * moveSpeed);
                }
                else
                    velocity.x = Mathf.Min((maxDashSpeed - dashFrame * diffPerFrame) * dashDirection, dashDirection * moveSpeed);
                dashFrame++;
            }
        }
    }

	public void SetDirectionalInput (List<Vector2> input) {
		directionalInputs = input[input.Count - 1];
        if (dashState == DashState.none) {
            if (directionalInputs.x > 0) {
                if (playerDirection == -1) changeDirectionActualFrame = framesToStartChangeDirection;
                
                playerDirection = 1;
            }
            else if (directionalInputs.x < 0) {
                if (playerDirection == 1) changeDirectionActualFrame = framesToStartChangeDirection;
                playerDirection = -1;
            }
        }
    }

    public int GetPlayerDirection() {
        return playerDirection;
    }

	public void OnJumpInputDown() {
		if (wallColliding) {
			if (wallDirX == directionalInputs.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else {
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}
		if (actualFrameLeaveGround > 0) {
			velocity.y = maxJumpVelocity;
		}
        else if (learnDoubleJump && canDoubleJump) {
            canDoubleJump = false;
            velocity.y = maxJumpVelocity * doubleJumpStrength;
        }
	}

	public void OnJumpInputUp() {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}
		

	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallColliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallColliding = true;

            if (!slideOnWall)
                return;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInputs.x != wallDirX && directionalInputs.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}

		}

	}

	void CalculateVelocity() {

        float targetVelocityX = directionalInputs.x * moveSpeed;

        if (dashState == DashState.none) {
            if (changeDirectionActualFrame <= 0 || velocity.y != 0) {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                changeDirectionActualFrame = 0;
            }
            else
                changeDirectionActualFrame--;

        }

        float gravity = this.gravity;
        if (velocity.y  < 0) {
            //gravity *= 1.3f;
        }

        if (dashState == DashState.deccel || dashState == DashState.continuous) { 
		    velocity.y += gravity * 0.5f * Time.deltaTime;
        }
        else if (dashState == DashState.none)
            velocity.y += gravity * Time.deltaTime;

        if (velocity.y < -maxFallVelocity) {
            velocity.y = -maxFallVelocity;
        }
    }
}
