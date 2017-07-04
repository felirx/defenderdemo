using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    bool pressing = false;
    void Update()
    {
        if (pressing)
        {
            // probably breaks encapsulation rules all around, Player should instead listen to an event from here
            if (MToolBox.GM.MyPlayer)
            {
                MToolBox.GM.MyPlayer.Fire();
            }
        }
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        pressing = false;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        pressing = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        pressing = false;
    }
}
