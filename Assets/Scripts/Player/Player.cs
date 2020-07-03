using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Jumping")]
    public float    jumpHeight = 3;
    public float    jumpAccend = .4f;
    public float    accelTimeAirborne = .2f;
    public float    accelTimeGrounded = .1f;

    [Header("Movement")]
    public float    moveSpeed = 3;
    public float    timeToMaxSpeed = 0.5f;
    public float    moveSpeedMin = 3;
    public float    moveSpeedMax = 8;

    [Header("WallJumping")]
    public Vector2      wallJumpClimb;
    public Vector2      wallJumpLeap;
    public Vector2      wallJumpRelease;
    public float        wallStickTime = 0.25f;
    private float       timeToWallUnstick;
    public float        wallSlideSpeedMax = 3;

    [Header("Boost")]
    public float        boostChargeTime;
    private float       boostForce;
    private float       resultingBoostForce;
    public Slider       boostSlider;
    public List<float>  boostNumbers;

    private float       gravity;
    private float       jumpForce;
    private Vector3     velocity;
    private float       velocityXSmoothing;
    private float       boostSmoothing;

    private Controller2D controller;

    void Awake()
    {
        controller  = GetComponent<Controller2D>();
        gravity     = -(2 * jumpHeight) / Mathf.Pow(jumpAccend, 2);
        jumpForce   = Mathf.Abs(gravity * jumpAccend);
    }

    void Update()
    {
        Vector2 input   = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX    = (controller.collisions.left) ? -1 : 1;
        
        float targetVelocityX   = input.x * moveSpeed;
        velocity.x              = Mathf.SmoothDamp(velocity.x, 
                                        targetVelocityX,
                                        ref velocityXSmoothing, 
                                        (controller.collisions.below) ? accelTimeGrounded : accelTimeAirborne);
        
        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) 
            && !controller.collisions.below 
            && velocity.y < 0)
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
        if (Input.GetButton("Jump"))
        {
            boostSlider.minValue = 0;
            boostSlider.maxValue = 1;
            boostSlider.value = boostForce;

            boostForce += Time.deltaTime / boostChargeTime;

            for (int i = 0; i < boostNumbers.Count; i++)
            {
                if (boostForce > 1/boostNumbers.Count)
                {
                    resultingBoostForce = boostNumbers[i];
                }
            }

            if (boostForce >= 1)
                boostForce = 0;
        }

        //this if statement was down instead of up
        if (Input.GetButtonUp("Jump"))
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
            }
            if (controller.collisions.below)
            {
                velocity.y = jumpForce * resultingBoostForce;
            }

            boostForce = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
