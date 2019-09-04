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

namespace NRKernal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class NREmulatorManager : MonoBehaviour
    {
        public static NREmulatorManager Instance { get; set; }

        public NativeEmulator NativeEmulatorApi { get; set; }

        public static int SIMPlaneID = 0;

        private void Start()
        {
#if UNITY_EDITOR
            DontDestroyOnLoad(this);
            Instance = this;
            NativeEmulatorApi = new NativeEmulator();
            CreateSimulator();
#endif
        }


        private void OnDestroy()
        {
#if UNITY_EDITOR
            NativeEmulatorApi.DestorySIMController();
#endif
        }

        public void CreateSimulator()
        {
            NativeEmulatorApi.CreateSIMTracking();
            NativeEmulatorApi.CreateSIMController();
        }

        public bool IsInGameView(Vector3 worldPos)
        {
            Camera cam = GameObject.Find("LeftCamera").GetComponent<Camera>();
            Transform camTransform = cam.transform;
            Vector2 viewPos = cam.WorldToViewportPoint(worldPos);
            Vector3 dir = (worldPos - camTransform.position).normalized;
            float dot = Vector3.Dot(camTransform.forward, dir);

            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

