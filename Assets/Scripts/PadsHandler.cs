using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbletonPush2;

public class PadsHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        Push2.padPressedDelegate += PadPressed;
        Push2.padReleasedDelegate += PadReleased;
        Push2.afterTouchDelegate += AfterTouch;
    }

    public void PadPressed(Pad pad, float velocity)
    {
        Push2.SetLED(pad, LED.Color.RGB.Red, LED.Animation.None);
    }

    public void PadReleased(Pad pad)
    {
        Push2.SetLED(pad, LED.Color.RGB.LightGray);
    }

    public void AfterTouch(float pressure)
    {
    }
}
