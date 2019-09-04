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
    /// @cond EXCLUDE_FROM_DOXYGEN
    public class NRDevice : SingleTon<NRDevice>
    {
        internal NativeHMD NativeHMD { get; set; }

        public NRDevice()
        {
            NativeHMD = new NativeHMD();
            NativeHMD.Create();
        }

        public void Destroy()
        {
            if (NativeHMD != null)
            {
                NativeHMD.Destroy();
                NativeHMD = null;
            }
        }
    }
    /// @endcond
}
