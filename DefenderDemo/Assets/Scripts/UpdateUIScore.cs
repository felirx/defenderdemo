using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class UpdateUIScore : MonoBehaviour
{
    protected UnityEngine.UI.Text label = null;

    // Use this for initialization
    void Start ()
    {
        label = GetComponent<UnityEngine.UI.Text>();

        UpdateScore();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateScore();
	}

    protected void UpdateScore()
    {
        if (label)
        {
            label.text = MToolBox.GM.GameScore.ToString();
        }
    }
}
