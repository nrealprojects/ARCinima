using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.OriginController
{
    public class ReplaceControllerProvider : MonoBehaviour
    {
        //change default android controller provider at Awake
        private void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if(NRAndroidComputingDevice.Info.IsOld)
                ControllerProviderFactory.androidControllerProviderType = typeof(OriginControllerProvider);
#endif
        }
    }
}
