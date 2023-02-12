using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreqParticles : MonoBehaviour
{
    public SoundCapture capture;

    public ParticleSystem particles;

    public float OffsetX = 0;
    public float scale = 10;

    Vector3 startPos;



    GameObject[] bars;

    // Start is called before the first frame update
    void Start()
    {
        startPos = Camera.main.transform.position;
        startPos.z += 14;
        startPos.y -= 6;
        startPos.x = startPos.x + OffsetX;
        transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        var emission = particles.emission; // Stores the module in a local variable
        particles.startSpeed = capture.maxRawVolume * scale;
        particles.startColor = new Color(50,50,150,1);
    }
}
