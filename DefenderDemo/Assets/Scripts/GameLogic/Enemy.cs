using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    protected float Velocity = 2.0f;
    protected PGeneric.Utilities.Timer ProjectileTimer = new PGeneric.Utilities.Timer();
    protected float FireRate = 2000.0f;

	// Use this for initialization
	void Start ()
    {
        MToolBox.IM.RegisterEnemy(gameObject);
        // MToolBox.GM.OnWorldWrappingUpdate += HandleWrapping;

        ProjectileTimer.Start(FireRate);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Player p = MToolBox.GM.MyPlayer;
        if (null == p)
        {
            rigidBody.velocity = Vector3.zero;
            return;
        }

        // does not understand world wrapping, the player could be closer by using it
        Vector3 direction = p.transform.position - transform.position;
        direction.Normalize();
        rigidBody.velocity = direction * Velocity;

        if (ProjectileTimer.Tick(Time.deltaTime))
        {
            MToolBox.GM.SpawnProjectile(gameObject, gameObject.transform.position + direction * 0.4f, direction, 4.0f, 5000.0f);
            ProjectileTimer.Start(FireRate);
        }

        HandleWrapping(p.transform.position, MToolBox.GM.WorldWidth * 0.5f, (direction.x < 0.0f));
	}

    public void Die()
    {
        MToolBox.IM.RemoveEnemy(gameObject);
        GameObject.Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Projectile")
        {
            MToolBox.GM.GameScore += 50;
            Die();
        }
    }
}
