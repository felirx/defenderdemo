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
    }

    public void UpdateScore(int score)
    {
        if (label)
        {
            label.text = score.ToString();
        }
    }
}
