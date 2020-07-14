using UnityEngine;
using System.Collections;

public class SurroundingCheck : RaycastController
{
    public CollisionsInfo collisions;

    public float raycastRange;
    
    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        UpdateRaycastOrigins();
        DoRayCastCheckHorizontal(true);     //right
        DoRayCastCheckHorizontal(false);    //left
        DoRayCastCheckVertical(true);       //top
        DoRayCastCheckVertical(false);      //bottom
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
                rayOrigin.y += (q * horiRaySpacing) + bounds.size.y / 3 * i;

                Vector2 rayDirection = (direction) ? Vector2.right : -Vector2.right;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastRange, colMask);
                if (hit)
                {
                    Debug.Log("i: " + i + "Dir: " + direction);
                    hithit = true;
                }

                Debug.DrawRay(rayOrigin, rayDirection * raycastRange, new Color(1.0f/i, 0.5f*i % 3, 0.3f * i));
            }



            switch (i)
            {
                case 2:
                    collisions.topLeft = (!direction) && hithit;
                    collisions.topRight = (direction) && hithit;
                    break;
                case 1:
                    collisions.midLeft = (!direction) && hithit;
                    collisions.midRight = (direction) && hithit;
                    break;
                case 0:
                    collisions.botLeft = (!direction) && hithit;
                    collisions.botRight = (direction) && hithit;
                    break;
                default:
                    break;
            }

            collisions.left = collisions.topLeft && collisions.midLeft && collisions.botLeft;
            collisions.right = collisions.topRight && collisions.midRight && collisions.botRight;

            //if (collisions.topLeft || collisions.topRight)
            //    Debug.Log("Top:" + (collisions.topLeft || collisions.topRight));
            //if (collisions.midLeft || collisions.midRight)
            //    Debug.Log("Mid:" + (collisions.midLeft || collisions.midRight));
            //if (collisions.botLeft || collisions.botRight)
            //    Debug.Log("Bot:" + (collisions.botLeft || collisions.botRight));
        }
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

        collisions.top      =  (direction) && hithit;
        collisions.bottom   = (!direction) && hithit;
    }
}
