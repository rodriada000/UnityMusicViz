using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float speed = 5.0f;

    MouseLook lookRef;

    // Start is called before the first frame update
    void Start()
    {
        lookRef = gameObject.GetComponent<MouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);
        }

        if (Input.GetKey(KeyCode.M) && !lookRef.enabled)
        {
            lookRef.enabled = true;
        }
        else if (!Input.GetKey(KeyCode.M))
        {
            lookRef.enabled = false;
        }
    }
}
