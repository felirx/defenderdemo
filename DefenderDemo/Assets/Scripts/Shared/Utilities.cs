using UnityEngine;
using System.Collections;

namespace PGeneric.Utilities
{
    public class Timer
    {
        /// <summary>
        /// Total time (the whole length) of this timer, in seconds
        /// </summary>
        public float TotalTime
        {
            get
            {
                return _TotalTime;
            }
        }

        /// <summary>
        /// Elapsed time on this timer, in seconds
        /// </summary>
        public float ElapsedTime
        {
            get
            {
                return _TotalTime - _CurrentTime;
            }
        }

        /// <summary>
        /// Remaining time on this timer, in seconds
        /// </summary>
        public float RemainingTime
        {
            get
            {
                return _CurrentTime;
            }
        }

        private float _TotalTime = 0.0f; // time of a "full" timer
        private float _CurrentTime = 0.0f; // current timer value, aka. Remaining time
        private bool _started = false;

        public void Start(float timeMS)
        {
            _TotalTime = timeMS / 1000f;
            _CurrentTime = _TotalTime;
            _started = true;
        }

        public void Restart()
        {
            _CurrentTime = _TotalTime;
        }

        public void Reset()
        {
            _TotalTime = 0.0f;
            _CurrentTime = 0.0f;
            _started = false;
        }

        public bool Started()
        {
            return _started;
        }

        /// <summary>
        /// Update the timer by deltaTime. Returns true if Timer has run out, otherwise false.
        /// </summary>
        /// <param name="delta">Delta time in seconds</param>
        /// <returns>True if Timer has run out, otherwise false</returns>
        public bool Tick(float delta)
        {
            if (!_started)
                return false;
            //Debug.Log(_CurrentTime);
            // don't overflow the time variable
            if (Ticked())
            {
                //Debug.Log("Ticked begin");
                return true;
            }

            _CurrentTime -= delta;
            return Ticked();
        }

        public bool Ticked()
        {
            if (0 >= _CurrentTime && _started)
            {
                //Debug.Log("Ticked");
                return true;
            }
            //Debug.Log("Not Ticked");
            return false;
        }
    }

    public static class HardwareUtilities
    {
        static bool isJunk = false;
        static bool isJunkCached = false;
        static bool isRetina = false;
        static bool isRetinaCached = false;

        static public bool LoadRetinaAssets()
        {
            if (Application.isEditor)
            {
                if (!Application.isPlaying)
                    return true; // Tweak for Buildtools
            }

            if (isRetinaCached)
                return isRetina;
            bool retVal = false;
#if UNITY_IPHONE
            if (IsDeviceOldPieceOfJunk())
                retVal = false;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone4S == UnityEngine.iOS.Device.generation)
                retVal = false;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone5 == UnityEngine.iOS.Device.generation)
                retVal = false;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone5S == UnityEngine.iOS.Device.generation)
                retVal = false;
            else if (UnityEngine.iOS.DeviceGeneration.iPad2Gen == UnityEngine.iOS.Device.generation)
                retVal = false;
            else if (UnityEngine.iOS.DeviceGeneration.iPadMini1Gen == UnityEngine.iOS.Device.generation)
                retVal = false;
            else
                retVal = true;
