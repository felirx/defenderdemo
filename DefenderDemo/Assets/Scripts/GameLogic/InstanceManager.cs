using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceManager
{
    protected List<Actor> _Actors = new List<Actor>();
    protected List<GameObject> _Terrains = new List<GameObject>();

    public List<Actor> Enemies { get { return _Actors; } }
    public List<GameObject> Terrains { get { return _Terrains; } }

    public void RegisterActor(Actor go)
    {
        if (_Actors.Contains(go))
        {
            Debug.LogError("Already registered " + go);
            return;
        }

        _Actors.Add(go);
    }

    public void RemoveActor(Actor go)
    {
        if (!_Actors.Contains(go))
        {
            Debug.LogError(go + " Not registered");
            return;
        }
        _Actors.Remove(go);
    }

    public void DestroyAllActors()
    {
        for (int i = 0; i < _Actors.Count; ++i)
        {
            if (_Actors[i])
            {
                GameObject.Destroy(_Actors[i].gameObject);
            }
        }
        _Actors.Clear();
    }

    public void RegisterTerrain(GameObject go)
    {
        AddGameObject(go, _Terrains);
    }

    public void RemoveTerrain(GameObject go)
    {
        RemoveGameObject(go, _Terrains);
    }


    protected void AddGameObject(GameObject go, List<GameObject> l)
    {
        if (l.Contains(go))
        {
            Debug.LogError("Already registered " + go);
            return;
        }

        l.Add(go);
    }

    protected void RemoveGameObject(GameObject go, List<GameObject> l)
    {
        if (!l.Contains(go))
        {
            Debug.LogError(go + " Not registered");
            return;
        }
        l.Remove(go);
    }
}
