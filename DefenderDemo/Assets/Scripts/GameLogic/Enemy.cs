using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    protected float Velocity = 1.0f;

	// Use this for initialization
	void Start ()
    {
        MToolBox.IM.RegisterEnemy(gameObject);
       // MToolBox.GM.OnWorldWrappingUpdate += HandleWrapping;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Player p = MToolBox.GM.MyPlayer;
        if (null == p)
            return;

        // does not understand world wrapping, the player could be closer by using it
        Vector2 direction = p.transform.position - transform.position;
        direction.Normalize();
        rigidBody.velocity = direction * Velocity;

        HandleWrapping(p.transform.position, MToolBox.GM.WorldWidth * 0.5f, (direction.x < 0.0f));
	}


}
