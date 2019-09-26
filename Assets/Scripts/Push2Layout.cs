using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbletonPush2;
using System.Linq;

public class Push2Layout : MonoBehaviour
{
    public Sequencer[] sequencers;
    public int partSelected = 0;
    public bool[] padSequence;

    private int PAD_SEQUENCER_ORIGIN = 92;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Push2.padPressedDelegate += PadPressed;
        Push2.padReleasedDelegate += PadReleased;
        Push2.afterTouchDelegate += AfterTouch;
    }
    void Start()
    {
    }

    void Update()
    {
        var sequencer = sequencers[partSelected];
        var sequence = sequencer.sequence;
        for(int i=0; i < sequence.Length; i++) {
            var pad = Pads.All.Find(e => { return e.number == SeqToPadsNumber(i); });
            if(sequencer.GetCurrentStep()-1 == i) {
                Push2.SetLED(pad, LED.Color.RGB.Red);
            } else {
                if(sequence[i] == true) {
                    Push2.SetLED(pad, LED.Color.RGB.Green);
                } else {
                    Push2.SetLED(pad, LED.Color.RGB.DarkGray);
                }
            }
        }

        // TODO: refresh pads lighting
    }

    public void PadPressed(Pad pad, float velocity)
    {
    }

    public void PadReleased(Pad pad)
    {
    }

    public void AfterTouch(float pressure)
    {
    }

    private int SeqToPadsNumber(int sequenceNumber) {
        // 0 = 92
        int modulo = sequenceNumber % 8;
        int div = sequenceNumber / 8;
        return PAD_SEQUENCER_ORIGIN - (div * 8) + modulo;
    }
}
