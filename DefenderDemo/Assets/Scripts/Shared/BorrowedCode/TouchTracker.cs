using UnityEngine;
using System.Collections.Generic;

namespace PMobile.MultiTouch
{
     public class TouchTracker
	 {
        int fingerID; 
        TouchInput mTouch;
        Vector2 StartPoint, PreviousPoint, currentPoint;
        bool Dirty;
        float totalTime;
        
        public bool isDirty { get { return Dirty; } }
        public int FingerId { get { return fingerID; } }
        public Vector2 PositionDelta { get { return  (currentPoint - PreviousPoint); } }
        public Vector2 PositionDeltaFromStart { get { return (currentPoint - StartPoint); } }
        public Vector2 Position { get { return currentPoint;} }
        public Vector2 StartPosition { get { return StartPoint; } }
        public float TimeFromStart { get { return totalTime; } }
        public TouchInput Touch { get { return mTouch; } set { mTouch = value; } }
        public bool Alive = true;
        
        int _tapCount = 0;
        public int TapCount { get { return _tapCount; } set { _tapCount = value; } }

        public bool Disabled = false;

        public TouchTracker()
        {
            Alive = false;
            StartPoint = Vector2.zero;
            PreviousPoint = Vector2.zero;
            currentPoint = Vector2.zero;
            totalTime = 0.0f;
        }
        public void SetTouch(TouchInput touch)
        {
            Alive = true;
            mTouch = touch;
            fingerID = touch.fingerId;

            StartPoint = touch.position;
            PreviousPoint = StartPoint;
            currentPoint = StartPoint;
            totalTime = 0.0f;

            Dirty = true;
        }
        public void ClearTouch()
        {
            Alive = false;
            StartPoint = Vector2.zero;
            PreviousPoint = Vector2.zero;
            currentPoint = Vector2.zero;
            totalTime = 0.0f;
			TouchPool.Clear ();
        }

        public TouchTracker(TouchInput touch)
        {
            SetTouch(touch);
        }
        public void Clean()
        {
            Dirty = false;
        }

        public List<TouchInput> TouchPool = new List<TouchInput>();

        public void Update(TouchInput touch)
        {
            totalTime += Time.deltaTime;
            touch.timestamp = totalTime;
            mTouch = touch;

            TouchPool.Add(touch);
            if(TouchPool.Count > 10)
                TouchPool.Remove(TouchPool[0]);

            PreviousPoint = currentPoint;
            currentPoint = touch.position;
            Dirty = true;
        }
        public void End()
        {
            Alive = false;
            ClearTouch();
        }
	}
}
