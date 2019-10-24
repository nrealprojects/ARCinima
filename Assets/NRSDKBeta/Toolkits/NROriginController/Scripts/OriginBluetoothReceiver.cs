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

    internal class OriginBluetoothReceiver : MonoBehaviour
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public Action InitializedAction;
        public Action DeInitializedAction;
        public Action<byte[]> DidUpdateSensorDataAction;
        public bool IsInited { get; private set; }

        private byte[] bytes;

        private void OnBatteryBluetoothData(string base64Data)
        {

        }

        private void OnSenserBluetoothData(string base64Data)
        {
            if (base64Data != null)
            {
                bytes = System.Convert.FromBase64String(base64Data);
                if (bytes.Length > 0)
                {
                    if (!IsInited)
                    {
                        IsInited = true;
                        if (InitializedAction != null)
                            InitializedAction();
                    }
                    if (DidUpdateSensorDataAction != null)
                        DidUpdateSensorDataAction(bytes);
                }
            }
        }

        public void OnDeInitialized()
        {
            IsInited = false;
            if (DeInitializedAction != null)
                DeInitializedAction();
        }
#endif
    }
}
