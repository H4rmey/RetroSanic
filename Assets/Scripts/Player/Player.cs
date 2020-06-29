using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Jumping")]
    public float jumpHeight = 3;
    public float jumpAccend = .4f;
    public float accelTimeAirborne = .2f;
    public float accelTimeGrounded = .1f;

    [Header("Movement")]
    public float moveSpeed = 3;
    public float moveSpeedMax = 8;
    public int maxJumpsToTopSpeed = 3;
    float moveFactor;
    float boostStored;

    [Header("WallJumping")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpLeap;
    public Vector2 wallJumpRelease;
    public float wallStickTime = 0.25f;
    float timeToWallUnstick;
    public float wallSlideSpeedMax = 3;

    [Header("Boost")]
    public float boostForceMax;
    public float boostChargeTime;
    float boostForce;
    float resultingBoostForce;
    public Slider boostSlider;

    float gravity;
    float jumpForce;
    Vector3 velocity;
    float velocityXSmoothing;
    float boostSmoothing;

    Controller2D controller;

    void Awake()
    {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(jumpAccend, 2);
        jumpForce = Mathf.Abs(gravity * jumpAccend);
        moveFactor = (moveSpeedMax - moveSpeed) / maxJumpsToTopSpeed;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;
        
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelTimeGrounded : accelTimeAirborne);

        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;
                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        //this did not exist
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log(boostForce);

            boostSlider.minValue = 0;
            boostSlider.maxValue = boostForceMax;

            boostSlider.value = boostForce;

            boostForce += Time.deltaTime;

            if (boostForce > 1)
            {
                resultingBoostForce = 1;
            }
            if (boostForce > 2)
            {
                resultingBoostForce = 1.5f;
            }
            if (boostForce > 3)
            {
                resultingBoostForce = 2;
            }

            if (boostForce >= boostForceMax)
                boostForce = 0;
        }
        //this if statement was down instead of up
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (wallSliding)
            {
                if (wallDirX == input.x)
                {
                    velocity.x = -wallDirX * wallJumpClimb.y;
                    velocity.y = wallJumpClimb.y;
                }
                else if (input.x == 0)
                {
                    velocity.x = -wallDirX * wallJumpRelease.x;
                    velocity.y = wallJumpRelease.y;
                }
                else
                {
                    velocity.x = -wallDirX * wallJumpLeap.x;
                    velocity.y = wallJumpLeap.y;
                }
                moveSpeed += moveFactor;
            }
            if (controller.collisions.below)
            {
                //this line was added
                Debug.Log(resultingBoostForce);
                velocity.y = jumpForce * resultingBoostForce;
                moveSpeed += moveFactor;
            }

            if (moveSpeed > moveSpeedMax)
            {
                moveSpeed = moveSpeedMax;
            }

            boostForce = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
