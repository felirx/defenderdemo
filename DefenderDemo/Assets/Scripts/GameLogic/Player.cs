using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    protected float CameraPeekAheadDist = 4.0f;
    protected PGeneric.Utilities.Timer FireTimer = new PGeneric.Utilities.Timer();
    protected float FireRate = 100.0f;
    // Use this for initialization
    void Start()
    {
        MToolBox.IM.RegisterPlayer(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        FireTimer.Tick(Time.deltaTime);

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

        if (Input.GetButtonDown("Fire1"))
        {
            if (FireTimer.Ticked() || !FireTimer.Started())
            {
                FireTimer.Start(FireRate);

                MToolBox.GM.SpawnProjectile(gameObject, gameObject.transform.position + gameObject.transform.right * 0.5f, gameObject.transform.right, 10.0f, 50000.0f);
            }
        }
    }

    public void Die()
    {
        // spawn death particles

        // signal the game manager
        MToolBox.GM.PlayerDeath();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Collided with " + col.gameObject);
        // just die away unless we collided with scene borders

        if (col.gameObject.tag != "WorldBorder")
            Die();
    }
}
