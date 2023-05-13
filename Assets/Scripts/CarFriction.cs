using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.Timeline;
using UnityEngine.UI;


public enum DriftWheels
{
    RearDrift2,wheels4,RightFrontRearDrift
}

public enum Control
{
    Keyboard,TouchPad
}

//to make struct visible in inspector we need to add [Serializable] attribute
[Serializable]
public struct WheelsDrift
{
    public DriftWheels DriftWheels;

}
public class CarFriction : MonoBehaviour
{
    [Header("Select Controle Mode")]
    [SerializeField] Control controlMode;




    [Header("Wheels And Transforms for meshes")]
   [SerializeField] Transform frontLeftWheel;
    [SerializeField] Transform frontRightWheel;
    [SerializeField] Transform rearLeftWheel;
    [SerializeField] Transform rearRightWheel;
    [SerializeField] WheelCollider frontLeftCollider;
    [SerializeField] WheelCollider frontRightCollider;
    [SerializeField] WheelCollider rearLeftCollider;
    [SerializeField] WheelCollider rearRightCollider;
  
    [Header("Select DriftWheels")]
[SerializeField] DriftWheels Dr;
    [Header("Settings")]
    [SerializeField] float maxSteerAngle = 30f;
    [SerializeField] float motorForce = 1000f;
    [SerializeField] float brakeForce = 2000f;
    public List<GameObject> Marks;

    [Header("Drift Settings")]
    [SerializeField] float driftFactor = 0.9f;
    [SerializeField] AnimationCurve driftCurve;
    [SerializeField] float maxDriftStiffness = 5f;
    [SerializeField] float driftSlip = 0.2f;
    [SerializeField] float driftStiffness = 2f;



    [Header("Meshes")]
    [SerializeField] GameObject frontLeftMesh;
    [SerializeField] GameObject frontRightMesh;
    [SerializeField] GameObject rearLeftMesh;
    [SerializeField] GameObject rearRightMesh;


    [Header("Private Settings")]
    private float currentSteerAngle;
    private float currentMotorForce;
    private float currentBrakeForce;
    private float currentDriftFactor;
    private float currentDriftStiffness;
    private bool isBraking = false;
    private bool isDrifting = false;

    [Header("Rigidbody")]
    [SerializeField] Rigidbody rb;


    [Header("AntiRoll")]
    [SerializeField] float AntiRollForce;


    [Header("Stabilization")]
    [SerializeField] float stabilizationFactor;



    [Header("Landed Car")]
    [SerializeField] float landDelay = 0.1f;
    private bool isLanded = false;
    private float landTime;


    [Header("Max Speed")]
    [SerializeField] float maxSpeed;
    [Header("Inputs")]
   private float steerInput;
    private float motorInput;
    private bool brakeInput;

    [Header("Slide Settings")]
    [SerializeField] float driftThreshold = 5.0f; // Minimum drift value required to trigger sliding
    [SerializeField] float slideSpeed = 10.0f;
    [SerializeField] KeyCode SlideKey = KeyCode.LeftControl; // The key used to initiate drifting
    private bool isSliding = false;


    [Header("Grounded")]
    [SerializeField] float jumpForce = 10f; // Adjust the jump force as needed
    [SerializeField] float gravity = 20f; // Adjust the gravity force as needed





