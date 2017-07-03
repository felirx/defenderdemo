using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    protected Rigidbody2D _rb = null;
    public Rigidbody2D rb
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

        rb.velocity = 5.0f * new Vector2(xAxis, yAxis);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // just die away
    }
}
