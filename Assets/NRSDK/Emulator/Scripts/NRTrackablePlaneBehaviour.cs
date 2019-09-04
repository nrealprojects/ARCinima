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
    using UnityEngine;
    public class NRTrackablePlaneBehaviour : NRTrackableBehaviour
    {
        private void Start()
        {
#if UNITY_EDITOR
            DatabaseIndex = NREmulatorManager.SIMPlaneID;
#else
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) Destroy(meshRenderer);
            MeshFilter mesh = GetComponent<MeshFilter>();
            if (mesh != null) Destroy(mesh);
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (NREmulatorManager.Instance.IsInGameView(transform.position))
            {
                NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackableData<NRTrackablePlane>
                (transform.position, transform.rotation, 0.4f, 0.4f, (uint)DatabaseIndex, TrackingState.Tracking);
            }
            else
            {
                NREmulatorManager.Instance.NativeEmulatorApi.UpdateTrackableData<NRTrackablePlane>
                (transform.position, transform.rotation, 0.4f, 0.4f, (uint)DatabaseIndex, TrackingState.Stopped);
            }
        }
#endif
    }
}