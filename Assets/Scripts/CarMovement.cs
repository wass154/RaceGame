using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private WheelCollider[] wheelColliders;
    [SerializeField] private Transform[] wheelMeshes;
    [SerializeField] private float maxTorque = 100f;
    [SerializeField] private float maxBrakeTorque = 100f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float driftFactor = 0.5f;
    [SerializeField] private float driftTimer = 0.5f;
    [SerializeField] private float driftDuration = 0.5f;
    [SerializeField] private Vector3 centerOfMass = Vector3.zero;

    private float motorInput;
    private float brakeInput;
    private float steeringAngle;
    private bool isDrifting;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        ApplyMotor();
        ApplyBrakes();
        ApplySteering();
        ApplyDrift();
        UpdateWheelMeshes();
    }

    public void SetMotorInput(float input)
    {
        motorInput = input;
    }

    public void SetBrakeInput(float input)
    {
        brakeInput = input;
    }

    public void SetSteeringAngle(float angle)
    {
        steeringAngle = angle;
    }

    public void SetDrift(bool isDrifting)
    {
        this.isDrifting = isDrifting;
    }
    private void ApplyMotor()
    {
        float motorTorque = maxTorque * motorInput;
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (wheelColliders[i].motorTorque < motorTorque)
            {
                wheelColliders[i].motorTorque = motorTorque;
            }
        }
    }
    private void ApplyBrakes()
    {
        float brakeTorque = maxBrakeTorque * brakeInput;
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            wheelColliders[i].brakeTorque = brakeTorque;
        }
    }

    private void ApplySteering()
    {
        float steerAngle = maxSteerAngle * steeringAngle;
        for (int i = 0; i < 2; i++)
        {
            wheelColliders[i].steerAngle = steerAngle;
        }
    }
    private void ApplyDrift()
    {
        if (isDrifting)
        {
            for (int i = 2; i < wheelColliders.Length; i++)
            {
                WheelFrictionCurve sidewaysFriction = wheelColliders[i].sidewaysFriction;
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, sidewaysFriction.stiffness);
                curve.AddKey(1f, driftFactor);
                sidewaysFriction.stiffness = curve.Evaluate(driftTimer / driftDuration);
                wheelColliders[i].sidewaysFriction = sidewaysFriction;
            }
            driftTimer += Time.deltaTime;
        }
        else
        {
            for (int i = 2; i < wheelColliders.Length; i++)
            {
                WheelFrictionCurve sidewaysFriction = wheelColliders[i].sidewaysFriction;
                sidewaysFriction.stiffness = 1f;
                wheelColliders[i].sidewaysFriction = sidewaysFriction;
            }
            driftTimer = 0f;
        }
    }
    private void UpdateWheelMeshes()
    {
        for (int i = 0; i < wheelMeshes.Length; i++)
        {
            Quaternion rotation;
            Vector3 position;
            wheelColliders[i].GetWorldPose(out position, out rotation);
            wheelMeshes[i].position = position;
            wheelMeshes[i].rotation = rotation;
        }
    }
}
