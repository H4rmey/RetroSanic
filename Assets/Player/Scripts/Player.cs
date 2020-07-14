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
    ///     isSliding
    /// </summary>

    private Inputs input;

    [Header("Jumping")]
    public float    jumpHeight = 3;
    public float    jumpAccend = .4f;


    [Header("Smooth Smooth, Cha Cha")]
    public float    smoothingAirborneX = .4f;
    public float    smoothingGroundedX = .1f;
    public float    smoothingWallSlidingY = .1f;
    public float    smoothingSlidingX = .75f;

    [Header("Movement")]
    public float    moveSpeed = 10;
    private bool    isSliding = false;

    [Header("WallJumping")]
    public float        wallSlideSpeed      = 3;
    private bool        isWallSliding       = false;

    private float       gravity;
    private float       jumpForce;

    private Vector2     smoothingFactor;
    private Vector2     targetVelocity;
    private Vector2     velocitySmoothing;
    [HideInInspector]
    public Vector3      velocity;

    private Controller2D controller;

    void Awake()
    {
        controller  = GetComponent<Controller2D>();
        gravity     = -(2 * jumpHeight) / Mathf.Pow(jumpAccend, 2);
        jumpForce   = Mathf.Abs(gravity * jumpAccend);
    }

    private void Update()
    {
        //get the user input
        input = controller.inputHandler.input;

        //reset set the velocity to zero. when grounded
        if (controller.collisions.below)
            velocity.y = 0;


        //get/set base parameters
        targetVelocity.x    = moveSpeed * input.pHorizontal;
        smoothingFactor.x   = (IsAirborne()) ? smoothingAirborneX : smoothingGroundedX;

        isSliding           = (input.pDown && !IsAirborne());
        isWallSliding       = (controller.collisions.left || controller.collisions.right);

        //handle that shit
        if (isWallSliding)
        {
            WallSliding();
            velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocitySmoothing.y, smoothingFactor.y);
        }
        if (!isWallSliding) //DO NOT MAKE ELSE IF, WILL BREAK RELEASING FROM WALLS <:o
        {
            Jump();
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocitySmoothing.x, smoothingFactor.x);
        }

        //final checks UwU
        if (controller.collisions.above)
            velocity.y = 0;

        if (!isWallSliding)
            velocity.y += gravity * Time.deltaTime;

        //apply to object
        controller.Move(velocity * Time.deltaTime);
    }

    private void WallSliding()
    {
        if (input.pDown || IsAirborne())
        {
            isWallSliding = false;
        }
        else if (input.pJump)
        {
            isWallSliding = false;

            //do a jump :)
            velocity.x = jumpForce * -controller.collisions.horizontal;
            velocity.y = jumpForce;
        }

        //release from walls
        if (controller.collisions.left)
        {
            isWallSliding       = !input.pRight;
            targetVelocity.y    = (input.pLeft) ? 0.0f : -wallSlideSpeed;
        }
        else if (controller.collisions.right)
        {
            isWallSliding       = !input.pLeft;
            targetVelocity.y    = (input.pRight) ? 0.0f : -wallSlideSpeed;
        }

        smoothingFactor.y   = smoothingWallSlidingY;
    }

    private void Jump()
    {
        if (input.pJump && controller.collisions.below)
        {
            //do a jump :D
            velocity.y = jumpForce;
        }
    }

    private bool IsAirborne()
    {
        if (!controller.collisions.below    &&
            !controller.collisions.left     &&
            !controller.collisions.right)
            return true;

        return false;
    }
}
