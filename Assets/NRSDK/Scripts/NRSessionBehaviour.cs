/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* NRSDK is distributed in the hope that it will be usefull                                                              
*                                                                                                                                                           
* https://www.nreal.ai/                 
* 
*****************************************************************************/

namespace NRKernal
{
    using UnityEngine;

    /**
    * @brief  Oprate AR system state and handles the session lifecycle for application layer.
    */
    public class NRSessionBehaviour : MonoBehaviour
    {
        /**
         * @brief The SessionConfig of nrsession.
         */
        [Tooltip("A scriptable object specifying the NRSDK session configuration.")]
        public NRSessionConfig SessionConfig;

        void Awake()
        {
#if !UNITY_EDITOR
            NRDebug.EnableLog = Debug.isDebugBuild;
#endif
            NRDebug.Log("[SessionBehaviour Awake: CreateSession]");
            NRSessionManager.Instance.CreateSession(this);
        }

        void Start()
        {
            NRDebug.Log("[SessionBehaviour DelayStart: StartSession]");
            NRSessionManager.Instance.StartSession();
            NRSessionManager.Instance.SetConfiguration(SessionConfig);
        }

        void OnDisable()
        {
            NRDebug.Log("[SessionBehaviour OnDisable: DisableSession]");
            NRSessionManager.Instance.DisableSession();
        }

        void OnDestroy()
        {
            NRDebug.Log("[SessionBehaviour OnDestroy DestroySession]");
            NRSessionManager.Instance.DestroySession();
        }
    }
}
