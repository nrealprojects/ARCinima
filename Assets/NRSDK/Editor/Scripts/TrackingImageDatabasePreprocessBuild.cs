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
    using UnityEditor;

    internal class TrackingImageDatabasePreprocessBuild : PreprocessBuildBase
    {
        public override void OnPreprocessBuild(BuildTarget target, string path)
        {
            var augmentedImageDatabaseGuids = AssetDatabase.FindAssets("t:NRTrackingImageDatabase");
            foreach (var databaseGuid in augmentedImageDatabaseGuids)
            {
                var database = AssetDatabase.LoadAssetAtPath<NRTrackingImageDatabase>(
                    AssetDatabase.GUIDToAssetPath(databaseGuid));

                TrackingImageDatabaseInspector.BuildDataBase(database);
                database.BuildIfNeeded();
            }
        }
    }
}
