using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    protected float CameraPeekAheadDist = 4.0f;
    protected PGeneric.Utilities.Timer FireTimer = new PGeneric.Utilities.Timer();
    protected float FireRate = 400.0f;

    protected SpriteRenderer Graphic = null;
    protected bool GoingLeft = false;
    // Use this for initialization
    void Start()
    {
        MToolBox.IM.RegisterActor(this);

        Graphic = PGeneric.Utilities.FindUtilities.SearchHierarchyForComponent<SpriteRenderer>(transform, "GFX");
    }

    // Update is called once per frame
    void Update()
    {
        FireTimer.Tick(Time.deltaTime);

        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector2 unityAxis = new Vector2(xAxis, yAxis);
        Vector2 uiAxis = MToolBox.GM.UIInputAxis;
        Vector2 finalAxis = Vector2.zero;

        // UI axis should have priority
        if (uiAxis.magnitude > 0.05f)
        {
            finalAxis.x = uiAxis.x;
            finalAxis.y = uiAxis.y;
        }
        else if (unityAxis.magnitude > 0.05f)
        {
            finalAxis.x = unityAxis.x;
            finalAxis.y = unityAxis.y;
        }

        rigidBody.velocity = 5.0f * finalAxis;

        if (Graphic)
        {
            if (finalAxis.magnitude > 0.05f)
            {
                if (finalAxis.x > 0.0f)
                {
                    Graphic.transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
                    GoingLeft = false;
                }
                else
                {
                    Graphic.transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);
                    GoingLeft = true;
                }
            }
        }
        bool left = finalAxis.x < 0.0f;

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

        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
    }

    public void Fire()
    {
        if (FireTimer.Ticked() || !FireTimer.Started())
        {
            FireTimer.Start(FireRate);

            Vector3 dir = GoingLeft ? -gameObject.transform.right : gameObject.transform.right;
            MToolBox.GM.SpawnProjectile(gameObject, gameObject.transform.position + dir * 0.5f, dir, 10.0f, 3000.0f);
        }
    }

    public void Die()
    {
        // spawn death particles

        // signal the game manager
        MToolBox.IM.RemoveActor(this);
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
