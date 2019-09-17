/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEditor;
    using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build.Reporting;
#endif

#if UNITY_2018_1_OR_NEWER
    internal class PreprocessBuildBase : IPreprocessBuildWithReport
#else
    internal class PreprocessBuildBase : IPreprocessBuild
#endif
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            OnPreprocessBuild(report.summary.platform, report.summary.outputPath);
        }
#endif

        public virtual void OnPreprocessBuild(BuildTarget target, string path)
        {
        }
    }
}