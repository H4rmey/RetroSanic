using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask colMask;
    public const float skinWidth = .015f;
    public int horiRayCount = 4;
    public int vertRayCount = 4;

    [HideInInspector]
    public float horiRaySpacing;
    [HideInInspector]
    public float vertRaySpacing;

    [HideInInspector]
    public BoxCollider2D collider;
    [HideInInspector]
    public RaycastOrigins raycastOrigins;

    // Start is called before the first frame update
    public virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalcRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.topLeft      = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight     = new Vector2(bounds.max.x, bounds.max.y);
        raycastOrigins.bottomLeft   = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight  = new Vector2(bounds.max.x, bounds.min.y);
    }

    public void CalcRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horiRayCount    = Mathf.Clamp(horiRayCount, 2, int.MaxValue);
        vertRayCount    = Mathf.Clamp(horiRayCount, 2, int.MaxValue);
        horiRaySpacing  = bounds.size.y / (horiRayCount - 1);
        vertRaySpacing  = bounds.size.x / (vertRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
