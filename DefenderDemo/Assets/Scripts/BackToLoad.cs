using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackToLoad : MonoBehaviour, IPointerDownHandler
{
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        MToolBox.IM.DestroyAllActors();
        MToolBox.ClearTools();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
