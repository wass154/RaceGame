using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C1 : MonoBehaviour
{

    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    public float maxSteerAngle = 30f;
    public float motorForce = 1000f;
    public float brakeForce = 2000f;
    public float driftFactor = 0.9f;
    public AnimationCurve driftCurve;
    public float maxDriftStiffness = 5f;

    private float currentSteerAngle;
    private float currentMotorForce;
    private float currentBrakeForce;
    private float currentDriftFactor;
    private float currentDriftStiffness;
    private bool isBraking = false;
    private bool isDrifting = false;

    public GameObject frontLeftMesh;
    public GameObject frontRightMesh;
    public GameObject rearLeftMesh;
    public GameObject rearRightMesh;

    void Start()
    {
        frontLeftCollider.transform.position = frontLeftWheel.position;
        frontRightCollider.transform.position = frontRightWheel.position;
        rearLeftCollider.transform.position = rearLeftWheel.position;
        rearRightCollider.transform.position = rearRightWheel.position;

        frontLeftCollider.transform.rotation = frontLeftWheel.rotation;
        frontRightCollider.transform.rotation = frontRightWheel.rotation;
        rearLeftCollider.transform.rotation = rearLeftWheel.rotation;
        rearRightCollider.transform.rotation = rearRightWheel.rotation;

        frontLeftMesh.transform.position = frontLeftWheel.position;
        frontRightMesh.transform.position = frontRightWheel.position;
        rearLeftMesh.transform.position = rearLeftWheel.position;
        rearRightMesh.transform.position = rearRightWheel.position;

        frontLeftMesh.transform.rotation = frontLeftWheel.rotation;
        frontRightMesh.transform.rotation = frontRightWheel.rotation;
        rearLeftMesh.transform.rotation = rearLeftWheel.rotation;
        rearRightMesh.transform.rotation = rearRightWheel.rotation;
    }

    void FixedUpdate()
    {
        float steerInput = Input.GetAxis("Horizontal");
        float motorInput = Input.GetAxis("Vertical");
        bool brakeInput = Input.GetKey(KeyCode.Space);

        currentSteerAngle = steerInput * maxSteerAngle;
        currentMotorForce = motorInput * motorForce;
        currentBrakeForce = brakeInput ? brakeForce : 0f;

        currentDriftStiffness = driftCurve.Evaluate(Mathf.Abs(steerInput)) * maxDriftStiffness;
        currentDriftFactor = isDrifting ? Mathf.Lerp(driftFactor, 1f, currentDriftStiffness) : 1f;

        frontLeftCollider.steerAngle = currentSteerAngle;
        frontRightCollider.steerAngle = currentSteerAngle;

        rearLeftCollider.motorTorque = currentMotorForce * currentDriftFactor;
        rearRightCollider.motorTorque = currentMotorForce * currentDriftFactor;

        if (isBraking)
        {
            frontLeftCollider.brakeTorque = currentBrakeForce;
            frontRightCollider.brakeTorque = currentBrakeForce;
            rearLeftCollider.brakeTorque = currentBrakeForce;
            rearRightCollider.brakeTorque = currentBrakeForce;
        }
        else
        {
            frontLeftCollider.brakeTorque = 0f;
            frontRightCollider.brakeTorque = 0f;
            rearLeftCollider.brakeTorque = 0f;
            rearRightCollider.brakeTorque = 0f;
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        if (brakeInput)
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

        UpdateWheelMesh(frontLeftCollider, frontLeftMesh);
        UpdateWheelMesh(frontRightCollider, frontRightMesh);
        UpdateWheelMesh(rearLeftCollider, rearLeftMesh);
        UpdateWheelMesh(rearRightCollider, rearRightMesh);
    }

    void UpdateWheelMesh(WheelCollider collider, GameObject mesh)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;
    }
}