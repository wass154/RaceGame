using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Buttons : MonoBehaviour
{
    public bool isPress;
    // Start is called before the first frame update
    void Start()
    {
        SetUpButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetUpButton()
    {
        EventTrigger EventTrig=gameObject.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID=EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => onClickDown());


        var pointerUp= new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => onClickup());

        EventTrig.triggers.Add(pointerDown);
        EventTrig.triggers.Add(pointerUp);
    }
    void onClickDown()
    {
isPress= true;
    }
    void onClickup()
    {
        isPress=false;
    }
}
