using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
   [SerializeField] Transform target;        // The object to follow
    [SerializeField] Vector3 offset;          // The camera offset from the target
    [SerializeField] float smoothSpeed;       // The speed at which the camera follows the target
    [SerializeField] float rotateSpeed;       // The speed at which the camera rotates to follow the car
    [SerializeField] float maxRotationAngle;

    private Vector3 velocity;
    private Quaternion targetRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        // Calculate the desired camera position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Make the camera look at the target
        transform.LookAt(target);
    }
}

