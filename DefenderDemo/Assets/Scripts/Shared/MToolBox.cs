using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RamminSpeed.Data;

/// <summary>
/// Project specific ToolAccessor Class for Project PAW
/// </summary>
public static class MToolBox
{
    #region ToolBoxMethods
    static void RegisterTool(SlimeTool key, object tool)
    {
        if (!ToolBox.ContainsTool((int)key))
        {
            //Logger.Log(Logger.Channel.Pasi, "registered: " + key.ToString()); 
            ToolBox.RegisterTool((int)key, tool);
        }
    }
    static T GetTool<T>(SlimeTool key)
    {
        return (T)ToolBox.GetTool((int)key);
    }

    public static void ClearTools()
    {
        GameObject.Destroy(GM.gameObject);
        ToolBox.ClearTools();
    }

    //Getters for convenience
    #endregion

    #region ToolAccessors


    //Persistent game manager
    public static GameManager GM
    {
        get
        {
            if (!ToolBox.ContainsTool((int)SlimeTool.GameManager))
            {
                GameManager gm = GameObject.FindObjectOfType<GameManager>();
                if(gm != null)
                {
                    RegisterTool(SlimeTool.GameManager, gm);
                }
                else
                {
                    GameObject ob = new GameObject("GameManager");
                    gm = ob.AddComponent<GameManager>();
                    RegisterTool(SlimeTool.GameManager, gm);
                }
            }
            return GetTool<GameManager>(SlimeTool.GameManager);
        }
    }

    // Persistent music player
    //public static MusicPlayer Music
    //{
    //    get
    //    {
    //        if (!ToolBox.ContainsTool((int)SlimeTool.MusicPlayer))
    //        {
    //            MusicPlayer mp = GameObject.FindObjectOfType<MusicPlayer>();
    //            if (mp == null)
    //            {
    //                GameObject ob = new GameObject("MusicPlayer");
    //                mp = ob.AddComponent<MusicPlayer>();
    //                mp.Init();
    //                RegisterTool(SlimeTool.MusicPlayer, mp);
    //                GameObject.DontDestroyOnLoad(ob);
    //            }
    //        }
    //        return GetTool<MusicPlayer>(SlimeTool.MusicPlayer);
    //    }
    //}

    public static InstanceManager IM
    {
        get
        {
            if (!ToolBox.ContainsTool((int)SlimeTool.InstanceManager))
            {
                RegisterTool(SlimeTool.InstanceManager, new InstanceManager());
            }
            return GetTool<InstanceManager>(SlimeTool.InstanceManager);
        }
    }

    //To add new Tool
    //1. Add new key to the PawTool enum above
    //2. Create accessor for the tool that creates it if it doesn not exists already.


    #endregion

    enum SlimeTool 
    {
        GameManager, InstanceManager, MusicPlayer
    }
}
