using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cr : MonoBehaviour
{
    public enum RearOrFront
    {
        Front,Rear
    }

    [Serializable]
    public struct Wheel
    {
       public GameObject wheelMesh;
        public WheelCollider wheelCollider;
        public RearOrFront RearOrFront;
    }
    [Header("Acceleration and Brake Settings")]
    [SerializeField] float Torque;
    [SerializeField] float B;
    [SerializeField] float Acceleration;
    [SerializeField] float Brake;
    [SerializeField] List<Wheel> wheelList;
    [SerializeField] float MoveX;
    private Rigidbody rb;

    [Header("Steering Settings")]
    [SerializeField] float Sensitivity;
    [SerializeField] float MaxAngleSteer;
    [SerializeField] float MoveY;

    [SerializeField] Vector3 CenterOfMass;
    [SerializeField] float Deceleration = 200f;
    [SerializeField] float driftFactor;
    private bool isBraking;


    
    // Start is called before the first frame update
    void Start()
    {
     
       rb =GetComponent<Rigidbody>();
        rb.centerOfMass = CenterOfMass;
    }

    // Update is called once per frame
    void Update()
    {
   


        GetInputs();
        UpdateWheel();
    }
    private void LateUpdate()
    {
        Move();
        Steer();
        Braking();
       // Drift();
    }
    void GetInputs()
    {
        MoveX = Input.GetAxis("Vertical");
        MoveY = Input.GetAxis("Horizontal");
        isBraking = Input.GetKey(KeyCode.Space);
    }
    void Move()
    {
        foreach(var wheel in wheelList)
        {
            wheel.wheelCollider.motorTorque=MoveX* Acceleration*Torque * Time.deltaTime;
        }
    }
    void Steer()
    {
        foreach(var wheel in wheelList)
        {
            if (wheel.RearOrFront == RearOrFront.Front)
            {
                wheel.wheelCollider.steerAngle = MoveY * MaxAngleSteer * Sensitivity;
            }
        }
    }
    void UpdateWheel()
    {
        foreach(var wheel in wheelList)
        {
            Quaternion Rot;
            Vector3 Pos;
            wheel.wheelCollider.GetWorldPose(out Pos, out Rot);
            wheel.wheelMesh.transform.position=Pos;
            wheel.wheelMesh.transform.rotation = Rot;
        }
    }
    void Drift()
    {
        foreach(var wheel in wheelList)
        {
            float driftAngle = Vector3.Angle(transform.forward, GetComponent<Rigidbody>().velocity);
            driftFactor = Mathf.Clamp01(1 - Mathf.Abs(driftAngle / MaxAngleSteer));

            if (wheel.RearOrFront == RearOrFront.Rear)
            {
              wheel.wheelCollider.motorTorque = Torque * (1 - driftFactor);
               wheel.wheelCollider.motorTorque = Torque * (1 - driftFactor);
            }

        }
    }
    void Braking()
    {
        
        if(Input.GetKey(KeyCode.Space))
        {
            foreach(var wheel in wheelList)
            {
                wheel.wheelCollider.brakeTorque= Deceleration*B*Time.deltaTime;
            }
        }
        else
        {
            foreach(var wheel in wheelList)
            {
                wheel.wheelCollider.brakeTorque= 0;
            }
        }
    }
}
