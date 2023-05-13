using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCam : MonoBehaviour
{
    // Camera movement variables
    public Transform target; // Target object to follow
    public float distance = 5f; // Distance between camera and target
    public float height = 2f; // Height offset from target
    public float damping = 5f; // Damping factor for camera movement
    public float rotationDamping = 10f; // Damping factor for camera rotation
    public float maxSpeed = 100f; // Maximum speed for camera movement
    public float acceleration = 20f; // Acceleration factor for camera movement
    public float brakeDeceleration = 30f; // Deceleration factor for camera braking
    public float airResistance = 1f; // Air resistance factor for camera movement

    // Private variables
    private float currentSpeed = 0f; // Current speed of camera movement
    private bool isBraking = false; // Flag for braking state
    private Vector3 lastPosition; // Last position of camera

    // Update is called once per frame
    void Update()
    {
        // Get input for acceleration and braking
        float accelerationInput = Input.GetAxis("Vertical");
        bool brakeInput = Input.GetKey(KeyCode.Space);

        // Calculate acceleration and deceleration based on input
        float accelerationFactor = accelerationInput * acceleration;
        float decelerationFactor = brakeInput ? brakeDeceleration : airResistance;

        // Apply acceleration and deceleration to current speed
        if (accelerationInput > 0f && currentSpeed < maxSpeed)
        {
            currentSpeed += accelerationFactor * Time.deltaTime;
        }
        else if (accelerationInput <= 0f && currentSpeed > 0f)
        {
            currentSpeed -= decelerationFactor * Time.deltaTime;
            if (currentSpeed < 0f) currentSpeed = 0f;
        }

        // Apply air resistance to slow down camera movement
        if (!isBraking && currentSpeed > 0f)
        {
            currentSpeed -= airResistance * Time.deltaTime;
            if (currentSpeed < 0f) currentSpeed = 0f;
        }

        // Calculate camera position and rotation based on current speed
        Vector3 targetPosition = target.position + Vector3.up * height - target.forward * distance;
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * damping * currentSpeed / maxSpeed);
        Quaternion newRotation = Quaternion.LookRotation(target.position - transform.position, target.up);

        // Smoothly rotate camera towards target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationDamping);

        // Move camera to new position
        transform.position = newPosition;

        // Update last position for air resistance calculation
        lastPosition = transform.position;
    }

    // LateUpdate is called after all Update functions have been called
    void LateUpdate()
    {
        // Apply air resistance to slow down camera movement
        if (!isBraking && currentSpeed > 0f)
        {
            float distanceTraveled = Vector3.Distance(transform.position, lastPosition);
            float resistanceFactor = distanceTraveled / (Time.deltaTime * currentSpeed);
            currentSpeed -= airResistance * resistanceFactor;
            if (currentSpeed < 0f) currentSpeed = 0f;
        }
    }
}

