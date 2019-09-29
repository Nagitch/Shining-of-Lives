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
    private int PAD_SEQUENCER_BOTTOM = 68;
    private int PAD_SAMPLER_ORIGIN = 36;

    private int PAD_COLOR_GREEN = 11;

    private PadSequenceState[] padSequenceState = new PadSequenceState[16];
    private PadSequenceState[] padSequenceStatePrev = new PadSequenceState[16];

    public PadSampleState[] padSampleState = new PadSampleState[16];
    private PadSampleState[] padSampleStatePrev = new PadSampleState[16];

    public enum PadSequenceState {
        BLANK,
        NOTE,
        CURRENTSTEP,
    }

    public enum PadSampleState {
        BLANK,
        ASSIGNED,
        PLAYING,
        PLAYING_ONESHOT,
        END_PLAYING_ONESHOT,
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
        for(int i=0; i < padSampleState.Length; i++) {
            padSampleState[i] = PadSampleState.BLANK;
        }
        for(int i=0; i < padSampleStatePrev.Length; i++) {
            padSampleStatePrev[i] = PadSampleState.BLANK;
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

        // cofirm current sequence
        for(int i=0; i < padSequenceState.Length; i++) {
            if(currentStep - 1 == i) {
                padSequenceState[i] = PadSequenceState.CURRENTSTEP;
            } else {
                padSequenceState[i] = sequence[i] == true ? PadSequenceState.NOTE : PadSequenceState.BLANK;
            }
        }

        // confirm current samples
        for(int i=0; i < padSampleState.Length; ++i) {
            if(padSampleState[i] != PadSampleState.PLAYING_ONESHOT) {
                if(sequencers[i].sequence[currentStep-1] == true ) {
                    padSampleState[i] = PadSampleState.PLAYING;
                } else {
                    padSampleState[i] = sequencers[i] != null ? PadSampleState.ASSIGNED : PadSampleState.BLANK;
                }
            }
        }

        // update pads lighting sequencer
        for(int i=0; i < sequence.Length; ++i) {
            var pad = Pads.All.Find(e => { return e.number == SeqToPadsNumber(i); });
            if(padSequenceState[i] != padSequenceStatePrev[i]) {
                var padColor = padSequenceState[i] == PadSequenceState.CURRENTSTEP ? LED.Color.RGB.Red : padSequenceState[i] == PadSequenceState.NOTE ? PAD_COLOR_GREEN : LED.Color.RGB.DarkGray;
                Push2.SetLED(pad, padColor);
            }
        }

        // update pads lighting sample
        for(int i=0; i < padSampleState.Length; i++) {
            var pad = Pads.All.Find(e => { return e.number == SampleToPadsNumber(i); });
            if(padSampleState[i] != padSampleStatePrev[i]) {
                var padColor = padSampleState[i] == PadSampleState.PLAYING_ONESHOT ? LED.Color.RGB.Red :
                padSampleState[i] == PadSampleState.END_PLAYING_ONESHOT ? LED.Color.RGB.Red :
                padSampleState[i] == PadSampleState.PLAYING ? LED.Color.RGB.Red :
                padSampleState[i] == PadSampleState.ASSIGNED ? PAD_COLOR_GREEN :
                LED.Color.RGB.DarkGray;
                Push2.SetLED(pad, padColor);
            }

            // Keep LED Color when State is not changed
        }

        // update previouse state
        partSelectedPrev = partSelected;
        currentStepPrev = currentStep;
        for(int i=0; i < padSequenceState.Length; i++) {
            padSequenceStatePrev[i] = padSequenceState[i];
        }
        for(int i=0; i < padSampleState.Length; i++) {
            padSampleStatePrev[i] = padSampleState[i];
        }
    }

    public void PadPressed(Pad pad, float velocity)
    {
        // sample area pressed
        int samplePos = PadsNumberToSample(pad.number);
        partSelected = samplePos != -1 ? samplePos : partSelected;
        if(samplePos != -1) {
            var sample = sequencers[partSelected].GetComponent<AudioSource>();
            sample.Play();
            padSampleState[partSelected] = PadSampleState.PLAYING_ONESHOT;
            StartCoroutine( PadSampleSetStateLater(partSelected, PadSampleState.END_PLAYING_ONESHOT, 0.2f) );
        }

        // sequencer area pressed
        PadsNumberToSeq(pad.number);
    }

    private IEnumerator PadSampleSetStateLater(int part, PadSampleState state, float timeout) {
        yield return new WaitForSeconds(timeout);
        padSampleState[part] = state;
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
        int modulo = sampleNumber % 4;
        int div = sampleNumber / 4;
        return PAD_SAMPLER_ORIGIN + (div * 8) + modulo;
    }

    private int PadsNumberToSeq(int padNumber) {
        if(padNumber < PAD_SEQUENCER_BOTTOM) {
            return -1;
        }
        var basePos = padNumber - PAD_SEQUENCER_BOTTOM;
        int modulo = basePos % 8;
        int div = basePos / 8;
        var col = 4 - div;
        var pos = (col-1) * 8 + modulo;
        return pos;
    }

    private int PadsNumberToSample(int padNumber) {
        int modulo = (padNumber - PAD_SAMPLER_ORIGIN) % 8;
        int div = (padNumber - PAD_SAMPLER_ORIGIN) / 8;
        int sampleNum = modulo + (div * 4);
        if(modulo >= 4 || sampleNum >= sequencers.Length) {
            return -1;
        }
        return sampleNum;
    }
}
