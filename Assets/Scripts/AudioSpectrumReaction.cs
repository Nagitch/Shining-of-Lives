using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrumReaction : MonoBehaviour
{
    public AudioSpectrum spectrum;
    private Vector3 originalScale;

    public int groupNumber = 0;
    public float scale = 1;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        var stretch = spectrum.MeanLevels[groupNumber] * scale;
        transform.localScale = new Vector3(originalScale.x, originalScale.y + stretch, originalScale.z);
    }
}
