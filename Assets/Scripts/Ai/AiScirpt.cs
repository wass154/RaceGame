using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiScirpt : MonoBehaviour
{
    [Header("WayPoints And Car AI Settings")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] float waypointRadius = 1f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float turnSpeed = 2f;
    [SerializeField] float driftAmount = 0.1f;
    [SerializeField] float driftTurnThreshold = 0.9f;

    [Header("WayPoints")]
    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentWaypoint = 0;

    private bool isDrifting = false;

    private Vector3 prevPosition;
    private Vector3 moveDirection;
    private Vector3 forwardDirection;
    private Quaternion targetRotation;
    private float currentDrift;
    void Start()
    {
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("placeDrift"))
        {
            isDrifting = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("placeDrift"))
        {
            isDrifting = false;
        }
    }

    void FixedUpdate()
    {
        // calculate target waypoint and move direction
        Vector3 targetWaypoint = pathPoints[currentWaypoint];
        moveDirection = targetWaypoint - transform.position;
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
        targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        // apply forward movement to the car
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // calculate drift if the car is turning and drifting is enabled
    if (isDrifting)
{
    float dot = Vector3.Dot(transform.right, moveDirection.normalized);
    if (dot > driftTurnThreshold)
    {
        // calculate the drift direction based on the turn direction and current forward direction
        Vector3 driftDirection = Vector3.Cross(moveDirection.normalized, Vector3.up);
        float targetDrift = dot * driftAmount;
      currentDrift = Mathf.Lerp(currentDrift, targetDrift, Time.deltaTime * 10f);
        transform.Translate(driftDirection * currentDrift * moveSpeed * Time.deltaTime, Space.World);
    }
    else
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
else
{
    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
}
    }
}