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
    using UnityEngine.EventSystems;

    /// @cond EXCLUDE_FROM_DOXYGEN
    public class NRExecutePointerEvents
    {

        public static readonly ExecuteEvents.EventFunction<IEventSystemHandler> PressEnterHandler = ExecuteEnter;
        private static void ExecuteEnter(IEventSystemHandler handler, BaseEventData eventData)
        {

        }

        public static readonly ExecuteEvents.EventFunction<IEventSystemHandler> PressExitHandler = ExecuteExit;
        private static void ExecuteExit(IEventSystemHandler handler, BaseEventData eventData)
        {

        }
    }
    /// @endcond
}
