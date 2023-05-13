using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float maxDistance = 10f;
    public float minDistance = 2f;
    public float zoomSpeed = 2f;
    public float rotationSpeed = 5f;
    public float shakeMagnitude = 0.1f;
    public float shakeDuration = 0.2f;
    public float accelEffectIntensity = 0.1f;
    public float mouseEffectIntensity = 0.1f;
    public float crunchEffectIntensity = 0.2f;
    public float brakeEffectIntensity = 0.2f;
    public float nitroEffectIntensity = 0.3f;
    public float slipEffectIntensity = 0.2f;
    public float stopEffectIntensity = 0.1f;
    public float startupEffectIntensity = 0.1f;
    public float upsideDownEffectIntensity = 0.5f;

    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    private Quaternion initialRotation;
    private float distance;
    private float mouseX;
    private float mouseY;
    private float shakeTimer;
    private Vector3 initialPosition;
    private bool isUpsideDown;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position and rotation
        Vector3 targetPos = target.position + offset;
        desiredPosition = targetPos - transform.forward * distance;
        desiredRotation = Quaternion.LookRotation(targetPos - transform.position);

        // Smoothly move camera to desired position and rotation
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed);

        // Apply camera effects
        ApplyAccelEffect();
        ApplyMouseEffect();
        ApplyCrunchEffect();
        ApplyBrakeEffect();
        ApplyNitroEffect();
        ApplySlipEffect();
        ApplyStopEffect();
        ApplyStartupEffect();
        ApplyUpsideDownEffect();
        // Handle camera shake
        if (shakeTimer > 0)
        {
            transform.position = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
    }
    private void ApplyAccelEffect()
    {
        float accelInput = Input.GetAxis("Vertical");
        transform.position += transform.forward * accelInput * accelEffectIntensity;
    }

    private void ApplyMouseEffect()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseEffectIntensity;
        mouseY -= Input.GetAxis("Mouse Y") * mouseEffectIntensity;
        mouseY = Mathf.Clamp(mouseY, -45f, 45f);
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
    }

    private void ApplyCrunchEffect()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            shakeTimer = shakeDuration;
            transform.position = initialPosition + Vector3.forward * crunchEffectIntensity;
        }
    }
    private void ApplyBrakeEffect()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += transform.forward * brakeEffectIntensity;
        }
    }

    private void ApplyNitroEffect()
    {
        if (Input.GetKey(KeyCode.N))
        {
            transform.position += transform.forward * nitroEffectIntensity;
        }
    }
    private void ApplySlipEffect()
    {
        if (isUpsideDown) return;

        float slipInput = Input.GetAxis("Horizontal");
        float slipAngle = slipInput * slipEffectIntensity;
        transform.Rotate(Vector3.up, slipAngle);
    }
    private void ApplyStopEffect()
    {
        if (target.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f)
        {
            transform.position += Vector3.up * stopEffectIntensity;
        }
    }

    private void ApplyStartupEffect()
    {
        if (target.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f && Input.GetKeyDown(KeyCode.W))
        {
            shakeTimer = shakeDuration;
            transform.position = initialPosition + Vector3.forward * startupEffectIntensity;
        }
    }

    private void ApplyUpsideDownEffect()
    {
        Vector3 up = transform.up;
        Vector3 targetUp = target.up;

        if (Vector3.Dot(up, targetUp) < 0f)
        {
            isUpsideDown = true;
            transform.position += Vector3.up * upsideDownEffectIntensity;
        }
        else
        {
            isUpsideDown = false;
        }
    }
}
