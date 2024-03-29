﻿/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using UnityEngine;

    /**
    * @brief HMDPoseTracker update the infomations of  pose tracker.
    * 
    * This component is used to initialize the camera parameter, update the device posture, 
    * In addition, application can change TrackingType through this component.
    */
    public class NRHMDPoseTracker : MonoBehaviour
    {
        /**
        * @brief HMD tracking type
        */
        public enum TrackingType
        {
            /**
            * Track the position an rotation.
            */
            Tracking6Dof = 0,

            /**
            * Track the rotation only.
            */
            Tracking3Dof = 1,
        }

        [SerializeField]
        private TrackingType m_TrackingType;

        public TrackingType TrackingMode
        {
            get
            {
                return m_TrackingType;
            }
        }

        /**
        * Use relative coordinates or not.
        */
        public bool UseRelative = false;

        public Camera leftCamera;
        public Camera centerCamera;
        public Camera rightCamera;

        private int m_LeftCullingMask;
        private int m_RightCullingMask;
        private bool isInited = false;

        void Awake()
        {
            m_LeftCullingMask = leftCamera.cullingMask;
            m_RightCullingMask = rightCamera.cullingMask;

#if UNITY_EDITOR
            leftCamera.cullingMask = 0;
            rightCamera.cullingMask = 0;
            centerCamera.cullingMask = -1;
            centerCamera.depth = 1;
#else
            leftCamera.cullingMask = 0;
            rightCamera.cullingMask = 0;
#endif
        }

        void Init()
        {
#if !UNITY_EDITOR
            bool result;
            var matrix_data = NRFrame.GetEyeProjectMatrix(out result, leftCamera.nearClipPlane, leftCamera.farClipPlane);
            if (result)
            {
                leftCamera.projectionMatrix = matrix_data.LEyeMatrix;
                rightCamera.projectionMatrix = matrix_data.REyeMatrix;

                var eyeposFromHead = NRFrame.EyePosFromHead;
                leftCamera.transform.localPosition = eyeposFromHead.LEyePose.position;
                leftCamera.transform.localRotation = eyeposFromHead.LEyePose.rotation;
                rightCamera.transform.localPosition = eyeposFromHead.REyePose.position;
                rightCamera.transform.localRotation = eyeposFromHead.REyePose.rotation;
                centerCamera.transform.localPosition = (leftCamera.transform.localPosition + rightCamera.transform.localPosition) * 0.5f;
                centerCamera.transform.localRotation = Quaternion.Lerp(leftCamera.transform.localRotation, rightCamera.transform.localRotation, 0.5f);

                isInited = true;
            }
#else
            isInited = true;
#endif
        }

        void Update()
        {
            if (NRSessionManager.Instance.IsInitialized && !isInited)
            {
                this.Init();
            }

            UpdatePoseByTrackingType();
        }

        /**
         * @brief Get the real pose of device in unity world coordinate by "UseRelative".
         * @return Real pose of device.
         */
        public void GetHeadPose(ref Pose pose)
        {
            if (!NRSessionManager.Instance.IsInitialized)
            {
                pose.position = Vector3.zero;
                pose.rotation = Quaternion.identity;
                return;
            }
            var poseTracker = NRSessionManager.Instance.NRHMDPoseTracker;
            pose.position = poseTracker.UseRelative ? gameObject.transform.localPosition : gameObject.transform.position;
            pose.rotation = poseTracker.UseRelative ? gameObject.transform.localRotation : gameObject.transform.rotation;
        }

        private void UpdatePoseByTrackingType()
        {
            Pose pose = Pose.identity;
            var result = NRFrame.GetHeadPoseByTime(ref pose);
            //Debug.LogErrorFormat("get pose result: {0} Lost tracking reason:{1} pose :{2}", result, NRFrame.LostTrackingReason, pose.ToString());
            if (result && NRFrame.LostTrackingReason == LostTrackingReason.NONE)
            {
                SetCameraByTrackingStatus(true);
            }
            //else
            //{
            //    SetCameraByTrackingStatus(false);
            //}

            // update pos
            switch (m_TrackingType)
            {
                case TrackingType.Tracking6Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = pose.rotation;
                        transform.localPosition = pose.position;
                    }
                    else
                    {
                        transform.rotation = pose.rotation;
                        transform.position = pose.position;
                    }
                    break;
                case TrackingType.Tracking3Dof:
                    if (UseRelative)
                    {
                        transform.localRotation = pose.rotation;
                    }
                    else
                    {
                        transform.rotation = pose.rotation;
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetCameraByTrackingStatus(bool isopen)
        {
            if (isopen)
            {
                leftCamera.cullingMask = m_LeftCullingMask;
                rightCamera.cullingMask = m_RightCullingMask;
            }
            else
            {
                leftCamera.cullingMask = 0;
                rightCamera.cullingMask = 0;
            }
        }
    }
}