    void Start()
    {




        //Setup Colliders Positions and Rotations
        frontLeftCollider.transform.position = frontLeftWheel.position;
        frontRightCollider.transform.position = frontRightWheel.position;
        rearLeftCollider.transform.position = rearLeftWheel.position;
        rearRightCollider.transform.position = rearRightWheel.position;

        frontLeftCollider.transform.rotation = frontLeftWheel.rotation;
        frontRightCollider.transform.rotation = frontRightWheel.rotation;
        rearLeftCollider.transform.rotation = rearLeftWheel.rotation;
        rearRightCollider.transform.rotation = rearRightWheel.rotation;

        //Setup Meshes Positions and Rotations
        frontLeftMesh.transform.position = frontLeftWheel.position;
        frontRightMesh.transform.position = frontRightWheel.position;
        rearLeftMesh.transform.position = rearLeftWheel.position;
        rearRightMesh.transform.position = rearRightWheel.position;

        frontLeftMesh.transform.rotation = frontLeftWheel.rotation;
        frontRightMesh.transform.rotation = frontRightWheel.rotation;
        rearLeftMesh.transform.rotation = rearLeftWheel.rotation;
        rearRightMesh.transform.rotation = rearRightWheel.rotation;


    }
    void MaxSpeed()
    {
        //get current speed from Rigidbody Speed
        float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;

        // limit the speed of the car to the maximum speed
        float newSpeed = Mathf.Clamp(currentSpeed, 0.0f, maxSpeed);

        // set the new speed of the car
        GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * newSpeed;
    }
    private void Update()
    {
        MaxSpeed();
    }
    void FixedUpdate()
    {


        Inputs();
      
        Drift4Whells();
        RearDrift();
        RightLeftDrift();

        MarkEffect();

        AntiRoll();

        //   Stabilize();

        Stabilize2();

        Landed();

// Slope();

        //  JumpCar();

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isSliding = !isSliding;
            if (isSliding)
            {
                SlideCar(transform.right); // Slide to the right
            }
        }



       

        currentSteerAngle = steerInput * maxSteerAngle;
      //  currentMotorForce = motorInput * motorForce;
        float Vitesse = rb.velocity.magnitude;
        if(Vitesse>0.1f && motorInput > 0)
        {
            currentMotorForce = motorInput * motorForce;
        }
        else
        {
            currentMotorForce = motorInput * motorForce*0.5f;
        }

        currentBrakeForce = brakeInput ? brakeForce : 0f;

        currentDriftStiffness = driftCurve.Evaluate(Mathf.Abs(steerInput)) * maxDriftStiffness;
        currentDriftFactor = isDrifting ? Mathf.Lerp(driftFactor, 1f, currentDriftStiffness) : 1f;

        float slip = isDrifting ? driftSlip : 1f;
        float stiffness = isDrifting ? Mathf.Lerp(1f, driftStiffness, currentDriftStiffness) : 1f;



        WheelFrictionCurve forwardFriction = frontLeftCollider.forwardFriction;
        WheelFrictionCurve sidewaysFriction = frontLeftCollider.sidewaysFriction;

        forwardFriction.stiffness = stiffness;
        forwardFriction.asymptoteValue = 2f * currentDriftFactor;
        forwardFriction.asymptoteSlip = 0.2f;
        sidewaysFriction.stiffness = stiffness * slip;
        sidewaysFriction.asymptoteValue = 2f * currentDriftFactor;
        sidewaysFriction.asymptoteSlip = 0.5f;
        frontLeftCollider.forwardFriction = forwardFriction;
        frontLeftCollider.sidewaysFriction = sidewaysFriction;
        frontRightCollider.forwardFriction = forwardFriction;
        frontRightCollider.sidewaysFriction = sidewaysFriction;
        rearLeftCollider.forwardFriction = forwardFriction;
        rearLeftCollider.sidewaysFriction = sidewaysFriction;
        rearRightCollider.forwardFriction = forwardFriction;
        rearRightCollider.sidewaysFriction = sidewaysFriction;

        frontLeftCollider.steerAngle = currentSteerAngle;
        frontRightCollider.steerAngle = currentSteerAngle;

      



      //  rearLeftCollider.motorTorque = currentMotorForce * currentDriftFactor;
       // rearRightCollider.motorTorque = currentMotorForce * currentDriftFactor;

       // frontLeftCollider.motorTorque= currentMotorForce * currentDriftFactor;
       // frontRightCollider.motorTorque=currentMotorForce* currentDriftFactor;



        rearLeftCollider.brakeTorque = currentBrakeForce;
        rearRightCollider.brakeTorque = currentBrakeForce;

        UpdateWheelMeshes(frontLeftCollider, frontLeftMesh);
        UpdateWheelMeshes(frontRightCollider, frontRightMesh);
        UpdateWheelMeshes(rearLeftCollider, rearLeftMesh);
        UpdateWheelMeshes(rearRightCollider, rearRightMesh);

