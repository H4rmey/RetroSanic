using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// Flags that are set and reset depending on the position/state of the player
    /// 
    /// CollisionInfo:
    ///     above, below;
    ///     left, right;
    ///     climbinSlope;
    ///     descendingSlope;
    ///     
    /// PlayerState:    
    ///     isWallSliding    
    /// </summary>

    private Inputs input;

    [Header("Jumping")]
    public float    jumpHeight = 3;
    public float    jumpAccend = .4f;
    public float    moveSmoothingAirborne = .4f;
    public float    moveSmoothingGrounded = .1f;

    [Header("Movement")]
    public float    moveSpeed = 10;

    [Header("WallJumping")]
    public float        wallSlideSpeedMax   = 3;
    private bool        isWallSliding       = false;

    private float       gravity;
    private float       jumpForce;
    private Vector3     velocity;
    private float       velocityXSmoothing;

    private Controller2D controller;

    void Awake()
    {
        controller  = GetComponent<Controller2D>();
        gravity     = -(2 * jumpHeight) / Mathf.Pow(jumpAccend, 2);
        jumpForce   = Mathf.Abs(gravity * jumpAccend);
    }

    private void Update()
    {
        input = controller.inputHandler.input;

        float targetVelocityX = moveSpeed * input.pHorizontal;
        Debug.Log(IsAirborne());
        if (IsAirborne())
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, moveSmoothingAirborne);
        else
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, moveSmoothingGrounded);

        //make function
        if (input.pJump                 && 
            controller.collisions.below &&
            !isWallSliding)
        {
            velocity.y = jumpForce;
        }

        //make function
        if (controller.collisions.left || controller.collisions.right)
        {
            isWallSliding = true;
        }

        if (isWallSliding)
        {
            if (controller.collisions.below ||
                (!controller.collisions.left &&
                !controller.collisions.right))
            {
                isWallSliding = false;
            }

            if (input.pLeft)
            {
                velocity.y = 0;
            }
            else
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (input.pJump)
            {
                isWallSliding = false;

                velocity.x = jumpForce * -controller.collisions.horizontal;
                velocity.y = jumpForce;
            }
        }

        if (!isWallSliding)
            velocity.y += gravity * Time.deltaTime;
       
        controller.Move(velocity * Time.deltaTime);
    }

    private bool IsAirborne()
    {
        if (!controller.collisions.above &&
            !controller.collisions.below &&
            !controller.collisions.left &&
            !controller.collisions.right)
            return true;
        return false;
    }

    //void Update()
    //{
    //    Vector2 input   = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //    int wallDirX    = (controller.collisions.left) ? -1 : 1;

    //    //smooth the movement isntead of snappy movement
    //    float targetVelocityX   = input.x * moveSpeed;
    //    velocity.x              = Mathf.SmoothDamp(velocity.x, 
    //                                    targetVelocityX,
    //                                    ref velocityXSmoothing, 
    //                                    (controller.collisions.below) ? accelTimeGrounded : accelTimeAirborne);
    //    if ((controller.collisions.left || controller.collisions.right) 
    //        && !controller.collisions.below 
    //        && velocity.y < 0)
    //    {
    //        isWallSliding = true;

    //        if (velocity.y < -wallSlideSpeedMax)
    //        {
    //            velocity.y = -wallSlideSpeedMax;
    //        }

    //        if (timeToWallUnstick > 0)
    //        {
    //            velocityXSmoothing = 0;
    //            velocity.x = 0;
    //            if (input.x != wallDirX && input.x != 0)
    //            {
    //                timeToWallUnstick -= Time.deltaTime;
    //            }
    //            else
    //            {
    //                timeToWallUnstick = wallStickTime;
    //            }
    //        }
    //        else
    //        {
    //            timeToWallUnstick = wallStickTime;
    //        }
    //    }

    //    if (controller.collisions.above || controller.collisions.below)
    //    {
    //        velocity.y = 0;
    //    }

    //    //this if statement was down instead of up
    //    if (Input.GetButtonUp("Jump"))
    //    {
    //        if (isWallSliding)
    //        {
    //            if (wallDirX == input.x)
    //            {
    //                velocity.x = -wallDirX * wallJumpClimb.y;
    //                velocity.y = wallJumpClimb.y;
    //            }
    //            else if (input.x == 0)
    //            {
    //                velocity.x = -wallDirX * wallJumpRelease.x;
    //                velocity.y = wallJumpRelease.y;
    //            }
    //            else
    //            {
    //                velocity.x = -wallDirX * wallJumpLeap.x;
    //                velocity.y = wallJumpLeap.y;
    //            }
    //        }
    //        if (controller.collisions.below)
    //        {
    //            velocity.y = jumpForce;
    //        }
    //    }

    //    velocity.y += gravity * Time.deltaTime;
    //    controller.Move(velocity * Time.deltaTime);
    //}
}
