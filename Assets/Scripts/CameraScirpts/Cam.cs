using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    [SerializeField] Transform Car;
    [SerializeField] Rigidbody carRb;
    [SerializeField] Vector3 Offset;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        Vector3 CarForward = (carRb.velocity + Car.transform.forward).normalized;
        transform.position=Vector3.Lerp(transform.position,Car.position+Car.transform.TransformVector(Offset)+CarForward*(-5f),speed*Time.deltaTime);
        transform.LookAt(Car);
    }
}
