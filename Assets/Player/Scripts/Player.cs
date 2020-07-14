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
    public float    smoothingAirborneX = .4f;
    public float    smoothingGroundedX = .1f;
    public float    smoothingAirborneY = .1f;

    [Header("Movement")]
    public float    moveSpeed = 10;
    private bool    isSliding = false;

    [Header("WallJumping")]
    public float        wallSlideSpeed   = 3;
    private bool        isWallSliding       = false;
    public float        slideSmoothing      = 2;
    private float       slideXSmoothing;

    private float       gravity;
    private float       jumpForce;
    private float       velocityXSmoothing;
    private float       velocityYSmoothing;
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

        //reset set the velocity to zero
        if (controller.collisions.below)
            velocity.y = 0;

        //smooth the input x speed
        isSliding = input.pDown;
        Movement();

        //make function
        if (input.pJump && !isWallSliding && controller.collisions.below)
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
            WallSliding();
        }
               

        if (controller.collisions.above)
            velocity.y = 0;

        if (!isWallSliding)
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Movement()
    {
        if (!isSliding)
        {
            slideXSmoothing = 0;

            float targetVelocityX = moveSpeed * input.pHorizontal;
            if (IsAirborne())
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothingAirborneX);
            }
            else
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothingGroundedX);
            }
        }
        else
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref slideXSmoothing, slideSmoothing);
        }
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

            velocity.x = jumpForce * -controller.collisions.horizontal;
            velocity.y = jumpForce;
        }

        float targetVelocityY = (input.pLeft || input.pRight) ? 0.0f : -wallSlideSpeed;
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, smoothingAirborneY);
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
