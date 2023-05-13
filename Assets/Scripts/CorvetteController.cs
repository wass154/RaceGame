using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorvetteController : MonoBehaviour
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
    [SerializeField] float driftFactor = 0.95f;
    [SerializeField] float driftTorque = 500f;


    [Header(" Private Settings")]
    private float motorTorque = 0f;
    private float steeringAngle = 0f;
    private float brakeTorque = 0f;
    private Rigidbody carRigidbody;
    private float speed = 0f;
    public Vector3 wheelRotationOffset;
    public float wheelRadius;
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
     //   SetSuspension(rearRightWheelCollider);
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

    private void FixedUpdate()
    {
        GetInput();
        ApplyMotorTorque();
        ApplySteeringAngle();
        ApplyBrakeTorque();
        ApplyWheelPoses();
         speed = carRigidbody.velocity.magnitude * 3.6f; // Convert m/s to km/h
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyDrift();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetInput()
    {
        motorTorque = maxMotorTorque * Input.GetAxis("Vertical");
        steeringAngle = maxSteeringAngle * Input.GetAxis("Horizontal");
        brakeTorque = Input.GetKey(KeyCode.X) ? maxBrakeTorque : 0f;
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
    private void ApplyDrift()
    {
        float driftAmount = 0f;

        if (speed > 20f && Mathf.Abs(steeringAngle) > 0f)
        {
            driftAmount = Mathf.Clamp01(1f - (speed / maxSpeed));
            Vector3 driftForce = -transform.right * driftTorque * driftAmount;
            carRigidbody.AddForce(driftForce, ForceMode.Force);
        }

        // Adjust wheel colliders to simulate drifting
        frontLeftWheelCollider.motorTorque = motorTorque * (1f - driftFactor * driftAmount);
        frontRightWheelCollider.motorTorque = motorTorque * (1f - driftFactor * driftAmount);
        rearLeftWheelCollider.motorTorque = motorTorque * (1f - driftFactor * driftAmount);
        rearRightWheelCollider.motorTorque = motorTorque * (1f - driftFactor * driftAmount);

        frontLeftWheelCollider.brakeTorque = maxBrakeTorque * driftAmount;
        frontRightWheelCollider.brakeTorque = maxBrakeTorque * driftAmount;
        rearLeftWheelCollider.brakeTorque = maxBrakeTorque * driftAmount;
        rearRightWheelCollider.brakeTorque = maxBrakeTorque * driftAmount;

        frontLeftWheelCollider.steerAngle = Mathf.Lerp(steeringAngle, -steeringAngle * driftFactor, driftAmount);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(steeringAngle, -steeringAngle * driftFactor, driftAmount);

        // Rotate the car based on the drift angle
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0f, steeringAngle * driftAmount * 3f, 0f) * Time.fixedDeltaTime);
        carRigidbody.MoveRotation(carRigidbody.rotation * deltaRotation);
    }
    private void Reset()
    {
        motorTorque = 0f;
        steeringAngle = 0f;
        brakeTorque = 0f;

        // Reset wheel colliders
        frontLeftWheelCollider.brakeTorque = 0f;
        frontRightWheelCollider.brakeTorque = 0f;
        rearLeftWheelCollider.brakeTorque = 0f;
        rearRightWheelCollider.brakeTorque = 0f;

        frontLeftWheelCollider.motorTorque = 0f;
        frontRightWheelCollider.motorTorque = 0f;
        rearLeftWheelCollider.motorTorque = 0f;
        rearRightWheelCollider.motorTorque = 0f;

        frontLeftWheelCollider.steerAngle = 0f;
        frontRightWheelCollider.steerAngle = 0f;

        // Reset the car's rotation
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        carRigidbody.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }



    private void ApplyWheelPoses()
    {
        ApplyWheelPose(frontLeftWheelCollider, frontLeftWheelMesh);
        ApplyWheelPose(frontRightWheelCollider, frontRightWheelMesh);
        ApplyWheelPose(rearLeftWheelCollider, rearLeftWheelMesh);
        ApplyWheelPose(rearRightWheelCollider, rearRightWheelMesh);
    }

    private void ApplyWheelPose(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation * Quaternion.Euler(wheelRotationOffset); // apply rotation offset

        // apply mesh rotation
        Vector3 wheelVelocity = carRigidbody.GetPointVelocity(collider.transform.position);
        float meshRotationAngle = -(Mathf.Rad2Deg * wheelVelocity.magnitude * Time.fixedDeltaTime / wheelRadius);
        Vector3 meshRotationAxis = collider.transform.right;
        wheelTransform.GetChild(0).localRotation *= Quaternion.AngleAxis(meshRotationAngle, meshRotationAxis);
    }
}
