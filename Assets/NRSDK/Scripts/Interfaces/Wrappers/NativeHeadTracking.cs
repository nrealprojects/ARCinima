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
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /**
    * @brief 6-dof Head Tracking's Native API .
    */
    internal partial class NativeHeadTracking
    {
        private NativeInterface m_NativeInterface;
        internal UInt64 headTrackingHandle = 0;

        public NativeHeadTracking(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        public bool Create()
        {
            var result = NativeApi.NRHeadTrackingCreate(m_NativeInterface.TrackingHandle, ref headTrackingHandle);
            NRDebug.Log("[NativeHeadTracking Create :]" + result.ToString());
            return result == NativeResult.Success;
        }

        public Pose GetHeadPose()
        {
            UInt64 headPoseHandle = 0;
            UInt64 hmd_nanos = 0;
            var result = NativeApi.NRTrackingGetHMDTimeNanos(m_NativeInterface.TrackingHandle, ref hmd_nanos);

            UInt64 predict_time = 0;
            NativeApi.NRHeadTrackingGetRecommendPredictTime(m_NativeInterface.TrackingHandle, headTrackingHandle, ref predict_time);
            hmd_nanos += predict_time;
            result = NativeApi.NRHeadTrackingAcquireTrackingPose(m_NativeInterface.TrackingHandle, headTrackingHandle, hmd_nanos, ref headPoseHandle);
            //NRDebug.Log("[NativeHeadTracking Create :]" + m_NativeInterface.TrackingHandle + " head tracking handle:" + headTrackingHandle.ToString());
            NativeMat4f headpos_native = new NativeMat4f(Matrix4x4.identity);
            result = NativeApi.NRTrackingPoseGetPose(m_NativeInterface.TrackingHandle, headPoseHandle, ref headpos_native);
            Pose head_pose = Pose.identity;
            if (result == NativeResult.Success)
            {
                ConversionUtility.ApiPoseToUnityPose(headpos_native, out head_pose);
            }
            NativeApi.NRTrackingPoseDestroy(m_NativeInterface.TrackingHandle, headPoseHandle);
            return head_pose;
        }

        public LostTrackingReason GetTrackingLostReason()
        {
            LostTrackingReason lost_tracking_reason = LostTrackingReason.NONE;
            NativeApi.NRTrackingPoseGetTrackingReason(m_NativeInterface.TrackingHandle, headTrackingHandle, ref lost_tracking_reason);
            return lost_tracking_reason;
        }

        public NativeResult Destroy()
        {
            return NativeApi.NRHeadTrackingDestroy(m_NativeInterface.TrackingHandle, headTrackingHandle);
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHeadTrackingCreate(UInt64 tracking_handle,
                ref UInt64 outHeadTrackingHandle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingGetHMDTimeNanos(UInt64 tracking_handle,
                ref UInt64 out_hmd_time_nanos);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHeadTrackingGetRecommendPredictTime(
                UInt64 tracking_handle, UInt64 head_tracking_handle, ref UInt64 out_predict_time_nanos);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHeadTrackingAcquireTrackingPose(UInt64 sessionHandle,
                UInt64 head_tracking_handle, UInt64 hmd_time_nanos, ref UInt64 out_tracking_pose_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingPoseGetPose(UInt64 tracking_handle,
                UInt64 tracking_pose_handle, ref NativeMat4f out_pose);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingPoseGetTrackingReason(UInt64 tracking_handle,
                UInt64 tracking_pose_handle, ref LostTrackingReason out_tracking_reason);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingPoseDestroy(UInt64 tracking_handle, UInt64 tracking_pose_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHeadTrackingDestroy(UInt64 tracking_handle, UInt64 head_tracking_handle);
        };
    }
}
