/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class GazeTracker : MonoBehaviour
    {
        [SerializeField]
        private NRPointerRaycaster m_Raycaster;
        private bool m_IsEnabled = false;

        private Transform CameraCenter
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        private void Start()
        {
            UpdateTracker();
        }

        private void Update()
        {
            if (CameraCenter == null)
                return;
            m_IsEnabled = NRInput.RaycastMode == RaycastModeEnum.Gaze;
            UpdateTracker();
        }

        private void UpdateTracker()
        {
            m_Raycaster.gameObject.SetActive(m_IsEnabled);
            if (m_IsEnabled)
            {
                transform.position = CameraCenter.position;
                transform.rotation = CameraCenter.rotation;
            }
        }
    }
    /// @endcond
}
