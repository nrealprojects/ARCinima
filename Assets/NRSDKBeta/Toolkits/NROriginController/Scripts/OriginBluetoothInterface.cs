/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/

namespace NRKernal.OriginController
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class OriginBluetoothInterface
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private static OriginBluetoothReceiver originBluetoothReceiver;
        private static AndroidJavaObject _android = null;

        public static OriginBluetoothReceiver Initialize(Action action)
        {
            string receiverName = "OriginBluetoothReceiver";
            originBluetoothReceiver = null;
            try
            {
                GameObject bluetoothReceiverObj = GameObject.Find(receiverName);
                if (bluetoothReceiverObj == null)
                {
                    bluetoothReceiverObj = new GameObject(receiverName);
                    originBluetoothReceiver = bluetoothReceiverObj.AddComponent<OriginBluetoothReceiver>();
                    if (originBluetoothReceiver != null)
                        originBluetoothReceiver.InitializedAction = action;
                }
                GameObject.DontDestroyOnLoad(bluetoothReceiverObj);

                if (_android == null)
                    _android = new AndroidJavaObject("ai.nreal.blejoylib.Controller");
                if (_android != null)
                {
                    AndroidJavaClass j = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject context = j.GetStatic<AndroidJavaObject>("currentActivity");
                    _android.Call("setUp", context);
                    _android.Call("set3DFGameObjAndFunc", receiverName, "OnSenserBluetoothData");
                    //_android.Call("setBatGameObjAndFunc", receiverName, "OnBatteryBluetoothData");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            return originBluetoothReceiver;
        }

        public static void DeInitialize(Action action = null)
        {
            if (originBluetoothReceiver && originBluetoothReceiver.IsInited)
            {
                originBluetoothReceiver.DeInitializedAction = action;
                originBluetoothReceiver.OnDeInitialized();
            }
        }

        public static void SubscribeSensorData(Action<byte[]> action)
        {
            if (originBluetoothReceiver)
                originBluetoothReceiver.DidUpdateSensorDataAction = action;
        }

        public static void ResetDevice()
        {
            TryAndroidFunc("resetAlgorithm");
        }

        public static void TryAndroidFunc(string func)
        {
            try
            {
                if (_android != null)
                    _android.Call(func);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            } 
        }
#endif
    }
}
