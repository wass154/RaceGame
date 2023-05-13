using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioSource audioSource;
    public float minPitch = 0.5f;
    public float maxPitch = 2f;
    public float speedMultiplier = 0.01f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        float speed = rb.velocity.magnitude * speedMultiplier;
        float pitch = Mathf.Lerp(minPitch, maxPitch, speed);
        audioSource.pitch = pitch;
    }
}