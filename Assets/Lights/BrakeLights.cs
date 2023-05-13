using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeLights : MonoBehaviour
{
    public GameObject brakeLight;
    public Color mainColor;
    public Color brightColor;
    public float colorLerpDuration = 0.5f;

    private bool isBraking = false;
    private float lerpTime = 0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            isBraking = true;
            lerpTime += Time.deltaTime;

            if (lerpTime > colorLerpDuration)
            {
                lerpTime = colorLerpDuration;
            }

            float t = lerpTime / colorLerpDuration;
            brakeLight.GetComponent<Renderer>().material.color = Color.Lerp(mainColor, brightColor, t * t);
        }
        else
        {
            isBraking = false;
            lerpTime -= Time.deltaTime;

            if (lerpTime < 0)
            {
                lerpTime = 0;
            }

            float t = lerpTime / colorLerpDuration;
            brakeLight.GetComponent<Renderer>().material.color = Color.Lerp(mainColor, brightColor, t * t);
        }
    }
}