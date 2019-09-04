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
    /// <summary>
    /// A filter for trackable queries.
    /// </summary>
    public enum NRTrackableQueryFilter
    {
        /// <summary>
        /// Indicates available trackables.
        /// </summary>
        All,

        /// <summary>
        /// Indicates new trackables detected in the current NRSDK Frame.
        /// </summary>
        New,
    }
}
