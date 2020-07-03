using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Controller2D     playerController;
    public Vector2          size;
    public float            vertOffset;
    private FocusArea       focusArea;

    private void Start()
    {
        focusArea = new FocusArea(playerController.GetComponent<BoxCollider2D>().bounds, size);
    }

    private void LateUpdate()
    {
        focusArea.Update(playerController.GetComponent<BoxCollider2D>().bounds);

        Vector2 focusPos    = focusArea.center + Vector2.up * vertOffset;
        transform.position  = (Vector3)focusPos + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red * 0.5f;
        Gizmos.DrawCube(focusArea.center, size);
    }

    public struct FocusArea
    {
        float left, right, top, bottom;
        public Vector2 center;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            center = new Vector2((right + left) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds bounds)
        {
            float shiftX = 0;

            if (bounds.min.x < left)
            {
                shiftX = left - bounds.min.x;
            }
            else if (bounds.max.x > right)
            {
                shiftX = right - bounds.max.x;
            }

            left -= shiftX;
            right -= shiftX;



            float shiftY = 0;

            if (bounds.min.y < bottom)
            {
                shiftY = bottom - bounds.min.y;
            }
            else if (bounds.max.y > top)
            {
                shiftY = top - bounds.max.y;
            }

            bottom -= shiftY;
            top -= shiftY;

            center = new Vector2((right + left) / 2, (top + bottom) / 2);
        }
    }
}
