using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Meter : MonoBehaviour
{

    public Rigidbody carRigidbody;
    public TextMeshProUGUI speedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // get speed with KM/H
        float speed = carRigidbody.velocity.magnitude * 3.6f;
        speedText.text =  Mathf.RoundToInt(speed).ToString();
/*
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if ((x == 0)&& (y == 0)){
            speedText.text = Mathf.RoundToInt(speed).ToString();
        }
*/
    }
}
