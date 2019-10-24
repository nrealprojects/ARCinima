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
    using System.IO;
    using UnityEngine;
    using static NRKernal.NRHMDPoseTracker;

    /**
    * @brief Manages AR system state and handles the session lifecycle.
    * 
    * Through this class, application can create a session, configure it, start/pause or stop it. 
    */
    internal class NRSessionManager
    {
        private static NRSessionManager m_Instance;

        public static NRSessionManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new NRSessionManager();
                }
                return m_Instance;
            }
        }

        private SessionState m_SessionStatus = SessionState.UnInitialize;

        /**
         * Current session status.
         */
        public SessionState SessionStatus
        {
            get
            {
                return m_SessionStatus;
            }
            private set
            {
                m_SessionStatus = value;
            }
        }

        /**
         * Current lost tracking reason.
         */
        public LostTrackingReason LostTrackingReason
        {
            get
            {
                if (m_IsInitialized)
                {
                    return NativeAPI.NativeHeadTracking.GetTrackingLostReason();
                }
                else
                {
                    return LostTrackingReason.INITIALIZING;
                }
            }
        }

        public NRSessionBehaviour SessionBehaviour { get; set; }

        public NRHMDPoseTracker NRHMDPoseTracker { get; set; }

        public NativeInterface NativeAPI { get; set; }

        private NRRenderer m_NRRenderringController { get; set; }

        private bool m_IsInitialized = false;
        public bool IsInitialized
        {
            get
            {
                return m_IsInitialized;
            }
        }

        public void CreateSession(NRSessionBehaviour session)
        {
            if (SessionBehaviour != null)
            {
                NRDebugger.LogError("Multiple SessionBehaviour components cannot exist in the scene. " +
                  "Destroying the newest.");
                GameObject.Destroy(session);
                return;
            }
            NativeAPI = new NativeInterface();
#if !UNITY_EDITOR_OSX
            SessionStatus = NativeAPI.NativeTracking.Create() ? SessionState.Created : SessionState.UnInitialize;
#endif
            SessionBehaviour = session;

            NRHMDPoseTracker = session.GetComponent<NRHMDPoseTracker>();
            TrackingMode mode = NRHMDPoseTracker.TrackingMode == TrackingType.Tracking3Dof ? TrackingMode.MODE_3DOF : TrackingMode.MODE_6DOF;
            SetTrackingMode(mode);

            this.DeployData();
        }

        private void DeployData()
        {
            if (SessionBehaviour.SessionConfig == null)
            {
                return;
            }
            var database = SessionBehaviour.SessionConfig.TrackingImageDatabase;
            if (database == null)
            {
                NRDebugger.Log("augmented image data base is null!");
                return;
            }
            string deploy_path = database.TrackingImageDataOutPutPath;
            NRDebugger.Log("[TrackingImageDatabase] DeployData to path :" + deploy_path);
            //if (Directory.Exists(database.TrackingImageDataPath))
            //{
            //    NRDebugger.Log("augmented image data is exit!");
            //    return;
            //}
            ZipUtility.UnzipFile(database.RawData, deploy_path, NativeConstants.ZipKey);
        }

        public void SetConfiguration(NRSessionConfig config)
        {
            if (config == null)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.Configration.UpdateConfig(config);
#endif
        }

        private void SetTrackingMode(TrackingMode mode)
        {
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.SetTrackingMode(mode);
#endif
        }

        public void Recenter()
        {
            if (!m_IsInitialized)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.Recenter();
#endif
        }

        public static void SetAppSettings(bool useOptimizedRendering)
        {
            Application.targetFrameRate = 240;
            QualitySettings.maxQueuedFrames = -1;
            QualitySettings.vSyncCount = useOptimizedRendering ? 0 : 1;
            Screen.fullScreen = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void StartSession()
        {
            if (m_IsInitialized)
            {
                return;
            }

            var config = SessionBehaviour.SessionConfig;
            if (config != null)
            {
                SetAppSettings(config.OptimizedRendering);
#if !UNITY_EDITOR
                if (config.OptimizedRendering)
                {
                    if (SessionBehaviour.gameObject.GetComponent<NRRenderer>() == null)
                    {
                        m_NRRenderringController = SessionBehaviour.gameObject.AddComponent<NRRenderer>();
                        m_NRRenderringController.Initialize(NRHMDPoseTracker.leftCamera, NRHMDPoseTracker.rightCamera, NRHMDPoseTracker.GetHeadPose);
                    }
                }
#endif
            }
            else
            {
                SetAppSettings(false);
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeTracking.Start();

            bool result = NativeAPI.NativeHeadTracking.Create();
            if (result)
            {
                SessionStatus = SessionState.Tracking;
            }
#endif

#if UNITY_EDITOR_WIN
            InitEmulator();
#endif
            m_IsInitialized = true;
        }

        public void DisableSession()
        {
            if (!m_IsInitialized)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            if (NativeAPI.NativeTracking.Pause()) SessionStatus = SessionState.Paused;
            if (m_NRRenderringController != null) m_NRRenderringController.Pause();
#endif
        }

        public void ResumeSession()
        {
            if (!m_IsInitialized)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            if (NativeAPI.NativeTracking.Resume()) SessionStatus = SessionState.Tracking;
            if (m_NRRenderringController != null) m_NRRenderringController.Resume();
#endif
        }

        public void DestroySession()
        {
            if (!m_IsInitialized)
            {
                return;
            }
#if !UNITY_EDITOR_OSX
            NativeAPI.NativeHeadTracking.Destroy();
            if (NativeAPI.NativeTracking.Destroy()) SessionStatus = SessionState.Stopped;
            NRDevice.Instance.Destroy();
            SessionBehaviour = null;
#endif
            m_IsInitialized = false;
        }

        private void InitEmulator()
        {
            if (!NREmulatorManager.Inited && !GameObject.Find("NREmulatorManager"))
            {
                NREmulatorManager.Inited = true;
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorManager"));
            }
            if (!GameObject.Find("NREmulatorHeadPos"))
            {
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorHeadPose"));
            }
        }
    }
}
