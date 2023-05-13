using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCam : MonoBehaviour
{

    public float Distance;
    public float Height;
    public float PositionDamp;
    public float RotationDamp;
    public Transform Target;





    public float heightDamping = 2.0f; // height damping factor
    public float driftAngleMultiplier = 5.0f; // angle multiplier for drift effect

    private float currentRotationAngle = 0.0f;
    private float currentHeight = 0.0f;
    private float currentDriftAngle = 0.0f;

    void Start()
    {



    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
       
    }
    void FollowTarget()
    {
        Vector3 TargetPos = Target.position - (Target.forward * Distance) + (Vector3.up * Height);
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * PositionDamp);
        Quaternion TargetRot = Quaternion.LookRotation(Target.position - transform.position, Target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * RotationDamp);
        transform.position -= Target.GetComponent<Rigidbody>().velocity * Time.deltaTime * 0.1f;
    }
    void Drift()
    {
        float desiredRotationAngle = Target.eulerAngles.y;
        float desiredHeight = Target.position.y + Height;

        // calculate current rotation angle and height with damping
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, desiredRotationAngle, RotationDamp * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, heightDamping * Time.deltaTime);

        // apply rotation and position to camera
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        Vector3 pos = Target.position - currentRotation * Vector3.forward * Distance ;
        pos.y = currentHeight;
        transform.position = pos;

        // add drift effect to camera rotation
        float driftAngle = Target.GetComponent<Rigidbody>().angularVelocity.y * driftAngleMultiplier;
        currentDriftAngle = Mathf.Lerp(currentDriftAngle, driftAngle, Time.deltaTime);
        Quaternion driftRotation = Quaternion.Euler(0, currentDriftAngle, 0);
        transform.rotation = currentRotation * driftRotation;
    }
    private void LateUpdate()
    {
        FollowTarget();
        Drift();
    }
}

   