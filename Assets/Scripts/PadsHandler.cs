using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbletonPush2;
using UnityEngine.Experimental.VFX;

public class PadsHandler : MonoBehaviour
{
    public VisualEffect kickReaction;
    // Start is called before the first frame update
    void Start()
    {
        // kickReaction = GetComponent<VisualEffect>();
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
        kickReaction.SetFloat("level", 1.0f);
    }

    public void PadReleased(Pad pad)
    {
        Push2.SetLED(pad, LED.Color.RGB.LightGray);
        kickReaction.SetFloat("level", 0.0f);
    }

    public void AfterTouch(float pressure)
    {
    }
}
