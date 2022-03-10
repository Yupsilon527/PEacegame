using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] GameObject player; // reference to the player
    private float movementSpeed = 10f; // movement speed

    public Vector3[] points; // array storing destination coordinates
    private int pointNumber = 0; // default number of a destination point
    private Vector3 currentTarget; // current target coordinates

    private void Start()
    {
        if (points.Length > 0)
        {
            currentTarget = points[0]; // setting a current target to point 0
        }
    }

    void Update()
    {

        if (transform.position != currentTarget)
        {
            MovePlatform(); // moving a platform towards the target
        }
        else
        {
            UpdateTarget(); // updating the target if the platform reached current target
        }
    }

   private void MovePlatform()
   {
        Vector3 target = currentTarget - transform.position; // calculating target
        transform.position += target * movementSpeed * Time.deltaTime; // moving towards the target smoothly
   }

    private void UpdateTarget()
    {
        pointNumber++; // setting a new target
        if (pointNumber >= points.Length)
        {
            pointNumber = 0; // snapping a target back to 0
        }
        currentTarget = points[pointNumber];
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            player.transform.parent = transform; // binds player to the moving platform
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            player.transform.parent = null; // detaches player from the parent platform
        }
    }

}
