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
    using UnityEngine;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public enum ControllerType
    {
        CONTROLLER_TYPE_EDITOR = 1001,
        CONTROLLER_TYPE_UNKNOWN = -1,
        CONTROLLER_TYPE_NREALLIGHT = 0,
        CONTROLLER_TYPE_PHONE = 1
    }

    public enum ControllerAvailableFeature
    {
        CONTROLLER_AVAILABLE_FEATURE_POSITION = (1 << 0),
        CONTROLLER_AVAILABLE_FEATURE_ROTATION = (1 << 1),
        CONTROLLER_AVAILABLE_FEATURE_GYRO = (1 << 2),
        CONTROLLER_AVAILABLE_FEATURE_ACCEL = (1 << 3),
        CONTROLLER_AVAILABLE_FEATURE_MAG = (1 << 4),
        CONTROLLER_AVAILABLE_FEATURE_BATTERY = (1 << 5),
        CONTROLLER_AVAILABLE_FEATURE_CHARGING = (1 << 6),
        CONTROLLER_AVAILABLE_FEATURE_RECENTER = (1 << 7),
        CONTROLLER_AVAILABLE_FEATURE_HAPTIC_VIBRATE = (1 << 8)
    }

    public enum ControllerButton
    {
        TRIGGER = 1 << 0,
        APP = 1 << 1,
        HOME = 1 << 2,
        GRIP = 1 << 3,
        TOUCHPAD_BUTTON = 1 << 4,
        TOUCHPAD_TOUCH = 1 << 5
    }

    public enum ControllerConnectionState
    {
        CONTROLLER_CONNECTION_STATE_ERROR = -1,
        CONTROLLER_CONNECTION_STATE_NOT_INITIALIZED = 0,
        CONTROLLER_CONNECTION_STATE_DISCONNECTED = 1,
        CONTROLLER_CONNECTION_STATE_CONNECTING = 2,
        CONTROLLER_CONNECTION_STATE_CONNECTED = 3
    }

    public class ControllerState
    {
        internal ControllerType controllerType;
        internal ControllerConnectionState connectionState;
        internal Quaternion rotation;
        internal Vector3 position;
        internal Vector3 gyro;
        internal Vector3 accel;
        internal Vector3 mag;
        internal Vector2 touchPos;
        internal bool isTouching;
        internal bool recentered;
        internal ControllerButton buttonsState;
        internal ControllerButton buttonsDown;
        internal ControllerButton buttonsUp;
        internal bool isCharging;
        internal int batteryLevel;
        internal ControllerAvailableFeature availableFeature;

        public ControllerState()
        {
            Reset();
        }

        public bool Is6dof
        {
            get
            {
                return IsFeatureAvailable(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION) && IsFeatureAvailable(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION);
            }
        }

        public bool IsFeatureAvailable(ControllerAvailableFeature feature)
        {
            return (availableFeature & feature) != 0;
        }

        public bool GetButton(ControllerButton button)
        {
            return (buttonsState & button) != 0;
        }

        public bool GetButtonDown(ControllerButton button)
        {
            return (buttonsDown & button) != 0;
        }

        public bool GetButtonUp(ControllerButton button)
        {
            return (buttonsUp & button) != 0;
        }

        public void Reset()
        {
            controllerType = ControllerType.CONTROLLER_TYPE_UNKNOWN;
            connectionState = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_NOT_INITIALIZED;
            rotation = Quaternion.identity;
            position = Vector3.zero;
            gyro = Vector3.zero;
            accel = Vector3.zero;
            mag = Vector3.zero;
            touchPos = Vector2.zero;
            isTouching = false;
            recentered = false;
            buttonsState = 0;
            buttonsDown = 0;
            buttonsUp = 0;
            isCharging = false;
            batteryLevel = 0;
            availableFeature = 0;
        }
    }
    /// @endcond
}