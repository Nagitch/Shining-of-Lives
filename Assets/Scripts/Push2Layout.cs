using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbletonPush2;
using System.Linq;

public class Push2Layout : MonoBehaviour
{
    public Sequencer[] sequencers;
    public int partSelected = 0;
    private int partSelectedPrev = 0;
    private bool[] sequencePrev = new bool[16];
    private int currentStepPrev = 1;
    private int PAD_SEQUENCER_ORIGIN = 92;

    private PadSequenceState[] padSequenceState = new PadSequenceState[16];
    private PadSequenceState[] padSequenceStatePrev = new PadSequenceState[16];

    public enum PadSequenceState {
        BLANK,
        NOTE,
        CURRENTSTEP,
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Push2.padPressedDelegate += PadPressed;
        Push2.padReleasedDelegate += PadReleased;
        Push2.afterTouchDelegate += AfterTouch;

        // initialize vars
        for(int i=0; i < padSequenceState.Length; i++) {
            padSequenceState[i] = PadSequenceState.BLANK;
        }
        for(int i=0; i < padSequenceStatePrev.Length; i++) {
            padSequenceState[i] = PadSequenceState.BLANK;
        }

    }
    void Start()
    {
    }

    void Update()
    {
        var sequencer = sequencers[partSelected];
        var sequence = sequencer.sequence;
        var currentStep = sequencer.GetCurrentStep();

        // cofirm cuurent sequence
        for(int i=0; i < padSequenceState.Length; i++) {
            if(currentStep - 1 == i) {
                padSequenceState[i] = PadSequenceState.CURRENTSTEP;
            } else {
                padSequenceState[i] = sequence[i] == true ? PadSequenceState.NOTE : PadSequenceState.BLANK;
            }
        }

        // update pads lighting
        for(int i=0; i < sequence.Length; i++) {
            var pad = Pads.All.Find(e => { return e.number == SeqToPadsNumber(i); });
            if(padSequenceState[i] != padSequenceStatePrev[i]) {
                var padColor = padSequenceState[i] == PadSequenceState.CURRENTSTEP ? LED.Color.RGB.Red : padSequenceState[i] == PadSequenceState.NOTE ? LED.Color.RGB.Green : LED.Color.RGB.DarkGray;
                Push2.SetLED(pad, padColor);
            }
        }

        // update previouse state
        partSelectedPrev = partSelected;
        currentStepPrev = currentStep;
        for(int i=0; i < padSequenceState.Length; i++) {
            padSequenceStatePrev[i] = padSequenceState[i];
        }
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

    private int SampleToPadsNumber(int sampleNumber) {
        return 0;
    }

    private int PadsNumberToSeq(int padNumber) {
        return 0;
    }
}
