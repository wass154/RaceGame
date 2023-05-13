using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraCinemachine : MonoBehaviour
{
    // Reference to the car transform
    public Transform carTransform;

    // Camera offset from the car
    public Vector3 cameraOffset;

    // Camera rotation offset from the car
    public Vector3 cameraRotationOffset;

    // Camera follow speed
    public float followSpeed = 10.0f;

    // Camera rotation speed
    public float rotationspeed = 10.0f;

    // Camera sensitivity to mouse movement
    public float mouseSensitivity = 10.0f;

    // Damping for camera position
    public float positionDamping = 0.1f;

    // Damping for camera rotation
    public float rotationDamping = 0.1f;

    // Current camera position
    private Vector3 cameraPosition;

    // Current camera rotation
    private Quaternion cameraRotation;

    // Target camera position
    private Vector3 targetPosition;

    // Target camera rotation
    private Quaternion targetRotation;

    // Use this for initialization
    void Start()
    {
        // Initialize camera position and rotation
        cameraPosition = carTransform.position + cameraOffset;
        cameraRotation = Quaternion.Euler(cameraRotationOffset);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate target position and rotation based on car transform
        targetPosition = carTransform.position + cameraOffset;
        targetRotation = Quaternion.Euler(cameraRotationOffset) * carTransform.rotation;

        // Add acceleration effect to camera movement
        float distance = Vector3.Distance(cameraPosition, targetPosition);
        float speed = Mathf.Clamp(followSpeed * Time.deltaTime * distance, 0.0f, 1.0f);
        cameraPosition = Vector3.Lerp(cameraPosition, targetPosition, speed);

        // Add acceleration effect to camera rotation
        float angle = Quaternion.Angle(cameraRotation, targetRotation);
        float rotationSpeed = Mathf.Clamp(angle * rotationspeed * Time.deltaTime, 0.0f, 1.0f);
        cameraRotation = Quaternion.Slerp(cameraRotation, targetRotation, rotationSpeed);

        // Apply mouse movement effect to camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraRotation *= Quaternion.Euler(-mouseY, mouseX, 0);

        // Apply damping to camera position and rotation
        cameraPosition = Vector3.Lerp(cameraPosition, targetPosition, positionDamping);
        cameraRotation = Quaternion.Slerp(cameraRotation, targetRotation, rotationDamping);

        // Update camera transform
        transform.position = cameraPosition;
        transform.rotation = cameraRotation;
    }

    // Set camera position to park position
    public void SetParkPosition()
    {
        cameraPosition = carTransform.position + cameraOffset;
        cameraRotation = Quaternion.Euler(cameraRotationOffset);
        transform.position = cameraPosition;
        transform.rotation = cameraRotation;
    }
}