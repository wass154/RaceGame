using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class c : MonoBehaviour
{
    [Header("Input Section")]
    private float Horizontal_Input;
    private float Vertical_Input;
    private bool isBreak;
    private float currentSteerAngle, currentbreakForce;


    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;
    [SerializeField] private float Acceleration;

    [SerializeField] WheelCollider LeftForward;
    [SerializeField] WheelCollider RightForward;
    [SerializeField] WheelCollider BackwardLeft;
    [SerializeField] WheelCollider BackwardRight;

    [SerializeField] Transform LeftForwardWheel;
    [SerializeField] Transform RightForwardWheel;
    [SerializeField] Transform LeftBackwardWheel;
    [SerializeField] Transform RightBackwardWheel;

    [SerializeField] float AntiRollForce;


    [Header("Drift Section")]
    [SerializeField] WheelCollider [] wheel;
    private float currentspeed;
    private float currentSteering;
    private float driftAngle;

    [SerializeField] float DriftFactor;
    [SerializeField] float DriftSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        GetInputs();
        Handle();
        Break();
        HndleAcceleration();
        UpdateWheels();
        AntiRoll();
    }


    void GetInputs()
    {
        Horizontal_Input = Input.GetAxisRaw("Horizontal");
       Vertical_Input = Input.GetAxisRaw("Vertical");
        isBreak = Input.GetKey(KeyCode.Space);

    }
    void Handle()
    {
        LeftForward.motorTorque = Vertical_Input * motorForce;
        RightForward.motorTorque=Vertical_Input* motorForce ;
        currentbreakForce = isBreak ? breakForce : 0f;
        Break();
    }
    void Break()
    {
        RightForward.brakeTorque = currentbreakForce;
        LeftForward.brakeTorque = currentbreakForce;
        BackwardLeft.brakeTorque = currentbreakForce;
       BackwardRight.brakeTorque = currentbreakForce;
    }


    void HndleAcceleration()
    {
        currentSteerAngle = maxSteerAngle * Horizontal_Input;
      LeftForward.steerAngle = currentSteerAngle;
       RightForward.steerAngle = currentSteerAngle;
    }
    void UpdateWheels()
    {
        SingleWheel(LeftForward, LeftForwardWheel);
        SingleWheel(RightForward, RightForwardWheel);
        SingleWheel(BackwardLeft, LeftBackwardWheel);
        SingleWheel(BackwardRight, RightBackwardWheel);
            
    }
    void SingleWheel(WheelCollider wheel,Transform wheelTransform)
    {
        Vector3 WheelPos;
        Quaternion WheelRot;
        wheel.GetWorldPose(out WheelPos, out WheelRot);
        wheelTransform.rotation = WheelRot;
        wheelTransform.position= WheelPos;  
    }
    void AntiRoll()
    {
        float frontTravel = 1 - Mathf.Clamp01(LeftForward.suspensionSpring.targetPosition / LeftForward.suspensionDistance);
        float rearTravel=1-Mathf.Clamp01(BackwardLeft.suspensionSpring.targetPosition/BackwardLeft.suspensionDistance);
        float AntiRol = frontTravel - rearTravel * AntiRollForce;
        if(LeftForward.isGrounded) {
            GetComponent<Rigidbody>().AddForceAtPosition(LeftForward.transform.up * -AntiRol, LeftForward.transform.position);
        }
        if(RightForward.isGrounded)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(RightForward.transform.up*-AntiRol, RightForward.transform.position);
        }
        if (BackwardLeft.isGrounded)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(BackwardLeft.transform.up * AntiRol, BackwardLeft.transform.position);
        }
        if (BackwardRight.isGrounded)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(BackwardRight.transform.up * AntiRol, BackwardRight.transform.position);
        }
    }

  
}
