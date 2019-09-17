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
    using UnityEngine.EventSystems;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class NRPointerEventData : PointerEventData
    {
        public readonly NRPointerRaycaster raycaster;

        public Vector3 position3D;
        public Quaternion rotation;

        public Vector3 position3DDelta;
        public Quaternion rotationDelta;

        public Vector3 pressPosition3D;
        public Quaternion pressRotation;

        public float pressDistance;
        public GameObject pressEnter;
        public bool pressPrecessed;

        public NRPointerEventData(NRPointerRaycaster raycaster, EventSystem eventSystem) : base(eventSystem)
        {
            this.raycaster = raycaster;
        }

        public virtual bool GetPress() { return NRInput.GetButton(ControllerButton.TRIGGER); }

        public virtual bool GetPressDown() { return NRInput.GetButtonDown(ControllerButton.TRIGGER); }

        public virtual bool GetPressUp() { return NRInput.GetButtonUp(ControllerButton.TRIGGER); }

    }
    /// @endcond
}