        if (Input.GetKey(KeyCode.LeftShift) && !isDrifting)
        {
            isDrifting = true;
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && isDrifting)
        {
            isDrifting = false;
        }
    }
    void UpdateWheelMeshes(WheelCollider collider, GameObject mesh)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;
    }
    void Inputs()
    {
     if(controlMode==Control.Keyboard)
        {
            steerInput = Input.GetAxis("Horizontal");
            motorInput = Input.GetAxis("Vertical");
            brakeInput = Input.GetKey(KeyCode.Space);
        }

    }
    void MobileMode()
    {
        if(controlMode== Control.TouchPad)
        {

            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToPortrait = true;
            if (Screen.orientation != ScreenOrientation.LandscapeLeft)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
        }
    }
    public void TouchPads(float input)
    {

        motorInput = input;
      
    }

    public void SteerInput(float input)
    {
        steerInput = input;
    }

    void Drift4Whells()
    {
        if (Dr == DriftWheels.wheels4) 
        {
            print("4 Wheels");
            rearLeftCollider.motorTorque = currentMotorForce * currentDriftFactor;
            rearRightCollider.motorTorque = currentMotorForce * currentDriftFactor;

            frontLeftCollider.motorTorque = currentMotorForce * currentDriftFactor;
            frontRightCollider.motorTorque = currentMotorForce * currentDriftFactor;
        }
       
    }
    void RearDrift()
    {
        if (Dr == DriftWheels.RearDrift2)
        {
            print("2 Rear Wheels");
            rearLeftCollider.motorTorque = currentMotorForce * currentDriftFactor;
            rearRightCollider.motorTorque = currentMotorForce * currentDriftFactor;
        }
    }
    void RightLeftDrift()
    {
        print("2 Right and Left Wheels");
        frontRightCollider.motorTorque=currentMotorForce* currentDriftFactor;
        rearRightCollider.motorTorque=currentMotorForce* currentDriftFactor;
    }

    void AntiRoll()
    {
        WheelHit hit;
        float TravelLeft = 1;
        float TravelRight = 1;
       //check if Wheel LeftAnd Right is Grounded
       bool GroundedLeft= rearLeftCollider.GetGroundHit(out hit);
        if(GroundedLeft)
        {
            TravelLeft = (-rearLeftCollider.transform.InverseTransformPoint(hit.point).y - rearLeftCollider.radius / rearLeftCollider.suspensionDistance);
        }
        bool GroundedRight = rearRightCollider.GetGroundHit(out hit);
        {
            TravelRight = (-rearRightCollider.transform.InverseTransformPoint(hit.point).y - rearRightCollider.radius / rearRightCollider.suspensionDistance);
        }
        float AntiForce = (TravelLeft - TravelRight) * AntiRollForce;

        if (GroundedLeft)
        {
            rb.AddForceAtPosition(rearLeftCollider.transform.up * -AntiForce, rearLeftCollider.transform.position);
        }
        if(GroundedRight)
        {
            rb.AddForceAtPosition(rearRightCollider.transform.up*AntiForce,rearRightCollider.transform.position);
        }

        

    }



    void Stabilize()
    {
        Vector3 localVelocity = rb.transform.InverseTransformDirection(rb.velocity);
        float roll = localVelocity.x * stabilizationFactor;
        float pitch = -localVelocity.z * stabilizationFactor;
        rb.AddTorque(rb.transform.right * roll);
        rb.AddTorque(rb.transform.forward * pitch);
    }

   void Stabilize2()
    {
        Vector3 uprightDirection = -rb.transform.up;

        // Calculate the current direction of gravity
        Vector3 gravityDirection = Physics.gravity.normalized;

        // Calculate the force needed to keep the Rigidbody upright
        Vector3 stabilizationForceVector = stabilizationFactor * Vector3.Cross(uprightDirection, gravityDirection);

        // Apply the stabilization force to the Rigidbody
        rb.AddForce(stabilizationForceVector, ForceMode.Force);
    }

    void Landed()
    {
        if (rb.velocity.y < 0.1f && !isLanded)
        {
            // car has landed
            isLanded = true;
            landTime = Time.time;
        }

        if (isLanded && Time.time > landTime + landDelay)
        {
            // car can move again
            isLanded = false;

            // check if any wheels are upside down and rotate them
            foreach (WheelCollider wheel in GetComponentsInChildren<WheelCollider>())
            {
                if (wheel.transform.up.y < 0)
                {
                    wheel.transform.rotation *= Quaternion.Euler(0, 180, 0);
                }
            }
        }
    }
    void Slope()
    {
        RaycastHit hit;
        float slopeAngle = 0f;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.5f))
        {
            Vector3 groundNormal = hit.normal;
            slopeAngle = Vector3.Angle(groundNormal, transform.forward);
            //check if the slope is steep enough to affect the car's movement
            if (slopeAngle > 10f)
            {
                //apply a downward force to simulate gravity
                rb.AddForce(-transform.up * slopeAngle * 500f);
                //apply a torque force to make the car turn downhill
                float torque = Mathf.Sign(slopeAngle - transform.localEulerAngles.x) * 1000f;
                rb.AddTorque(Vector3.up * torque);
            }
        }
    }
    void Slope2()
    {
        RaycastHit hit;
        float slopeAngle = 0f;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.5f))
        {
            Vector3 groundNormal = hit.normal;
            slopeAngle = Vector3.Angle(groundNormal, transform.forward);
            //check if the slope is steep enough to affect the car's movement
            if (slopeAngle > 10f)
            {
                //apply a downward force to simulate gravity
                rb.AddForce(-transform.up * slopeAngle * 500f);
                //apply a torque force to make the car turn downhill
                float torque = Mathf.Sign(slopeAngle - transform.localEulerAngles.x) * 1000f;
                rb.AddTorque(Vector3.up * torque);

                // adjust suspension distance for each wheel
                frontLeftCollider.suspensionDistance = Mathf.Lerp(frontLeftCollider.suspensionDistance, 0.2f, Time.deltaTime * 5f);
                frontRightCollider.suspensionDistance = Mathf.Lerp(frontRightCollider.suspensionDistance, 0.2f, Time.deltaTime * 5f);
                rearLeftCollider.suspensionDistance = Mathf.Lerp(rearLeftCollider.suspensionDistance, 0.2f, Time.deltaTime * 5f);
                rearRightCollider.suspensionDistance = Mathf.Lerp(rearRightCollider.suspensionDistance, 0.2f, Time.deltaTime * 5f);
            }
            else
            {
                // reset suspension distance for each wheel
                frontLeftCollider.suspensionDistance = Mathf.Lerp(frontLeftCollider.suspensionDistance, 0.5f, Time.deltaTime * 5f);
                frontRightCollider.suspensionDistance = Mathf.Lerp(frontRightCollider.suspensionDistance, 0.5f, Time.deltaTime * 5f);
                rearLeftCollider.suspensionDistance = Mathf.Lerp(rearLeftCollider.suspensionDistance, 0.5f, Time.deltaTime * 5f);
                rearRightCollider.suspensionDistance = Mathf.Lerp(rearRightCollider.suspensionDistance, 0.5f, Time.deltaTime * 5f);
            }
        }
    }



    void SlideCar(Vector3 slideDirection)
    {
        if (isSliding)
        {
            // Apply a sideways force to the car
            float slideForce = slideSpeed * rb.mass;
            rb.AddForce(slideDirection * slideForce, ForceMode.Force);
        }
    }

    void JumpCar()
    {
        if (rb.velocity.y > 2)
        {
            // Add a force to the car's Rigidbody component to make it jump
           rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

         
        }
        else
        {
            // Apply a downward force to the car's Rigidbody component to simulate gravity
            rb.AddForce(Vector3.down* gravity * Time.deltaTime, ForceMode.Impulse);
}
    }
       


    void MarkEffect()
    {
foreach(var Mark in Marks)
        {
            if(Input.GetKey(KeyCode.Space))
            {
           Mark.GetComponentInChildren<TrailRenderer>().emitting = true;

            }
            else
            {
             Mark.GetComponentInChildren<TrailRenderer>().emitting = false;
                
            }
        }
    }
}
