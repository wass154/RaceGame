using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassMeter : MonoBehaviour
{
    [SerializeField] GameObject compass;
    [SerializeField] float StartPos;
    [SerializeField] float EndPos;
    [SerializeField] float Desired;
    [SerializeField] float speed;
    [SerializeField] Rigidbody rb;
    public TextMeshProUGUI speedText;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
     speed = rb.velocity.magnitude * 3.6f;
        if(speed>=0)
        {
            speedText.text = "1";
        }
      if (speed >= 20)
        {
            speedText.text = "2";
        }
      if (speed >= 40)
        {
            speedText.text = "3";
        }
     if (speed >= 60)
        {
            speedText.text = "4";
        }
   if (speed >= 80)
        {
            speedText.text = "5";
        }

    }
    private void FixedUpdate()
    {
        Desired = StartPos - EndPos;
        float Temporaire = speed / 180;
        //rotate compass on z axis
        compass.transform.eulerAngles = new Vector3(0, 0, (StartPos - Temporaire * Desired));
    }
}
