using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Actor
{
    public float LifeTime
    {
        get; set;
    }

    public float Velocity
    {
        get; set;
    }

    public Vector3 Direction
    {
        get; set;
    }

    protected SpriteRenderer gfx = null;
    void Start()
    {
        MToolBox.IM.RegisterActor(this);

        gfx = PGeneric.Utilities.FindUtilities.SearchHierarchyForComponent<SpriteRenderer>(transform, "GFX");

        if (gfx)
        {
            // hack
            if (LayerMask.NameToLayer("Player") == gameObject.layer)
            {
                gfx.color = Color.white;
            }
            else
            {
                gfx.color = Color.red;
            }
        }
    }

    protected PGeneric.Utilities.Timer lifeTimer = new PGeneric.Utilities.Timer();
    // Update is called once per frame
    void Update()
    {
        Player p = MToolBox.GM.MyPlayer;
        if (null == p)
        {
            rigidBody.velocity = Vector3.zero;
            return;
        }
 
        if (!lifeTimer.Started())
        {
            lifeTimer.Start(LifeTime);
        }

        if (lifeTimer.Tick(Time.deltaTime))
        {
            Die();
        }

        rigidBody.velocity = Velocity * Direction;

        HandleWrapping(p.transform.position, MToolBox.GM.WorldWidth * 0.5f, (Direction.x < 0.0f));
    }

    public void Die()
    {
        MToolBox.IM.RemoveActor(this);
        GameObject.Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Die();
    }
}
