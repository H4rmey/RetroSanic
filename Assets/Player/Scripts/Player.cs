using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent JumpTrigger;

    void Awake()
    {
        controller  = GetComponent<Controller2D>();
        gravity     = -(2 * jumpHeight) / Mathf.Pow(jumpAccend, 2);
        jumpForce   = Mathf.Abs(gravity * jumpAccend);

        input.pHorizontal = 1;
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
            if (input.pJump)
                Jump(new Vector2(0, jumpForce));

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocitySmoothing.x, smoothingFactor.x);
        }

        if (input.pInteractPressed && controller.collisions.below) StartCoroutine("Attack");

        //final checks UwU
        if (controller.collisions.above)
            velocity.y = 0;

        if (input.pDown || IsAirborne())
            velocity.y += gravity * Time.deltaTime;

        //apply to object
        controller.Move(velocity * Time.deltaTime);
    }

    private void WallSliding()
    {
        if (input.pJump)
        {
            isWallSliding = false;

            //do a jump :)
            Jump(new Vector2(jumpForce * -controller.collisions.horizontal, jumpForce));
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

    public bool IsAirborne()
    {
        if (controller.collisions.below    ||
            controller.collisions.left     ||
            controller.collisions.right)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private IEnumerator Attack()
    {
        Collider2D[] targets        = Physics2D.OverlapCircleAll(transform.position, attackRange, controller.colMask);
        Collider2D targetCollider   = GetClosestEnemy(targets);
        Vector3 target              = targetCollider.transform.position;
        Vector3 direction           = (target - transform.position);
        float distance              = attackRange;

        while (distance > attackContinue && targetCollider != null)
        {
            velocity.x = (target.x - transform.position.x) * attackForce;
            velocity.y = (target.y - transform.position.y) * attackForce;

            controller.Move(velocity * Time.deltaTime);

            distance = (target - transform.position).sqrMagnitude;
            yield return null;
        }

        Destroy(targetCollider.transform.gameObject);
        Jump(new Vector2((target - transform.position).x * jumpForce, -(target - transform.position).y * jumpForce));

        yield return null;
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
