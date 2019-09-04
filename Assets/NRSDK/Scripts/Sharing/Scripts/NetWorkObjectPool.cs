/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* NRSDK is distributed in the hope that it will be usefull                                                              
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRToolkit.Sharing
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "NetWorkObjectPool", menuName = "NRInternal/NetWorkObjectPool", order = 1)]
    public class NetWorkObjectPool : ScriptableObject
    {
        public List<NetworkBehaviour> NetObjects = new List<NetworkBehaviour>();
        private Dictionary<string, NetworkBehaviour> m_NetObjectDict = new Dictionary<string, NetworkBehaviour>();

        public void Init()
        {
            if (NetObjects != null)
            {
                foreach (var item in NetObjects)
                {
                    string key = item.GetType().Name.ToString();
                    if (m_NetObjectDict.ContainsKey(key))
                    {
                        Debug.Log("Already has the key:" + key);
                        continue;
                    }
                    m_NetObjectDict.Add(key, item);
                }
            }
        }

        public bool TryGetNetObject(string key, out NetworkBehaviour netobject)
        {
            return m_NetObjectDict.TryGetValue(key, out netobject);
        }

        public bool ContainsKey(string name)
        {
            return m_NetObjectDict.ContainsKey(name);
        }
    }
}
