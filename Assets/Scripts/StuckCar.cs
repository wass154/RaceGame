using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]

public class StuckCar : MonoBehaviour
{
    [SerializeField] Transform wheel;
    [SerializeField] float RayNumbers;
    [SerializeField] float RayMaxAngle;

    private WheelCollider Wheel;
    private float Radius;
    private void Awake()
    {
        Wheel = GetComponent<WheelCollider>();
        Radius = Wheel.radius;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i <= RayNumbers; i++)
        {
            Vector3 rayDistance = Quaternion.AngleAxis(i*(RayMaxAngle/RayNumbers),transform.right) * transform.forward;
            if(Physics.Raycast(wheel.position,rayDistance,out RaycastHit hit, Wheel.radius))
            {
                Debug.DrawLine(wheel.position, rayDistance * Radius,Color.green);
            }
        }
    }
}
