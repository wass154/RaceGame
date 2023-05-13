using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 Offset;
    [Range(0, 1)]
    [SerializeField] float Smooth;

    
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
            Vector3 TargetPos=target.TransformPoint(Offset);
        transform.position = Vector3.Lerp(transform.position, TargetPos, Smooth);
        transform.LookAt(target);
    }
}