#endif
//#if UNITY_EDITOR
//            retVal = true;
//#endif
            isRetinaCached = true;
            isRetina = retVal;
            return isRetina;
        }
        static public bool IsDeviceOldPieceOfJunk()
        {
            if (isJunkCached)
                return isJunk;

            bool retVal = false;
#if UNITY_IPHONE
            if (UnityEngine.iOS.DeviceGeneration.iPad1Gen == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone3G == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone3GS == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPhone4 == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen == UnityEngine.iOS.Device.generation)
                retVal =  true;
            else if (UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen == UnityEngine.iOS.Device.generation)
                retVal =  true;
#endif
#if UNITY_ANDROID
            // android is harder...

            // if we are not at least android 4.0, it's an old device
            string androidVersion = SystemInfo.operatingSystem;
            int sdkPos = androidVersion.IndexOf("API-");

            int iVersionNumber = -1;
            try
            {
                iVersionNumber = int.Parse(androidVersion.Substring(sdkPos + 4, 2).ToString());
                if (iVersionNumber < 4)
                    retVal = true;
            }
            catch (System.FormatException fe)
            {
#if UNITY_EDITOR
                Debug.Log(fe.Message);
                isJunk = false;
                isJunkCached = true;
                return isJunk;
#else
                // something went wrong, err to junk
                Debug.Log(androidVersion);
                Debug.Log(fe.Message);

                retVal = true;
#endif
            }

            // if we only have one core, we are probably a piece of junk
            if (SystemInfo.processorCount < 2)
                retVal = true;

            // if screen is smaller than 720p lets say we are a piece of junk
            if (Display.main.renderingHeight < 720)
                retVal = true;

            // if we have less than half a gig of ram, it's probably a piece of junk
            if (SystemInfo.systemMemorySize < 511)
                retVal = true;
#endif
            isJunkCached = true;
            isJunk = retVal;
            return isJunk;
        }
    }

    public static class FindUtilities
    {
        static public void SearchAllThatContains(Transform current, string match, System.Collections.Generic.List<Transform> output)
        {
            if (null == output)
                output = new System.Collections.Generic.List<Transform>();

            if (current.name.Contains(match))
            {
                output.Add(current);
            }

            for (int i = 0; i < current.childCount; ++i)
            {
                SearchAllThatContains(current.GetChild(i), match, output);
            }
        }
        static public Transform SearchHierarchyForTransform(Transform current, string name)
        {
            // check if the current bone is the bone we're looking for, if so return it
            if (current.name == name)
                return current;

            // search through child bones for the bone we're looking for
            for (int i = 0; i < current.childCount; ++i)
            {
                // the recursive step; repeat the search one step deeper in the hierarchy
                Transform found = SearchHierarchyForTransform(current.GetChild(i), name);

                // a transform was returned by the search above that is not null,
                // it must be the bone we're looking for
                if (found != null)
                    return found;
            }

            // bone with name was not found
            return null;
        }

        static public T SearchHierarchyForComponent<T>(Transform current, string name) where T : Component
        {
            Transform t = SearchHierarchyForTransform(current, name);
            if (t)
            {
                return t.GetComponent<T>();
            }
            return null;
        }

    }

    public static class PhysicsUtilities
    {
        public class PhysicsFunctor
        {
            public void Execute(Rigidbody[] bodies)
            {
                foreach (Rigidbody rb in bodies)
                    Execute(rb);
            }

            // internal implementation for one rigid body
            public virtual void Execute(Rigidbody rb)
            {
                if (rb.isKinematic)
                    return;

            }
        }

        public class SetKinematicFunctor : PhysicsFunctor
        {
            public bool _flag = false;

            public void SetData(bool flag)
            {
                _flag = flag;
            }

            public override void Execute(Rigidbody rb)
            {
                rb.isKinematic = _flag;
            }
        }

        public class SetVelocityFunctor : PhysicsFunctor
        {
            public Vector3 _velocity = Vector3.zero;

            public void SetData(Vector3 vel)
            {
                _velocity = vel;
            }

            public override void Execute(Rigidbody rb)
            {
                if (rb.isKinematic)
                    return;

                rb.velocity = _velocity;
            }
        }

        public class AddExplosionForceFunctor : PhysicsFunctor
        {
            public float _force = 0.0f;
            public Vector3 _pos = Vector3.zero;
            public float _radius = 1.0f;
            public float _upMod = 1.0f;
            public ForceMode _forceMode = ForceMode.Impulse;

            public void SetData(float force, Vector3 pos, float radius, float upmod, ForceMode forceMode)
            {
                _force = force;
                _pos = pos;
                _radius = radius;
                _upMod = upmod;
                _forceMode = forceMode;
            }

            public override void Execute(Rigidbody rb)
            {
                if (rb.isKinematic)
                    return;

                rb.AddExplosionForce(_force, _pos, _radius, _upMod, _forceMode);
            }
        }

        public class AddMassDependentExplosionForceFunctor : PhysicsFunctor
        {
            public float _force = 0.0f;
            public Vector3 _pos = Vector3.zero;
            public float _radius = 1.0f;
            public float _upMod = 1.0f;
            public ForceMode _forceMode = ForceMode.Impulse;

            public void SetData(float force, Vector3 pos, float radius, float upmod, ForceMode forceMode)
            {
                _force = force;
                _pos = pos;
                _radius = radius;
                _upMod = upmod;
                _forceMode = forceMode;
            }

            public override void Execute(Rigidbody rb)
            {
                if (rb.isKinematic)
                    return;

                rb.AddExplosionForce(0.5f * _force * rb.mass * rb.mass, _pos, _radius, _upMod, _forceMode);
            }
        }

        public class ResetForcesFunctor : PhysicsFunctor
        {
            public override void Execute(Rigidbody rb)
            {
                if (rb.isKinematic)
                    return;

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        public class LimitVelocityFunctor : PhysicsFunctor
        {
            public float _MaxVelocity = 10.0f;

            public void SetData(float maxVelocity)
            {
                _MaxVelocity = maxVelocity;
            }

            public override void Execute(Rigidbody rb)
            {
                if (rb.isKinematic)
                    return;

                if (rb.velocity.magnitude > _MaxVelocity)
                    rb.velocity = rb.velocity.normalized * _MaxVelocity;
                //Debug.DrawRay(rb.transform.position, rb.velocity);
                //rb.velocity = Vector3.ClampMagnitude(rb.velocity, _MaxVelocity);
            }
        }

        static public SetKinematicFunctor _staticKinematicFunctor = new SetKinematicFunctor();
        static public SetVelocityFunctor _staticVelocityFunctor = new SetVelocityFunctor();
        static public AddExplosionForceFunctor _staticAddExplosionForceFunctor = new AddExplosionForceFunctor();
        static public AddMassDependentExplosionForceFunctor _staticAddMassDependentExplosionForceFunctor = new AddMassDependentExplosionForceFunctor();
        static public ResetForcesFunctor _staticResetForcesFunctor = new ResetForcesFunctor();
        static public LimitVelocityFunctor _staticLimitVelocityFunctor = new LimitVelocityFunctor();

        static public void ExecutePhysicsFunctor(GameObject parent, PhysicsFunctor functor)
        {
            if (null == parent || null == functor)
                return;

            functor.Execute(parent.GetComponentsInChildren<Rigidbody>());
        }

        // fishes out the rigid bodies, does nothing if the rigidbody is kinematic
        static public void ExecutePhysicsFunctor(Collider[] colliders, PhysicsFunctor functor)
        {
            foreach (Collider cs in colliders)
            {
                Rigidbody rb = cs.GetComponent<Rigidbody>();
                if (rb)
                    functor.Execute(rb);
            }
        }

        static public void ToggleColliders(GameObject target, bool value, int layermask = ~0)
        {
            if (target.GetComponent<Collider>() != null)
            {
                if (((1 << target.layer) & layermask) != 0)
                    target.GetComponent<Collider>().enabled = value;
            }
                
            Collider[] c = target.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < c.Length; i++)
            {
                if (((1 << c[i].gameObject.layer) & layermask) != 0)
                    c[i].enabled = value;
            }
        }

        static public Vector3 CalculateBallisticInitialVelocity(float heightDif, Vector3 start, Vector3 end, float startAngle)
        {
            Vector3 dir = end - start;
            float g = -Physics.gravity.y;
            float dist = MathUtilities.DistanceXZ(end, start);
            float onepercos = 1 / Mathf.Cos(startAngle);
            float undersqr = (g * dist * dist / 2) / (dist * Mathf.Tan(startAngle) + heightDif);
            float v = onepercos * Mathf.Sqrt(undersqr);

            Vector3 dur = dir;
            dur.y = 0.0f;
            dur.Normalize();
            Vector3 rotaxis = Vector3.Cross(dur, Vector3.up);
            dur = Quaternion.AngleAxis(startAngle * Mathf.Rad2Deg, rotaxis) * dur;

            return dur * v;
        }
    }

    public static class MathUtilities
    {
        public static Quaternion RandomQuaternion()
        {
            float u1 = Random.Range(0.0f, 1.0f);
            float u2 = Random.Range(0.0f, 1.0f);
            float u3 = Random.Range(0.0f, 1.0f);

            float u1sqrt = Mathf.Sqrt(u1);
            float u1m1sqrt = Mathf.Sqrt(1 - u1);
            float x = u1m1sqrt * Mathf.Sin(2 * Mathf.PI * u2);
            float y = u1m1sqrt * Mathf.Cos(2 * Mathf.PI * u2);
            float z = u1sqrt * Mathf.Sin(2 * Mathf.PI * u3);
            float w = u1sqrt * Mathf.Cos(2 * Mathf.PI * u3);

            return new Quaternion(x, y, w, z);
        }

        public static Vector3 Lerp(Vector3 from, Vector3 to, Vector3 t)
        {
            return new Vector3(
                Mathf.Lerp(from.x, to.x, t.x),
                Mathf.Lerp(from.y, to.y, t.y),
                Mathf.Lerp(from.z, to.z, t.z));
        }

        public static Vector2 ConstantValueInterpolation(Vector2 from, Vector2 to, float value)
        {
            // are we equal enough?
            if (from == to)
                return to;

            Vector2 o = Vector2.zero;
            // components
            for (int i = 0; i < 2; ++i)
            {
                o[i] = ConstantValueInterpolation(from[i], to[i], value);
            }

            return o;
        }

        public static Vector3 ConstantValueInterpolation(Vector3 from, Vector3 to, float value)
        {
            // are we equal enough?
            if (from == to)
                return to;

            Vector3 o = Vector3.zero;
            // components
            for (int i = 0; i < 3; ++i)
            {
                o[i] = ConstantValueInterpolation(from[i], to[i], value);
            }

            return o;
        }

        public static Color ConstantValueInterpolation(Color from, Color to, float value)
        {
            // are we equal enough?
            if (from == to)
                return to;

            Color o = Color.black;
            // components
            for (int i = 0; i < 4; ++i)
            {
                o[i] = ConstantValueInterpolation(from[i], to[i], value);
            }

            return o;
        }


        public static float ConstantValueInterpolation(float from, float to, float value)
        {
            // needs proper float comparison?
            if (from == to)
                return to;

            float o = from;
            if (from >= to)
            {
                // we are decreasing
                o = from - value;
                if (o <= to)
                    o = to;
            }
            else
            {
                // we are increasing
                o = from + value;
                if (o >= to)
                    o = to;
            }

            return o;
        }

        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            a.y = 0.0f;
            b.y = 0.0f;
            return Vector3.Distance(a, b);
        }

        public static float DistanceXY(Vector3 a, Vector3 b)
        {
            a.z = 0.0f;
            b.z = 0.0f;
            return Vector3.Distance(a, b);
        }

        public static float DistanceYZ(Vector3 a, Vector3 b)
        {
            a.x = 0.0f;
            b.x = 0.0f;
            return Vector3.Distance(a, b);
        }
    }

    public static class ColorUtilities
    {
        public static string  ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}