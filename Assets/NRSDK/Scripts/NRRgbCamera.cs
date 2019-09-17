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
    using System.Runtime.InteropServices;
    using UnityEngine;

    /**
    * @brief Manage the life cycle of the entire rgbcamera.
    */
    public class NRRgbCamera
    {
        public delegate void CaptureEvent();
        public delegate void CaptureErrorEvent(string msg);
        public static CaptureEvent OnImageUpdate;
        public static CaptureErrorEvent OnError;

        private static NativeCamera m_NativeCamera { get; set; }

        /// @cond EXCLUDE_FROM_DOXYGEN
        public static CameraImageFormat ImageFormat = CameraImageFormat.RGB_888;
        public static IntPtr TexturePtr = IntPtr.Zero;
        public static int RawDataSize;
        public static NativeResolution Resolution = new NativeResolution(1280, 720);

        public enum CaptureState
        {
            UnInitialized = 0,
            Initialized,
            Running,
            Stopped
        }

        public static CaptureState CurrentState = CaptureState.UnInitialized;
        public static int FrameCount = 0;
        public static byte[] TexuteData = null;
        /// @endcond

        public static void Initialize()
        {
            if (CurrentState != CaptureState.UnInitialized)
            {
                return;
            }

            NRDebugger.Log("[NRRgbCamera] Initialize");
            m_NativeCamera = new NativeCamera();
#if !UNITY_EDITOR
            m_NativeCamera.Create();
            m_NativeCamera.SetCaptureCallback(Capture);
#endif
            CurrentState = CaptureState.Initialized;
        }

        public static void SetImageFormat(CameraImageFormat format)
        {
            if (CurrentState == CaptureState.UnInitialized)
            {
                Initialize();
            }
#if !UNITY_EDITOR
            m_NativeCamera.SetImageFormat(format);
#endif
            ImageFormat = format;
            NRDebugger.Log("[NRRgbCamera] SetImageFormat : " + format.ToString());
        }

        private static void Capture(UInt64 rgb_camera_handle, UInt64 rgb_camera_image_handle)
        {
            FrameCount++;
            if (TexturePtr == IntPtr.Zero)
            {
                m_NativeCamera.GetRawData(rgb_camera_image_handle, ref TexturePtr, ref RawDataSize);
                Resolution = m_NativeCamera.GetResolution(rgb_camera_image_handle);
                m_NativeCamera.DestroyImage(rgb_camera_image_handle);
                TexuteData = new byte[RawDataSize];

                Debug.Log(string.Format("[NRRgbCamera] on first fram ready textureptr:{0} rawdatasize:{1} Resolution:{2}",
                   TexturePtr, RawDataSize, Resolution.ToString()));
                return;
            }

            m_NativeCamera.GetRawData(rgb_camera_image_handle, ref TexturePtr, ref RawDataSize);
            Marshal.Copy(TexturePtr, TexuteData, 0, RawDataSize);
            if (OnImageUpdate != null)
            {
                OnImageUpdate();
            }
            m_NativeCamera.DestroyImage(rgb_camera_image_handle);

        }

        /**
        * @brief Start to play rgb camera.
        */
        public static void Play()
        {
            if (CurrentState == CaptureState.UnInitialized)
            {
                Initialize();
            }
            if (CurrentState == CaptureState.UnInitialized || CurrentState == CaptureState.Running)
            {
                StateError(string.Format("Can not start in state:{0}", CurrentState));
                return;
            }
            NRDebugger.Log("[NRRgbCamera] Start to play");
#if !UNITY_EDITOR
            m_NativeCamera.StartCapture();
#endif
            CurrentState = CaptureState.Running;
        }

        /**
        * @brief Stop the rgb camera.
        */
        public static void Stop()
        {
            if (CurrentState != CaptureState.Running)
            {
                StateError(string.Format("Can not stop in state:{0}", CurrentState));
                return;
            }
            NRDebugger.Log("[NRRgbCamera] Start to Stop");
#if !UNITY_EDITOR
            m_NativeCamera.StopCapture();
#endif
            CurrentState = CaptureState.Stopped;
        }

        /**
        * @brief Release the rgb camera.
        */
        public static void Release()
        {
            if (m_NativeCamera != null)
            {
                NRDebugger.Log("[NRRgbCamera] Start to Release");
#if !UNITY_EDITOR
                m_NativeCamera.Release();
#endif
                CurrentState = CaptureState.UnInitialized;
            }
        }

        private static void StateError(string msg)
        {
            if (OnError != null)
            {
                OnError(msg);
            }
        }
    }
}
