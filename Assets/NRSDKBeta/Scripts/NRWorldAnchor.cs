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
    using System;
    using UnityEngine;

    /**
     * @brief Manages AR mapping system.
     * 
     * Through this class, application can save、load a map. 
     */
    internal class NRWorldAnchor
    {
        internal UInt64 AnchorNativeHandle = 0;

        internal NativeMapping NativeInterface;

        internal NRWorldAnchor(UInt64 trackableNativeHandle, NativeMapping nativeinterface)
        {
            AnchorNativeHandle = trackableNativeHandle;
            NativeInterface = nativeinterface;
        }

        /**
         * Get the tracking state of current trackable.
         */
        public TrackingState GetTrackingState()
        {
            return NativeInterface.GetTrackingState(AnchorNativeHandle);
        }


        /**
         * Get the center pose of current trackable.
         */
        public virtual Pose GetCenterPose()
        {
            return NativeInterface.GetAnchorPose(AnchorNativeHandle);
        }
    }
}
