using UnityEngine;
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
    public int frameWindowWallCollision;
    public int frameWindowChangeWallJump;

    int actualFrameWallCollision;

    bool wallColliding;
    float timeToWallUnstick;
    int wallDirX;

    enum WallJump { none, slow, normal, fast};

    WallJump lastWallJump;

    int lastWallJumpDirection;
    int actualFrameLastWallJump;
    
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

    PlayerGhost playerGhost;

    Vector2 previousVelocity;

	void Start() {
		controller = GetComponent<Controller2D> ();
        playerGhost = GetComponent<PlayerGhost>();
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


        CalculateVelocity();
        CalculateDashVelocity();
	    HandleWallSliding ();
        ResetDash();
		controller.Move (velocity * Time.deltaTime, directionalInputs);

		if (controller.collisions.above || controller.collisions.below) velocity.y = 0;
		
        UpdateLeaveGroundFrameWindow();
        previousVelocity = velocity;
    }

    public int TotalDashDuration() {
        return framesToAccelDash + framesToDeccelDash + framesToConstantDashDuration;
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

    public bool IsDeccelerating() {
        return Mathf.Abs(previousVelocity.x) > Mathf.Abs(velocity.x);
    }

    public void OnDashInput() {
        if (dashState == DashState.none && canDash && !onDashCooldown && learnDash) {
            playerGhost.CreateGhost();
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
                EndDash();

            }
            else {
                float diffPerFrame = (maxDashSpeed - moveSpeed) / framesToDeccelDash;
                if (dashDirection == 1) {
                    velocity.x = Mathf.Max((maxDashSpeed - dashFrame * diffPerFrame) * dashDirection, dashDirection * moveSpeed);
                }
                else
                    velocity.x = Mathf.Min((maxDashSpeed - dashFrame * diffPerFrame) * dashDirection, dashDirection * moveSpeed);
                dashFrame++;
            }
        }
    }

    void EndDash() {
        dashFrame = 0;
        dashState = DashState.none;

        if (directionalInputs.x == 0)
            velocity.x = 0;
        else {
            velocity.x = moveSpeed * dashDirection;
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
        ChangeWallJump();        
    }

    void ChangeWallJump() {
        if (actualFrameLastWallJump > 0) {
            if (lastWallJump == WallJump.slow || lastWallJump == WallJump.normal) {
                if (lastWallJumpDirection == playerDirection) {
                    FastWallJump();
                }
            }
            actualFrameLastWallJump--;
        }
    }

    public int GetPlayerDirection() {
        return playerDirection;
    }

	public void OnJumpInputDown() {
		if (wallColliding) {
            lastWallJumpDirection = -wallDirX;
            actualFrameLastWallJump = frameWindowChangeWallJump;

            if (wallDirX == directionalInputs.x) {
                NormalWallJump();
			}
            else if (directionalInputs.x == 0) {
                NormalWallJump();
            }
			else {
                FastWallJump();
			}
            return;
		}
		if (actualFrameLeaveGround > 0) {
			velocity.y = maxJumpVelocity;
		}
        else if (learnDoubleJump && canDoubleJump) {
            canDoubleJump = false;
            velocity.y = maxJumpVelocity * doubleJumpStrength;
        }
	}

    void NormalWallJump() {
        
        velocity.x = -wallDirX * wallJumpClimb.x;
        velocity.y = wallJumpClimb.y;
        lastWallJump = WallJump.normal;
    }

    void SlowWallJump() {
        velocity.x = -wallDirX * wallJumpClimb.x / 2;
        velocity.y = wallJumpClimb.y / 2;
        lastWallJump = WallJump.slow;
    }

    void FastWallJump() {
        velocity.x = -wallDirX * wallLeap.x;
        velocity.y = wallLeap.y;
        playerGhost.CreateGhost(false);
        lastWallJump = WallJump.fast;
    }

	public void OnJumpInputUp() {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}
	
    public void EnterWallCollision() {
        if (dashState != DashState.none)
            EndDash();
    }

    public void ExitWallCollision() {

    }

    public void EnterGroundCollision() {
        lastWallJump = WallJump.none;
        Debug.Log("Touch Ground");
    }

	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallColliding = false;
        if ((controller.collisions.left || controller.collisions.right || controller.wallCollision || actualFrameWallCollision > 0) && !controller.collisions.below) {
            wallColliding = true;

            if (!(controller.collisions.left || controller.collisions.right || controller.wallCollision)) {
                actualFrameWallCollision--;
            }
            else {
                actualFrameWallCollision = frameWindowWallCollision;
            }   

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
        else if (actualFrameWallCollision > 0) {
            actualFrameWallCollision--;
        }

	}

	void CalculateVelocity() {

        float targetVelocityX = directionalInputs.x * moveSpeed;

        ///If player is normal walljumping, moving against the jump's direction will not make a difference
        if (lastWallJump == WallJump.normal && Mathf.Sign(targetVelocityX) != Mathf.Sign(velocity.x)) {
            targetVelocityX = 0;
			Debug.Log("aqui");
        }

        if (dashState == DashState.none) {
            if (changeDirectionActualFrame <= 0 || velocity.y != 0) {

                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
                changeDirectionActualFrame = 0;
            }
            else
                changeDirectionActualFrame--;

        }

        if (dashState == DashState.deccel || dashState == DashState.continuous) { 
		    velocity.y += gravity * 0.5f * Time.deltaTime;
        }
        else if (dashState == DashState.none)
            velocity.y += gravity * Time.deltaTime;

        if (velocity.y < -maxFallVelocity) {
            velocity.y = -maxFallVelocity;
        }

		if (Mathf.Abs(velocity.x) < wallJumpClimb.x / 2 && lastWallJump == WallJump.normal) {
			Debug.Log("desacelerou");
			lastWallJump = WallJump.none;
		} else {
			Debug.Log("n desacelerou");
		}



	}
}
