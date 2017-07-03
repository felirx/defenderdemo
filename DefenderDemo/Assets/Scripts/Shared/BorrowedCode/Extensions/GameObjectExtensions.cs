/*
 By: Josh Sutphin
 http://www.third-helix.com/2013/09/adding-to-unitys-built-in-classes-using-extension-methods/
 http://gamasutra.com/blogs/JoshSutphin/20131007/201829/Adding_to_Unitys_BuiltIn_Classes_Using_Extension_Methods.php
 */
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

#if NETFX_CORE
using SharpSerializer;
#endif

// fun extensions
public static class GameObjectExtensions
{
    // Set the layer of this GameObject and all of its children.
    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform t in gameObject.transform)
            t.gameObject.SetLayerRecursively(layer);
    }

    public static void SetCollisionRecursively(this GameObject gameObject, bool tf)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
            collider.enabled = tf;
    }

    public static void SetVisualRecursively(this GameObject gameObject, bool tf)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.enabled = tf;
    }

    public static T[] GetComponentsInChildrenWithTag<T>(this GameObject gameObject, string tag)
        where T : Component
    {
        List<T> results = new List<T>();

        if (gameObject.CompareTag(tag))
            results.Add(gameObject.GetComponent<T>());

        foreach (Transform t in gameObject.transform)
            results.AddRange(t.gameObject.GetComponentsInChildrenWithTag<T>(tag));

        return results.ToArray();
    }

    public static T GetComponentInParents<T>(this GameObject gameObject)
        where T : Component
    {
        for (Transform t = gameObject.transform; t != null; t = t.parent)
        {
            T result = t.GetComponent<T>();
            if (result != null)
                return result;
        }
        return null;
    }

    public static T[] GetComponentsInParents<T>(this GameObject gameObject)
        where T : Component
    {
        List<T> results = new List<T>();
        for (Transform t = gameObject.transform; t != null; t = t.parent)
        {
            T result = t.GetComponent<T>();
            if (result != null)
                results.Add(result);
        }

        return results.ToArray();
    }

    public static int GetCollisionMask(this GameObject gameObject, int layer = -1)
    {
        if (layer == -1)
            layer = gameObject.layer;

        int mask = 0;
        for (int i = 0; i < 32; i++)
            mask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1) << i;

        return mask;
    }

    public static GameObject FindClosestGameObject(this GameObject gameObject, GameObject[] objects)
    {
        float closestDistance = -1.0f; 
       // int targetNumber;
        GameObject closestGO = gameObject;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                if (closestDistance < 0)
                {
                    closestDistance = (objects[i].transform.position - gameObject.transform.position).sqrMagnitude;
                    closestGO = objects[i];
                }
                else
                {
                    float compr = (objects[i].transform.position - gameObject.transform.position).sqrMagnitude;
                    if (compr < closestDistance)
                    {
                        //targetNumber = i;
                        closestGO = objects[i];
                    }
                }
            }
        }

        if (closestDistance >= 0)
            return closestGO;
        else
            return null;
    }
    public static GameObject FindClosestGameObject(this GameObject gameObject, List<GameObject> objects)
    {
        float closestDistance = -1.0f;
       // int targetNumber;
        GameObject closestGO = gameObject;

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != null)
            {
                if (closestDistance < 0)
                {
                    closestDistance = (objects[i].transform.position - gameObject.transform.position).sqrMagnitude;
                    closestGO = objects[i];
                }
                else
                {
                    float compr = (objects[i].transform.position - gameObject.transform.position).sqrMagnitude;
                    if (compr < closestDistance)
                    {
                      //  targetNumber = i;
                        closestGO = objects[i];
                    }
                }
            }
        }

        if (closestDistance >= 0)
            return closestGO;
        else
            return null;


    }
	/// <summary>
	/// Distances to game object. Usage example:
	/// GameObject go = GameObject.Find("Cube") as GameObject;
	/// float distancetoCube = gameobject.DistanceToGameObject(go);
	/// </summary>
	/// <returns>distance to the to game object.</returns>
	/// <param name="gameObject">Game object. or -1.0f if gameobject is null</param>
	/// <param name="other">Other.</param>
    public static float DistanceToGameObject(this GameObject gameObject, GameObject other)
    {
		if(other == null)
		{
			Debug.LogWarning("Unable to measure distance, other Gameobject is null ("+gameObject.name+")");
			return -1.0f;
		}
		
        return (gameObject.transform.position - other.transform.position).sqrMagnitude;
    }
	/// <summary>
	/// Gets or add a component. Usage example:
	/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
	/// </summary>
	public static  T GetOrAddComponent<T> (this Component child) where T: Component {
		T result = child.GetComponent<T>();
		if (result == null) {
			result = child.gameObject.AddComponent<T>();
		}
		return result;
	}
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);

        for (int i = 0; i < pinfos.Length; i++)
        {
            if (pinfos[i].CanWrite)
            {
                try
                {
                    pinfos[i].SetValue(comp, pinfos[i].GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);

        for (int i = 0; i < finfos.Length; i++ )
        {
            finfos[i].SetValue(comp, finfos[i].GetValue(other));
        }

        return comp as T;
    }
    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
    /// <summary>
    /// Finds the given meta position from scene.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="positionName"></param>
    /// <returns></returns>
    public static Vector3 FindMetaPosition(this GameObject gameObject, string positionName)
    {
        GameObject ob = GameObject.FindGameObjectWithTag("MetaData");
        if (ob == null)
        {
            Debug.LogError("No MetaData Gameobject in scene");

            ob = GameObject.Find(positionName);

            if (ob == null)
                return Vector3.zero;
            else
            {
                Debug.Log("Found object named outside of Metadata" + positionName);
                return ob.transform.position;
            }
            
        }

        Transform t = ob.transform.Find(positionName);

        if (t == null)
        {
            Debug.LogError("No position found from: " + positionName);
        }
            
        return t.position;
    }

    public static T FindInParents<T>(this GameObject go) where T : Component
    {
        if (go == null) return null;
        T comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}

