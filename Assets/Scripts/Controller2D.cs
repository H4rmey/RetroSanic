using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    private float           maxClimbAngle = 45;
    private float           maxDescendAngle = 45;
    public CollisionsInfo   collisions;
    [HideInInspector]
    public InputHandler     inputHandler;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;

        inputHandler = GameObject.Find("SGameManager").GetComponent<InputHandler>();
    }

    public void Move(Vector3 velocity, bool isStandingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.x > -0.001 && velocity.x < 0.001)
            velocity.x = 0;

        if (velocity.x != 0)
            collisions.faceDir = (int)Mathf.Sign(velocity.x);

        if (velocity.y <= 0)
            DescendSlope(ref velocity);

        HoriCollisions(ref velocity);

        if (velocity.y != 0)
            VertCollisions(ref velocity);

        transform.Translate(velocity);

        if (isStandingOnPlatform)
        {
            collisions.below = true;
            collisions.vertical = -1;
        }

        //Debug.Log(velocity.x);
    }

    void HoriCollisions(ref Vector3 velocity)
    {
        float dirX = collisions.faceDir;
        float raylen = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            raylen = 2 * skinWidth;
        }

        for (int i = 0; i < vertRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horiRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, raylen, colMask);
            Debug.DrawRay(rayOrigin, Vector2.right * dirX * raylen, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i==0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * dirX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * dirX;
                }

                if (!collisions.climbinSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    raylen = hit.distance;

                    if (collisions.climbinSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left         = dirX == -1;
                    collisions.right        = dirX == 1;
                    collisions.horizontal   = (int)dirX;
                }
            }
        }
    }

    void VertCollisions(ref Vector3 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);
        float raylen = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < vertRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (vertRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, raylen, colMask);
            Debug.DrawRay(rayOrigin, Vector2.up * dirY * raylen, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * dirY;
                raylen = hit.distance;

                if (collisions.climbinSlope)
                {
                    velocity.x = Mathf.Abs(velocity.y) / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below    = dirY == -1;
                collisions.above    = dirY == 1;
                collisions.vertical = (int)dirY;
            }
        }

        if (collisions.climbinSlope)
        {
            float dirX = Mathf.Sign(velocity.x);
            raylen = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((dirX == 1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, raylen, colMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDist = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);

            collisions.below        = true;
            collisions.vertical     = -1;
            collisions.climbinSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);

        Vector2 rayOrigin = ((dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, colMask);
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == dirX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDist = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.below            = true;
                        collisions.vertical         = -1;
                        collisions.descendingSlope  = true;
                        collisions.slopeAngle       = slopeAngle;
                    }
                }
            }
        }
    }
      
}

public struct CollisionsInfo
{
    public bool     above, below;
    public bool     left, right;
    public int      horizontal, vertical;

    public bool     climbinSlope;
    public bool     descendingSlope;
    public float    slopeAngle, slopeAngleOld;
    public Vector3  velocityOld;
    public int      faceDir;

    public List<GameObject> otherGameObject;

    public void Reset()
    {
        above = below = false;
        left = right = false;
        climbinSlope = false;
        descendingSlope = false;

        slopeAngleOld = slopeAngle;
        slopeAngle = 0;
    }

    public bool topLeft, topRight;
    public bool midLeft, midRight;
    public bool botLeft, botRight;
    public bool top, bottom;
}