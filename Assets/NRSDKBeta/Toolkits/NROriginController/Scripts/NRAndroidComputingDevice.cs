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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class NRAndroidComputingDeviceInfo
    {
        public string ProductName { get; private set; }
        public string DeviceType { get; private set; }
        public string BuildVersion { get; private set; }
        public bool IsOld { get; private set; }

        public NRAndroidComputingDeviceInfo()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass build = new AndroidJavaClass("android.os.Build");
        ProductName = build.CallStatic<string>("getString", "ro.product.name");
        DeviceType = build.CallStatic<string>("getString", "ro.product.device");
        BuildVersion = build.CallStatic<string>("getString", "ro.build.version.incremental");
#endif
            IsOld = IsOldVersion();
        }

        private bool IsOldVersion()
        {
            if (string.IsNullOrEmpty(BuildVersion))
                return false;
            if (BuildVersion.Contains("2018"))
                return true;
            string year = "2019";
            int maxMonth = 7;
            int maxDay = 23;
            if (BuildVersion.Contains(year))
            {
                try
                {
                    int start = BuildVersion.IndexOf(year);
                    int month = int.Parse(BuildVersion.Substring(start + year.Length, 2));
                    int day = int.Parse(BuildVersion.Substring(start + year.Length + 2, 2));
                    if (month < maxMonth)
                        return true;
                    else if (month == maxMonth && day <= maxDay)
                        return true;
                }
                catch (System.Exception err)
                {
                    Debug.LogError("Build version check failed: " + err.ToString());
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("ProductNamme: {0}, DeviceType: {1}, BuildVersion: {2}, IsOldVersion: {3}", ProductName, DeviceType, BuildVersion, IsOld);
        }
    }

    public class NRAndroidComputingDevice
    {
        private static NRAndroidComputingDeviceInfo m_DeviceInfo;

        public static NRAndroidComputingDeviceInfo Info
        {
            get
            {
                if (m_DeviceInfo == null)
                    m_DeviceInfo = new NRAndroidComputingDeviceInfo();
                return m_DeviceInfo;
            }
        }
    }
}
