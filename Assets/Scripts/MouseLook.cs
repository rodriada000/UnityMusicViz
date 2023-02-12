using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Vector2 sensitivity;
    public Vector2 acceleration;
    public float inputLagPeriod = 0.05f;
    public float maxVertAngleFromHorizon;


    private Vector2 velocity;
    private Vector2 rotation;

    private Vector2 lastInputEvent;
    private float inputLagTimer;

    private float ClampVerticalAngle(float angle) {
        return Mathf.Clamp(angle, -maxVertAngleFromHorizon, maxVertAngleFromHorizon);
    }

    private Vector2 GetInput()
    {
        inputLagTimer += Time.deltaTime;

        Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (!(Mathf.Approximately(0, input.x) && Mathf.Approximately(0, input.y)) || inputLagTimer >= inputLagPeriod)
        {
            lastInputEvent = input;
            inputLagTimer = 0;
        }

        return lastInputEvent;
    }

    private void OnEnable() {
        // reset the state
        velocity = Vector2.zero;
        inputLagTimer = 0;
        lastInputEvent = Vector2.zero;

        Vector3 euler = transform.localEulerAngles;
        if (euler.x >= 180) {
            euler.x -= 360;
        }
        euler.x = ClampVerticalAngle(euler.x);

        transform.localEulerAngles = euler;

        rotation = new Vector2(euler.y, euler.x);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 wantedVelocity = GetInput() * sensitivity;

        velocity = new Vector2(
            Mathf.MoveTowards(velocity.x, wantedVelocity.x, acceleration.x * Time.deltaTime), 
            Mathf.MoveTowards(velocity.y, wantedVelocity.y, acceleration.y * Time.deltaTime));

        rotation += velocity * Time.deltaTime;
        rotation.y = ClampVerticalAngle(rotation.y);

        transform.localEulerAngles = new Vector3(rotation.y, rotation.x, 0);
    }
}
