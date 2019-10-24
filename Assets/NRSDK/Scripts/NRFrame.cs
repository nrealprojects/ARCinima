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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /**
     * @brief Holds information about NR Device's pose in the world coordinate, trackables, etc..
     * 
     * Through this class, application can get the infomation of current frame. 
     * It contains session status,lost tracking reason,device pose,trackables, etc..
     */
    internal partial class NRFrame
    {
        /**
         * @brief Get the tracking state of HMD.
         */
        public static SessionState SessionStatus
        {
            get
            {
                return NRSessionManager.Instance.SessionStatus;
            }
        }

        /**
         * @brief Get the lost tracking reason of HMD.
         */
        public static LostTrackingReason LostTrackingReason
        {
            get
            {
                return NRSessionManager.Instance.LostTrackingReason;
            }
        }

        private static Pose m_HeadPose;

        /**
         * @brief Get the pose of device in unity world coordinate.
         * @return Pose of device.
         */
        public static Pose HeadPose
        {
            get
            {
                if (SessionStatus == SessionState.Tracking)
                {
                    Pose pose = Pose.identity;
                    var result = GetHeadPoseByTime(ref pose);
                    if (result)
                    {
                        m_HeadPose = pose;
                    }
                }
                return m_HeadPose;
            }
        }

        public static bool GetHeadPoseByTime(ref Pose pose, UInt64 timestamp = 0, UInt64 predict = 0)
        {
            if (SessionStatus == SessionState.Tracking)
            {
                return NRSessionManager.Instance.NativeAPI.NativeHeadTracking.GetHeadPose(ref pose, timestamp, predict);
            }
            return false;
        }

        /**
         * @brief Get the pose of center camera between left eye and right eye.
         */
        public static Pose CenterEyePose
        {
            get
            {
                if (SessionStatus != SessionState.Tracking)
                {
                    return HeadPose;
                }
                Transform leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.transform;
                Transform rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.transform;

                Vector3 centerEye_pos = (leftCamera.position + rightCamera.position) * 0.5f;
                Quaternion centerEye_rot = Quaternion.Lerp(leftCamera.rotation, rightCamera.rotation, 0.5f);

                return new Pose(centerEye_pos, centerEye_rot);
            }
        }

        private static EyePoseData m_EyePosFromHead;

        /**
         * @brief Get the offset position between eye and head.
         */
        public static EyePoseData EyePosFromHead
        {
            get
            {
                if (SessionStatus == SessionState.Tracking)
                {
                    m_EyePosFromHead.LEyePose = NRDevice.Instance.NativeHMD.GetEyePoseFromHead(NativeEye.LEFT);
                    m_EyePosFromHead.REyePose = NRDevice.Instance.NativeHMD.GetEyePoseFromHead(NativeEye.RIGHT);
                    m_EyePosFromHead.RGBEyePos = NRDevice.Instance.NativeHMD.GetEyePoseFromHead(NativeEye.RGB);
                }
                return m_EyePosFromHead;
            }
        }

        /**
         * @brief Get the project matrix of camera in unity.
         * @return project matrix of camera.
         */
        public static EyeProjectMatrixData GetEyeProjectMatrix(out bool result, float znear, float zfar)
        {
            result = false;
            EyeProjectMatrixData m_EyeProjectMatrix = new EyeProjectMatrixData();
            result = NRDevice.Instance.NativeHMD.GetProjectionMatrix(ref m_EyeProjectMatrix, znear, zfar);
            return m_EyeProjectMatrix;
        }

        /**
         * @brief Get the list of trackables with specified filter.
         * @param[out] trackableList A list where the returned trackable stored. The previous values will be cleared.
         * @param filter Query filter.
         */
        public static void GetTrackables<T>(List<T> trackables, NRTrackableQueryFilter filter) where T : NRTrackable
        {
            trackables.Clear();
#if !UNITY_EDITOR
            if (SessionStatus != SessionState.Tracking)
            {
                return;
            }
#endif
            NRSessionManager.Instance.NativeAPI.TrackableFactory.GetTrackables<T>(trackables, filter);
        }


    }
}
