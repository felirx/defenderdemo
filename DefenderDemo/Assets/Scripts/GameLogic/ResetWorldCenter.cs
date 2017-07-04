using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResetWorldCenter
{
    public static void ResetWorldToOrigin(Vector3 resetDelta)
    {
        List<Actor> actors = MToolBox.IM.Enemies;
        List<GameObject> terrains = MToolBox.IM.Terrains;

        for (int i = 0; i < actors.Count; ++i)
        {
            if (actors[i])
                actors[i].gameObject.transform.position -= resetDelta;
        }
        for (int i = 0; i < terrains.Count; ++i)
        {
            if (terrains[i])
                terrains[i].gameObject.transform.position -= resetDelta;
        }
    }
}
