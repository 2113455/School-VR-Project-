using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMover : MonoBehaviour
{
    private Transform waypoint1;
    private Transform waypoint2;
    private Transform target;

    private const float CLOSE_DISTANCE = 0.7f; // smaller threshold for switching
    private const float SPEED = 2.0f;          // slower movement for visibility

    [SerializeField]
    private bool flipLookDirection = false;

    void Start()
    {
        // Find the two empty waypoints in the scene
        waypoint1 = GameObject.Find("Waypoint1").transform;
        waypoint2 = GameObject.Find("Waypoint2").transform;

        // Start by moving toward the first waypoint
        target = waypoint1;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0; // keep it flat on the ground
        float distance = direction.magnitude;

        if (distance > 0)
        {
            Quaternion rotation = flipLookDirection
                ? Quaternion.LookRotation(-direction, Vector3.up)
                : Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = rotation;

            // Move slowly toward the current target
            transform.position += transform.forward * SPEED * Time.deltaTime;
        }

        // Switch target when close enough
        if (distance < CLOSE_DISTANCE)
        {
            target = (target == waypoint1) ? waypoint2 : waypoint1;
        }
    }
}


