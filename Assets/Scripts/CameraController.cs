using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Controller2D     playerController;

    public float            overShotMagnitude;
    public float            overShotTime;

    private Vector3         previousPosition;
    private Vector3         velocity;

    private void Start()
    {
    }

    private void LateUpdate()
    {
        velocity = (transform.position - previousPosition);
        previousPosition = transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, playerController.transform.position, ref velocity, overShotTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        //Debug.Log(velocity);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red * 0.5f;
    //    Gizmos.DrawSphere(playerController.transform.position, overShotMagnitude);
    //}
}
