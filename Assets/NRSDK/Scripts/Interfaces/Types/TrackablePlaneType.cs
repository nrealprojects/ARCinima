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
    /**
    * @brief Trackable plane type.
    */
    public enum TrackablePlaneType
    {
        /**
         * HORIZONTAL trackable plane.
         */
        HORIZONTAL = 0,

        /**
         * VERTICAL trackable plane.
         */
        VERTICAL = 1,

        /**
         * INVALID trackable plane.
         */
        INVALID = 2
    }

    /**
   * @brief Trackable plane's finding mode.
   */
    public enum TrackablePlaneFindingMode
    {
        /**
        * Disable plane detection.
        */
        DISABLE = 0,

        /**
         * Enable plane detection.
         */
        HORIZONTAL = 1,
    }
}
