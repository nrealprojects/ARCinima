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
    using UnityEngine;
    using System.Runtime.InteropServices;

    /**
    * @brief Enumeration of handedness
    */
    public enum ControllerHandEnum
    {
        Right = 0,
        Left = 1
    }

    /**
    * @brief Enumeration of raycast mode. Normally, suggest using "Laser" mode.
    */
    public enum RaycastModeEnum
    {
        Gaze,
        Laser
    }

    /**
    * @brief Enumeration of controller visual types.
    */
    public enum ControllerVisualType
    {
        None = 0,
        NrealLight = 1,
        Phone = 2
    }

    /**
    * @brief The main class to handle controller related things, such as to get controller states, update controller states
    * 
    * Through this class, application would create a controllerProvider which could be custom, then the controllerProvider 
    * iteself would define how to update the controller states, so that every frame NRInput could get the right states. There
    * are max two states for one controllerProvider.
    */
    public partial class NRInput : MonoBehaviour
    {
        [SerializeField]
        private Transform m_OverrideCameraCenter;
        [SerializeField]
        private ControllerAnchorsHelper m_AnchorHelper;
        [SerializeField]
        private RaycastModeEnum m_RaycastMode = RaycastModeEnum.Laser;
        private ControllerVisualManager m_VisualManager;
        private int m_LastControllerCount;
        private bool m_ReticleVisualActive = true;
        private bool m_LaserVisualActive = true;
        private bool m_ControllerVisualActive = true;
        private bool m_HapticVibrationEnabled = true;

        private static NRInput m_Instance = null;
        private static ControllerHandEnum m_DomainHand = ControllerHandEnum.Right;
        private static ControllerProviderBase m_ControllerProvider;
        private static ControllerState[] m_States = new ControllerState[MAX_CONTROLLER_STATE_COUNT]
        {
            new ControllerState(),
            new ControllerState()
        };
        private static NRInput Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = FindObjectOfType<NRInput>();
                if (m_Instance == null)
                    Debug.LogError("Can not find NRInput class in the scene!");
                return m_Instance;
            }
        }

        /**
        * @brief Max count of controllerstates supported per frame
        */
        public const int MAX_CONTROLLER_STATE_COUNT = 2;

        /**
        * @brief Event invoked whenever the domain hand has changed.
        */
        public static Action<ControllerHandEnum> OnDomainHandChanged;

        /**
        * @brief Event invoked whenever a controller device is connected.
        */
        public static Action OnControllerConnected;

        /**
        * @brief Event invoked whenever a controller device is disconnected.
        */
        public static Action OnControllerDisconnected;

        /**
        * @brief Event invoked whenever a controller device is recentered.
        */
        public static Action<ControllerHandEnum> OnControllerRecentered;

        /**
        * @brief Determine whether to show reticle visuals, could be get and set at runtime.
        */
        public static bool ReticleVisualActive { get { return Instance.m_ReticleVisualActive; } set { Instance.m_ReticleVisualActive = value; } }

        /**
        * @brief Determine whether to show laser visuals, could be get and set at runtime.
        */
        public static bool LaserVisualActive { get { return Instance.m_LaserVisualActive; } set { Instance.m_LaserVisualActive = value; } }

        /**
        * @brief Determine whether to show controller visuals, could be get and set at runtime.
        */
        public static bool ControllerVisualActive { get { return Instance.m_ControllerVisualActive; } set { Instance.m_ControllerVisualActive = value; } }

        /**
        * @brief Determine whether enable haptic vibration.
        */
        public static bool HapticVibrationEnabled { get { return Instance.m_HapticVibrationEnabled; } set { Instance.m_HapticVibrationEnabled = value; } }

        /**
        * @brief It's a helper to get controller anchors which are frequently used.
        */
        public static ControllerAnchorsHelper AnchorsHelper { get { return Instance.m_AnchorHelper; } }

        /**
        * @brief Get the current enumeration of handedness.
        */
        public static ControllerHandEnum DomainHand { get { return m_DomainHand; } }

        /**
        * @brief Determine which raycast mode to use.
        */
        public static RaycastModeEnum RaycastMode { get { return Instance.m_RaycastMode; } set { Instance.m_RaycastMode = value; } }

        /**
        * @brief Get the transform of the camera which controllers are following.
        */
        public static Transform CameraCenter { get { return Instance.GetCameraCenter(); } }

        private void Awake()
        {
            if (m_Instance == null)
                m_Instance = this;
        }

        private void Start()
        {
            //Should not be inited at Awake, because that default controller provider may be changed at Awake
            Init();
#if UNITY_EDITOR_WIN
            // For Emulator Init
            InitEmulator();
#endif
        }

        private void Update()
        {
            if (m_ControllerProvider == null)
                return;
            m_ControllerProvider.Update();
            if (!m_ControllerProvider.Inited)
                return;
            m_VisualManager.UpdateAllVisuals();
            CheckControllerConnection();
            CheckControllerRecentered();
        }

        private void OnEnable()
        {
            if (m_ControllerProvider != null)
                m_ControllerProvider.OnResume();
        }

        private void OnDisable()
        {
            if (m_ControllerProvider != null)
                m_ControllerProvider.OnPause();
        }

        private void OnDestroy()
        {
            if (m_ControllerProvider != null)
            {
                m_ControllerProvider.OnDestroy();
                m_ControllerProvider = null;
            }
        }

        private void CheckControllerConnection()
        {
            int currentControllerCount = GetAvailableControllersCount();
            if (m_LastControllerCount < currentControllerCount)
            {
                if (OnControllerConnected != null)
                    OnControllerConnected();
            }
            else if (m_LastControllerCount > currentControllerCount)
            {
                if (OnControllerDisconnected != null)
                    OnControllerDisconnected();
            }
            m_LastControllerCount = currentControllerCount;
        }

        private void CheckControllerRecentered()
        {
            if (GetControllerState(ControllerHandEnum.Right).recentered && OnControllerRecentered != null)
                OnControllerRecentered(ControllerHandEnum.Right);
            if (GetControllerState(ControllerHandEnum.Left).recentered && OnControllerRecentered != null)
                OnControllerRecentered(ControllerHandEnum.Left);
        }

        private void OnApplicationPause(bool paused)
        {
            if (m_ControllerProvider == null || !m_ControllerProvider.Inited)
                return;
            if (paused)
                m_ControllerProvider.OnPause();
            else
                m_ControllerProvider.OnResume();
        }

        private void Init()
        {
            m_VisualManager = gameObject.AddComponent<ControllerVisualManager>();
            m_VisualManager.Init(m_States);
            m_ControllerProvider = ControllerProviderFactory.CreateControllerProvider(m_States);
        }

        private void InitEmulator()
        {
            if (!NREmulatorManager.Inited && !GameObject.Find("NREmulatorManager"))
            {
                NREmulatorManager.Inited = true;
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorManager"));
            }
            if (!GameObject.Find("NREmulatorController"))
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/NREmulatorController"));
            }
        }

        private Transform GetCameraCenter()
        {
            if (m_OverrideCameraCenter)
                return m_OverrideCameraCenter;
            if (Camera.main)
                return Camera.main.transform;
            return null;
        }

        private static int ConvertHandToIndex(ControllerHandEnum handEnum)
        {
            if (GetAvailableControllersCount() < 2)
                return 0;
            return (int)handEnum;
        }

        private static ControllerState GetControllerState(ControllerHandEnum hand)
        {
            return m_States[ConvertHandToIndex(hand)];
        }

        /**
        * @brief Set the current enumeration of handedness.
        */
        public static void SetDomainHandMode(ControllerHandEnum handEnum)
        {
            if (m_DomainHand == handEnum)
                return;
            m_DomainHand = handEnum;
            if (OnDomainHandChanged != null)
                OnDomainHandChanged(m_DomainHand);
        }

        /**
        * @brief Get the current count of controllers which are connected and available
        */
        public static int GetAvailableControllersCount()
        {
            if (m_ControllerProvider == null)
                return 0;
            return m_ControllerProvider.ControllerCount;
        }

        /**
        * @brief Get the ControllerType of current controller
        */
        public static ControllerType GetControllerType()
        {
            return GetControllerState(DomainHand).controllerType;
        }

        /**
        * @brief Get the ConnectionState of current controller
        */
        public static ControllerConnectionState GetControllerConnectionState()
        {
            return GetControllerState(DomainHand).connectionState;
        }

        /**
        * @brief Returns true if the controller 
        */
        public static bool CheckControllerAvailable(ControllerHandEnum handEnum)
        {
            int availableCount = GetAvailableControllersCount();
            if (availableCount == 2)
                return true;
            if (availableCount == 1)
                return handEnum == DomainHand;
            return false;
        }

        /**
        * @brief Returns true if the current controller supports the certain feature.
        */
        public static bool GetControllerAvailableFeature(ControllerAvailableFeature feature)
        {
            if (GetAvailableControllersCount() == 0)
                return false;
            return GetControllerState(m_DomainHand).IsFeatureAvailable(feature);
        }

        /**
        * @brief Returns true if the button is currently pressed this frame.
        */
        public static bool GetButton(ControllerButton button)
        {
            return GetButton(m_DomainHand, button);
        }

        /**
        * @brief Returns true if the button was pressed down this frame.
        */
        public static bool GetButtonDown(ControllerButton button)
        {
            return GetButtonDown(m_DomainHand, button);
        }

        /**
        * @brief Returns true if the button was released this frame.
        */
        public static bool GetButtonUp(ControllerButton button)
        {
            return GetButtonUp(m_DomainHand, button);
        }

        /**
        * @brief Returns a Vector2 touch position on touchpad of the domain controller, range: x(-1f ~ 1f), y(-1f ~ 1f);
        */
        public static Vector2 GetTouch()
        {
            return GetTouch(m_DomainHand);
        }

        /**
        * @brief Returns the current position of the domain controller if 6dof, otherwise returns Vector3.zero
        */
        public static Vector3 GetPosition()
        {
            return GetPosition(m_DomainHand);
        }

        /**
        * @brief Returns the current rotation of the domain controller
        */
        public static Quaternion GetRotation()
        {
            return GetRotation(m_DomainHand);
        }

        /**
        * @brief Returns the gyro sensor value of the domain controller
        */
        public static Vector3 GetGyro()
        {
            return GetGyro(m_DomainHand);
        }

        /**
        * @brief Returns the accel sensor value of the domain controller
        */
        public static Vector3 GetAccel()
        {
            return GetAccel(m_DomainHand);
        }

        /**
        * @brief Returns the magnetic sensor value of the domain controller
        */
        public static Vector3 GetMag()
        {
            return GetMag(m_DomainHand);
        }

        /**
        * @brief Returns the battery level of the domain controller
        */
        public static int GetControllerBattery()
        {
            return GetControllerBattery(DomainHand);
        }

        /**
        * @brief Trigger vibration of the domain controller
        */
        public static void TriggerHapticVibration(float durationSeconds = 0.1f, float frequency = 200f, float amplitude = 0.8f)
        {
            TriggerHapticVibration(m_DomainHand, durationSeconds, frequency, amplitude);
        }

        /**
        * @brief Returns true if the button is currently pressed this frame on a certain handedness controller.
        */
        public static bool GetButton(ControllerHandEnum hand, ControllerButton button)
        {
            return GetControllerState(hand).GetButton(button);
        }

        /**
        * @brief Returns true if the button was pressed down this frame on a certain handedness controller.
        */
        public static bool GetButtonDown(ControllerHandEnum hand, ControllerButton button)
        {
            return GetControllerState(hand).GetButtonDown(button);
        }

        /**
        * @brief Returns true if the button was released this frame on a certain handedness controller.
        */
        public static bool GetButtonUp(ControllerHandEnum hand, ControllerButton button)
        {
            return GetControllerState(hand).GetButtonUp(button);
        }

        /**
        * @brief Returns a Vector2 touch position on touchpad of a certain handedness controller, range: x(-1f ~ 1f), y(-1f ~ 1f).
        */
        public static Vector2 GetTouch(ControllerHandEnum hand)
        {
            return GetControllerState(hand).touchPos;
        }

        /**
        * @brief Returns the current position of a certain handedness controller if 6dof, otherwise returns Vector3.zero
        */
        public static Vector3 GetPosition(ControllerHandEnum hand)
        {
            return GetControllerState(hand).position;
        }

        /**
        * @brief Returns the current rotation of a certain handedness controller
        */
        public static Quaternion GetRotation(ControllerHandEnum hand)
        {
            return GetControllerState(hand).rotation;
        }

        /**
        * @brief Returns the gyro sensor value of a certain handedness controller
        */
        public static Vector3 GetGyro(ControllerHandEnum hand)
        {
            return GetControllerState(hand).gyro;
        }

        /**
        * @brief Returns the accel sensor value of a certain handedness controller
        */
        public static Vector3 GetAccel(ControllerHandEnum hand)
        {
            return GetControllerState(hand).accel;
        }

        /**
        * @brief Returns the magnetic sensor value of a certain handedness controller
        */
        public static Vector3 GetMag(ControllerHandEnum hand)
        {
            return GetControllerState(hand).mag;
        }

        /**
        * @brief Returns the battery level of a certain handedness controller, range from 0 to 100
        */
        public static int GetControllerBattery(ControllerHandEnum hand)
        {
            return GetControllerState(hand).batteryLevel;
        }

        /**
        * @brief Trigger vibration of a certain handedness controller
        */
        public static void TriggerHapticVibration(ControllerHandEnum hand, float durationSeconds = 0.1f, float frequency = 200f, float amplitude = 0.8f)
        {
            if (HapticVibrationEnabled == false)
                return;
            if (GetAvailableControllersCount() == 0)
                return;
            m_ControllerProvider.TriggerHapticVibration(ConvertHandToIndex(hand), durationSeconds, frequency, amplitude);
        }
    }
}
