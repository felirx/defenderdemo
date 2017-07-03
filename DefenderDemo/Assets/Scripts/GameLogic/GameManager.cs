using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    protected float ResetDistance = 10.0f;
    protected Player _player = null;

    public Player MyPlayer { get { return _player; } }

    public void Init()
    {
        Debug.Log("Initializing Game Manager");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (null == MyPlayer)
        {
            // shit solution
            _player = FindObjectOfType<Player>();
            if (null == MyPlayer)
            {
                Debug.LogError("Could not find player");
                return;
            }
        }

        // did the player go past reset distance?
        if (MyPlayer.transform.position.x < -ResetDistance || MyPlayer.transform.position.y > ResetDistance)
        {
            ResetWorldCenter.ResetWorldToOrigin(new Vector3(MyPlayer.transform.position.x, 0.0f, 0.0f));
        }
	}
}
