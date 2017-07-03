using UnityEngine;

namespace PMobile.MultiTouch
{
    /// <summary>
    /// Struct Made to fix on the shortcomings of UnityEngine.touch
    /// </summary>
	public struct TouchInput
	{
        public int fingerId;
        public int tapCount;
        public Vector2 position;
        public bool Active;
        public float timestamp;
        public TouchPhase phase;
	}
}
