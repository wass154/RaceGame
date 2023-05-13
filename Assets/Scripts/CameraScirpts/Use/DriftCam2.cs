using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftCam2 : MonoBehaviour
{
    public Transform target; // the target to follow
    public float distance = 5.0f; // distance from the target
    public float height = 2.0f; // height above the target
    public float rotationDamping = 3.0f; // rotation damping factor
    public float heightDamping = 2.0f; // height damping factor
    public float driftAngleMultiplier = 5.0f; // angle multiplier for drift effect

    private float currentRotationAngle = 0.0f;
    private float currentHeight = 0.0f;
    private float currentDriftAngle = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        // calculate desired rotation angle and height
        float desiredRotationAngle = target.eulerAngles.y;
        float desiredHeight = target.position.y + height;

        // calculate current rotation angle and height with damping
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, desiredRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, heightDamping * Time.deltaTime);

        // apply rotation and position to camera
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        Vector3 pos = target.position - currentRotation * Vector3.forward * distance;
        pos.y = currentHeight;
        transform.position = pos;

        // add drift effect to camera rotation
        float driftAngle = target.GetComponent<Rigidbody>().angularVelocity.y * driftAngleMultiplier;
        currentDriftAngle = Mathf.Lerp(currentDriftAngle, driftAngle, Time.deltaTime);
        Quaternion driftRotation = Quaternion.Euler(0, currentDriftAngle, 0);
        transform.rotation = currentRotation * driftRotation;
    }
}
