using UnityEngine;
using System.Collections;

#if UNITY_WINRT || UNITY_WINRT_8_0 || UNITY_WINRT_8_1 || UNITY_WINRT_10_0
public class ThreadedGameLogicObject : MonoBehaviour
{
}
#else
using System.Threading;

public class ThreadedGameLogicObject : MonoBehaviour
{
    private Thread thread;
    private Transform thisTransform;
    private Vector3 currentPos;

    private void Start()
    {
        thisTransform = this.transform;
        thread = new Thread(ThreadUpdate);  
        thread.Start();
    }
    private void Update()
    {
        thisTransform.position = currentPos;
    }
    //I used FixedUpdate function to modulate the speed at which the threads execute
    //there is potentially like a dozen different ways to do this, and this one might not be ideal
    //my game used no physics, so I could change the fixed timestep to whatever I needed to make this work
    private void FixedUpdate()
    {
        updateThread = true;
    }
    private bool runThread = true;
    private bool updateThread;
    private void ThreadUpdate()
    {
        while (runThread)
        {
            if (updateThread)
            {
                updateThread = false;
                currentPos += Vector3.up * Time.deltaTime; //some crazy ass function that takes forever to do here
            }
        }   
    }
    private void OnApplicationQuit()
    {
        EndThreads();   
    }
    //This must be called from OnApplicationQuit AND before the loading of a new level.
    //Threads spawned from this class must be ended when this class is destroyed in level changes.
    public void EndThreads()
    {
        runThread = false;
        //you could use thread.abort() but that has issues on iOS
        while (thread.IsAlive)
        {
            //simply have main loop wait till thread ends
        }
    }
}
#endif