using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResetWorldCenter
{
    public static void ResetWorldToOrigin(Vector3 resetDelta)
    {
        List<GameObject> enemies = MToolBox.IM.Enemies;
        List<GameObject> players = MToolBox.IM.Players;
        List<GameObject> civilians = MToolBox.IM.Civilians;
        List<GameObject> terrains = MToolBox.IM.Terrains;
        List<GameObject> projectiles = MToolBox.IM.Projectiles;

        for (int i = 0; i < enemies.Count; ++i)
            enemies[i].gameObject.transform.position -= resetDelta;
        for (int i = 0; i < players.Count; ++i)
            players[i].gameObject.transform.position -= resetDelta;
        for (int i = 0; i < civilians.Count; ++i)
            civilians[i].gameObject.transform.position -= resetDelta;
        for (int i = 0; i < terrains.Count; ++i)
            terrains[i].gameObject.transform.position -= resetDelta;
        for (int i = 0; i < projectiles.Count; ++i)
            projectiles[i].gameObject.transform.position -= resetDelta;
    }
}
