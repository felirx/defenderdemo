using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PGeneric.FSM;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PMobile.MultiTouch;

public class TouchControls : MonoBehaviour 
{
    TouchTracker[] Trackers;
	public static TouchControls instance;
    Vector2 runtimeScaleModifier;
    public float runtimeDistanceModifier;
    int _trackerCount;

    public string CurrentStateName
    { 
        get 
        {
            if (Controls != null)
                return Controls.GetType().ToString();
            else
                return "Null";
        } 
    }

    public void DisableTracker(int i)
    {
        if (i < 0)
            i = Math.Abs(i) - 1;

        Trackers[i].Disabled = true;
    }
    public void EnableTracker(int i)
    {
        if (i < 0)
            i = Math.Abs(i) - 1;

        Trackers[i].Disabled = false;
    }

    public int AliveTrackers
    { 
        get 
        {
            int count = 0;
            for (int i = 0; i < Trackers.Length; i++)
            {
                if (Trackers[i].Alive == true)
                    count++;
            }

            return count;
        } 
    }

    public void Awake()
    {
        TouchControls[] tc = FindObjectsOfType<TouchControls>();
        if(tc.Length > 1)
        {
            Debug.LogError("More than two Touch controllers!");
        }
		instance = this;
        Trackers = new TouchTracker[10];
        for (int i = 0; i < Trackers.Length; i++)
        {
            Trackers[i] = new TouchTracker();
        }

        SwitchState(new TouchControlState());
        runtimeScaleModifier = new Vector2((float)Screen.width / 1024.0f, (float)Screen.height / 768.0f);
        runtimeDistanceModifier = (runtimeScaleModifier.x + runtimeScaleModifier.y) / 2;
    }

    public void Update()
    {
        if (Input.touchCount < 1)
            HandleMouseInputs();
        else
            HandleTouchInputs();
        
        Controls.OnControlUpdate();
    }

    /// <summary>
    /// Only Mouse 1 seems to return hit normals etc. 
    /// </summary>
    void HandleMouseInputs()
    {
        if (Trackers == null) //trackers need to be initialized at start.
            return;

        for (int i = 0; i < 3; i++)
        {
            if(Input.GetMouseButtonDown(i))
            {
                if (IsPointerOverUIObject(Input.mousePosition))
                    return;

                if(Trackers[i] == null)    
                    return;
            
                TouchInput t = CreateMouseTouch(i);
                Trackers[i].SetTouch(t);

                OnTouchBegin(Trackers[i]);
                Trackers[i].Alive = true;
            }
            else if(Input.GetMouseButton(i))
            {   
                if (Trackers[i] != null && !Trackers[i].Alive)
                    return;

                Trackers[i].Update(CreateMouseTouch(i));

                if (Trackers[i].PositionDelta.magnitude > 0.1f)
                    OnDrag(Trackers[i]);
                else
                    OnStationary(Trackers[i]);
            }
            else if(Input.GetMouseButtonUp(i))
            {
                if (!Trackers[i].Alive)
                    return;
                
                Trackers[i].Update(CreateMouseTouch(i));
                OnTouchEnd(Trackers[i]);
                Trackers[i].Alive = false;
            }
        }
    }

    void HandleTouchInputs()
    {
        for(int i = 0; i < Input.touches.Length; i++)
        {
            Touch t = Input.touches[i];

            switch(t.phase)
            {
                case TouchPhase.Began:
                    {
                        if (IsPointerOverUIObject(t.position))
                            continue;


                        Trackers[t.fingerId].SetTouch(CreateTouchInput(t));
                        Trackers[t.fingerId].Alive = true;

                        OnTouchBegin(Trackers[i]);
                    }
                    break;
                case TouchPhase.Ended:
                    {
                        if (!Trackers[t.fingerId].Alive)
                            continue;

                        Trackers[t.fingerId].Update(CreateTouchInput(t));
                        OnTouchEnd(Trackers[t.fingerId]);

                        Trackers[t.fingerId].Alive = false;
                    }
                    break;
                case TouchPhase.Moved:
                    {
                        if (!Trackers[t.fingerId].Alive)
                            continue;

                        Trackers[t.fingerId].Update(CreateTouchInput(t));
                        OnDrag(Trackers[t.fingerId]);
                    }
                    break;
                case TouchPhase.Stationary:
                    {
                        if (!Trackers[t.fingerId].Alive)
                            continue;

                        Trackers[t.fingerId].Update(CreateTouchInput(t));
                        OnStationary(Trackers[t.fingerId]);
                    }
                    break;
                case TouchPhase.Canceled: ///``??????????????
                    {
                        if (!Trackers[i].Alive)
                            continue;

                        Trackers[t.fingerId].Update(CreateTouchInput(t));
                        OnTouchEnd(Trackers[t.fingerId]);

                        Trackers[t.fingerId].Alive = false;
                    }
                    break;
            }
        }
    }

