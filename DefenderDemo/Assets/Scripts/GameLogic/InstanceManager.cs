using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceManager
{
    protected List<GameObject> _Enemies = new List<GameObject>();
    protected List<GameObject> _Players = new List<GameObject>();
    protected List<GameObject> _Civilians = new List<GameObject>();
    protected List<GameObject> _Terrains = new List<GameObject>();
    protected List<GameObject> _Projectiles = new List<GameObject>();

    public List<GameObject> Enemies { get { return _Enemies; } }
    public List<GameObject> Players { get { return _Players; } }
    public List<GameObject> Civilians { get { return _Civilians; } }
    public List<GameObject> Terrains { get { return _Terrains; } }
    public List<GameObject> Projectiles { get { return _Projectiles; } }

    public void Init()
    {
        Debug.Log("Initializing Instance Manager");
    }

    public void DropAllReferences()
    {
        // ???
        _Enemies.Clear();
        _Players.Clear();
        _Civilians.Clear();
        _Terrains.Clear();
    }

    public void RegisterEnemy(GameObject go)
    {
        AddGameObject(go, _Enemies);
    }

    public void RemoveEnemy(GameObject go)
    {
        RemoveGameObject(go, _Enemies);
    }

    public void RegisterPlayer(GameObject go)
    {
        AddGameObject(go, _Players);
    }

    public void RemovePlayer(GameObject go)
    {
        RemoveGameObject(go, _Players);
    }

    public void RegisterCivilian(GameObject go)
    {
        AddGameObject(go, _Civilians);
    }

    public void RemoveCivilian(GameObject go)
    {
        RemoveGameObject(go, _Civilians);
    }

    public void RegisterTerrain(GameObject go)
    {
        AddGameObject(go, _Terrains);
    }

    public void RemoveTerrain(GameObject go)
    {
        RemoveGameObject(go, _Terrains);
    }

    public void RegisterProjectile(GameObject go)
    {
        AddGameObject(go, _Projectiles);
    }

    public void RemoveProjectile(GameObject go)
    {
        RemoveGameObject(go, _Projectiles);
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
