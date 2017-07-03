using UnityEngine;
using System.Collections;
using PGeneric.FSM;
using PMobile.MultiTouch;

public class TouchControlState 
{
    protected TouchControls Control;
    protected TouchTracker _curTouch;

    public virtual void Enter(TouchControls ctrl) { Control = ctrl; }
    public virtual void End() { }

    public virtual void OnTouchBegin(TouchTracker touch) { }//Debug.Log("Touch Begun [fingerID = " + touch.FingerId + "] [Position =" + touch.Position + "]"); }
    public virtual void OnTouchUpdate(TouchTracker touch) { }//Debug.Log("Touch Begun [fingerID = " + touch.FingerId + "] [Position =" + touch.Position + "] [Delta = " + touch.PositionDelta + "][Time = " + touch.TimeFromStart + "]"); }
    public virtual void OnTouchStationary(TouchTracker touch) { }
    public virtual void OnTouchMoved(TouchTracker touch) { }
    public virtual void OnTouchEnd(TouchTracker touch) { }//Debug.Log("Touch End [fingerID = " + touch.FingerId + "] [Position =" + touch.Position + "]");  }
    public virtual void OnTouchDoubleTap(TouchTracker touch) { }
    public virtual void OnPinch(float pinchdelta) { }
    public virtual void OnTwist(float twistdelta) { }
    public virtual void OnSwipe(Vector2 swipespeed) { }

    //Mainly for PC controls
    public virtual void OnControlUpdate(){}
}
