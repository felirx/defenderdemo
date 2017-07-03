using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RamminSpeed.Data
{ 
    /// <summary>
    /// Generic ToolboxClass with dictonary that contains miscelanous non-monobehavior tools
    /// Used to avoid problems related to singletons and static classes.
    /// </summary>
    public class ToolBox 
    {
        protected static ToolBox _instance;
        protected Dictionary<int, object> Tools;
    
        public static ToolBox Instance 
        { 
            get 
            {
                if (_instance == null)
                    _instance = new ToolBox();

                return _instance; 
            } 
        }
        public static bool ContainsTool(int key)
        {
            if (Instance.Tools.ContainsKey(key))
            {
                return true;
            }
            else
                return false;
        }

        private ToolBox()
        {
            Tools = new Dictionary<int, object>();
        }

        public static void RegisterTool(int key, object tool)
        {
            if (Instance.Tools.ContainsKey(key))
            {
                Debug.LogWarning("Key already found, overwriting");
                Instance.Tools[key] = tool;
            }
            else
            {
                Instance.Tools.Add(key, tool);
            }        
        }
        public static void DeRegisterTool(int key)
        {
            if (Instance != null)
            {
                Instance.Tools.Remove(key);
            }
        }
        public static void ClearTools()
        {
            if (Instance != null)
            {
                Instance.Tools.Clear();
            }
        }
    
        public static object GetTool(int key)
        {
            if (Instance.Tools.ContainsKey(key))
                return Instance.Tools[key];
            else { return null; }
        }
    }
}