    //GraphicRaycaster caster;
    List<BaseRaycaster> casters;
    private bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        if (casters == null)
        {
            casters = new List<BaseRaycaster>(FindObjectsOfType<BaseRaycaster>());
            
            //Logger.Log(Logger.Channel.Pasi, "casters count:" + casters.Count); 
        }
        else
        {
            bool refresh = false;
            for (int i = 0; i < casters.Count; i++)
            {
                if (casters[i] == null)
                {
                    refresh = true;
                   // Logger.LogWarning(Logger.Channel.Pasi, "<color=red>Unnecesesary raycasters in scene..</color>");
                }
            }

            //caster(s) been destroyed, better to fetch them again.
            if (refresh)
            {
                casters = new List<BaseRaycaster>(FindObjectsOfType<BaseRaycaster>());
            }
        }

        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;

        for (int i = 0; i < casters.Count; i++)
        {
            BaseRaycaster uiRaycaster = casters[i];
            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(eventDataCurrentPosition, results);

            if (results.Count > 0)
            {
                //for (int j = 0; j < results.Count; j++)
                //{
                //    Logger.Log(Logger.Channel.Pasi, "<Color=Yellow>" + results[j].gameObject.name + "</color>");
                //}
                return true;
            }
        }
        return false;
    }

    TouchInput CreateMouseTouch(int index)
    {
        return new TouchInput() { Active = true, fingerId = index, position = Input.mousePosition, tapCount = 0, phase = TouchPhase.Began, timestamp = 0.0f};
    }

    TouchInput CreateTouchInput(Touch t)
    {
        TouchInput inp = new TouchInput();
        //inp.position = t.position;
        inp.position = ScaleInput(t.position.x, t.position.y); 
        inp.tapCount = t.tapCount;
        inp.fingerId = t.fingerId;
        return inp;
    }

    /// <summary>
    /// Oldshit
    /// </summary>
    /// <param name="touchID"></param>
    public virtual void CreateTouch(int touchID)
    {
        if(touchID < 0)
            touchID = Math.Abs(touchID) -1;

        Trackers[touchID].SetTouch(CreateMouseTouch(touchID));
        OnTouchBegin(Trackers[touchID]);
    }

    public TouchTracker _currentTouch;
    TouchControlState _controlState, _previousState;
    public TouchControlState Controls { get { return _controlState; } }

    public void OnTouchBegin(TouchTracker touch)
    {
        if (touch.Disabled)
            return;

        //Debug.Log(touch.FingerId + " TOUCH");
        _trackerCount++;
        _currentTouch = touch;
        if (Controls != null)
        {
            Controls.OnTouchBegin(touch);
        }
        else
        {
            Debug.Log("Control state is null");
        }
    }
    public void OnTouchEnd(TouchTracker touch)
    {
        if (touch.Disabled)
            return;

        //Debug.Log(touch.FingerId + "TOUCHEND");
        _trackerCount--;
        _currentTouch = touch;
        if (Controls != null)
        {
            Controls.OnTouchEnd(touch);
        }
    }
    public void OnDrag(TouchTracker touch)
    {
        if (touch.Disabled)
            return;

        //Debug.Log(touch.FingerId + "Dragging");
        _currentTouch = touch;
        if (Controls != null)
        {
            Controls.OnTouchUpdate(touch);
            Controls.OnTouchMoved(touch);
        }
    }
    public void OnStationary(TouchTracker touch)
    {
        if (touch.Disabled)
            return;

        //Debug.Log(touch.FingerId + "stationary");
        _currentTouch = touch;
        if (Controls != null)
        {
            Controls.OnTouchUpdate(touch);
            Controls.OnTouchStationary(touch);
        }
    }

    /// <summary>
    /// Changes current control scheme, e.g when throwing objects touch controls work differently for duration of the throw.
    /// </summary>
    /// <param name="ctrState"></param>
    public void SwitchState(TouchControlState ctrState)
    {
        if (ctrState == null)
           return;
    
        if (_controlState == null || ctrState.GetType() != _controlState.GetType())
            _previousState = _controlState;
    
        if (_controlState != null)
            _controlState.End();

        _controlState = ctrState;
        _controlState.Enter(this);
    }
    public void SwitchToPreviousState()
    {
        if(_previousState != null)
        {
            SwitchState(_previousState);
        }
        else
            Debug.LogError("No Previous state");  
    }
    
    public float _minimumDistance = 0.0f, _allowedVariance = 0.0f, xDeltaAbs = 0.0f, yDeltaAbs = 0.0f, minimumDistance = 0.0f, allowedVariance = 0.0f;
    public Vector2 ScaleInput(float x, float y)
    {
        _minimumDistance = minimumDistance * runtimeDistanceModifier;
        _allowedVariance = allowedVariance * runtimeDistanceModifier;
        xDeltaAbs = Mathf.Abs(x);
        yDeltaAbs = Mathf.Abs(y);


        return new Vector2(x,y);
    }
}
