using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [HideInInspector]
    public Inputs input;

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
    [HideInInspector]
    public bool        isWallSliding       = false;

    [Header("Attacka")]
    public float        attackRange     = 10;
    public float        attackForce     = 5;
    public float        attackContinue  = 2;

    private float       gravity;
    private float       jumpForce;

    private Vector2     smoothingFactor;
    private Vector2     targetVelocity;
    private Vector2     velocitySmoothing;
    [HideInInspector]
    public Vector3      velocity;

    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public SurroundingCheck surrounding;

    public UnityEvent JumpTrigger;

    public enum STATE {WALKING, AIRBORNE, IDLE, WALLSLIDING, SLIDING};
    public STATE state = STATE.IDLE;

    void Awake()
    {
        controller  = GetComponent<Controller2D>();
        surrounding = GetComponent<SurroundingCheck>();

        gravity     = -(2 * jumpHeight) / Mathf.Pow(jumpAccend, 2);
        jumpForce   = Mathf.Abs(gravity * jumpAccend);

        input.pHorizontal = 1;
        state = STATE.IDLE;
    }

    private void Update()
    {
        //get the user input
        input = controller.inputHandler.input;

        //reset set the velocity to zero. when grounded
        if (controller.collisions.below)
            velocity.y = 0;

        //set state to idle if not input is present
        if (input.pHorizontal == 0)
            state = STATE.IDLE;
        else if (!input.pDown)
            state = STATE.WALKING;

        if (state != STATE.AIRBORNE && input.pDown)
            state = STATE.SLIDING;

        if (controller.collisions.left || controller.collisions.right)
            state = STATE.WALLSLIDING;
        
        if (!controller.collisions.below && !controller.collisions.left && !controller.collisions.right)
            state = STATE.AIRBORNE;

        //get/set base parameters
        targetVelocity.x = moveSpeed * input.pHorizontal;
        smoothingFactor.x = (state == STATE.AIRBORNE) ? smoothingAirborneX : smoothingGroundedX;

        //TODO: needs some extra work for stopping power
        if (state == STATE.SLIDING)
        {
            targetVelocity.x = 0;
            smoothingFactor.x = smoothingSlidingX;
            if (velocity.x < 0.2)
                state = STATE.IDLE;
        }

        //handle that shit
        if (state == STATE.WALLSLIDING)
        {
            WallSliding();
            velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocitySmoothing.y, smoothingFactor.y);
        }

        if (state != STATE.WALLSLIDING) //DO NOT MAKE ELSE IF, WILL BREAK RELEASING FROM WALLS <:o
        {
            if (input.pJumpPressed && surrounding.collisions.below)
                Jump(new Vector2(0, jumpForce));

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocitySmoothing.x, smoothingFactor.x);
        }

        //final checks UwU
        if (controller.collisions.above)
            velocity.y = 0;

        if (state != STATE.WALLSLIDING || input.pDown)
            velocity.y += gravity * Time.deltaTime;

        //apply to object
        controller.Move(velocity * Time.deltaTime);

        //Debug.Log(state);
    }

    private void WallSliding()
    {
        if (input.pJumpPressed)
        {
            state = STATE.AIRBORNE;

            //do a jump :)
            Jump(new Vector2(jumpForce * -controller.collisions.horizontal, jumpForce));
        }

        //release from walls
        if (controller.collisions.left)
        {
            if (input.pRight)
                state = STATE.AIRBORNE;
            targetVelocity.y = (input.pLeft) ? 0.0f : -wallSlideSpeed;
        }
        if (controller.collisions.right)
        {
            if (input.pLeft)
                state = STATE.AIRBORNE;
            targetVelocity.y    = (input.pRight) ? 0.0f : -wallSlideSpeed;
        }

        smoothingFactor.y   = smoothingWallSlidingY;
    }

    private void Jump(Vector2 aDirection)
    {
        JumpTrigger.Invoke();

        if (aDirection.y == 0 && aDirection.x != 0)
            velocity.x = aDirection.x;
        else if (aDirection.x == 0 && aDirection.y != 0)
            velocity.y = aDirection.y;
        else
            velocity = aDirection;
    }
        
    private Collider2D GetClosestEnemy(Collider2D[] aEnemies)
    {
        Collider2D bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D enemy in aEnemies)
        {
            Transform potentialTarget = enemy.transform;

            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr && potentialTarget.tag == "Enemy")
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red * 0.5f;
        Gizmos.DrawSphere(this.transform.position, attackRange);
    }
}
