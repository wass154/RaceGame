using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField] GameObject FL;
    [SerializeField] GameObject FR;
    [SerializeField] GameObject RL;
    [SerializeField] GameObject RR;
    [SerializeField] GameObject SmokePrefab;

    [SerializeField] Transform wheelFL;
    [SerializeField] Transform wheelFR;
    [SerializeField] Transform wheelRL;
    [SerializeField] Transform wheelRR;

    [SerializeField] WheelCollider ColliderFL;
    [SerializeField] WheelCollider ColliderFR;
    [SerializeField] WheelCollider ColliderRL;
    [SerializeField] WheelCollider ColliderRR;

    [SerializeField] float smokeDuration = 2f; // Duration of smoke effects for front wheels
    [SerializeField] float smokeInterval = 0.1f; // Interval at which to instantiate smoke effects for front wheels

    private List<GameObject> frontSmokeEffects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSmoke();
    }

    void FixedUpdate()
    {
        CreateSmoke();
    }

    void CreateSmoke()
    {
        WheelHit[] hit = new WheelHit[4];
        ColliderFL.GetGroundHit(out hit[0]);
        ColliderFR.GetGroundHit(out hit[1]);
        ColliderRL.GetGroundHit(out hit[2]);
        ColliderRR.GetGroundHit(out hit[3]);

        float slipThreshold = 0.1f;
        if ((Mathf.Abs(hit[2].sidewaysSlip) + Mathf.Abs(hit[2].forwardSlip) > slipThreshold) ||
            (Mathf.Abs(hit[3].sidewaysSlip) + Mathf.Abs(hit[3].forwardSlip) > slipThreshold))
        {
            if (RL == null)
            {
                RL = Instantiate(SmokePrefab, wheelRL.position, Quaternion.identity);
                RL.transform.parent = wheelRL;
            }
            if (RR == null)
            {
                RR = Instantiate(SmokePrefab, wheelRR.position, Quaternion.identity);
                RR.transform.parent = wheelRR;
            }
        }
        else
        {
            if (RL != null)
            {
                Destroy(RL);
            }
            if (RR != null)
            {
                Destroy(RR);
            }
        }
    }

    void UpdateSmoke()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            for (int i = 0; i < 2; i++)
            {
                if (frontSmokeEffects.Count <= i || frontSmokeEffects[i] == null)
                {
                    GameObject smokeEffect = Instantiate(SmokePrefab, i == 0 ? wheelFL.position : wheelFR.position, Quaternion.identity);
                    frontSmokeEffects.Insert(i, smokeEffect);
                }
            }
            StartCoroutine(DestroySmokeAfterDelay(frontSmokeEffects, smokeDuration));
        }
        else
        {
            DestroyAllSmoke(frontSmokeEffects);
        }
    }

    IEnumerator DestroySmokeAfterDelay(List<GameObject> smokeEffects, float delay)
    {
        float startTime = Time.time;
        float endTime = startTime + delay;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / delay;
            float alphaThreshold = Mathf.Lerp(1f, 0f, t); // Gradually reduce the alpha threshold from 1 to 0
            foreach (GameObject smokeEffect in smokeEffects)
            {
                if (smokeEffect != null)
                {
                    Material smokeMaterial = smokeEffect.GetComponent<Renderer>().material;
                    smokeMaterial.SetFloat("_AlphaThreshold", alphaThreshold); // Set the alpha threshold
                }
            }
            yield return null;
        }
        DestroyAllSmoke(smokeEffects);
    }

    void DestroyAllSmoke(List<GameObject> smokeEffects)
    {
        foreach (GameObject smokeEffect in smokeEffects)
        {
            if (smokeEffect != null)
            {
                Destroy(smokeEffect);
            }
        }
        smokeEffects.Clear();
    }
}