using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float damping = 0.5f;
    public float accelerationDamping = 0.5f;
    public float decelerationDamping = 0.5f;
    public float maxSpeed = 100f;
    public float brakeSpeed = 100f;
    public float accelerationEffectMultiplier = 2f;
    public float decelerationEffectMultiplier = 2f;
    public float nitroSpeedMultiplier = 2f;
    public float leanAngle = 10f;
    public float maxLeanAngle = 30f;
    public float leanSmoothTime = 0.2f;

    private Rigidbody targetRigidbody;
    private float targetSpeed;
    private float accelerationEffect;
    private float decelerationEffect;
    private float nitroEffect;
    private float leanAmount;
    private float leanVelocity;

    public Camera cam;


    void Start()
    {
        targetRigidbody = target.GetComponent<Rigidbody>();
        targetSpeed = 0f;
        accelerationEffect = 0f;
        decelerationEffect = 0f;
        nitroEffect = 0f;
        leanAmount = 0f;
        leanVelocity = 0f;

    }

    void Update()
    {
        Calcul();
    }

    void LateUpdate()
    {
        UpdatePosRot();


    }


    void UpdatePosRot() {
        // Calculate camera target position
        Vector3 targetPosition = target.position - target.forward * distance + target.up * height;

        // Apply camera follow damping
        transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime);
       /*
                // Apply camera acceleration and deceleration effects
                float targetAccelerationDamping = Mathf.Lerp(1f, accelerationDamping, accelerationEffect * accelerationEffectMultiplier);
                float targetDecelerationDamping = Mathf.Lerp(1f, decelerationDamping, decelerationEffect * decelerationEffectMultiplier);
                float targetDamping = Mathf.Lerp(targetAccelerationDamping, targetDecelerationDamping, Mathf.Max(accelerationEffect, decelerationEffect));
                transform.position = Vector3.Lerp(transform.position, targetPosition, targetDamping * nitroEffect * Time.deltaTime);
       */
                // Apply camera lean

    
                transform.rotation = Quaternion.Euler(0f, target.rotation.eulerAngles.y, 0f);


                transform.rotation *= Quaternion.Euler(0f, 0f, -leanAmount);
    


    }
    void Calcul()
    {
        // Calculate target speed
        targetSpeed = Mathf.Clamp(targetRigidbody.velocity.magnitude, 0f, maxSpeed);

        // Calculate acceleration and deceleration effects
        accelerationEffect = Mathf.Clamp01(targetRigidbody.velocity.magnitude / targetSpeed);
        decelerationEffect = Mathf.Clamp01(-targetRigidbody.velocity.magnitude / brakeSpeed);

        // Calculate nitro effect
        nitroEffect = Mathf.Clamp01(targetRigidbody.velocity.magnitude / maxSpeed) * nitroSpeedMultiplier;
        nitroEffect = Mathf.SmoothStep(1f, nitroEffect, Mathf.PingPong(Time.time, 1f));
        nitroEffect = Mathf.Lerp(1f, nitroEffect, accelerationEffect);

        // Calculate lean amount based on drift
        //  Vector3 relativeVelocity = targetRigidbody.transform.InverseTransformDirection(targetRigidbody.velocity);
        //  float turnAmount = Mathf.Atan2(relativeVelocity.x, relativeVelocity.z);
        //  leanAmount = Mathf.Lerp(leanAmount, turnAmount * leanAngle, Time.deltaTime / leanSmoothTime);
        //  leanAmount = Mathf.Clamp(leanAmount, -maxLeanAngle, maxLeanAngle);
    }
}