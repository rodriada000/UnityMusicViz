using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public GameObject audioBars;
    public GameObject twinkleStars;
    public GameObject freqParticles;


    public bool isBarsVisible = true;
    public bool isTwinkleVisible = true;
    public bool isFreqParticlesVisible = true;

    // Start is called before the first frame update
    void Start()
    {
        audioBars?.SetActive(isBarsVisible);
        twinkleStars?.SetActive(isTwinkleVisible);
        freqParticles?.SetActive(freqParticles);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isBarsVisible = !isBarsVisible;
            audioBars?.SetActive(isBarsVisible);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isTwinkleVisible = !isTwinkleVisible;
            twinkleStars?.SetActive(isTwinkleVisible);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            isFreqParticlesVisible = !isFreqParticlesVisible;
            freqParticles?.SetActive(isFreqParticlesVisible);
        }

        // individual cone blasts
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            var child = freqParticles.transform.GetChild(0).gameObject;
            child.SetActive(!child.activeSelf);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            var child = freqParticles.transform.GetChild(1).gameObject;
            child.SetActive(!child.activeSelf);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            var child = freqParticles.transform.GetChild(2).gameObject;
            child.SetActive(!child.activeSelf);
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            var child = freqParticles.transform.GetChild(3).gameObject;
            child.SetActive(!child.activeSelf);
        }
        else if (Input.GetKeyDown(KeyCode.Equals))
        {
            var child = freqParticles.transform.GetChild(4).gameObject;
            child.SetActive(!child.activeSelf);
        }
    }
}
