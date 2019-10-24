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
    /**
    * \if english
    * NRSession holds information about NR Device's pose in the world coordinate, trackables, etc..
    * \else
    * @brief NR Device当前帧的快照。
    * 
    * ARFrame持有NR Device的设备位置，trackables以及其他信息。
    * \endif
    */
    internal partial class NRFrame
    {
        private static EyePoseData m_EyePose;
        /**
         * \if english
         * @brief Get the pose of device in unity world coordinate.
         * @return Pose of device.
         * \else
         * @brief 获取设备在unity世界坐标系的位姿。
         * @return 设备的位姿。
         * \endif
         */
        public static EyePoseData EyePose
        {
            get
            {
                if (NRSessionManager.Instance.SessionStatus == SessionState.Tracking)
                {
                    NRSessionManager.Instance.NativeAPI.NativeHeadTracking.GetEyePose(ref m_EyePose.LEyePose, ref m_EyePose.REyePose);
                }
                return m_EyePose;
            }
        }

        //public static void StartGainMarkerData()
        //{
        //    var arSession = NRARSessionManager.Instance;
        //    if (arSession != null && arSession.SessionStatus == TrackingState.Tracking)
        //    {
        //        arSession.NativeAPI.NativeTrackable.GainMarkerDataStart();
        //    }
        //}

        //public static void StopGainMarkerData()
        //{
        //    var arSession = NRARSessionManager.Instance;
        //    if (arSession != null && arSession.SessionStatus == TrackingState.Tracking)
        //    {
        //        arSession.NativeAPI.NativeTrackable.GainMarkerDataStop();
        //    }
        //}
    }
}
