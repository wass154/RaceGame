using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCar : MonoBehaviour
{

    public float jumpForce = 10f; // Adjust the jump force as needed
    public float gravity = 20f; // Adjust the gravity force as needed

 public  bool isGrounded = false; // Keep track of whether the car is on the ground
    public Rigidbody rb; // Reference to the car's Rigidbody component

    void Start()
    {
    }

    void Update()
    {
      

      

        // Check if the car is on the ground
        if (isGrounded)
        {
                // Add a force to the car's Rigidbody component to make it jump
               rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // Set isGrounded to false so the car can't jump again until it lands
                isGrounded = false;
            }
        else
        {
            // Apply a downward force to the car's Rigidbody component to simulate gravity
            rb.AddForce(Vector3.down * gravity * Time.deltaTime, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the car enters a trigger tagged as "Ground"
        if (other.gameObject.CompareTag("Ground"))
        {
            // Set isGrounded to true so the car can jump again
            isGrounded = true;

            // Stop the car's vertical velocity to prevent it from bouncing on the ground
          rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the car exits a trigger tagged as "Ground"
        if (other.gameObject.CompareTag("Ground"))
        {
            // Set isGrounded to false
            isGrounded = false;
        }
    }
}