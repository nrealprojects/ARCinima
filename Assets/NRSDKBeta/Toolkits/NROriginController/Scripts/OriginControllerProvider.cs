/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/
using System;
using UnityEngine;
using NRKernal;
using NRKernal.OriginController;

public class OriginControllerProvider : ControllerProviderBase
{
    private int m_ProcessedFrame;
    private bool m_NeedInit = true;
    private byte[] _dataBytes = null;

    public OriginControllerProvider(ControllerState[] states) : base(states)
    {

    }

    public override int ControllerCount
    {
        get
        {
            if (!Inited)
                return 0;
            return 1;
        }
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }


    private void OnControllerConnected()
    {
        Inited = true;
    }

    public override void Update()
    {
        if (m_ProcessedFrame == Time.frameCount)
            return;
        m_ProcessedFrame = Time.frameCount;
        if (m_NeedInit)
        {
            InitNativeController();
            return;
        }
        if (!Inited)
            return;
        int availableCount = ControllerCount;
        for (int i = 0; i < availableCount; i++)
        {
            UpdateControllerState(i);
        }
    }

    public override void OnDestroy()
    {

    }

    public override void TriggerHapticVibration(int index, float durationSeconds = 0.1f, float frequency = 1000f, float amplitude = 0.5f)
    {
        if (!Inited)
            return;
        if (NRInput.GetAvailableControllersCount() > 0 && NRInput.GetControllerType() == ControllerType.CONTROLLER_TYPE_PHONE)
            OriginPhoneVibrateTool.TriggerVibrate(durationSeconds);
    }

    private void InitNativeController()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        OriginBluetoothInterface.Initialize(OnControllerConnected);
        OriginBluetoothInterface.SubscribeSensorData(
                (bytes) =>
                {
                    if (bytes != null)
                        _dataBytes = bytes;
                }
            );
#endif
        m_NeedInit = false;
    }

    private Quaternion _rotation = Quaternion.identity;
    private Vector2 _touch;
    private float r_x;
    private float r_y;
    private float r_z;
    private float r_w;
    private bool _touch_status;
    private bool _physical_button;
    private byte[] _deviceData;
    private bool _isPhone;
    private short device_touch_x;
    private short device_touch_y;
    private float _touch_resolution_x;
    private float _touch_resolution_y;

    private void UpdateControllerState(int index)
    {
        if (!Inited || _dataBytes == null)
            return;
        _isPhone = _dataBytes[26] > 0;
        try
        {
            r_x = BitConverter.ToSingle(new byte[] { _dataBytes[4], _dataBytes[5], _dataBytes[6], _dataBytes[7] }, 0);
            r_y = BitConverter.ToSingle(new byte[] { _dataBytes[8], _dataBytes[9], _dataBytes[10], _dataBytes[11] }, 0);
            r_z = BitConverter.ToSingle(new byte[] { _dataBytes[12], _dataBytes[13], _dataBytes[14], _dataBytes[15] }, 0);
            r_w = r_x * r_x + r_y * r_y + r_z * r_z;
            r_w = (float)Math.Sqrt(1 - r_w);
            _rotation = new Quaternion(r_x, r_y, r_z, r_w);
            device_touch_x = (short)(((_dataBytes[16] << 4) & 0x0FF0) | ((_dataBytes[17] >> 4) & 0x0F));
            device_touch_y = (short)((_dataBytes[17] & 0x0F) << 8 | _dataBytes[18] & 0xFF);

            if (device_touch_x == 0 && device_touch_y == 0)
            {
                _touch.x = 0f;
                _touch.y = 0f;
            }
            else
            {
                _touch_resolution_x = _isPhone ? 1000f : 3840f;
                _touch_resolution_y = _isPhone ? 1000f : 1080f;
                _touch.x = (device_touch_x / _touch_resolution_x - 0.5f) * 2f;
                _touch.y = -(device_touch_y / _touch_resolution_y - 0.5f) * 2f;
            }
            _physical_button = (_dataBytes[19] & 0x1) > 0;
        }
        catch (Exception)
        {
            Debug.LogError("Controller Data Error");
        }

        states[index].controllerType = _isPhone ? ControllerType.CONTROLLER_TYPE_PHONE : ControllerType.CONTROLLER_TYPE_NREALLIGHT;
        states[index].availableFeature = ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION;
        states[index].connectionState = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_CONNECTED;
        states[index].rotation = _rotation;
        states[index].touchPos = _touch;
        states[index].isTouching = false;
        states[index].recentered = false;
        states[index].isCharging = false;
        states[index].buttonsState = _physical_button ? ControllerButton.TRIGGER : 0;
        states[index].buttonsDown = 0;
        states[index].buttonsUp = 0;

        IControllerStateParser stateParser = ControllerStateParseUtility.GetControllerStateParser(states[index].controllerType, index);
        if (stateParser != null)
            stateParser.ParserControllerState(states[index]);
        CheckRecenter(index);
    }

    private void CheckRecenter(int index)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (states[index].GetButtonDown(ControllerButton.APP))
        {
            states[index].recentered = true;
            OriginBluetoothInterface.ResetDevice();
        }
#endif
    }
}