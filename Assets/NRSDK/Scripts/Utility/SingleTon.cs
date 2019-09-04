/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* NRSDK is distributed in the hope that it will be usefull                                                              
*                                                                                                                                                           
* https://www.nreal.ai/                                                                                                                         
*****************************************************************************/

namespace NRKernal
{
    /// @cond EXCLUDE_FROM_DOXYGEN
    public class SingleTon<T> where T : new()
    {
        private static readonly T instance = new T();

        public static T Instance
        {
            get
            {
                return instance;
            }
        }
    }
    /// @endcond
}
