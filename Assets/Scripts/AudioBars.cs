using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioBars : MonoBehaviour
{

    public SoundCapture capture;

    // public GradientColorKey[] colorKeys;
    // public GradientAlphaKey[] alphaKeys;

    public Gradient gradient;

    Vector3 startPos;
    GameObject[] bars;

    // Use this for initialization
    void Start()
    {
        startPos = Camera.main.transform.position;
        startPos.z += 12;
        startPos.y -= 6;
        startPos.x -= capture.numBars / 2;

        MakeBars();

        // gradient.SetKeys(colorKeys, alphaKeys);
    }

    void MakeBars()
    {

        bars = new GameObject[capture.numBars];
        for (int i = 0; i < bars.Length; i++)
        {
            GameObject visiBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visiBar.transform.position = new Vector3(startPos.x + i, startPos.y, startPos.z);
            visiBar.transform.parent = transform;
            visiBar.name = "VisiBar " + i;
            bars[i] = visiBar;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bars.Length != capture.numBars)
        {
            foreach (GameObject bar in bars)
            {
                Destroy(bar);
            }

            MakeBars();
        }

        // Since this is being changed on a seperate thread we do this to be safe
        lock (capture.barLock)
        {
            Color? newColor = null;
            if (capture.barData.Length > 0)
            {
                var freqMag = capture.GetMagnitudeByFrequence();
                if (freqMag.Count > 0)
                {
                    var freqMax = freqMag.Max(f => f.Item2);
                    var freq = freqMag.Where(f => f.Item2 == freqMax).Select(f => f.Item1).FirstOrDefault();
                    var maxBandIndex = capture.GetFftBandIndex(freq);

                    float mapped = map(maxBandIndex, 0, freqMag.Where(f => f.Item1 > capture.minFreq && f.Item1 < capture.maxFreq).Count(), 0, 1);

                    Debug.Log($"freqMax = {freqMax}; maxBandIndex = {maxBandIndex}; freq = {freq}; mapped = {mapped}");
                    newColor = gradient.Evaluate(mapped);
                }

            }

            for (int i = 0; i < capture.barData.Length; i++)
            {
                // Don't make the bars too short
                float curData = Mathf.Max(0.01f, capture.barData[i]);

                // Set offset so they stretch off the ground instead of expand in the air
                bars[i].transform.position = new Vector3(startPos.x + i, startPos.y + (curData / 2.0f * 10.0f), startPos.z);
                bars[i].transform.localScale = new Vector3(1, curData * 10.0f, 1);

                if (newColor.HasValue) {
                    bars[i].GetComponent<Renderer>().material.color = newColor.Value;
                }
            }
        }


    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
