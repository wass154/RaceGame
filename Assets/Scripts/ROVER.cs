using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROVER : MonoBehaviour
{

    [Header("Wheels Section")]
    [SerializeField] WheelCollider frontLeftWheelCollider;
    [SerializeField] WheelCollider frontRightWheelCollider;
    [SerializeField] WheelCollider rearLeftWheelCollider;
    [SerializeField] WheelCollider rearRightWheelCollider;


    [Header("Meshes Section")]
    [SerializeField] Transform frontLeftWheelMesh;
    [SerializeField] Transform frontRightWheelMesh;
    [SerializeField] Transform rearLeftWheelMesh;
    [SerializeField] Transform rearRightWheelMesh;


    [Header("Settings")]
    [SerializeField] float maxMotorTorque = 600f;
    [SerializeField] float maxSteeringAngle = 20f;
    [SerializeField] float maxBrakeTorque = 200f;
    [SerializeField] float maxSpeed = 100f;
    [SerializeField] float maxSuspensionDistance = 0.3f;
    [SerializeField] float suspensionSpring = 5000f;
    [SerializeField] float suspensionDamper = 1000f;
    [SerializeField] float suspensionTargetPosition = 0.3f;

    [Header(" Private Settings")]
    private float motorTorque = 0f;
    private float steeringAngle = 0f;
    private float brakeTorque = 0f;
    private Rigidbody carRigidbody;
    private float speed = 0f;
    [SerializeField] Vector3 centerOfMass;





    // Start is called before the first frame update
    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = centerOfMass;

        // Set up the suspension for each wheel collider
      //  SetSuspension(frontLeftWheelCollider);
       // SetSuspension(frontRightWheelCollider);
       // SetSuspension(rearLeftWheelCollider);
       // SetSuspension(rearRightWheelCollider);
    }
    private void SetSuspension(WheelCollider wheelCollider)
    {
        JointSpring suspensionSpringSettings = wheelCollider.suspensionSpring;
        suspensionSpringSettings.spring = suspensionSpring;
        suspensionSpringSettings.damper = suspensionDamper;
        suspensionSpringSettings.targetPosition = suspensionTargetPosition;
        wheelCollider.suspensionSpring = suspensionSpringSettings;

        wheelCollider.suspensionDistance = maxSuspensionDistance;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        GetInput();
        ApplyMotorTorque();
        ApplySteeringAngle();
        ApplyBrakeTorque();
        UpdateWheelPoses();
      //  speed = carRigidbody.velocity.magnitude * 3.6f; // Convert m/s to km/h
    }


    private void GetInput()
    {
        motorTorque = maxMotorTorque * Input.GetAxis("Vertical");
        steeringAngle = maxSteeringAngle * Input.GetAxis("Horizontal");
        brakeTorque = Input.GetKey(KeyCode.Space) ? maxBrakeTorque : 0f;
    }

    private void ApplyMotorTorque()
    {
        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
        rearLeftWheelCollider.motorTorque = motorTorque;
        rearRightWheelCollider.motorTorque = motorTorque;
    }

    private void ApplySteeringAngle()
    {
        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;
    }

    private void ApplyBrakeTorque()
    {
        frontLeftWheelCollider.brakeTorque = brakeTorque;
        frontRightWheelCollider.brakeTorque = brakeTorque;
        rearLeftWheelCollider.brakeTorque = brakeTorque;
        rearRightWheelCollider.brakeTorque = brakeTorque;
    }
    private void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos,out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(frontLeftWheelCollider, frontLeftWheelMesh);
        UpdateWheelPose(frontRightWheelCollider, frontRightWheelMesh);
        UpdateWheelPose(rearLeftWheelCollider, rearLeftWheelMesh);
        UpdateWheelPose(rearRightWheelCollider, rearRightWheelMesh);
    }
}
