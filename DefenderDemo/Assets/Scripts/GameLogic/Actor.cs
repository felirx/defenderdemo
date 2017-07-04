using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    protected Rigidbody2D _rb = null;
    public Rigidbody2D rigidBody
    {
        get
        {
            if (null == _rb)
            {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    protected void HandleWrapping(Vector2 focus, float maxDistance, bool left)
    {
        if (Mathf.Abs(transform.position.x - focus.x) > maxDistance)
        {
            Vector3 pos = transform.position;
            pos.x += left ? -MToolBox.GM.WorldWidth : MToolBox.GM.WorldWidth;
            transform.position = pos;
        }
    }
}
