/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          

* NRSDK is distributed in the hope that it will be usefull                                                              

* https://www.nreal.ai/         
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Collections.Generic;
    using System.IO;
    using System;
    using UnityEngine;

    /**
     * @brief Manages AR mapping system.
     * 
     * Through this class, application can save、load a map. 
     */
    internal class NRWorldMap : SingleTon<NRWorldMap>
    {
        public NativeMapping NativeMapping { get; set; }

        private Dictionary<UInt64, NRWorldAnchor> m_AnchorDict = new Dictionary<ulong, NRWorldAnchor>();

        private List<NRWorldAnchor> AnchorAddList = new List<NRWorldAnchor>();

        public string MapOutPath
        {
            get
            {
                string path = Path.Combine(NRTools.GetSdcardPath(), "NrealMaps");
                return path;
            }
        }

        public string MapFilePath
        {
            get
            {
                return Path.Combine(MapOutPath, "localmap.dat");
            }
        }

        public NRWorldMap()
        {
            NativeMapping = new NativeMapping(NRSessionManager.Instance.NativeAPI);
        }

        public void Create()
        {
            var result = NativeMapping.CreateDataBase();
        }

        public void GetAnchors(List<NRWorldAnchor> anchorlist)
        {
            if (anchorlist == null)
            {
                Debug.LogError("Anchor list can not be null!");
                return;
            }
            anchorlist.Clear();

            var listhandle = NativeMapping.CreateAnchorList();
            var size = NativeMapping.GetAnchorListSize(listhandle);
            for (int i = 0; i < size; i++)
            {
                var anchorhandle = NativeMapping.AcquireItem(listhandle, i);
                NRWorldAnchor anchor = null;
                m_AnchorDict.TryGetValue(anchorhandle, out anchor);
                if (anchor == null)
                {
                    anchor = CreateAnchor(anchorhandle);
                    m_AnchorDict.Add(anchorhandle, anchor);
                }
                anchorlist.Add(anchor);
            }
            anchorlist.AddRange(AnchorAddList);
            NativeMapping.DestroyAnchorList(listhandle);
        }

        public void AddAnchor(Pose worldPose)
        {
            var handle = NativeMapping.AddAnchor(worldPose);
            if (handle == 0)
            {
                Debug.LogError("Add anchor failed anchor handle:" + handle);
                return;
            }
            NRWorldAnchor anchor = null;
            m_AnchorDict.TryGetValue(handle, out anchor);
            if (anchor == null)
            {
                anchor = CreateAnchor(handle);
                m_AnchorDict.Add(handle, anchor);
            }
            AnchorAddList.Add(anchor);
        }

        private NRWorldAnchor CreateAnchor(UInt64 handle)
        {
            NRWorldAnchor anchor = new NRWorldAnchor(handle, NativeMapping);
            return anchor;
        }

        public void WriteWorldMap()
        {
            if (!Directory.Exists(MapOutPath))
            {
                Directory.CreateDirectory(MapOutPath);
            }
            NativeMapping.SaveMap(MapFilePath);
        }

        public void LoadWorldMap()
        {
            NativeMapping.LoadMap(MapFilePath);
        }

        public void Destroy()
        {
            NativeMapping.DestroyDataBase();
        }
    }
}
