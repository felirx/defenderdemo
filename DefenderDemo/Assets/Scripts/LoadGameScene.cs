using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameScene : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
}
