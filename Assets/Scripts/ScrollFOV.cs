using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollFOV : MonoBehaviour
{
    public Camera myCamera;
    public float sensitivity = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            myCamera.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        }
    }
}
