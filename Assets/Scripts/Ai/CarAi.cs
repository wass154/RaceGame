using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAi : MonoBehaviour
{
    public Transform[] waypoints;
    public float waypointRadius = 1f;
    public float moveSpeed = 10f;
    public float turnSpeed = 2f;
    public float driftAmount = 0.1f;
    public float driftTurnThreshold = 0.9f;

    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentWaypoint = 0;

    public Rigidbody rb;
    public Vector3 Mass;
    void Start()
    {
        rb.centerOfMass = Mass;
        CalculatePath();
    }

    void CalculatePath()
    {
        // initialize the list of path points with the waypoints
        foreach (Transform waypoint in waypoints)
        {
            pathPoints.Add(waypoint.position);
        }

        // add points between the waypoints to make the path smoother
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 pointA = pathPoints[i];
            Vector3 pointB = pathPoints[i + 1];
            Vector3 direction = (pointB - pointA).normalized;
            float distance = Vector3.Distance(pointA, pointB);
            int numPoints = Mathf.FloorToInt(distance / (2f * waypointRadius));
            for (int j = 0; j < numPoints; j++)
            {
                Vector3 point = pointA + (j + 1) * distance / (numPoints + 1) * direction + Random.insideUnitSphere * 0.1f;
                pathPoints.Insert(i + 1 + j, point);
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 targetWaypoint = pathPoints[currentWaypoint];
        Vector3 moveDirection = targetWaypoint - transform.position;
        float distanceToWaypoint = moveDirection.magnitude;

        // if the car is close enough to the current waypoint, move to the next one
        if (distanceToWaypoint < waypointRadius)
        {
            currentWaypoint++;
            if (currentWaypoint >= pathPoints.Count)
            {
                currentWaypoint = 0;
            }
        }

        // calculate the desired rotation of the car based on the move direction
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        // apply forward movement to the car
        if (Physics.Raycast(transform.position, Vector3.down, 1f))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        // apply drift to the car when turning
        float dot = Vector3.Dot(transform.right, moveDirection.normalized);
        if (dot > driftTurnThreshold || distanceToWaypoint < 2 * waypointRadius || Vector3.Angle(transform.forward, moveDirection) > 30f)
        {
            float drift = dot * driftAmount;
            transform.Translate(Vector3.right * drift * moveSpeed * Time.deltaTime);
        }
    }
}

