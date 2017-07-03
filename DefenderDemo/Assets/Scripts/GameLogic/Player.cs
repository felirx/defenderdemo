using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    protected float CameraPeekAheadDist = 4.0f;
    // Use this for initialization
    void Start()
    {
        MToolBox.IM.RegisterPlayer(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        rigidBody.velocity = 5.0f * new Vector2(xAxis, yAxis);

        bool left = xAxis < 0.0f;

        if (Camera.main)
        {
            // keep camera locked at world space 4
            Vector3 camPos = Camera.main.transform.position;
            camPos.y = 4.0f;
            Camera.main.transform.position = camPos;

            // look ahead with the camera
            camPos = Camera.main.transform.localPosition;
            if (rigidBody.velocity.magnitude > 0.05f)
                camPos.x = PGeneric.Utilities.MathUtilities.ConstantValueInterpolation(camPos.x, left ? -CameraPeekAheadDist : CameraPeekAheadDist, 10.0f * Time.deltaTime);
            else
                camPos.x = PGeneric.Utilities.MathUtilities.ConstantValueInterpolation(camPos.x, 0.0f, 10.0f * Time.deltaTime);
            Camera.main.transform.localPosition = camPos;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // just die away
    }
}
