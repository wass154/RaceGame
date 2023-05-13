using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTest : MonoBehaviour
{

    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

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

        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;

        rearLeftWheel.motorTorque = currentMotorForce * currentDriftFactor;
        rearRightWheel.motorTorque = currentMotorForce * currentDriftFactor;

        if (isBraking)
        {
            frontLeftWheel.brakeTorque = currentBrakeForce;
            frontRightWheel.brakeTorque = currentBrakeForce;
            rearLeftWheel.brakeTorque = currentBrakeForce;
            rearRightWheel.brakeTorque = currentBrakeForce;
        }
        else
        {
            frontLeftWheel.brakeTorque = 0f;
            frontRightWheel.brakeTorque = 0f;
            rearLeftWheel.brakeTorque = 0f;
            rearRightWheel.brakeTorque = 0f;
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }
    }
}