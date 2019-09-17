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
    using System.Collections;
    using System.Threading;
    using UnityEngine;

    /**
    * @brief Manage the image update of rgb camera.
    * 
    * Through this class, application can play rgb camera, configure it, start or stop it. 
    */
    public class NRCameraCapture : MonoBehaviour
    {
        public NRRgbCamera.CaptureEvent OnFirstFrameReady;
        public NRRgbCamera.CaptureErrorEvent OnError;
        private AutoResetEvent m_Event = new AutoResetEvent(true);

        /**
        * @brief Image format of rgb camera.
        */
        public CameraImageFormat ImageFormat
        {
            get
            {
                return NRRgbCamera.ImageFormat;
            }
            set
            {
                NRRgbCamera.SetImageFormat(value);
            }
        }

        /**
        * @brief Current frame count of rgb camera.
        */
        public int FrameCount
        {
            get
            {
                return NRRgbCamera.FrameCount;
            }
        }

        /**
        * @brief Texture ptr of rgb camera.
        */
        public IntPtr TexturePtr
        {
            get
            {
                return NRRgbCamera.TexturePtr;
            }
        }

        /**
        * @brief Texture2D of rgb camera.
        */
        public Texture2D Texture = null;

        /**
        * @brief Resolution of rgb camera.
        */
        public NativeResolution Resolution
        {
            get
            {
                return NRRgbCamera.Resolution;
            }
        }

        /*
         * frame rate should not lower then 1 and higher then 30 
         */
        private int m_UpdateFrameRate = 60;
        public int UpdateFrameRate
        {
            get
            {
                return m_UpdateFrameRate;
            }
            set
            {
                if (m_UpdateFrameRate <= 0 || m_UpdateFrameRate > 30)
                {
                    NRDebugger.Log("Update frame rate should not lower then 1 and higher then 30.");
                    return;
                }
                m_UpdateFrameRate = value;
            }
        }

        private bool m_NeedRefreshImage = false;

        private void OnEnable()
        {
            NRRgbCamera.OnImageUpdate += OnImageUpdate;
            NRRgbCamera.OnError += StateError;
        }

        private void OnDisable()
        {
            NRRgbCamera.OnImageUpdate -= OnImageUpdate;
            NRRgbCamera.OnError -= StateError;
        }

        // Do not do any unity Built-in operation. 
        private void OnImageUpdate()
        {
            m_NeedRefreshImage = true;
            m_Event.WaitOne();
        }

        private void FirstFramReady()
        {
            Texture = new Texture2D(Resolution.width, Resolution.height, GetFormatByCamraImageFormat(ImageFormat), false);
            NRDebugger.Log("[NRCameraCapture] FirstFramReady : " + Resolution.ToString());

            if (OnFirstFrameReady != null)
            {
                OnFirstFrameReady();
            }
        }

        private void ApplyTex()
        {
            if (!m_NeedRefreshImage || NRRgbCamera.CurrentState != NRRgbCamera.CaptureState.Running)
            {
                return;
            }
            if (Texture == null)
            {
                FirstFramReady();
            }
            if (NRRgbCamera.TexuteData != null && NRRgbCamera.TexturePtr != IntPtr.Zero && NRRgbCamera.RawDataSize != 0)
            {
                Texture.LoadRawTextureData(NRRgbCamera.TexuteData);
                Texture.Apply();
            }
            m_NeedRefreshImage = false;

            m_Event.Set();
        }

        /**
        * @brief Start to play rgb camera.
        */
        public void Play()
        {
            NRRgbCamera.Play();
            StopCoroutine("RefreshTex");
            StartCoroutine("RefreshTex");
        }

        private TextureFormat GetFormatByCamraImageFormat(CameraImageFormat format)
        {
            TextureFormat out_format = TextureFormat.RGB24;
            switch (format)
            {
                case CameraImageFormat.YUV_420_888:
                    out_format = TextureFormat.YUY2;
                    break;
                case CameraImageFormat.RGB_888:
                    out_format = TextureFormat.RGB24;
                    break;
                default:
                    out_format = TextureFormat.RGB24;
                    break;
            }

            return out_format;
        }

        private IEnumerator RefreshTex()
        {
            while (true)
            {
                float time_delta = 1.0f / UpdateFrameRate;
                yield return new WaitForSeconds(time_delta);
                this.ApplyTex();
            }
        }

        /**
        * @brief Stop rgb camera.
        */
        public void Stop()
        {
            NRRgbCamera.Stop();
            StopCoroutine("RefreshTex");
        }

        /**
        * @brief Release the rgb camera.
        */
        public void Release()
        {
            NRRgbCamera.Release();
        }

        private void StateError(string msg)
        {
            if (OnError != null)
            {
                OnError(msg);
            }
        }

        private void OnDestroy()
        {
            this.Release();
        }
    }
}
