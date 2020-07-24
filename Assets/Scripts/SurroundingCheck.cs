using UnityEngine;
using System.Collections;

public class SurroundingCheck : RaycastController
{
    public TriggerInfo collisions;

    public float raycastRange;
    
    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// needs fixing
    /// horizontal check does not work
    /// insert coroutines to fix?
    /// aaaaa!!
    /// </summary>
    private void Update()
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        DoRayCastCheckHorizontal(false);    //left
        DoRayCastCheckHorizontal(true);     //right
        DoRayCastCheckVertical(true);       //top
        DoRayCastCheckVertical(false);      //bottom

        //Debug.Log("left: " + collisions.horizontal);
    }

    public void DoRayCastCheckHorizontal(bool direction)
    {
        CalcRaySpacing();
        for (int i = 0; i < 3; i++)
        {
            bool hithit = false;
            for (int q = 0; q < horiRayCount / 3; q++)
            {
                Vector2 rayOrigin = (direction) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
                rayOrigin.y += (q * horiRaySpacing)     + bounds.size.y / 3 * i     + horiRaySpacing/2;

                Vector2 rayDirection = (direction) ? Vector2.right : -Vector2.right;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastRange, colMask);
                if (hit)
                {
                    hithit = true;
                }

                if (!hithit)
                    Debug.DrawRay(rayOrigin, rayDirection * raycastRange, new Color(1.0f/i, 0.5f*i % 3, 0.3f * i));
                else
                    Debug.DrawRay(rayOrigin, rayDirection * raycastRange, Color.blue);
            }

            switch (i)
            {
                case 2:
                    collisions.topLeft = ((!direction) && hithit);
                    collisions.topRight = ((direction) && hithit);
                    break;
                case 1:
                    collisions.midLeft = ((!direction) && hithit);
                    collisions.midRight = ((direction) && hithit);
                    break;
                case 0:
                    collisions.botLeft = ((!direction) && hithit);
                    collisions.botRight = ((direction) && hithit);
                    break;
                default:
                    break;
            }

            collisions.left = (collisions.topLeft || collisions.midLeft || collisions.botLeft);
            collisions.right = (collisions.topRight || collisions.midRight || collisions.botRight);

            if (collisions.left)                            collisions.horizontal = -1;
            if (collisions.right)                           collisions.horizontal = 1;
            if (!collisions.right && !collisions.left)      collisions.horizontal = 0;
        }

        //yield return null;
    }

    public void DoRayCastCheckVertical(bool direction)
    {
        bool hithit = false;
        for (int q = 0; q < vertRayCount; q++)
        {
            Vector2 rayOrigin = (direction) ? raycastOrigins.topLeft : raycastOrigins.bottomLeft;
            rayOrigin.x = rayOrigin.x + (q * vertRaySpacing);

            Vector2 rayDirection = (direction) ? Vector2.up : -Vector2.up;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastRange, colMask);

            Debug.DrawRay(rayOrigin, rayDirection * raycastRange, Color.red);

            if (hit) hithit = true;
        }

        collisions.above      =  (direction) && hithit;
        collisions.below   = (!direction) && hithit;
    }     
    
}

public struct TriggerInfo
{
    public bool above, below;
    public int horizontal, vertical;

    public bool left, right;
    public bool topLeft, topRight;
    public bool midLeft, midRight;
    public bool botLeft, botRight;

    public void Reset()
    {
        above = below = false;
        left = right = false;
        horizontal = vertical = 0;

        topLeft = topRight = false;
        midLeft = midRight = false;
        botLeft = botRight = false;
    }
}
