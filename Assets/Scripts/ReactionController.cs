using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class ReactionController : MonoBehaviour
{
    public AudioSpectrum spectrum;
    VisualEffect vfx;
    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        vfx.SetFloat("level", spectrum.Levels[1]);
    }
}
